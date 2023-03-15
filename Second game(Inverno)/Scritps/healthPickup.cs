using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class healthPickup : ScriptableObject
{
    public float healthAmount;
    public float destroyTimer;
    public GameObject model;
}
