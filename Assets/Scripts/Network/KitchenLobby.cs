using System;
using System.Collections;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Random = UnityEngine.Random;

namespace Network
{
	public class KitchenLobby : Singleton<KitchenLobby>
	{
		public Lobby Lobby => joinedLobby;
		private Lobby joinedLobby;

		public bool IsHost => joinedLobby is not null && joinedLobby.HostId.Equals(AuthenticationService.Instance.PlayerId);

		private const float heartbeatInterval = 15;
		private WaitForSecondsRealtime waitHeartbeat;

		public event UnityAction OnCreatingLobby;
		public event UnityAction<string> OnCreatingLobbyFailed;
		public event UnityAction OnJoining;
		public event UnityAction<string> OnJoiningFailed;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			InitAuth();

			waitHeartbeat = new WaitForSecondsRealtime(heartbeatInterval);
		}

		private void Start()
		{
			StartCoroutine(HandleHeartbeat());
		}

		private async void InitAuth()
		{
			if (UnityServices.State == ServicesInitializationState.Initialized) return;

			var initOptions = new InitializationOptions();
			initOptions.SetProfile(Random.Range(0, 10000).ToString());

			await UnityServices.InitializeAsync(initOptions);
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		public async void CreateLobby(string lobbyName, bool isPrivate)
		{
			OnCreatingLobby?.Invoke();

			try
			{
				joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.Instance.MAX_PLAYERS_COUNT,
					new CreateLobbyOptions { IsPrivate = isPrivate });

				KitchenGameMultiplayer.Instance.StartHost();
				Loader.LoadNetwork(Loader.Scenes.CharacterSelectScene);
			}
			catch (LobbyServiceException e)
			{
				OnCreatingLobbyFailed?.Invoke(e.Message);
				Console.WriteLine(e);
				throw;
			}
		}

		public async void QuickJoin()
		{
			OnJoining?.Invoke();
			try
			{
				joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

				KitchenGameMultiplayer.Instance.StartClient();
			}
			catch (LobbyServiceException e)
			{
				OnJoiningFailed?.Invoke(e.Message);
				Console.WriteLine(e);
				throw;
			}
		}

		public async void JoinByCode(string lobbyCode)
		{
			OnJoining?.Invoke();

			try
			{
				joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

				KitchenGameMultiplayer.Instance.StartClient();
			}
			catch (LobbyServiceException e)
			{
				OnJoiningFailed?.Invoke(e.Message);

				Console.WriteLine(e);
				throw;
			}
		}

		public async void DeleteLobby()
		{
			if (joinedLobby is null) return;

			try
			{
				await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
				joinedLobby = null;
			}
			catch (LobbyServiceException e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public async void LeaveLobby()
		{
			if (joinedLobby is null) return;

			try
			{
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
				joinedLobby = null;
			}
			catch (LobbyServiceException e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public async void KickPlayer(string playerId)
		{
			if (!IsHost) return;

			try
			{
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
				joinedLobby = null;
			}
			catch (LobbyServiceException e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private IEnumerator HandleHeartbeat()
		{
			while (IsHost)
			{
				yield return waitHeartbeat;

				LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
			}
		}
	}
}