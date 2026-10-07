using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour
{
    [Header("Runner Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    private Rigidbody2D rb;
    private float horizontalInput;

    [Header("Runner System")]
    private Vector3 currentCheckpoint; 

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody2D>();
        currentCheckpoint = transform.position;

        // ==========================================
        // PEMISAHAN PERAN (ASYMMETRIC CO-OP)
        // ==========================================
        if (IsServer && IsOwner) 
        {
            // --- POV THE OPERATOR (HOST) ---
            // 1. Matikan wujud fisik agar kapsul Host tidak ikut jatuh atau terkena laser
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            rb.simulated = false; // Mematikan gravitasi & fisika

            // 2. Munculkan layar CMD hitam menutupi seluruh layar
            if (GameManager.Instance != null && GameManager.Instance.terminalPanel != null)
            {
                GameManager.Instance.terminalPanel.SetActive(true);
            }
        }
        else if (!IsServer && IsOwner) 
        {
            // --- POV THE RUNNER (CLIENT) ---
            // 1. Wujud fisik tetap utuh untuk main parkour (Beri warna khusus jika mau)
            GetComponent<SpriteRenderer>().color = Color.yellow;
            
            // 2. Pastikan layar CMD mati agar dunia 2D terlihat jelas
            if (GameManager.Instance != null && GameManager.Instance.terminalPanel != null)
            {
                GameManager.Instance.terminalPanel.SetActive(false);
            }
        }
        else if (!IsServer && !IsOwner)
        {
            // Opsi: Sembunyikan wujud kapsul The Operator di layar The Runner 
            // agar The Runner tidak melihat ada kapsul "hantu" melayang di titik spawn
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void Update()
    {
        // ==========================================
        // LOGIKA KONTROL CLIENT (THE RUNNER)
        // ==========================================
        if (!IsOwner) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner || rb == null) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // --- MEKANIK HAZARD (SISTEM PENALTI WAKTU) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;

        if (collision.CompareTag("Hazard"))
        {
            HitHazardServerRpc(15f);
            RespawnRunner();
        }
    }

    [ServerRpc]
    private void HitHazardServerRpc(float timePenalty)
    {
        // Melaporkan penalti ke GameManager pusat
        GameManager.Instance.DeductTime(timePenalty);
    }

    private void RespawnRunner()
    {
        transform.position = currentCheckpoint;
        rb.linearVelocity = Vector2.zero; 
    }
}