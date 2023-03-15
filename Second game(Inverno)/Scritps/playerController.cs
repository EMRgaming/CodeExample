using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] AnimationCurve slideCurve;
    [SerializeField] public Animator anim;
    [SerializeField] public GameObject UI;
    [SerializeField] GameObject HPBarBack;
    [SerializeField] public Image HPBar;
    [SerializeField] public Image HPBarAnim;
    [SerializeField] GameObject StamBarBack;
    [SerializeField] public Image StamBar;
    [SerializeField] public Image StamBarAnim;
    [SerializeField] GameObject dagger;
    [SerializeField] public Transform cam;
    [SerializeField] public managerJoystick MJ;
    public GameObject mobAbil1;
    public GameObject mobAbil2;
    public GameObject mobAbil3;

    [Header("----- Player Stats -----")]
    [SerializeField] float HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMod;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float slideTime;
    [SerializeField] float stamina;
    [SerializeField] float combatTimer;
    [SerializeField] float stamTimer;
    [SerializeField] int animLerpSpeed;
    [SerializeField] float dodgeTime;
    [SerializeField] abilityHolder abilHolder;
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject hitEffectBack;
    [SerializeField] float turnSmoothTime = 0.1f;
    

    [Header("---- Audio ----")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)] [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)] [SerializeField] float audHurtVol;
    [SerializeField] AudioClip audDash;
    [Range(0, 1)] [SerializeField] float audDashVol;
    [SerializeField] AudioClip audDodge;
    [Range(0, 1)][SerializeField] float audDodgeVol;
    [SerializeField] AudioClip[] audWalk;
    [Range(0, 1)][SerializeField] float audWalkVol;


    Vector3 moveDir;
    private Vector3 playerVelocity;
    int jumpsTimes;
    float playerSpeedOrig;
    float HPOrig;
    float stamOrig;
    float combatTime;
    float stamBarTimer;
    bool isSprinting;
    bool isDodging;
    bool isUsingMoveAbility = false;
    bool dashedOnce = false;
    bool isJumping;
    bool isGrounded;
    [HideInInspector]
    public bool playerIsDead;
    float turnSmoothVelocity;
    float horizontal;
    float vertical;
    


    private void Start()
    {
        gameManager.instance.menuOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isSprinting = false;
        playerSpeedOrig = playerSpeed;
        HPOrig = HP;
        stamOrig = stamina;
        playerRespawn();
        updatePlayerHPBar();
        mobAbil1 = gameManager.instance.MA1;
        mobAbil2 = gameManager.instance.MA2;
        mobAbil3 = gameManager.instance.MA3;

        if (gameManager.instance.mobileBuild)
        MJ = GameObject.Find("joystickBackground").GetComponent<managerJoystick>();
    }

    void Update()
    {
        playerIsDead = false;
        isGrounded = controller.isGrounded;
        movement();
        walkAnim();
        //UIHPCombat();
        stamUpdate();
        StartCoroutine(Dash());

        if (controller.isGrounded)
        {
            jumpsTimes = 0;
            playerVelocity.y = 0f;
            dashedOnce = false;
            isJumping = false;
            anim.SetBool("Jumping", false);
            anim.SetBool("DoubleJump", false);
        }
    }

    void movement()
    {
        horizontal = MJ.inputHorizontal();
        vertical = MJ.inputVertical();

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * playerSpeed * Time.deltaTime);
        }

        sprint();

        jump();

        if(Input.GetButton("Dodge") && !isUsingMoveAbility && jumpsTimes == 0 && stamina > 10)
        {
            StartCoroutine(Dodge());
        } 
    }

    void sprint()
    {
        if (!isSprinting && Input.GetButtonDown("Sprint") && (Input.GetButton("Vertical") || Input.GetButton("Horizontal")) && stamina > 0)
        {
            anim.SetBool("Sprinting", true);
            playerSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (isSprinting && controller.velocity.magnitude == 0)
        {
            isSprinting = false;
            playerSpeed = playerSpeedOrig;
            anim.SetBool("Sprinting", false);
        }
        if(stamina < 0)
        {
            isSprinting = false;
            playerSpeed = playerSpeedOrig;
            anim.SetBool("Sprinting", false);
        }
    }

    public void jump()
    {
        
        if (Input.GetButtonDown("Jump") && jumpsTimes < jumpsMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpsTimes++;
            anim.SetBool("Jumping", true);
            playerVelocity.y = jumpHeight;
            isJumping = true;
        }

        if(Input.GetButtonDown("Jump") && jumpsTimes > 1)
        {
            anim.SetBool("DoubleJump", true);
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator Dash()
    {
        if (jumpsTimes > 0
            && moveDir.magnitude > 0 
            && Input.GetMouseButtonDown(1) 
            && !isUsingMoveAbility 
            && !dashedOnce
            && stamina > 10)
        {
            isUsingMoveAbility = true;
            stamina -= 10;
            stamBarTimer = 0f;
            playerSpeed = dashSpeed;
            float gravOrig = gravityValue;
            float startTime = Time.time;

            aud.PlayOneShot(audDash, audDashVol);

            anim.SetBool("Dashing", true);

            while (Time.time < startTime + dashTime)
            {
                controller.Move(moveDir * dashSpeed * Time.deltaTime);
                gravityValue = 0;

                yield return null;
            }
            playerVelocity.y = 0;
            yield return new WaitForSeconds(.2f);
            gravityValue = gravOrig;
            dashedOnce = true;
            anim.SetBool("DoubleJump", false);
            anim.SetBool("Dashing", false);
            playerSpeed = playerSpeedOrig;
            isUsingMoveAbility = false;
            sprint();
        }
    }

    public void dodge()
    {
        StartCoroutine(Dodge());
    }
    IEnumerator Dodge()
    {
        isDodging = true;
        stamina -= 10;
        stamBarTimer = 0f;
        anim.SetBool("Dodging", true);
        isUsingMoveAbility = true;
        float elapsedTime = 0;
        aud.PlayOneShot(audDodge, audDodgeVol);

        while (elapsedTime < dodgeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        anim.SetBool("Dodging", false);

        isDodging = false;
        isUsingMoveAbility = false;
    }

    public void damage(float dmg)
    {
        if (isDodging)
            return;
        else if(!isDodging)
        {
            HP -= dmg;

            combatTime = Time.time;

            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

            updatePlayerHPBar();

            StartCoroutine(gameManager.instance.playerDamageFlash());

            if (HP <= 0)
            {
                playerIsDead = true;
                gameManager.instance.playerDeadMenu.SetActive(true);
                gameManager.instance.pauseGame();
            }
        }
    }

    public void healthPickup(float health)
    {
        if(health > HPOrig - HP)
            HP = HPOrig;
        else
            HP += health;

        updatePlayerHPBar();
    }

    void updatePlayerHPBar()
    {
        HPBar.fillAmount = (float)HP / (float)HPOrig;
    }

    public void playerRespawn()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.spawnPos.transform.position;
        HP = HPOrig;
        controller.enabled = true;
        playerIsDead = false;
        updatePlayerHPBar();
    }

    void walkAnim()
    {
        if (isSprinting == true)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), 4, Time.deltaTime * animLerpSpeed));
        }
        else if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), 1f, Time.deltaTime * animLerpSpeed));
        }
        else if(controller.velocity.magnitude == 0)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), 0, Time.deltaTime * animLerpSpeed));
        }
    }
    //void UIHPCombat()
    //{
    //    if(Time.time - combatTime > combatTimer)
    //        HPBarBack.SetActive(false);
    //    else
    //    {
    //        if(HP != HPOrig)
    //            HPBarBack.SetActive(true);
    //        else if(Time.time < combatTimer)
    //            HPBarBack.SetActive(false);
    //    }
    //}

    void stamUpdate()
    {
        if(isSprinting)
        {
            stamina -= Time.deltaTime;
            stamBarTimer = 0f;
        }
        else if(!isSprinting && stamina < stamOrig)
            stamina += Time.deltaTime * 2;
        else
        {
            stamina = 50f;
            stamBarTimer += Time.deltaTime;
        }

        StamBar.fillAmount = stamina / stamOrig;

        if(stamBarTimer >= stamTimer)
            StamBarBack.SetActive(false);
        else
            StamBarBack.SetActive(true);
    }
    void walkSound()
    {
        aud.PlayOneShot(audWalk[Random.Range(0, audWalk.Length)], audWalkVol);
    }

}