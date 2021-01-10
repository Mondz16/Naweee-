using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject mainMenu;
    public GameObject inGame;
    public GameObject gameOver;
    public GameObject loadingScreen;
    public GameObject pauseGame;

    [Header("Difficulty")]
    public Sprite normalBtn;
    public Sprite hardBtn;
    public Sprite extremeBtn;
    public Image[] imgDifficultyBtns;

    [Header("Pre-Game")]
    public GameObject turboBoost;
    public GameObject slowmoBoost;
    public Slider effectTimeSlider;

    [Header("In-Game")]
    public Text scoreCounter;
    public Text coinCounter;

    public int score;
    public int coinsPerObjects;
    public int coinsInGame;
    public float exp;

    [Header("Post-Game")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI totalScoreHolder;
    public TextMeshProUGUI coinsIngameCollected;
    public TextMeshProUGUI coinsCollected;
    public TextMeshProUGUI totalCollected;
    public Slider experienceGainedSlider;
    public GameObject retryButton;
    public GameObject menuButton;

    public GameObject newHighScore;
    public int totalScore = 0;
    public int totalCoins = 0;
    public float totalExperienceGained = 0;
    public float totalExperienceLeft;
    [SerializeField]private float timeDelay;
    public Animator anim;

    #region Singleton
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instance = new GameObject("UIManager");
                instance.AddComponent<UIManager>();
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
        if (SceneManager.GetActiveScene().name == "Game")
        {
            mainMenu.SetActive(false);
            inGame.SetActive(true);
            totalExperienceLeft = 1000 + (1000 * (.05f * GameManager.Instance.playerData.level));

            totalScoreHolder.text = "0";
            coinsCollected.text = "0";
            coinsIngameCollected.text = "0";
            totalCollected.text = "0";
            experienceGainedSlider.maxValue = totalExperienceLeft;
            experienceGainedSlider.value = GameManager.Instance.playerData.experience;
        }
        else if (SceneManager.GetActiveScene().name == "Main")
        {
            mainMenu.SetActive(true);

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        if (GameManager.Instance.playerData.highScore >= 50)
                        {
                            imgDifficultyBtns[0].sprite = normalBtn;
                            imgDifficultyBtns[0].transform.GetChild(0).gameObject.SetActive(false);
                        }
                        break;
                    case 1:
                        if (GameManager.Instance.playerData.highScore >= 150)
                        {
                            imgDifficultyBtns[1].sprite = hardBtn;
                            imgDifficultyBtns[1].transform.GetChild(0).gameObject.SetActive(false);
                        }
                        break;
                    case 2:
                        if (GameManager.Instance.playerData.highScore >= 250)
                        {
                            imgDifficultyBtns[2].sprite = extremeBtn;
                            imgDifficultyBtns[2].transform.GetChild(0).gameObject.SetActive(false);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }


    private void Update()
    {
        if (Time.time >= 1)
            loadingScreen.GetComponent<Canvas>().sortingOrder = 0;

        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (isDead() == true)
            {
                inGame.SetActive(false);
                if (totalScore < score)
                {
                    StartCoroutine(TotalScore());
                }

                if (totalCoins < coinsPerObjects + coinsInGame)
                {
                    GameManager.Instance.audioCoin.volume = .5f;
                    GameManager.Instance.audioCoin.Play();
                    StartCoroutine(CoinsCollected());
                }

                if (exp >= 1)
                {
                    StartCoroutine(LevelExperienceGained());
                }

                StopAllCoroutines();
            }
        }
    }


    public void ScoreCount()
    {
        score = (int)Time.timeSinceLevelLoad - 5;
        scoreCounter.text = score.ToString();
    }

    public void ExpCount()
    {
        exp = score + ((coinsPerObjects + coinsInGame) / 2);
    }

    public bool isDead()
    {
        return GameManager.Instance.isDead;
    }

    public void ClickSoundeffect()
    {
        GameManager.Instance.audioCoin.clip = GameManager.Instance.audioClick;
        GameManager.Instance.audioCoin.Play();
    }

    public void chooseDifficulty(int i)
    {
        switch (i)
        {
            case 0:
                GameManager.Instance.playerData.userDifficulty = Difficulty.Easy;
                SceneManager.LoadScene("Game");
                break;
            case 1:
                if (GameManager.Instance.playerData.highScore >= 50)
                {
                    GameManager.Instance.playerData.userDifficulty = Difficulty.Normal;
                    SceneManager.LoadScene("Game");
                }
                break;
            case 2:
                if (GameManager.Instance.playerData.highScore >= 150)
                {
                    GameManager.Instance.playerData.userDifficulty = Difficulty.Hard;
                    SceneManager.LoadScene("Game");
                }
                break;
            case 3:
                if (GameManager.Instance.playerData.highScore >= 250)
                {
                    GameManager.Instance.playerData.userDifficulty = Difficulty.Extreme;
                    SceneManager.LoadScene("Game");
                }
                break;
            default:
                break;
        }
    }

    public void Pause(bool t)
    {
        if (t)
        {
            Time.timeScale = 0;
            pauseGame.SetActive(true);
        }
        else if (!t)
        {
            Time.timeScale = 1;
            pauseGame.SetActive(false);
        }
    }


    IEnumerator CoinsCollected()
    {
        while (true)
        {
            totalCoins += 1;
            coinsCollected.text = "+" + coinsPerObjects.ToString(); 
            coinsIngameCollected.text = "+" + coinsInGame.ToString();
            totalCollected.text = "+" + totalCoins.ToString();
            yield return new WaitForSeconds(10);
        }
    }

    IEnumerator TotalScore()
    {
        while (true)
        {
            totalScore += 1;
            totalScoreHolder.text = totalScore.ToString();
            if (totalScoreHolder.fontSize >= 130) totalScoreHolder.fontSize = 130;
            else totalScoreHolder.fontSize += .1f;

            yield return new WaitForSeconds(10);
        }
    }

    IEnumerator LevelExperienceGained()
    {
        while (true)
        {
            exp -= 1;
            totalExperienceGained += 1;

            if (GameManager.Instance.playerData.experience >= totalExperienceLeft) LevelUp();
            else
            {
                Debug.Log(exp);
                experienceGainedSlider.value += 1;
            }

            levelText.text = "Rank " + GameManager.Instance.playerData.level;

            yield return new WaitForSeconds(.5f);
        }
    }

    private void LevelUp()
    {
        experienceGainedSlider.value = totalExperienceGained;
        PlayerPrefs.SetFloat("Experience", totalExperienceGained);
        PlayerPrefs.SetFloat("Skillpoints", GameManager.Instance.playerData.skillPoints += 1);
        PlayerPrefs.SetInt("Level", GameManager.Instance.playerData.level += 1);
        GameManager.Instance.playerData.level = PlayerPrefs.GetInt("Level");
        GameManager.Instance.playerData.experience = PlayerPrefs.GetInt("Experience");
    }
}
