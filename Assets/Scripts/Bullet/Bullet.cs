using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float destroyTime = 3f;

    // Menyimpan data siapa yang menembakkan peluru ini
    public PlayerController shooter;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            Invoke(nameof(DestroyBullet), destroyTime);
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); // Vector3.right untuk 2D
    }

    // ==========================================
    // JIKA KAMU MENGGUNAKAN COLLIDER 2D (RIGIDBODY2D, BOXCOLLIDER2D)
    // ==========================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController targetPlayer))
        {
            // Jangan sampai peluru melukai diri sendiri
            if (targetPlayer == shooter) return;

            // Kurangi HP musuh
            targetPlayer.TakeDamage(damageAmount);

            // Tambahkan skor ke penembak (misal +10 skor per hit)
            if (shooter != null)
            {
                shooter.score.Value += 10; 
            }

            DestroyBullet();
        }
    }

    // ==========================================
    // JIKA KAMU MENGGUNAKAN COLLIDER 3D BIASA (RIGIDBODY, BOXCOLLIDER)
    // ==========================================
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<PlayerController>(out PlayerController targetPlayer))
        {
            if (targetPlayer == shooter) return;

            targetPlayer.TakeDamage(damageAmount);

            if (shooter != null)
            {
                shooter.score.Value += 10;
            }

            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        if (IsServer && NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
    }
}   