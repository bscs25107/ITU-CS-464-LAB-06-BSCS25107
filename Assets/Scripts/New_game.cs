using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class New_game : MonoBehaviour
{
    public static New_game Instance { get; private set; }

    [Header("Survival Settings")]
    [SerializeField] int lives = 3;
    [SerializeField] float roundDuration = 40f; // 40 seconds survival target
    [SerializeField] int targetScore = 10;

    [Header("Respawn & Player")]
    [SerializeField] float respawnDelay = 0.6f;
    [SerializeField] float spawnProtection = 1.5f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;

    [Header("UI & Panels")]
    [SerializeField] TMP_Text scoreText, livesText, timerText;
    [SerializeField] GameObject winPanel, losePanel;

    int score;
    float timeRemaining;
    bool isGameOver;

    void Awake() => Instance = this;

    void Start()
    {
        timeRemaining = roundDuration;
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        timeRemaining -= Time.deltaTime;

        // If the player survives until the time runs out, they WIN
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            Win();
        }

        UpdateTimerUI();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        UpdateUI();
        if (score >= targetScore) Win();
    }

    public void ReachGoal()
    {
        if (isGameOver) return;
        Win();
    }

    public void UpdateUI()
    {
        livesText.text = $"Lives: {lives}  Health: {Mathf.Max(player.Current, 0)} / {player.Max}";
        scoreText.text = $"Score: {score} / {targetScore}";
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    public void PlayerDied()
    {
        if (isGameOver) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            Lose();
        }
        else
        {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position;
        body.linearVelocity = Vector3.zero;
        player.Restore(spawnProtection);
    }

    public void Win()
    {
        if (isGameOver) return;
        isGameOver = true;
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        if (isGameOver) return;
        isGameOver = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}