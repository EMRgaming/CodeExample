using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class creditsScreen : MonoBehaviour
{
    public GameObject creditsOne;
    public GameObject creditsTwo;

    private void Update()
    {
        StartCoroutine(credits());
    }



    IEnumerator credits()
    {
        creditsOne.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        creditsOne.SetActive(false);
        creditsTwo.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        //creditsTwo.SetActive(false);
        SceneManager.LoadScene("Title Screen Main Menu");
    }
}
