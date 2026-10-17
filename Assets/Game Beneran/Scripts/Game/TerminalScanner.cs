using UnityEngine;
using Unity.Netcode;

public class TerminalScanner : NetworkBehaviour
{
    [Header("Scanner Settings")]
    [Tooltip("Pesan yang akan muncul di layar Operator")]
    public string warningMessage = "WARNING: Obstacle detected ahead!";
    
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (!hasTriggered && collision.CompareTag("Player"))
        {
            // Mencetak teks custom dari Inspector ke Terminal Operator
            TerminalManager.Instance.PrintLog($"<color=red>[Terminal]</color> {warningMessage}");
            hasTriggered = true; 
        }
    }
}