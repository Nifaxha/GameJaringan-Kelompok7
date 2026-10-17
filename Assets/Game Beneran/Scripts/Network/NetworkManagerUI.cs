using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject lobbyPanel; // Panel background yang menutupi layar game

    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Awake()
    {
        // Memastikan menu utama menyala saat game pertama kali dibuka
        if (lobbyPanel != null) lobbyPanel.SetActive(true);

        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
    }

    private void OnHostButtonClicked()
    {
        statusText.text = "> INITIALIZING SYSTEM AS OPERATOR...";
        statusText.color = Color.yellow;
        DisableButtons();

        if (NetworkManager.Singleton.StartHost())
        {
            // Berhasil start host, sembunyikan seluruh layar lobby
            if (lobbyPanel != null) lobbyPanel.SetActive(false);
        }
        else
        {
            statusText.text = "> <color=red>FATAL ERROR: Address already in use.</color>";
            EnableButtons();
        }
    }

    private void OnClientButtonClicked()
    {
        statusText.text = "> ESTABLISHING CONNECTION AS RUNNER...";
        statusText.color = Color.yellow;
        DisableButtons();

        if (NetworkManager.Singleton.StartClient())
        {
            // Berhasil connect, sembunyikan seluruh layar lobby
            if (lobbyPanel != null) lobbyPanel.SetActive(false);
        }
        else
        {
            statusText.text = "> <color=red>CONNECTION REFUSED: Server not found.</color>";
            EnableButtons();
        }
    }

    private void DisableButtons()
    {
        hostButton.interactable = false;
        clientButton.interactable = false;
    }

    private void EnableButtons()
    {
        hostButton.interactable = true;
        clientButton.interactable = true;
    }
}