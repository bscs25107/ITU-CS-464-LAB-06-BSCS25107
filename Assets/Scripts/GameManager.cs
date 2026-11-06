using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] int targetScore = 10, lives = 3;
    [SerializeField] float respawnDelay = 0.6f, spawnProtection = 1.5f;
    [SerializeField] float roundDuration = 60f;
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;
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

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            lives--;
            if (lives <= 0)
            {
                PlayerDied();
            }
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

    // Call this when the player reaches the goal zone/finish line
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
        if (lives <= 0) Lose();
        else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position;
        body.linearVelocity = Vector3.zero;
        player.Restore(spawnProtection);
        timeRemaining = roundDuration;
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