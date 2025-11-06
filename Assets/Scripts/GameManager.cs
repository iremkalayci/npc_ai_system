using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elemanları")]
    public Text timerText;
    public Text checkpointText;
    public GameObject winPanel;
    public GameObject gameOverPanel;

    [Header("Zaman Ayarları")]
    public float totalTime = 180f; // 3 dakika
    private float currentTime;
    private bool gameEnded = false;

    [Header("Checkpoint Ayarları")]
    public Transform player;
    public Transform[] checkpoints; // CheckPoint1, 2, 3, Final
    private int currentCheckpointIndex = 0;
    private Vector3 lastCheckpointPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        currentTime = totalTime;
        UpdateCheckpointUI();
        lastCheckpointPosition = player.position;
        HideEndPanels();
    }

    private void Update()
    {
        if (gameEnded) return;

        // Süre geri sayımı
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            GameOver();
        }
        UpdateTimerUI();
    }

    public void PlayerDied()
    {
        // Oyuncu öldüğünde son checkpoint pozisyonuna dön
        player.position = lastCheckpointPosition;
    }

    public void CollectCheckpoint(GameObject checkpoint)
    {
        // Sırayla alınması gereken checkpoint kontrolü
        if (checkpoint == checkpoints[currentCheckpointIndex].gameObject)
        {
            Debug.Log($"Checkpoint {currentCheckpointIndex + 1} alındı!");
            checkpoint.SetActive(false);

            lastCheckpointPosition = checkpoint.transform.position;
            currentCheckpointIndex++;
            UpdateCheckpointUI();

            // Final checkpoint alındıysa kazan
            if (currentCheckpointIndex == checkpoints.Length)
            {
                WinGame();
            }
        }
        else
        {
            Debug.Log("🚫 Sıradaki checkpoint bu değil!");
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void UpdateCheckpointUI()
    {
        checkpointText.text = $"Checkpoint: {currentCheckpointIndex}/{checkpoints.Length}";
    }

    private void HideEndPanels()
    {
        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void WinGame()
    {
        gameEnded = true;
        winPanel.SetActive(true);
        Debug.Log("🏆 Kazandın!");
    }

    public void GameOver()
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);
        Debug.Log("💀 Süre doldu! Game Over!");
    }
}
