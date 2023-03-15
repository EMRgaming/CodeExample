using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Net;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;

    [Header("----- World Stuff -----")]
    public int WorldHP;

    [Header("----- UI -----")]
    public GameObject pauseMenu;
    public GameObject loseMenu;
    public GameObject playerDeadMenu;
    public GameObject winMenu;
    public GameObject nextLevelMenu;
    public GameObject playerDamageScreen;
    public GameObject audioMenu;
    public TextMeshProUGUI enemiesLeft;
    public Image playerHPBar;
    public Image playerHPBarAnim;
    public Image playerStamBar;
    public Image playerStamBarAnim;
    public Image WorldHPBar;
    [HideInInspector] public Image ability1;
    [HideInInspector] public Image ability2;
    [HideInInspector] public Image ability3;
    [SerializeField] Image mobileAbility1;
    [SerializeField] Image mobileAbility2;
    [SerializeField] Image mobileAbility3;
    [SerializeField] Image PCAbility1;
    [SerializeField] Image PCAbility2;
    [SerializeField] Image PCAbility3;
    public GameObject loadScreen;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioSource musicAud;
    public GameObject mobileUI;
    public GameObject PCUI;
    public GameObject Joystick;
    public GameObject MA1;
    public GameObject MA2;
    public GameObject MA3;

    [Header("---- Audio ----")]
    [SerializeField] AudioClip[] audClick;
    [Range(0, 1)][SerializeField] float audClickVol;
    [SerializeField] AudioClip[] audMusic;
    [Range(0, 1)][SerializeField] float audMusicVol;
    [SerializeField] AudioClip audWind;
    [Range(0, 1)][SerializeField] float audWindVol;


    public int enemiesToKill;
    public bool bossKilled;
    public GameObject spawnPos;
    bool isPlayingMusic;
    int worldHealthOrig;
    public float timer;
    public bool mobileBuild;
    public bool menuOpen;
    bool isPlayingWind;

    public bool isPaused;

    float HPTimer, StamTimer = 0f;

    public List<snowHealth> snowAreas = new List<snowHealth>();
    public List<snowHealth> snowAreasTwo = new List<snowHealth>();
    GameObject snow;

    // Start is called before the first frame update
    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            mobileBuild = true;
            mobileUI.SetActive(true);
            PCUI.SetActive(false);
            Joystick.SetActive(true);
            ability1 = mobileAbility1;
            ability2 = mobileAbility2;
            ability3 = mobileAbility3;
        }
        else if (Application.platform != RuntimePlatform.Android)
        {
            mobileBuild = false;
            mobileUI.SetActive(false);
            PCUI.SetActive(true);
            Joystick.SetActive(false);
            ability1 = PCAbility1;
            ability2 = PCAbility2;
            ability3 = PCAbility3;
        }

        instance = this;
        unPauseGame();
        bossKilled = false;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        spawnPos = GameObject.FindGameObjectWithTag("Spawn Pos");
        playerHPBar = instance.playerScript.HPBar;
        playerHPBarAnim = instance.playerScript.HPBarAnim;
        playerStamBar = instance.playerScript.StamBar;
        playerStamBarAnim = instance.playerScript.StamBarAnim;
    }

    private void Start()
    {
        loadScreen.SetActive(false);


        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Snow").Length; i++)
        {
            snow = GameObject.FindGameObjectsWithTag("Snow")[i];

            snowAreas.Add(snow.GetComponent<snowHealth>());
        }

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Snow 2").Length; i++)
        {
            snow = GameObject.FindGameObjectsWithTag("Snow 2")[i];

            snowAreasTwo.Add(snow.GetComponent<snowHealth>());
        }


        for (int i = 0; i < snowAreas.Count; i++)
        {
            WorldHP += snowAreas[i].HP;
        }
        for (int i = 0; i < snowAreasTwo.Count; i++)
        {
            WorldHP += snowAreas[i].HP;
        }

        worldHealthOrig = WorldHP;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !menuOpen/*!playerDeadMenu.activeSelf && !winMenu.activeSelf && !audioMenu.activeSelf && !loseMenu.activeSelf && !nextLevelMenu.activeSelf*/)
        {
            aud.PlayOneShot(audClick[Random.Range(0, audClick.Length)], audClickVol);
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);

            if (isPaused)
                pauseGame();
            else
                unPauseGame();
        }

        timer += Time.deltaTime;

        playMusic();

        backgroundWind();

        updateUIBars();

        if (WorldHP == 0 && SceneManager.GetActiveScene().buildIndex != 3)
        {
            youLose();
        }

        if (enemiesToKill == 0)
        {
            if(!bossKilled)
                Invoke("nextLevel", 1f);
            else if (bossKilled)
                Invoke("youWin", 1f);
        }
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuOpen = true;
    }

    public void unPauseGame()
    {
        menuOpen = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public IEnumerator playerDamageFlash()
    {
        playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerDamageScreen.SetActive(false);
    }

    public void youWin()
    {
        menuOpen = true;
        winMenu.SetActive(true);
        pauseGame();
    }

    public void nextLevel()
    {
        menuOpen = true;
        nextLevelMenu.SetActive(true);
        pauseGame();
    }

    public void youLose()
    {
        menuOpen = true;
        loseMenu.SetActive(true);
        pauseGame();
    }

    public void updateLevelHealth()
    {
        WorldHPBar.fillAmount = (float)WorldHP / (float)worldHealthOrig;
    }

    void playMusic()
    {
        if (!isPlayingMusic)
        {
            isPlayingMusic = true;

            musicAud.PlayOneShot(audMusic[Random.Range(0, audMusic.Length)], audMusicVol);
        }


        if (musicAud.isPlaying == false)
        {
            isPlayingMusic = false;
        }
    }
    void backgroundWind()
    {
        if (!isPlayingWind)
        {
            isPlayingWind = true;

            aud.PlayOneShot(audWind, audWindVol);
        }


        if (aud.isPlaying == false)
        {
            isPlayingWind = false;
        }
    }

    public void updateUIBars()
    {
        if (playerHPBarAnim.fillAmount != playerHPBar.fillAmount)
        {
            playerHPBarAnim.fillAmount = Mathf.Lerp(playerHPBarAnim.fillAmount, playerHPBar.fillAmount, HPTimer);
            HPTimer += 0.25f * Time.deltaTime;
        }
        else { HPTimer = 0f; }

        if (playerStamBarAnim.fillAmount != playerStamBar.fillAmount)
        {
            playerStamBarAnim.fillAmount = Mathf.Lerp(playerStamBarAnim.fillAmount, playerStamBar.fillAmount, StamTimer);
            StamTimer += 0.25f * Time.deltaTime;
        }
        else {StamTimer = 0f; }
    }

    public void changeSpawnPos(GameObject newSpawn)
    {
        gameManager.instance.spawnPos = newSpawn;
    }
}