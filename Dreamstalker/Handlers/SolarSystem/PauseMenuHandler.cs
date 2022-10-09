using NewHorizons.Handlers;
using OWML.Common.Menus;
using OWML.ModHelper.Menus;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class PauseMenuHandler : BaseHandler
{
	private string _retryText;
	private string _okText;
	private string _cancelText;

	private IModMessagePopup _popup;

	protected override void Awake()
	{
		base.Awake();

		_main.ModHelper.Menus.PauseMenu.OnInit += OnPauseMenuInit;
	}

	private void OnDestroy()
	{
		if (_main != null)
		{
			_main.ModHelper.Menus.PauseMenu.OnInit -= OnPauseMenuInit;
		}
	}

	private void OnPauseMenuInit()
	{
		var text = TranslationHandler.GetTranslation("DREAMSTALKER_RETRYBUTTON", TranslationHandler.TextType.UI).ToUpper();
		var retryButton = _main.ModHelper.Menus.PauseMenu.OptionsButton.Duplicate(text);
		retryButton.OnClick += OnRetryButtonClicked;
	}

	private void OnRetryButtonClicked()
	{
		Main.Log($"Retry button clicked!");

		_retryText = TranslationHandler.GetTranslation("DREAMSTALKER_RETRYTEXT", TranslationHandler.TextType.UI);
		_okText = UITextLibrary.GetString(UITextType.MenuConfirm);
		_cancelText = UITextLibrary.GetString(UITextType.MenuCancel);

		ClosePopup();

		_popup = _main.ModHelper.Menus.PopupManager.CreateMessagePopup(_retryText, true, _okText, _cancelText);
		_popup.Menu._addToMenuStackManager = true;
		_popup.Menu.EnableMenu(true);
		(_popup.Menu as PopupMenu)._popupCanvas.overrideSorting = true;
		_popup.OnConfirm += Popup_OnConfirm;
		_popup.OnCancel += ClosePopup;
	}

	private void Popup_OnConfirm()
	{
		ClosePopup();
		Locator.GetDeathManager().KillPlayer(DeathType.Meditation);
		Locator.GetSceneMenuManager().pauseMenu._pauseMenu.EnableMenu(false);
	}

	private void ClosePopup()
	{
		if (_popup != null)
		{
			// Thanks OWML for throwing exceptions when destroying the popup!
			_popup.DestroySelf();
			_popup = null;
		}
	}
}
