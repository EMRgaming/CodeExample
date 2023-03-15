using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowHealth : MonoBehaviour
{
    public int HP;
    [SerializeField] GameObject beacon;

    GameObject holder;


    public void damage(int dmg)
    {
        HP -= dmg;

        holder = Instantiate(beacon, transform);

        Destroy(holder, 5);

        gameManager.instance.WorldHP -= dmg;

        gameManager.instance.updateLevelHealth();

        if (HP <= 0)
        {
            if (gameObject.tag == "Snow")
            {
                gameManager.instance.snowAreas.RemoveAt(0);
                Destroy(gameObject);
            }
            else if (gameObject.tag == "Snow 2")
            {
                gameManager.instance.snowAreasTwo.RemoveAt(0);
                Destroy(gameObject);
            }

        }
    }
}
