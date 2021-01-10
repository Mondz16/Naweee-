using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Extreme
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private Player player;
    public PlayerDataBank playerData;
    public bool isDead;
    public Transform spawnPointsHolder;

    public event Action onChallengeMOdeTriggerEvent;
    private event Action SaveTriggerEvent;
    public GameObject background;
    private GameObject obj;
    public Color[] backgroundColor;

    [Header("Game Audios")]
    public AudioSource audioSource;
    public AudioSource audioCoin;
    public AudioClip audioClick;
    public AudioClip audioGame;
    public AudioClip audioGameOver;

    #region Singleton
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instance = new GameObject();
                instance.AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private void Start()
    {
        playerData.experience = PlayerPrefs.GetFloat("Experience");
        playerData.highScore = PlayerPrefs.GetInt("Highscore");
        playerData.level = 1 + PlayerPrefs.GetInt("Level");
        if(PlayerPrefs.GetInt("Coins") < 0)
        {
            PlayerPrefs.SetInt("Coins", 0);
            playerData.coin = PlayerPrefs.GetInt("Coins");
        }
        else playerData.coin = PlayerPrefs.GetInt("Coins");

        if (SceneManager.GetActiveScene().name == "Game")
        {
            Debug.Log("GameManagerStart!");
            audioSource.clip = audioGame;
            audioSource.Play();
            isDead = false;
            Time.timeScale = 1;
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            isDead = false;

            //Set the difficulty of the level
            switch (playerData.userDifficulty)
            {
                case Difficulty.Easy:
                    ObjectHandler.Instance.speed = 3;
                    ObjectHandler.Instance.timer = .30f;
                    ObjectHandler.Instance.timeBetweenSpawn = .07f;
                    ObjectHandler.Instance.pool[0].size += 5;
                    break;
                case Difficulty.Normal:
                    ObjectHandler.Instance.speed = 3.5f;
                    ObjectHandler.Instance.timer = .25f;
                    ObjectHandler.Instance.timeBetweenSpawn = .06f;
                    ObjectHandler.Instance.pool[0].size += 10;
                    break;
                case Difficulty.Hard:
                    ObjectHandler.Instance.speed = 5;
                    ObjectHandler.Instance.timer = .20f;
                    ObjectHandler.Instance.timeBetweenSpawn = .05f;
                    ObjectHandler.Instance.pool[0].size += 15;
                    break;
                case Difficulty.Extreme:
                    ObjectHandler.Instance.speed = 5.5f;
                    ObjectHandler.Instance.timer = .15f;
                    ObjectHandler.Instance.timeBetweenSpawn = .04f;
                    ObjectHandler.Instance.pool[0].size += 20;
                    break;
                default:
                    break;
            }

            onChallengeMOdeTriggerEvent += RandomColors;
            onChallengeMOdeTriggerEvent += ChangeRotation;
            SaveTriggerEvent += SaveData;

            if (playerData.boosts[0].boostLevel != 0) UIManager.Instance.turboBoost.SetActive(true);
            else UIManager.Instance.turboBoost.SetActive(false);
        }
        else
        {
            audioSource.clip = audioGameOver;
            audioSource.Play();
        }
    }

    #region GameEvents

    //-----------------------EventTriggers---------------------------
    public void OnChangeRotationObjects() 
    { onChallengeMOdeTriggerEvent?.Invoke();}

    public void OnSaveEvent() { SaveTriggerEvent?.Invoke(); }

    //-----------------------EventListeners---------------------------
    private void RandomColors()
    {
        background.GetComponent<SpriteRenderer>().color =
            Color.Lerp(background.GetComponent<SpriteRenderer>().color,
            backgroundColor[UnityEngine.Random.Range(0, backgroundColor.Length)], .1f);
    }

    public void ChangeRotation()
    {
        if (ObjectHandler.Instance.ChangeRotation == 1)
        {
            obj.transform.rotation = Quaternion.Euler(0, 0, 10);
            spawnPointsHolder.position = new Vector3(spawnPointsHolder.position.x, 2.5f, 0);
        }
        else if (ObjectHandler.Instance.ChangeRotation == 2)
        {
            obj.transform.rotation = Quaternion.Euler(0, 0, -10);
            spawnPointsHolder.position = new Vector3(spawnPointsHolder.position.x, -2.5f, 0);
        }
        else if (ObjectHandler.Instance.ChangeRotation == 3)
        {
            obj.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-10 , 10 ));
            spawnPointsHolder.position = new Vector3(spawnPointsHolder.position.x, 
                UnityEngine.Random.Range(-2.5f, 2.5f), 0);
        }
        else if (ObjectHandler.Instance.ChangeRotation == 0)
        {
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);
            spawnPointsHolder.position = new Vector3(spawnPointsHolder.position.x, 0, 0);
        }
    }

    private void SaveData()
    {
        UIManager.Instance.loadingScreen.GetComponent<Canvas>().sortingOrder = 1;

        //Add the total coin to the players inventory coins
        PlayerPrefs.SetInt("Coins", playerData.coin += UIManager.Instance.totalCoins);
        playerData.coin = PlayerPrefs.GetInt("Coins");
        Debug.Log(playerData.coin.ToString());

        //Add the totalExp gained from the game to the players experience
        PlayerPrefs.SetFloat("Experience", playerData.experience += UIManager.Instance.totalExperienceGained);
        playerData.experience = PlayerPrefs.GetFloat("Experience");
    }

    #endregion

    #region GameMovements
    public void ObstacleMovement(float speedMultiplier , GameObject obj)
    {
        //Control the movement of the obstacle
        obj.SetActive(true);
        obj.transform.Translate((obj.transform.right * -1) * speedMultiplier * Time.deltaTime);
        this.obj = obj;
    }
    
    public void PlayerMovement(GameObject player, Quaternion rotation , Vector3 pos)
    {
        //Control the movement/rotation of the player
        player.transform.position = new Vector3(player.transform.position.x , player.transform.position.y + ((pos.y  * Time.deltaTime)) , 0);
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation , rotation , .8f);
    }

    #endregion

    #region GameOver
    public void GameOver()
    {
        StartCoroutine(GameOverTimer());
    }

    public IEnumerator GameOverTimer()
    {
        Time.timeScale = Mathf.Lerp(1, 0 , .8f);
        audioSource.volume = Mathf.Lerp(1, .5f, .1f);
        audioSource.clip = audioGameOver;
        audioSource.Play();
        //Check if the score is bigger than the highscore
        if (UIManager.Instance.score > playerData.highScore)
        {
            UIManager.Instance.newHighScore.SetActive(true);
            PlayerPrefs.SetInt("Highscore", UIManager.Instance.score);
            playerData.highScore = PlayerPrefs.GetInt("Highscore");
        }

        player.Dead();
        ObjectHandler.Instance.started = false;

        yield return new WaitForSeconds(.2f);

        //Trigger GameOverCanvas
        isDead = true;
        UIManager.Instance.gameOver.SetActive(true);

        yield return new WaitForSeconds(.5f);
        UIManager.Instance.retryButton.SetActive(true);
        UIManager.Instance.menuButton.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("isDead");
    }

    #endregion

    #region Powerups
    public void onPowerUps(string name)
    {
        UIManager.Instance.effectTimeSlider.gameObject.SetActive(true);

        if (name.ToLower() == "turbo") StartCoroutine(BoostTurbo(playerData.boosts[0].boostLevel));
        else if (name.ToLower() == "slowmo") StartCoroutine(SlowMotion(playerData.boosts[1].boostLevel));
    }

    IEnumerator BoostTurbo(int effectTime)
    {
        if (effectTime != 0) 
        {
            //Allow player to pass through objects
            PostProccesingEffect.Instance.ChangeVignetteColor(Color.red);
            player.tag = "Untagged";
            Time.timeScale += 10;
            audioSource.pitch = 1.5f;
            StartCoroutine(EffectTime(20 , 0));
            yield return new WaitForSeconds(effectTime * 20);
            PostProccesingEffect.Instance.ChangeVignetteColor(Color.black);
            UIManager.Instance.effectTimeSlider.gameObject.SetActive(false);
            audioSource.pitch = Mathf.Lerp(1.5f, 1, 8f);
            Time.timeScale = Mathf.Lerp(10, 1, .1f);
            Time.timeScale = 1;
            player.tag = "Player";
        }
    }

    private IEnumerator EffectTime(int effectTime, int powerUpID)
    {
        if (powerUpID == 0)
        {
            UIManager.Instance.effectTimeSlider.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = Color.red;
            UIManager.Instance.effectTimeSlider.maxValue = playerData.boosts[0].boostLevel * effectTime;
            UIManager.Instance.effectTimeSlider.value = playerData.boosts[0].boostLevel * effectTime;
            while (UIManager.Instance.effectTimeSlider.value >= 0)
            {
                UIManager.Instance.effectTimeSlider.value -= Time.deltaTime;
                yield return new WaitForSeconds(.1f);
            }
        }
        else if(powerUpID == 1)
        {
            PostProccesingEffect.Instance.ChangeVignetteColor(Color.blue);
            UIManager.Instance.effectTimeSlider.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = Color.blue;
            UIManager.Instance.effectTimeSlider.maxValue = playerData.boosts[1].boostLevel * effectTime;
            UIManager.Instance.effectTimeSlider.value = playerData.boosts[1].boostLevel * effectTime;
            while (UIManager.Instance.effectTimeSlider.value >= 0)
            {
                UIManager.Instance.effectTimeSlider.value -= .1f;
                yield return new WaitForSeconds(.1f);
            }
        }
    }

    IEnumerator SlowMotion(int effectTime)
    {
        if (effectTime != 0)
        {
            PostProccesingEffect.Instance.ChangeVignetteColor(Color.blue);
            Time.timeScale = .3f;
            audioSource.pitch = Mathf.Lerp(1, .3f, 8f);
            audioSource.pitch = .3f;
            StartCoroutine(EffectTime(1 , 1));
            yield return new WaitForSeconds(effectTime * 1);
            PostProccesingEffect.Instance.ChangeVignetteColor(Color.black);
            UIManager.Instance.effectTimeSlider.gameObject.SetActive(false);
            audioSource.pitch = Mathf.Lerp(.3f, 1, 8f);
            Time.timeScale = Mathf.Lerp(.3f, 1, .8f);
            Time.timeScale = 1;
        }
    }
    #endregion
}
