using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class fireballAbility : ability
{
    [SerializeField] GameObject fireball;

    public override void Activate(Transform cast)
    {
        Instantiate(fireball, cast.position, cast.localRotation);
    }
}