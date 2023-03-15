using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] float timer;
    [SerializeField] GameObject fireEffect;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audFire;
    [Range(0, 1)][SerializeField] float audFireVol;

    GameObject VFX;
    Vector3 dir;

    void Start()
    {
        //transform.LookAt(gameManager.instance.player.transform);
        VFX = Instantiate(fireEffect, transform.position, transform.rotation, transform);
        dir = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        aud.PlayOneShot(audFire, audFireVol);
    }

    void Update()
    {
        if(timer > 0 && !gameManager.instance.isPaused)
        {
            timer -= Time.deltaTime;

            transform.Translate(dir * Time.deltaTime);
        }
        else
        {
            if(!gameManager.instance.isPaused)
            {
                Destroy(VFX);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.damage(damage);
            Destroy(VFX);
            Destroy(gameObject);
        }
        else if(other.CompareTag("Environment"))
        {
            Destroy(VFX);
            Destroy(gameObject);
        }
    }
}