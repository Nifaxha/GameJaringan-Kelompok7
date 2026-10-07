using System;
using Unity.Netcode;
using UnityEngine;
using TMPro; // Wajib ditambahkan untuk memanggil UI TextMeshPro

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State
    {
        WaitingForPlayers,
        Countdown,
        Gameplay,
        GameOver
    }

    public NetworkVariable<State> gameState = new NetworkVariable<State>(State.WaitingForPlayers);
    public NetworkVariable<float> systemTimer = new NetworkVariable<float>(300f); 

    [Header("Match Settings")]
    [SerializeField] private float countdownTimer = 3f;

    // --- TAMBAHAN REFERENSI UI TIMER ---
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    public GameObject terminalPanel; 

    public event Action<State> OnGameStateChanged;
    public event Action<bool> OnGameOver; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        gameState.OnValueChanged += (oldState, newState) =>
        {
            OnGameStateChanged?.Invoke(newState);
        };

        // --- Mendaftarkan Listener Timer & Inisialisasi Teks ---
        systemTimer.OnValueChanged += OnTimerChanged;
        UpdateTimerUI(systemTimer.Value);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    // --- Mencabut Listener saat objek hancur (Mencegah Error) ---
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        systemTimer.OnValueChanged -= OnTimerChanged;
    }

    // --- Fungsi Pembaruan UI Timer ---
    private void OnTimerChanged(float previousValue, float newValue)
    {
        UpdateTimerUI(newValue);
    }

    private void UpdateTimerUI(float currentTime)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60F);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
            timerText.text = string.Format("System Overload: {0:00}:{1:00}", minutes, seconds);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            gameState.Value = State.Countdown;
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        switch (gameState.Value)
        {
            case State.Countdown:
                countdownTimer -= Time.deltaTime;
                if (countdownTimer <= 0f)
                {
                    gameState.Value = State.Gameplay;
                }
                break;

            case State.Gameplay:
                if (systemTimer.Value > 0)
                {
                    systemTimer.Value -= Time.deltaTime;
                }
                else
                {
                    TriggerGameOver(false); 
                }
                break;
        }
    }

    public void DeductTime(float penalty)
    {
        if (!IsServer || gameState.Value != State.Gameplay) return;
        systemTimer.Value = Mathf.Max(0, systemTimer.Value - penalty);
    }

    public void RunnerReachedMainframe()
    {
        if (!IsServer || gameState.Value != State.Gameplay) return;
        TriggerGameOver(true);
    }

    private void TriggerGameOver(bool isWin)
    {
        gameState.Value = State.GameOver;
        NotifyGameOverClientRpc(isWin);
    }

    [ClientRpc]
    private void NotifyGameOverClientRpc(bool isWin)
    {
        OnGameOver?.Invoke(isWin);
    }
}