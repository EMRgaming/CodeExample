using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class snowball : MonoBehaviour
{
    [SerializeField] public GameObject sball;

    [SerializeField] SnowballScript abil;
    [SerializeField] Collider ball;
    [SerializeField] public int throwForce;
    [SerializeField] public int throwUpForce;
    [SerializeField] public float activeTime;
    [SerializeField] public Transform cam; 
    Vector3 forceDirection;

    float damage;

    private void Start()
    {
        damage = abil.damage;
        Throw();
    }
    private void Update()
    {
        if (activeTime > 0)
        {
            activeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(sball);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<enemyAI>() != null && other == other.GetComponent<enemyAI>().dmgColl)
        {
            other.GetComponent<IDamage>().takeDamage(damage);
            Destroy(sball);
        }
        if (other.GetComponent<enemyAI>() == null)
        {
            Destroy(sball);
        }

    }

    public void Throw()
    {
        Rigidbody attackRb = sball.GetComponent<Rigidbody>();

        forceDirection = gameManager.instance.player.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, 5000))
        {
            forceDirection = (hit.point - transform.position).normalized;
        }
        else
        {
            forceDirection = Camera.main.transform.forward;
        }

        Vector3 forceToAdd = forceDirection * throwForce + gameManager.instance.player.transform.up * throwUpForce;

        attackRb.AddForce(forceToAdd, ForceMode.Impulse);
    }
}
