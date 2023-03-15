using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class snowBlastAbility : ability
{
    [SerializeField] public int damage;
    [SerializeField] GameObject snowBlast;
    [SerializeField] int castTimes;


    public override void Activate(Transform cast)
    {
        //MonoInstance.instance.StartCoroutine(snowBlastCast(cast));
        Instantiate(snowBlast, cast.position, Camera.main.transform.rotation * snowBlast.transform.rotation);
    }

    IEnumerator snowBlastCast(Transform cast)
    {
        for (int i = 0; i < castTimes; i++)
        {
            

            yield return new WaitForSeconds(activeTime / castTimes);
        }
    }
}