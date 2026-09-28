using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static bool recommencerDirectement = false;

    [Header("Vitesse du Jeu")]
    public float initialSpeed = 8f;
    public float speedIncreaseRate = 0.15f;
    public float maxSpeed = 25f;
    [HideInInspector] public float currentSpeed;

    [Header("Panneaux UI")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject scoreEnJeuHUD;

    [Header("Textes TextMeshPro")]
    public TextMeshProUGUI scoreEnJeuText;
    public TextMeshProUGUI menuHighScoreText;
    public TextMeshProUGUI scoreFinalText;
    public TextMeshProUGUI gameOverHighScoreText;

    [Header("États du Jeu")]
    public bool isPlaying = false;
    public bool isGameOver = false;

    public float score = 0f;
    private int highScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (recommencerDirectement)
        {
            recommencerDirectement = false;
            LancerPartie();
        }
        else
        {
            Time.timeScale = 0f;
            isPlaying = false;
            isGameOver = false;

            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (scoreEnJeuHUD != null) scoreEnJeuHUD.SetActive(false);

            if (menuHighScoreText != null)
            {
                menuHighScoreText.text = "Meilleur Score : " + highScore;
            }
        }
    }

    private void Update()
    {
        if (!isPlaying || isGameOver) return;

        if (currentSpeed < maxSpeed)
        {
            currentSpeed += speedIncreaseRate * Time.deltaTime;
        }

        score += currentSpeed * Time.deltaTime * 2f;

        if (scoreEnJeuText != null)
        {
            scoreEnJeuText.text = "Score : " + Mathf.FloorToInt(score);
        }
    }

    public void AjouterScore(int points)
    {
        if (isPlaying && !isGameOver)
        {
            score += points;
            if (scoreEnJeuText != null)
            {
                scoreEnJeuText.text = "Score : " + Mathf.FloorToInt(score);
            }

            string sceneActuelle = SceneManager.GetActiveScene().name;

            if (score >= 1000 && sceneActuelle == "Niveau1")
            
            {
                ChangerVersNiveau2();
            }
        }
    } 

    private void ChangerVersNiveau2()
    {
        SceneManager.LoadScene("Niveau2");
    }


    public void LancerPartie()
    {
        isPlaying = true;
        isGameOver = false;
        score = 0f;
        currentSpeed = initialSpeed;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (scoreEnJeuHUD != null) scoreEnJeuHUD.SetActive(true);

        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        isPlaying = false;
        currentSpeed = 0f;

        int finalScore = Mathf.FloorToInt(score);

        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (scoreEnJeuHUD != null) scoreEnJeuHUD.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (scoreFinalText != null)
        {
            scoreFinalText.text = "Score : " + finalScore;
        }

        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "Record : " + highScore;
        }

        Time.timeScale = 0f;
    }

    public void RecommencerPartie()
    {
        recommencerDirectement = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RetournerAuMenu()
    {
        recommencerDirectement = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitterJeu()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}