using UnityEngine;
using Unity.Netcode;

public class TerminalScanner : NetworkBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Hanya server (Terminal) yang mengolah peringatan ini

        // Jika The Runner melewati sensor ini
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            // Cetak peringatan ke Terminal milik The Operator
            TerminalManager.Instance.PrintLog("<color=red>[Terminal]</color> WARNING: Laser obstacle detected ahead!");
            hasTriggered = true; // Agar peringatan tidak spam berkali-kali
        }
    }
}