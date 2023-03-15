using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----  Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] GameObject UI;
    [SerializeField] Image HPBar;
    [SerializeField] Image HPBarAnim;
    [SerializeField] AudioSource aud;
    [SerializeField] public CapsuleCollider dmgColl;
    [SerializeField] List<ability> abilities;
    [SerializeField] Transform cast;


    [Header("---- Enemy Stats ----")]
    [SerializeField] float HP;
    [SerializeField] int damage;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int speedChase;
    [SerializeField] int sightDist;
    [SerializeField] int sightAngle;
    [SerializeField] int roamDist;
    [SerializeField] int animLerpSpeed;
    [SerializeField] float attackRate;
    [SerializeField] GameObject headPos;
    [SerializeField] GameObject healthPick;
    [SerializeField] int healthDropChance;


    [Header("---- Audio ----")]
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audAttack;
    [Range(0, 1)][SerializeField] float audAttackVol;

    Color origColor;
    bool isAttacking;
    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;
    float distToPlayer;
    Vector3 startingPos;
    float stoppingDistOrig;
    float HPOrig;
    float angleToSnow;
    float distToSnow;
    Vector3 snowDir;
    int sightDistOrig;
    float stopDistOrig;
    int enemyNum;

    float HPTimer = 0f;
    float deathTimer = 5f;

    // ability handling
    int abil;
    float lastCast;
    float _spikeActiveTime;
    float _spikeCooldownTime;
    float _fireballActiveTime;
    float _fireBallCooldownTime;
    float _summonActiveTime;
    float _summonCooldownTime;
    public List<snowHealth> snowLocation = new List<snowHealth>();
    public List<snowHealth> snowLocationTwo = new List<snowHealth>();
    GameObject snow;
    enum abilityState
    {
        ready,
        active,
        cooldown
    }

    abilityState spikeState = abilityState.ready;
    abilityState fireballState = abilityState.ready;
    abilityState summonState = abilityState.ready;

    // Start is called before the first frame update
    void Start()
    {
        enemyNum = Random.Range(1, 3);

        if (enemyNum == 1)
        {
            gameObject.tag = "Snow Enemy";
        }
        else if (enemyNum == 2)
        {
            gameObject.tag = "Snow Enemy 2";
        }

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Snow").Length; i++)
        {
            if(gameObject.tag == "Snow Enemy")
            {
                snow = GameObject.FindGameObjectsWithTag("Snow")[i];

                snowLocation.Add(snow.GetComponent<snowHealth>());
            }
        }

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Snow 2").Length; i++)
        {
            if (gameObject.tag == "Snow Enemy 2")
            {
                snow = GameObject.FindGameObjectsWithTag("Snow 2")[i];

                snowLocationTwo.Add(snow.GetComponent<snowHealth>());
            }
        }

        sightDistOrig = sightDist;
        stopDistOrig = agent.stoppingDistance;

        HPOrig = HP;

        //gameManager.instance.enemiesToKill += 1;

        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        origColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        updateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.snowAreas.Count > 0)
        {
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animLerpSpeed));

            //Debug.Log(distToPlayer);

            if (agent.enabled)
            {
                if (playerInRange)
                {
                    canSeePlayer();
                    abilityUse();
                }
                else if (agent.remainingDistance < 0.1f && agent.destination != gameManager.instance.player.transform.position && gameObject.tag != "Snow Enemy" && gameObject.tag != "Snow Enemy 2")
                {
                    roam();
                }

                snowEnemy();
            }
        }

        //if (snowLocation[0] == null && gameObject.tag == "Snow Enemy" && snowLocation.Count > 0)
        //{
        //    snowLocation.RemoveAt(0);
        //}
        //if (snowLocationTwo[0] == null && gameObject.tag == "Snow Enemy 2" && snowLocationTwo.Count > 0)
        //{
        //    snowLocationTwo.RemoveAt(0);
        //}

        if (HPBarAnim.fillAmount != HPBar.fillAmount)
        {
            HPBarAnim.fillAmount = Mathf.Lerp(HPBarAnim.fillAmount, HPBar.fillAmount, HPTimer);
            HPTimer += 0.25f * Time.deltaTime;
        }
        else { HPTimer = 0f; }

        if (HP <= 0)
        {
            if (deathTimer > 0)
            {
                deathTimer -= Time.deltaTime;
                transform.position -= new Vector3(0, 0.01f, 0);
            }
            else
                Destroy(gameObject);
        }
    }

    void canSeePlayer()
    {
        playerDir = (gameManager.instance.player.transform.position - headPos.transform.position);

        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        distToPlayer = Vector3.Distance(playerDir, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(headPos.transform.position, playerDir, out hit))
        {
            Debug.DrawRay(headPos.transform.position, playerDir);

            //Debug.Log(agent.remainingDistance);

            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;

                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    facePlayer();
                }
            }
        }
    }

    void roam()
    {
        agent.stoppingDistance = 0;

        Vector3 randomDir = Random.insideUnitSphere * roamDist;

        randomDir += startingPos;

        NavMeshHit hit;

        NavMesh.SamplePosition(new Vector3(randomDir.x, 0, randomDir.z), out hit, 1, 1);

        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(hit.position, path);

        agent.SetPath(path);
    }

    void facePlayer()
    {
        playerDir.y = 0;

        Quaternion rotation = Quaternion.LookRotation(playerDir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    public void takeDamage(float dmg)
    {
        HP -= dmg;

        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        updateHPBar();

        agent.stoppingDistance = 0;

        anim.SetTrigger("Damaged");

        agent.SetDestination(gameManager.instance.player.transform.position);

        //StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            if (healthDropChance > Random.Range(0, 100))
                Instantiate(healthPick, cast.transform.position, Quaternion.Euler(0, 0, 90));

            anim.SetBool("Dead", true);
            UI.SetActive(false);
            GetComponent<Collider>().enabled = false;
            agent.enabled = false;

            gameManager.instance.enemiesToKill -= 1;

            if(gameObject.CompareTag("Boss Enemy"))
                gameManager.instance.bossKilled = true;
        }
    }

    void updateHPBar()
    {
        HPBar.fillAmount = (float)HP / (float)HPOrig;
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        model.material.color = origColor;
    }
    void snowEnemy()
    {
        if (gameManager.instance.playerScript.playerIsDead == true)
        {
            playerInRange = false;
        }
        if (!playerInRange)
        {
            agent.stoppingDistance = stopDistOrig;
            sightDist = sightDistOrig;

            if (snowLocation.Count != 0 && gameObject.tag == "Snow Enemy")
            {
                snowDir = snowLocation[0].transform.position - headPos.transform.position;

                agent.SetDestination(snowLocation[0].transform.position);

                distToSnow = Vector3.Distance(snowDir, transform.forward);

                Quaternion rotation = Quaternion.LookRotation(snowDir);

                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
            }
            else if (snowLocation.Count == 0 && gameObject.tag == "Snow Enemy")
            {
                snowDir = snowLocationTwo[0].transform.position - headPos.transform.position;

                agent.SetDestination(snowLocationTwo[0].transform.position);
                distToSnow = Vector3.Distance(snowDir, transform.forward);

                Quaternion rotation = Quaternion.LookRotation(snowDir);

                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
            }

            if (snowLocationTwo.Count != 0 && gameObject.tag == "Snow Enemy 2")
            {
                snowDir = snowLocationTwo[0].transform.position - headPos.transform.position;

                agent.SetDestination(snowLocationTwo[0].transform.position);

                distToSnow = Vector3.Distance(snowDir, transform.forward);

                Quaternion rotation = Quaternion.LookRotation(snowDir);

                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
            }
            else if (snowLocationTwo.Count == 0 && gameObject.tag == "Snow Enemy 2")
            {
                //gameObject.tag = "Snow Enemy";

                snowDir = snowLocation[0].transform.position - headPos.transform.position;

                agent.SetDestination(snowLocation[0].transform.position);

                distToSnow = Vector3.Distance(snowDir, transform.forward);

                Quaternion rotation = Quaternion.LookRotation(snowDir);

                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
            }
            else if (snowLocation.Count == 0 && snowLocationTwo.Count == 0)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
            }

            if (!isAttacking && distToSnow <= sightDist)
            {
                anim.SetTrigger("AttackSnow");
                anim.SetBool("IsAttacking", true);
            }
        }
        else if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);

            agent.stoppingDistance = 5;
            sightDist = 5;

            if (!isAttacking && distToPlayer <= sightDist)
            {
                anim.SetTrigger("AttackPlayer");
                anim.SetBool("IsAttacking", true);
            }
        }
    }

    void abilityUse()
    {
        if (Time.time - lastCast > attackRate)
        {
            abil = Random.Range(0, abilities.Count);

            switch (spikeState)
            {
                case abilityState.ready:
                    if (playerInRange && !isAttacking && abil == 0 && (gameObject.layer == LayerMask.NameToLayer("Boss Enemy") ||
                                                                    gameObject.layer == LayerMask.NameToLayer("Large Enemy")))
                    {
                        isAttacking = true;
                        abilities[0].Activate(cast);
                        spikeState = abilityState.active;
                        _spikeActiveTime = abilities[0].activeTime;
                        lastCast = Time.time;
                        //anim.SetBool("IsAttacking", false);
                        //anim.SetBool("IsAttacking", true);
                        //anim.SetTrigger("Spikes");
                    }
                    break;
                case abilityState.active:
                    if (_spikeActiveTime > 0)
                        _spikeActiveTime -= Time.deltaTime;
                    else
                    {
                        spikeState = abilityState.cooldown;
                        _spikeCooldownTime = abilities[0].cooldownTime;
                        isAttacking = false;
                    }
                    break;
                case abilityState.cooldown:
                    if (_spikeCooldownTime > 0)
                        _spikeCooldownTime -= Time.deltaTime;
                    else
                        spikeState = abilityState.ready;
                    break;
            }

            switch (fireballState)
            {
                case abilityState.ready:
                    if (playerInRange && !isAttacking && abil == 1 && (gameObject.layer == LayerMask.NameToLayer("Boss Enemy") ||
                                                                    gameObject.layer == LayerMask.NameToLayer("Medium Enemy")) && HP != 0)
                    {
                        anim.SetBool("IsAttacking", true);
                        anim.SetTrigger("Fire Ball");
                    }
                    break;
                case abilityState.active:
                    if (_fireballActiveTime > 0)
                        _fireballActiveTime -= Time.deltaTime;
                    else
                    {
                        fireballState = abilityState.cooldown;
                        _fireBallCooldownTime = abilities[1].cooldownTime;
                    }
                    break;
                case abilityState.cooldown:
                    if (_fireBallCooldownTime > 0)
                        _fireBallCooldownTime -= Time.deltaTime;
                    else
                        fireballState = abilityState.ready;
                    break;
            }

            switch (summonState)
            {
                case abilityState.ready:
                    if (playerInRange && !isAttacking && abil == 2 && (gameObject.layer == LayerMask.NameToLayer("Boss Enemy")))
                    {
                        isAttacking = true;
                        abilities[2].Activate(cast);
                        summonState = abilityState.active;
                        _summonActiveTime = abilities[abil].activeTime;
                        lastCast = Time.time;
                        //anim.SetBool("IsAttacking", false);
                        //anim.SetBool("IsAttacking", true);
                        //anim.SetTrigger("Spawner");
                    }
                    break;
                case abilityState.active:
                    if (_summonActiveTime > 0)
                        _summonActiveTime -= Time.deltaTime;

                    else
                    {
                        summonState = abilityState.cooldown;
                        _summonCooldownTime = abilities[2].cooldownTime;
                        isAttacking = false;
                    }
                    break;
                case abilityState.cooldown:
                    if (_summonCooldownTime > 0)
                        _summonCooldownTime -= Time.deltaTime;
                    else
                        summonState = abilityState.ready;
                    break;
            }
        }
    }

    IEnumerator attackSnow()
    {
        isAttacking = true;

        if (gameObject.tag == "Snow Enemy" && snowLocation.Count != 0)
        {
            snowLocation[0].damage(damage);
            aud.PlayOneShot(audAttack[1], audAttackVol);
        }
        else if (gameObject.tag == "Snow Enemy 2" && snowLocationTwo.Count != 0)
        {
            snowLocationTwo[0].damage(damage);
            aud.PlayOneShot(audAttack[1], audAttackVol);
        }

        yield return new WaitForSeconds(attackRate);

        anim.SetBool("IsAttacking", false);

        isAttacking = false;
    }
    IEnumerator attackPlayer()
    {
        isAttacking = true;

        aud.PlayOneShot(audAttack[1], audAttackVol);

        gameManager.instance.playerScript.damage(damage);

        yield return new WaitForSeconds(attackRate);

        anim.SetBool("IsAttacking", false);

        isAttacking = false;
    }

    void fireBall()
    {
        isAttacking = true;
        abilities[1].Activate(cast);
        aud.PlayOneShot(audAttack[0], audAttackVol);
        fireballState = abilityState.active;
        _fireballActiveTime = abilities[1].activeTime;
        lastCast = Time.time;
        anim.SetBool("IsAttacking", false);
        isAttacking = false;
    }
    void spike()
    {
        //isAttacking = true;
        //abilities[0].Activate(cast);
        //spikeState = abilityState.active;
        //_spikeActiveTime = abilities[0].activeTime;
        //lastCast = Time.time;
        //anim.SetBool("IsAttacking", false);
    }
    void summon()
    {
        //isAttacking = true;
        //abilities[2].Activate(cast);
        //summonState = abilityState.active;
        //_summonActiveTime = abilities[2].activeTime;
        //lastCast = Time.time;
        //anim.SetBool("IsAttacking", false);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        else if (other.CompareTag("Snow"))
        {
            anim.SetTrigger("Attack");
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

