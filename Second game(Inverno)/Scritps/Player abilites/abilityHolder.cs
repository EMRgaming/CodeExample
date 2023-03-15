using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class abilityHolder : MonoBehaviour
{
    public int currentAbilityIndex;
    [SerializeField] List<ability> abilites;
    [SerializeField] GameObject dagger;
    float snowBall_cooldownTime;
    float snowBlast_cooldownTime;
    float IceBlade_cooldownTime;
    float _activeTime;
    public Image snowBall_abilityIcon;
    public Image snowBlast_abilityIcon;
    public Image iceBlade_abilityIcon;
    public Image selectedAbility1;
    public Image selectedAbility2;
    public Image selectedAbility3;
    public Transform cast;

    enum abilityState
    {
        ready,
        active,
        cooldown
    }
    abilityState snowBallState = abilityState.ready;
    abilityState snowBlastState = abilityState.ready;
    abilityState iceBladeState = abilityState.ready;

    private void Start()
    {
        currentAbilityIndex = 0;
        snowBall_abilityIcon = gameManager.instance.ability1;
        snowBlast_abilityIcon = gameManager.instance.ability2;
        iceBlade_abilityIcon = gameManager.instance.ability3;
    }

    private void Update()
    {
        //if (gameManager.instance.mobileBuild == true)
        //{

        //}
        if (gameManager.instance.mobileBuild == false)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && currentAbilityIndex < abilites.Count - 1)
            {
                currentAbilityIndex++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentAbilityIndex > 0)
            {
                currentAbilityIndex--;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentAbilityIndex == 0)
            {
                currentAbilityIndex = abilites.Count - 1;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0 && currentAbilityIndex == abilites.Count - 1)
            {
                currentAbilityIndex = 0;
            }

            if (currentAbilityIndex == 2)
            {
                dagger.SetActive(true);
            }
            else
            {
                dagger.SetActive(false);
            }

            if(currentAbilityIndex == 0)
            {
                selectedAbility2.enabled = false;
                selectedAbility3.enabled = false;
                selectedAbility1.enabled = true;
            }
            else if (currentAbilityIndex == 1)
            {
                selectedAbility1.enabled = false;
                selectedAbility3.enabled = false;
                selectedAbility2.enabled = true;
            }
            else if (currentAbilityIndex == 2)
            {
                selectedAbility1.enabled = false;
                selectedAbility2.enabled = false;
                selectedAbility3.enabled = true;
            }
        }



        //Snowball cooldown timer
        switch (snowBallState)
        {
            case abilityState.ready:
                if (gameManager.instance.mobileBuild == true)
                {
                    if (Input.GetTouch(1).position.x >= Screen.width / 2
                        && Input.GetTouch(1).position.x <= (Screen.width / 6) * 4
                        && Input.GetTouch(1).position.y >= 0
                        && Input.GetTouch(1).position.y <= Screen.height / 4
                        && gameManager.instance.playerScript.anim.GetBool("Casting") == false)
                    {
                        gameManager.instance.playerScript.anim.SetBool("Casting", true);
                        gameManager.instance.playerScript.anim.SetTrigger("SnowBall");
                    }
                }
                else if (gameManager.instance.mobileBuild == false)
                {
                    if (Input.GetButton("Shoot") && currentAbilityIndex == 0 && gameManager.instance.playerScript.anim.GetBool("Casting") == false)
                    {
                        gameManager.instance.playerScript.anim.SetBool("Casting", true);
                        gameManager.instance.playerScript.anim.SetTrigger("SnowBall");
                    }
                }


                break;
            case abilityState.active:
                if (_activeTime > 0)
                    _activeTime -= Time.deltaTime;
                else
                {
                    snowBallState = abilityState.cooldown;
                    snowBall_cooldownTime = abilites[0].cooldownTime;
                }
                break;
            case abilityState.cooldown:
                if (snowBall_cooldownTime > 0)
                {
                    snowBall_cooldownTime -= Time.deltaTime;
                    snowBall_abilityIcon.fillAmount = snowBall_cooldownTime / abilites[0].cooldownTime;
                    //snowBall_abilityIcon.fillAmount = Mathf.Lerp(snowBall_cooldownTime, abilites[0].cooldownTime, .25f);
                }
                else
                {
                    snowBallState = abilityState.ready;
                    snowBall_abilityIcon.fillAmount = 0;
                }
                break;
        }



        //SnowBlast cooldown timer
        switch (snowBlastState)
        {
            case abilityState.ready:
                if (gameManager.instance.mobileBuild == true)
                {
                    if (Input.GetTouch(1).position.x >= (Screen.width / 3) * 2
                        && Input.GetTouch(1).position.x <= (Screen.width / 6) * 5
                        && Input.GetTouch(1).position.y >= 0
                        && Input.GetTouch(1).position.y <= Screen.height / 4
                        && gameManager.instance.playerScript.anim.GetBool("Casting") == false)
                    {
                        gameManager.instance.playerScript.anim.SetBool("Casting", true);
                        gameManager.instance.playerScript.anim.SetTrigger("SnowBlast");
                    }
                }
                else if (gameManager.instance.mobileBuild == false)
                {
                    if (Input.GetButton("Shoot") && currentAbilityIndex == 1 && gameManager.instance.playerScript.anim.GetBool("Casting") == false)
                    {
                        gameManager.instance.playerScript.anim.SetBool("Casting", true);
                        gameManager.instance.playerScript.anim.SetTrigger("SnowBlast");
                    }
                }
                break;

            case abilityState.active:
                if (_activeTime > 0)
                    _activeTime -= Time.deltaTime;
                else
                {
                    snowBlastState = abilityState.cooldown;
                    snowBlast_cooldownTime = abilites[1].cooldownTime;
                }
                break;
            case abilityState.cooldown:
                if (snowBlast_cooldownTime > 0)
                {
                    snowBlast_cooldownTime -= Time.deltaTime;
                    snowBlast_abilityIcon.fillAmount = snowBlast_cooldownTime / abilites[1].cooldownTime;
                    //snowBlast_abilityIcon.fillAmount = Mathf.Lerp(snowBlast_cooldownTime, abilites[1].cooldownTime, .25f);
                }
                else
                {
                    snowBlastState = abilityState.ready;
                    snowBlast_abilityIcon.fillAmount = 0;
                }
                break;
        }



        //IceBlade cooldown timer
        switch (iceBladeState)
        {
            case abilityState.ready:
                if (gameManager.instance.mobileBuild == true)
                {
                    if (Input.GetTouch(1).position.x >= (Screen.width / 6) * 5
                        && Input.GetTouch(1).position.x <= Screen.width
                        && Input.GetTouch(1).position.y >= 0
                        && Input.GetTouch(1).position.y <= Screen.height / 4
                        && gameManager.instance.playerScript.anim.GetBool("Casting") == false)
                    {
                        gameManager.instance.playerScript.anim.SetBool("Casting", true);
                        gameManager.instance.playerScript.anim.SetTrigger("IceBlade");
                    }
                }
                else if (gameManager.instance.mobileBuild == false)
                {
                    if (Input.GetButton("Shoot") && currentAbilityIndex == 2 && gameManager.instance.playerScript.anim.GetBool("Casting") == false)
                    {
                        gameManager.instance.playerScript.anim.SetBool("Casting", true);
                        gameManager.instance.playerScript.anim.SetTrigger("IceBlade");
                    }
                }
                break;
            case abilityState.active:
                if (_activeTime > 0)
                    _activeTime -= Time.deltaTime;
                else
                {
                    iceBladeState = abilityState.cooldown;
                    IceBlade_cooldownTime = abilites[2].cooldownTime;
                }
                break;
            case abilityState.cooldown:
                if (IceBlade_cooldownTime > 0)
                {
                    IceBlade_cooldownTime -= Time.deltaTime;
                    iceBlade_abilityIcon.fillAmount = IceBlade_cooldownTime / abilites[2].cooldownTime;
                    //IceBlade_abilityIcon.fillAmount = Mathf.Lerp(IceBlade_cooldownTime, abilites[2].cooldownTime, .25f);
                }
                else
                {
                    iceBladeState = abilityState.ready;
                    iceBlade_abilityIcon.fillAmount = 0;
                }
                break;
        }

    }

    public void SnowBall()
    {
        abilites[0].Activate(cast);
        snowBallState = abilityState.active;
        _activeTime = abilites[0].activeTime;
        gameManager.instance.playerScript.anim.SetBool("Casting", false);
    }

    public void snowBlast()
    {
        abilites[1].Activate(cast);
        snowBlastState = abilityState.active;
        _activeTime = abilites[1].activeTime;
        gameManager.instance.playerScript.anim.SetBool("Casting", false);

    }

    public void iceBlade()
    {
        abilites[2].Activate(cast);
        iceBladeState = abilityState.active;
        _activeTime = abilites[2].activeTime;
        gameManager.instance.playerScript.anim.SetBool("Casting", false);
    }
}

