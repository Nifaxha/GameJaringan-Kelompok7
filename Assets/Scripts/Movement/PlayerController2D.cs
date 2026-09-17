using Unity.Netcode;
using UnityEngine;

public class PlayerController2D : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Mengatur warna indikator kepemilikan lokal menggunakan SpriteRenderer
        if (IsOwner)
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.green; // Player Lokal = Hijau

            // Mengatur posisi spawn acak pada bidang 2D (sumbu X dan Y)
            float randomX = Random.Range(-3f, 3f);
            float randomY = Random.Range(-3f, 3f);
            transform.position = new Vector3(randomX, randomY, 0f);
        }
        else
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.red;   // Player Lain = Merah
        }
    }

    private void Update()
    {
        // KUNCI UTAMA MULTIPLAYER:
        // Proses input dan pergerakan HANYA dijalankan oleh pemilik objek lokal
        if (!IsOwner) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        // Membaca input sumbu horizontal dan vertikal
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 moveDirection = new Vector2(horizontal, vertical).normalized;

        if (moveDirection.sqrMagnitude >= 0.01f)
        {
            // Memindahkan posisi karakter dalam koordinat 2D (World Space)
            transform.Translate((Vector3)moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}