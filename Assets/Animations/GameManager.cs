using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float initialSpeed = 8f;
    public float speedIncreaseRate = 0.15f;
    public float maxSpeed = 25f;

    [HideInInspector]
    public float currentSpeed;

    public float score = 0f;
    public int highScore = 0;

    public bool isGameOver = false;

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
        currentSpeed = initialSpeed;

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isGameOver) return;

        if (currentSpeed < maxSpeed)
        {
            currentSpeed += speedIncreaseRate * Time.deltaTime;
        }

        score += currentSpeed * Time.deltaTime * 2f;
    }

    public void AjouterScore(int points)
    {
        if (!isGameOver)
        {
            score += points;
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        currentSpeed = 0f;

        int finalScore = Mathf.FloorToInt(score);
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        Debug.Log("GAME OVER ! Score : " + finalScore + " | Record : " + highScore);
    }

    public void RecommencerPartie()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}