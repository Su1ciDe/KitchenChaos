using Network;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelect
{
	public class CharacterColorSelectButtonUI : MonoBehaviour
	{
		[SerializeField] private Image image;
		[SerializeField] private GameObject selected;

		private int colorId;

		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(OnColorSelectClicked);
		}

		private void Start()
		{
			UpdateSelected();
		}

		private void OnEnable()
		{
			KitchenGameMultiplayer.Instance.OnPlayersDataChanged += OnPlayersDataChanged;
		}

		private void OnDisable()
		{
			KitchenGameMultiplayer.Instance.OnPlayersDataChanged -= OnPlayersDataChanged;
		}

		public void Init(int colorIndex)
		{
			colorId = colorIndex;
			image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);

			UpdateSelected();
		}

		private void OnPlayersDataChanged()
		{
			UpdateSelected();
		}

		private void OnColorSelectClicked()
		{
			KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
		}

		public void UpdateSelected()
		{
			selected.SetActive(KitchenGameMultiplayer.Instance.GetPlayerData().ColorId.Equals(colorId));
		}
	}
}