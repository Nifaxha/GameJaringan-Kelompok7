using UnityEngine;
using TMPro;
using Unity.Netcode;

public class TerminalManager : NetworkBehaviour
{
    public static TerminalManager Instance { get; private set; }

    [Header("Terminal UI")]
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_InputField commandInput;

    // Menyesuaikan dengan gaya direktori username pada gambar referensi
    private string promptPrefix = @"C:\Users\Haris>"; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Terminal hanya berfungsi di layar Host (The Operator)
        {
            // Mendaftarkan event saat tombol "Enter" ditekan setelah mengetik
            commandInput.onSubmit.AddListener(OnCommandSubmitted);
            
            PrintLog("<color=yellow>[Terminal]</color> System connected. Waiting for Runner telemetry...");
            commandInput.ActivateInputField();
        }
    }

    // Fungsi untuk mencetak teks ke layar Terminal
    public void PrintLog(string message)
    {
        if (!IsServer || logText == null) return;
        logText.text += "\n" + message;
    }

    private void OnCommandSubmitted(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        // Cetak apa yang diketik Operator beserta usernamenya
        PrintLog($"{promptPrefix} {input}");

        // Proses logika perintahnya
        ProcessCommand(input.ToLower().Trim());

        // Kosongkan kolom input agar siap diketik kembali
        commandInput.text = "";
        commandInput.ActivateInputField();
    }

    private void ProcessCommand(string cmd)
    {
        if (cmd == "erase the laser")
        {
            EraseAllHazards();
            PrintLog("<color=green>[Terminal]</color> Executed: Laser obstacle erased from the system.");
        }
        else if (cmd == "open gate")
        {
            OpenGate();
            PrintLog("<color=green>[Terminal]</color> Access Granted: Gate opened.");
        }
        else if (cmd == "spawn platform")
        {
            SpawnPlatforms();
            PrintLog("<color=green>[Terminal]</color> Executed: Hologram platform activated.");
        }
        else
        {
            PrintLog($"<color=red>[Terminal]</color> Syntax Error: '{cmd}' is not recognized.");
        }
    }

    private void EraseAllHazards()
    {
        // Mencari semua objek ber-tag Hazard dan menghilangkannya dari server
        GameObject[] hazards = GameObject.FindGameObjectsWithTag("Hazard");
        foreach (GameObject hazard in hazards)
        {
            if (hazard.TryGetComponent(out NetworkObject netObj))
            {
                netObj.Despawn(); // Menghapus secara tersinkronisasi di jaringan
            }
        }
    }

    private void OpenGate()
    {
        // Mencari pintu besi (Gate) dan menghapusnya
        GameObject[] gates = GameObject.FindGameObjectsWithTag("Gate");
        foreach (GameObject gate in gates)
        {
            if (gate.TryGetComponent(out NetworkObject netObj))
            {
                netObj.Despawn(); 
            }
        }
    }

    private void SpawnPlatforms()
    {
        // Mencari semua platform hologram dan menyalakannya
        HologramPlatform[] platforms = FindObjectsByType<HologramPlatform>(FindObjectsSortMode.None);
        foreach (HologramPlatform platform in platforms)
        {
            platform.ActivatePlatform();
        }
    }
}