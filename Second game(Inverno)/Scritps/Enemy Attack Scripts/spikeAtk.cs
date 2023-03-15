using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeAtk : MonoBehaviour
{
    [SerializeField] float objTimer;
    [SerializeField] float upForce;
    [SerializeField] float KBTimer;
    [SerializeField] GameObject rock;
    [SerializeField] GameObject spike;
    public float damage;

    void Start()
    {
        rock.transform.localRotation = Quaternion.Euler(-90, Random.Range(-360, 361), 0);
        spike.transform.localRotation = Quaternion.Euler(Random.Range(-85, -74), 0, 0);
    }

    void Update()
    {
        if(objTimer > 0)
            objTimer -= Time.deltaTime;
        else
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.damage(damage);

            Vector3 dir = (other.transform.position - transform.position);

            dir.y *= upForce;

            while(KBTimer > 0)
            {
                KBTimer -= Time.deltaTime;

                gameManager.instance.playerScript.GetComponent<CharacterController>().Move(dir * Time.deltaTime);
            }
        }
    }
}