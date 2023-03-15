using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowBlast : MonoBehaviour
{
    [Header("----- Snow blast -----")]
    [SerializeField] GameObject snowEffect;

    [Header("----- Snow blast info -----")]
    [SerializeField] snowBlastAbility abil;
    [SerializeField] float minDamage;
    [SerializeField] float speed;
    [SerializeField] float timer;
    [SerializeField] float dist;
    [SerializeField] Vector3 maxSize;
    [SerializeField] Vector3 scaleChange;

    float damage;
    private GameObject VFX;
    

    private void Start()
    {
        damage = abil.damage;
        VFX = Instantiate(snowEffect, transform.position, Quaternion.identity, transform);
    }

    private void Update()
    {
        if (timer > 0 && !gameManager.instance.isPaused)
        {
            if (maxSize.x > transform.localScale.x && maxSize.z > transform.localScale.z)
                transform.localScale += scaleChange;

            if (damage > minDamage)
                damage -= Time.deltaTime / 2;

            transform.Translate(-transform.forward * speed * Time.deltaTime);

            timer -= Time.deltaTime;
        }
        else if (timer <= 0)
        {
            Destroy(VFX);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<enemyAI>() != null && other == other.GetComponent<enemyAI>().dmgColl)
        {
            other.GetComponent<IDamage>().takeDamage(damage);
        }

        //if (other.CompareTag("Environment"))
        //{
        //    Destroy(VFX);
        //    Destroy(gameObject);
        //}
    }
}
