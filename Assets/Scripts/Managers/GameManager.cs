using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
	public class GameManager : NetworkBehaviour
	{
		public static GameManager Instance { get; private set; }

		public bool IsPlaying => state.Value == State.GamePlaying;
		public bool IsWaitingToStart => state.Value == State.WaitingToStart;
		public bool IsLocalPlayerReady { get; private set; }

		[SerializeField] private float gamePlayingTimerMax = 10;
		private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3);
		private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0);

		public enum State
		{
			WaitingToStart,
			CountdownToStart,
			GamePlaying,
			GameOver
		}

		private Dictionary<ulong, bool> playersReadyDictionary;
		private Dictionary<ulong, bool> playersPauseDictionary;

		private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);

		private bool isLocalGamePaused;
		private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

		public event UnityAction<State> OnStateChanged;
		public event UnityAction OnLocalGamePaused;
		public event UnityAction OnLocalGameUnpaused;
		public event UnityAction OnMultiplayerGamePaused;
		public event UnityAction OnMultiplayerGameUnpaused;
		public event UnityAction OnLocalPlayerReadyChanged;

		private void Awake()
		{
			Instance = this;

			playersReadyDictionary = new Dictionary<ulong, bool>();
			playersPauseDictionary = new Dictionary<ulong, bool>();
		}

		private void Start()
		{
			GameInput.OnPauseAction += OnPause;
			GameInput.OnInteractAction += OnInteractAction;
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			state.OnValueChanged += OnStateValueChanged;
			isGamePaused.OnValueChanged += OnPauseValueChanged;
			
			if(IsServer)
				NetworkManager.Singleton.OnClientDisconnectCallback +=OnClientDisconnected;
		}


		private void Update()
		{
			if (!IsServer) return;

			switch (state.Value)
			{
				case State.WaitingToStart:
					break;
				case State.CountdownToStart:
					countdownToStartTimer.Value -= Time.deltaTime;
					if (countdownToStartTimer.Value < 0f)
					{
						state.Value = State.GamePlaying;
						gamePlayingTimer.Value = gamePlayingTimerMax;
					}

					break;
				case State.GamePlaying:
					gamePlayingTimer.Value -= Time.deltaTime;
					if (gamePlayingTimer.Value < 0f)
					{
						state.Value = State.GameOver;
					}

					break;
				case State.GameOver:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnStateValueChanged(State previousValue, State newValue)
		{
			OnStateChanged?.Invoke(state.Value);
		}

		private void OnInteractAction(object sender, EventArgs e)
		{
			if (state.Value != State.WaitingToStart) return;

			IsLocalPlayerReady = true;
			OnLocalPlayerReadyChanged?.Invoke();

			SetPlayerReadyServerRpc();

			GameInput.OnInteractAction -= OnInteractAction;
		}

		[ServerRpc(RequireOwnership = false)]
		private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
		{
			// Handling the client id on server side to combat cheaters
			playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

			bool allClientsReady = true;
			foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				if (!playersReadyDictionary.ContainsKey(clientId) || !playersReadyDictionary[clientId])
				{
					// This player is not ready
					allClientsReady = false;
					break;
				}
			}

			if (allClientsReady)
				state.Value = State.CountdownToStart;
		}

		public float GetCountdownToStartTimer()
		{
			return countdownToStartTimer.Value;
		}

		public float GetGamePlayingTimerNormalized()
		{
			return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
		}

		private void OnPause(object sender, EventArgs e)
		{
			TogglePauseGame();
		}

		public void TogglePauseGame()
		{
			isLocalGamePaused = !isLocalGamePaused;
			if (isLocalGamePaused)
			{
				PauseGameServerRpc();
				OnLocalGamePaused?.Invoke();
			}
			else
			{
				UnpauseGameServerRpc();
				OnLocalGameUnpaused?.Invoke();
			}
		}

		private void OnPauseValueChanged(bool previousvalue, bool newvalue)
		{
			if (isGamePaused.Value)
			{
				Time.timeScale = 0;
				OnMultiplayerGamePaused?.Invoke();
			}
			else
			{
				Time.timeScale = 1;
				OnMultiplayerGameUnpaused?.Invoke();
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
		{
			playersPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
			TestGamePausedState();
		}

		[ServerRpc(RequireOwnership = false)]
		private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
		{
			playersPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
			TestGamePausedState();
		}

		private void TestGamePausedState()
		{
			foreach (var clientsId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				if (playersPauseDictionary.ContainsKey(clientsId) && playersPauseDictionary[clientsId])
				{
					isGamePaused.Value = true;
					return;
				}
			}

			// All players are unpaused
			isGamePaused.Value = false;
		}
		
		
		private void OnClientDisconnected(ulong clientId)
		{
			StartCoroutine(TestGamePausedStateCoroutine());
		}

		private IEnumerator TestGamePausedStateCoroutine()
		{
			yield return null;
			TestGamePausedState();
		}
	}
}