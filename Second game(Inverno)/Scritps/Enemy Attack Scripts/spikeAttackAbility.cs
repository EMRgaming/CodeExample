using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class spikeAttackAbility : ability
{
    [Header("----- Attak Attributes -----")]
    [SerializeField] GameObject spike;
    [SerializeField] float damage;
    [SerializeField] float timeBetweenWaves;
    [SerializeField] float dist;
    [SerializeField] float spawnArea;

    [Header("----- Wave 2 Attributes -----")]
    [SerializeField] float dmgMod1;
    [SerializeField] float scaleMod1;

    [Header("----- Wave 3 Attributes -----")]
    [SerializeField] float dmgMod2;
    [SerializeField] float scaleMod2;

    float wait;
    float yPos;
    Vector3 origSize;


    public override void Activate(Transform cast)
    {
        origSize = spike.transform.localScale;
        yPos = Terrain.activeTerrain.SampleHeight(cast.position);

        gameManager.instance.StartCoroutine(CastAbil(cast));
    }

    public IEnumerator CastAbil(Transform cast)
    {
        spike.GetComponent<spikeAtk>().damage = damage;
        
        for(int i = 0; i < 3; ++i)
        {
            Instantiate(spike, new Vector3(cast.position.x + Random.Range(-spawnArea, spawnArea),
                                            yPos + 0.4f, 
                                            cast.position.z + Random.Range(-spawnArea, spawnArea)) + cast.transform.forward,
                        Quaternion.identity);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        spike.transform.localScale = origSize * scaleMod1;

        for(int i = 0; i < 5; ++i)
        {
            Instantiate(spike, new Vector3(cast.position.x + Random.Range(-spawnArea * 1.25f, spawnArea * 1.25f),
                                            yPos + 0.4f, 
                                            cast.position.z + Random.Range(-spawnArea * 1.25f, spawnArea * 1.25f)) + (cast.transform.forward * dist),
                        Quaternion.identity);
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        spike.transform.localScale = origSize * scaleMod2;

        for(int i = 0; i < 7; ++i)
        {
            Instantiate(spike, new Vector3(cast.position.x + Random.Range(-spawnArea * 1.6f, spawnArea * 1.6f),
                                            yPos + 0.6f, 
                                            cast.position.z + Random.Range(-spawnArea * 1.6f, spawnArea * 1.6f)) + (cast.transform.forward * (dist * 1.75f)),
                        Quaternion.identity);
        }

        spike.transform.localScale = origSize;

        yield return null;
    }
}
