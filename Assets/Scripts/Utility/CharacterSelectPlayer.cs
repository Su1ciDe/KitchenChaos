using Gameplay;
using Managers;
using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
	public class CharacterSelectPlayer : MonoBehaviour
	{
		[SerializeField] private int playerIndex;
		[Space]
		[SerializeField] private TextMeshPro txtPlayerName;
		[SerializeField] private TextMeshPro txtReady;
		[SerializeField] private Button btnKick;

		private PlayerVisual playerVisual;

		private void Awake()
		{
			playerVisual = GetComponentInChildren<PlayerVisual>();
			btnKick.onClick.AddListener(Kick);
		}

		private void Kick()
		{
			var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
			KitchenLobby.Instance.KickPlayer(playerData.PlayerId.ToString());
			KitchenGameMultiplayer.Instance.KickPlayer(playerData.ClientId);
		}

		private void Start()
		{
			btnKick.gameObject.SetActive(NetworkManager.Singleton.IsServer);

			UpdatePlayer();
			CharacterSelectManager.Instance.OnReadyChanged += OnPlayerReadyChanged;
		}

		private void OnEnable()
		{
			KitchenGameMultiplayer.Instance.OnPlayersDataChanged += OnPlayersDataChanged;
		}

		private void OnDestroy()
		{
			KitchenGameMultiplayer.Instance.OnPlayersDataChanged -= OnPlayersDataChanged;
		}

		private void OnPlayersDataChanged()
		{
			UpdatePlayer();
		}

		private void OnPlayerReadyChanged()
		{
			UpdatePlayer();
		}

		private void UpdatePlayer()
		{
			if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
			{
				Show();

				var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
				txtReady.gameObject.SetActive(CharacterSelectManager.Instance.IsPlayerReady(playerData.ClientId));
				playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.ColorId));
				txtPlayerName.SetText(playerData.PlayerName.ToString());
			}
			else
				Hide();
		}

		private void Show()
		{
			gameObject.SetActive(true);
		}

		private void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}