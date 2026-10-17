using Unity.Netcode;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 2, -10); // Z harus -10 agar kamera berada di luar layar 2D
    private Transform target;

    void Update()
    {
        // 1. Jika ini adalah layar Operator (Server), hentikan seluruh logika kamera di sini.
        // Kamera Operator akan diam selamanya di titik awal.
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer) 
        {
            return; 
        }

        // 2. Jika target belum ditemukan di layar Runner (Client), cari objek karakternya
        if (target == null)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
            {
                var playerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                if (playerObj != null)
                {
                    // Jadikan karakter lokal sebagai target kamera
                    target = playerObj.transform;
                }
            }
            return; // Tunggu sampai target benar-benar ditemukan
        }

        // 3. Kamera mengikuti The Runner secara halus
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}