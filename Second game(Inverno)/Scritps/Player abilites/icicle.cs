using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class icicle : MonoBehaviour
{
    public Rigidbody ice;
    public float throwForce;
    public float activeTime;

    [SerializeField] icicleAbility abil;
    Vector3 forceDirection;


    private void Start()
    {
        forceDirection = gameManager.instance.player.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, 5000))
        {
            forceDirection = (hit.point - transform.position).normalized;
        }
        else
        {
            forceDirection = Camera.main.transform.forward;
        }

        ice.velocity = (forceDirection) * throwForce;

        if (activeTime > 0)
        {
            activeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<enemyAI>() != null && other == other.GetComponent<enemyAI>().dmgColl)
        {
            other.GetComponent<IDamage>().takeDamage(abil.damage);
            Destroy(gameObject);
        }
        else if (other.GetComponent<enemyAI>() == null)
        {
            Destroy(gameObject);
        }


    }
}
