using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ability : ScriptableObject
{
    [SerializeField] public new string name;
    [SerializeField] public float cooldownTime;
    [SerializeField] public float activeTime;

    public virtual void Activate(Transform cast) {}
}
