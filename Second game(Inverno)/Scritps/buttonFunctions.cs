using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] AudioSource aud;

    [Header("---- Audio ----")]
    [SerializeField] AudioClip audClick;
    [Range(0, 1)][SerializeField] float audClickVol;

    public void resume()
    {
        aud.PlayOneShot(audClick);
        gameManager.instance.pauseMenu.SetActive(false);
        gameManager.instance.unPauseGame();
        gameManager.instance.isPaused = false;
        gameManager.instance.unPauseGame();
    }

    public void restart()
    {
        gameManager.instance.unPauseGame();

        //gameManager.instance.loadScreen.SetActive(true);

        reloadScene();
    }

    public void credits()
    {
        SceneManager.LoadScene("CreditsScreen");
    }

    public void quit()
    {
        aud.PlayOneShot(audClick);
        Application.Quit();
    }
    public void respawn()
    {
        aud.PlayOneShot(audClick);
        gameManager.instance.unPauseGame();
        gameManager.instance.playerScript.playerRespawn();
        gameManager.instance.playerDeadMenu.SetActive(false);
    }

    public void audioButton()
    {
        aud.PlayOneShot(audClick);
        gameManager.instance.pauseMenu.SetActive(false);
        gameManager.instance.audioMenu.SetActive(true);
    }

    public void audioBackButton()
    {
        aud.PlayOneShot(audClick);
        gameManager.instance.audioMenu.SetActive(false);
        gameManager.instance.pauseMenu.SetActive(true);
    }


    void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool snowballAbility()
    {
        gameManager.instance.player.GetComponent<abilityHolder>().currentAbilityIndex = 0;
        return true;
    }
    public bool snowblastAbility()
    {
        gameManager.instance.player.GetComponent<abilityHolder>().currentAbilityIndex = 1;
        return true;
    }
    public bool icicleAbility()
    {
        gameManager.instance.player.GetComponent<abilityHolder>().currentAbilityIndex = 2;
        return true;
    }

    public void nextLevelButton()
    {
        gameManager.instance.unPauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void continueButton()
    {
        gameManager.instance.unPauseGame();
        SceneManager.LoadScene("CreditsScreen");
    }
    public void mainMenu()
    {
        SceneManager.LoadScene("Title Screen Main Menu");
    }
}
