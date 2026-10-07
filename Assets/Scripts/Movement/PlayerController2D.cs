using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController2D : NetworkBehaviour
{
    [Header("Network Sync Variables")]
    public NetworkVariable<int> health = new NetworkVariable<int>(100);
    public NetworkVariable<int> score = new NetworkVariable<int>(0);

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Combat Settings (Modul 5)")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem muzzleFlashVFX;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        health.OnValueChanged += OnHealthChanged;
        score.OnValueChanged += OnScoreChanged;

        UpdateHealthUI(health.Value);
        UpdateScoreUI(score.Value);

        if (IsOwner)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        health.OnValueChanged -= OnHealthChanged;
        score.OnValueChanged -= OnScoreChanged;
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        UpdateHealthUI(newValue);
    }

    private void OnScoreChanged(int previousValue, int newValue)
    {
        UpdateScoreUI(newValue);
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;
        health.Value = Mathf.Max(0, health.Value - amount);
    }

    private void UpdateHealthUI(int currentHealth)
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
    }

    private void UpdateScoreUI(int currentScore)
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
    }

    private void Update()
    {
        if (!IsOwner) return;

        // --- KODE PERGERAKAN 2D ---
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical"); 

        Vector3 moveDirection = new Vector3(moveX, moveY, 0).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Membalik arah hadap karakter menggunakan Rotasi (agar FirePoint ikut berputar 180 derajat)
        if (moveX > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Menghadap kanan
        }
        else if (moveX < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // Menghadap kiri
        }
        // --------------------------
        // -----------------------

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamageServerRpc(10);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            AddScoreServerRpc(5);
        }

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
        {
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void TakeDamageServerRpc(int damageAmount)
    {
        health.Value = Mathf.Max(0, health.Value - damageAmount);
    }

    [ServerRpc]
    private void AddScoreServerRpc(int scoreAmount)
    {
        score.Value += scoreAmount;
    }

   [ServerRpc]
    private void ShootServerRpc()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // --- TAMBAHAN BARU: Beritahu peluru siapa penembaknya ---
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.shooter2d = this;
        }
        // --------------------------------------------------------

        NetworkObject bulletNetworkObject = bulletInstance.GetComponent<NetworkObject>();
        bulletNetworkObject.Spawn();

        PlayShootEffectsClientRpc();
    }

    
    [ClientRpc]
    private void PlayShootEffectsClientRpc()
    {
        if (muzzleFlashVFX != null)
        {
            muzzleFlashVFX.Play();
        }
    }
}