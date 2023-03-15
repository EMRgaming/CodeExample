using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class summonAbility : ability
{
    [SerializeField] float spawnArea;
    [SerializeField] int minEnemiesToSpawn;
    [SerializeField] int maxEnemiesToSpawn;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] GameObject smallEnemy;
    [SerializeField] GameObject medEnemy;

    int enemiesToSpawn;
    float yPos;

    public override void Activate(Transform cast)
    {
        yPos = Terrain.activeTerrain.SampleHeight(cast.transform.position);
        gameManager.instance.StartCoroutine(summonCast(cast));
    }

    IEnumerator summonCast(Transform cast)
    {
        enemiesToSpawn = Random.Range(minEnemiesToSpawn, maxEnemiesToSpawn + 1);

        for (int i = 0; i < enemiesToSpawn; ++i)
        {
            Instantiate(smallEnemy, new Vector3(cast.transform.position.x + Random.Range(-spawnArea, spawnArea),
                                                yPos,
                                                cast.transform.position.z + Random.Range(-spawnArea, spawnArea)),
                        cast.transform.rotation);

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if(Random.Range(0, 100) < 25)
        {
            Instantiate(medEnemy, new Vector3(cast.transform.position.x + Random.Range(-spawnArea, spawnArea),
                                                yPos,
                                                cast.transform.position.z + Random.Range(-spawnArea, spawnArea)),
                        cast.transform.rotation);
        }

        yield return null;
    }
}
