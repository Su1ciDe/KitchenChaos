using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class NetcodeTestUI : MonoBehaviour
	{
		[SerializeField] private Button btnHost;
		[SerializeField] private Button btnClient;

		private void Awake()
		{
			btnHost.onClick.AddListener(HostStartClicked);
			btnClient.onClick.AddListener(ClientStartClicked);
		}

		private void HostStartClicked()
		{
			NetworkManager.Singleton.StartHost();
			Hide();
		}

		private void ClientStartClicked()
		{
			NetworkManager.Singleton.StartClient();
			Hide();
		}

		private void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}