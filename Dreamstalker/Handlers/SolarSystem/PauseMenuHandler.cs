using NewHorizons.Handlers;
using OWML.Common.Menus;
using OWML.ModHelper.Menus;
using System;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class PauseMenuHandler : BaseHandler
{
	private IModMessagePopup _currentPopup;

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
		var quitText = UITextLibrary.GetString(UITextType.PauseQuit).ToUpper();
		var quitButton = _main.ModHelper.Menus.PauseMenu.OptionsButton.Duplicate(quitText);
		quitButton.OnClick += OnQuitButtonClicked;

		var retryText = TranslationHandler.GetTranslation("DREAMSTALKER_RETRY_BUTTON", TranslationHandler.TextType.UI).ToUpper();
		var retryButton = _main.ModHelper.Menus.PauseMenu.OptionsButton.Duplicate(retryText);
		retryButton.OnClick += OnRetryButtonClicked;
	}

	private void OnRetryButtonClicked()
	{
		Main.Log($"Retry button clicked!");

		var retryText = TranslationHandler.GetTranslation("DREAMSTALKER_RETRY_TEXT", TranslationHandler.TextType.UI);
		MakePopup(retryText, RestartPopup_OnConfirm);
	}

	private void OnQuitButtonClicked()
	{
		Main.Log($"Quit button clicked!");

		var quitText = TranslationHandler.GetTranslation("DREAMSTALKER_QUIT_TEXT", TranslationHandler.TextType.UI);
		MakePopup(quitText, QuitPopup_OnConfirm);
	}

	private void MakePopup(string text, Action confirmAction)
	{
		ClosePopup();

		var okText = UITextLibrary.GetString(UITextType.MenuConfirm);
		var cancelText = UITextLibrary.GetString(UITextType.MenuCancel);

		_currentPopup = _main.ModHelper.Menus.PopupManager.CreateMessagePopup(text.ToUpper(), true, okText, cancelText);
		_currentPopup.Menu._addToMenuStackManager = true;
		_currentPopup.Menu.EnableMenu(true);
		(_currentPopup.Menu as PopupMenu)._popupCanvas.overrideSorting = true;
		_currentPopup.OnConfirm += confirmAction;
		_currentPopup.OnCancel += ClosePopup;
	}

	private void QuitPopup_OnConfirm()
	{
		LoadManager.LoadScene(OWScene.TitleScreen, LoadManager.FadeType.ToBlack, 1f, true);
	}

	private void RestartPopup_OnConfirm()
	{
		ClosePopup();
		Locator.GetDeathManager().KillPlayer(DeathType.Meditation);
		Locator.GetSceneMenuManager().pauseMenu._pauseMenu.EnableMenu(false);
	}

	private void ClosePopup()
	{
		if (_currentPopup != null)
		{
			// Thanks OWML for throwing exceptions when destroying the popup!
			_currentPopup.DestroySelf();
			_currentPopup = null;
		}
	}
}
