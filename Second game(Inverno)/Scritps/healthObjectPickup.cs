using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthObjectPickup : MonoBehaviour
{
    [SerializeField] healthPickup health;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip audPickup;
    [Range(0, 1)] [SerializeField] float audPickupVol;
    [SerializeField] float degreesPerSecond;
    [SerializeField] float range;
    [SerializeField] float speed;
    
    private float destroyTimer;
    private float yOrigin;
    private float yPos;


    void Start()
    {
        destroyTimer = health.destroyTimer;
        yOrigin = transform.position.y;
    }

    void Update()
    {
        destroyTimer -= Time.deltaTime;

        if(destroyTimer <= 0)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.Rotate(degreesPerSecond * Time.deltaTime, 0, 0);

        yPos = Mathf.PingPong(speed * Time.time, 1) * range + yOrigin;
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.healthPickup(health.healthAmount);
            aud.PlayOneShot(audPickup, audPickupVol);
            Destroy(gameObject);
        }
    }
}
