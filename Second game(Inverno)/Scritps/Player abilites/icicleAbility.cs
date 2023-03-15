using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class icicleAbility : ability
{
    [SerializeField] public int damage;
    [SerializeField] GameObject icicle;
    public int daggers;

    public override void Activate(Transform cast)
    {
        Instantiate(icicle, cast.position, gameManager.instance.playerScript.cam.rotation * icicle.transform.localRotation);
    }
}

