using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatFunctions : MonoBehaviour
{
    public Transform cam;
    public Transform attackPoint;
    public GameObject attackItem;
    public float cooldown;

    void snowball()
    {
        if(Input.GetMouseButton(0) && cooldown <= 0)
        {
            GameObject snowball = Instantiate(attackItem, attackPoint.position, cam.rotation);


        }
    }
}
