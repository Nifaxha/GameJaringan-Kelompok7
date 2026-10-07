using Unity.Netcode;
using UnityEngine;
using TMPro;

public class AsymmetricObstacle : NetworkBehaviour
{
    [Header("Visual References")]
    [SerializeField] private SpriteRenderer obstacleArt;
    [SerializeField] private GameObject warningUI; // Canvas yang berisi teks peringatan

    [Header("Client Proximity Settings")]
    [SerializeField] private float detectionRadius = 5f; // Jarak deteksi teks muncul
    private Transform clientPlayerTransform;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // ==========================================
            // POV HOST (THE OPERATOR):
            // Perlihatkan Art fisik, sembunyikan teks peringatan
            // ==========================================
            if (obstacleArt != null) obstacleArt.enabled = true;
            if (warningUI != null) warningUI.SetActive(false);
        }
        else
        {
            // ==========================================
            // POV CLIENT (THE RUNNER):
            // Sembunyikan Art fisik, sembunyikan teks awal
            // ==========================================
            if (obstacleArt != null) obstacleArt.enabled = false;
            if (warningUI != null) warningUI.SetActive(false);
        }
    }

    private void Update()
    {
        // Pengecekan jarak ini HANYA berjalan di layar Client
        if (IsServer) return;

        // Mencari objek The Runner (Client) jika belum ditemukan
        if (clientPlayerTransform == null)
        {
            if (NetworkManager.Singleton.SpawnManager != null && 
                NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null)
            {
                clientPlayerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
            }
            return; // Tunggu sampai The Runner ter-spawn
        }

        // Hitung jarak antara rintangan ini dengan The Runner
        float distance = Vector2.Distance(transform.position, clientPlayerTransform.position);

        // Jika The Runner masuk ke dalam radius, munculkan teks. Jika keluar, sembunyikan.
        if (warningUI != null)
        {
            warningUI.SetActive(distance <= detectionRadius);
        }
    }

    // Untuk memudahkan visualisasi radius deteksi di Unity Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}