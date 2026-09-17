using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Tambahkan ini untuk LoadSceneMode

public class NetworkManagerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton; //[cite: 2]
    [SerializeField] private Button clientButton; //[cite: 2]
    [SerializeField] private TextMeshProUGUI statusText; //[cite: 2]

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "GameplayScene"; // Ganti dengan nama scene gameplay Anda

    private void Awake()
    {
        // Menambahkan listener event pada tombol UI[cite: 2]
        hostButton.onClick.AddListener(OnHostButtonClicked); //[cite: 2]
        clientButton.onClick.AddListener(OnClientButtonClicked); //[cite: 2]
    }

    private void OnHostButtonClicked()
    {
        // Menjalankan fungsi StartHost dari NetworkManager NGO[cite: 2]
        if (NetworkManager.Singleton.StartHost()) //[cite: 2]
        {
            UpdateUIStatus("Status: Connected as HOST"); //[cite: 2]

            // Host berpindah scene menggunakan NetworkSceneManager. 
            // LoadSceneMode.Single akan mengganti scene saat ini (Main Menu) ke scene Gameplay.
            NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        }
        else
        {
            UpdateUIStatus("Status: Failed to Start Host"); //[cite: 2]
        }
    }
    
    private void OnClientButtonClicked()
    {
        // Menjalankan fungsi StartClient dari NetworkManager NGO[cite: 2]
        if (NetworkManager.Singleton.StartClient()) //[cite: 2]
        {
            UpdateUIStatus("Status: Connecting as CLIENT..."); //[cite: 2]
            
            // PENTING: Client TIDAK PERLU memanggil LoadScene. 
            // NGO akan secara otomatis menarik Client ini ke scene yang sedang dimainkan oleh Host.
        }
        else
        {
            UpdateUIStatus("Status: Failed to Start Client"); //[cite: 2]
        }
    }

    private void UpdateUIStatus(string message)
    {
        if (statusText != null) //[cite: 2]
        {
            statusText.text = message; //[cite: 2]
        }
        // Menyembunyikan tombol pilihan setelah role dipilih[cite: 2]
        hostButton.gameObject.SetActive(false); //[cite: 2]
        clientButton.gameObject.SetActive(false); //[cite: 2]
    }
}