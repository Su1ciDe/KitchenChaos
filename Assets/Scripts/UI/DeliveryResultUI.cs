using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class DeliveryResultUI : BaseUI
	{
		[SerializeField] private Color successColor;
		[SerializeField] private Color failColor;
		[SerializeField] private Sprite successSprite;
		[SerializeField] private Sprite failSprite;
		[Multiline]
		[SerializeField] private string successText;
		[Multiline]
		[SerializeField] private string failText;

		[Space]
		[SerializeField] private Image backgroundImage;
		[SerializeField] private Image iconImage;
		[SerializeField] private TMP_Text txtMessage;

		private Animator animator;
		private static readonly int popup = Animator.StringToHash("Popup");

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
			DeliveryManager.Instance.OnRecipeFail += OnRecipeFailed;

			Hide();
		}

		private void OnRecipeSuccess(Vector3 position)
		{
			Show();
			backgroundImage.color = successColor;
			iconImage.sprite = successSprite;
			txtMessage.SetText(successText);
		}

		private void OnRecipeFailed(Vector3 position)
		{
			Show();
			backgroundImage.color = failColor;
			iconImage.sprite = failSprite;
			txtMessage.SetText(failText);
		}

		protected override void Show()
		{
			base.Show();
			animator.SetTrigger(popup);
		}
	}
}