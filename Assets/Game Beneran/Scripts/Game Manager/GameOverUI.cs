using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        gameOverPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += HandleGameOver;
        }

        restartButton.onClick.AddListener(() =>
        {
            // Shutdown network koneksi dan muat ulang scene
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    private void HandleGameOver(bool isWin)
    {
        gameOverPanel.SetActive(true);

        if (isWin)
        {
            statusText.text = "SYSTEM RESTORED\n(Runner Escaped!)";
            statusText.color = Color.green;
        }
        else
        {
            statusText.text = "SYSTEM OVERLOAD\n(Time's Up)";
            statusText.color = Color.red;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= HandleGameOver;
        }
    }
}