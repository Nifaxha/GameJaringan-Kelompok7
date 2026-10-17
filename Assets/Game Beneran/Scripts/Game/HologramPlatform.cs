using Unity.Netcode;
using UnityEngine;

public class HologramPlatform : NetworkBehaviour
{
    // Variabel jaringan: False = Hilang/Tak bisa diinjak, True = Muncul/Padat
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false);

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public override void OnNetworkSpawn()
    {
        // Sinkronisasi status jika operator mengetik perintah
        isActive.OnValueChanged += (oldVal, newVal) => UpdatePlatformState(newVal);
        UpdatePlatformState(isActive.Value); // Sembunyikan saat awal game
    }

    private void UpdatePlatformState(bool state)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = state;
        if (boxCollider != null) boxCollider.enabled = state;
    }

    // Dipanggil oleh TerminalManager
    public void ActivatePlatform()
    {
        if (IsServer) isActive.Value = true;
    }
}