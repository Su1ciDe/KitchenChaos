using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;
using Random = UnityEngine.Random;

namespace Network
{
	public class KitchenLobby : Singleton<KitchenLobby>
	{
		public Lobby Lobby => joinedLobby;
		private Lobby joinedLobby;

		public bool IsHost => joinedLobby is not null && joinedLobby.HostId.Equals(AuthenticationService.Instance.PlayerId);

		private const float heartbeatInterval = 15f;
		private WaitForSecondsRealtime waitHeartbeat;

		private const float lobbyListRefreshInterval = 1f;
		private WaitForSecondsRealtime waitLobbyRefresh;

		private const string JOIN_CODE_KEY = "RelayJoinCode";

		public event UnityAction OnCreatingLobby;
		public event UnityAction<string> OnCreatingLobbyFailed;
		public event UnityAction OnJoining;
		public event UnityAction<string> OnJoiningFailed;

		public event UnityAction<List<Lobby>> OnLobbyListChanged;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);

			InitAuth();

			waitHeartbeat = new WaitForSecondsRealtime(heartbeatInterval);
			waitLobbyRefresh = new WaitForSecondsRealtime(lobbyListRefreshInterval);
		}

		private void Start()
		{
			StartCoroutine(HandleHeartbeat());
		}

		private void OnDestroy()
		{
			StopCoroutine(HandleHeartbeat());
			StopCoroutine(RefreshLobbiesCoroutine());
		}

		private async void InitAuth()
		{
			if (UnityServices.State == ServicesInitializationState.Initialized) return;

			var initOptions = new InitializationOptions();
			initOptions.SetProfile(Random.Range(0, 10000).ToString());

			await UnityServices.InitializeAsync(initOptions);
			await AuthenticationService.Instance.SignInAnonymouslyAsync();

			StartCoroutine(RefreshLobbiesCoroutine());
		}

		private async void ListLobbies()
		{
			try
			{
				var queryLobbiesOptions = new QueryLobbiesOptions
				{
					Filters = new List<QueryFilter> { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) }
				};
				var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
				OnLobbyListChanged?.Invoke(queryResponse.Results);
			}
			catch (LobbyServiceException e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		private async Task<Allocation> AllocateRelay()
		{
			try
			{
				var allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.Instance.MAX_PLAYERS_COUNT - 1);
				return allocation;
			}
			catch (RelayServiceException e)
			{
				Console.WriteLine(e);
				return default;
			}
		}

		private async Task<string> GetRelayJoinCode(Allocation allocation)
		{
			try
			{
				var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
				return relayJoinCode;
			}
			catch (RelayServiceException e)
			{
				Console.WriteLine(e);
				return default;
			}
		}

		private async Task<JoinAllocation> JoinRelay(string joinCode)
		{
			try
			{
				var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
				return joinAllocation;
			}
			catch (RelayServiceException e)
			{
				Console.WriteLine(e);
				return default;
			}
		}

		public async void CreateLobby(string lobbyName, bool isPrivate)
		{
			OnCreatingLobby?.Invoke();

			try
			{
				joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.Instance.MAX_PLAYERS_COUNT,
					new CreateLobbyOptions { IsPrivate = isPrivate });

				var allocation = await AllocateRelay();
				var relayJoinCode = await GetRelayJoinCode(allocation);
				await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id,
					new UpdateLobbyOptions
					{
						Data = new Dictionary<string, DataObject> { { JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) } }
					});
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

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
				var relayJoinCode = joinedLobby.Data[JOIN_CODE_KEY].Value;
				var joinAllocation = await JoinRelay(relayJoinCode);
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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

				var relayJoinCode = joinedLobby.Data[JOIN_CODE_KEY].Value;
				var joinAllocation = await JoinRelay(relayJoinCode);
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

				KitchenGameMultiplayer.Instance.StartClient();
			}
			catch (LobbyServiceException e)
			{
				OnJoiningFailed?.Invoke(e.Message);

				Console.WriteLine(e);
				throw;
			}
		}

		public async void JoinById(string lobbyId)
		{
			OnJoining?.Invoke();

			try
			{
				joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

				var relayJoinCode = joinedLobby.Data[JOIN_CODE_KEY].Value;
				var joinAllocation = await JoinRelay(relayJoinCode);
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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

		private IEnumerator RefreshLobbiesCoroutine()
		{
			yield return new WaitUntil(() => AuthenticationService.Instance.IsSignedIn);
			while (joinedLobby is null && SceneManager.GetActiveScene().name.Equals(Loader.Scenes.LobbyScene.ToString()))
			{
				ListLobbies();
				yield return waitLobbyRefresh;
			}
		}
	}
}