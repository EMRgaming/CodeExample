using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SnowballScript : ability
{
    [SerializeField] public int damage;
    [SerializeField] GameObject snowball;


    public override void Activate(Transform cast)
    {
        Instantiate(snowball, cast.position, cast.rotation * snowball.transform.rotation);
    }
}
