using Unity.Netcode;
using UnityEngine;

public class Mainframe : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hanya Server yang berhak mengecek dan mengesahkan kemenangan
        if (!IsServer) return;

        // Jika yang menyentuh adalah The Runner (Client)
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.RunnerReachedMainframe();
        }
    }
}