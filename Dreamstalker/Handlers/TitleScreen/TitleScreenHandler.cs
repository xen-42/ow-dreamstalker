using UnityEngine;

namespace Dreamstalker.Handlers.TitleScreen;

internal class TitleScreenHandler : BaseHandler
{
	protected override void Awake()
	{
		base.Awake();

		_main.TitleScreenAwake.AddListener(OnTitleScreenAwake);
		_main.ModHelper.Menus.MainMenu.OnInit += OnMainMenuInit;
	}

	protected void OnDestroy()
	{
		if (_main != null)
		{
			_main.TitleScreenAwake.RemoveListener(OnTitleScreenAwake);
			_main.ModHelper.Menus.MainMenu.OnInit -= OnMainMenuInit;
		}
	}

	private void OnTitleScreenAwake()
	{
		// Sounds
		var music = GameObject.Find("AudioSource_Music").GetComponent<OWAudioSource>();
		music.AssignAudioLibraryClip(AudioType.DreamRuinsBaseTrack);

		var ambience = GameObject.Find("AudioSource_Ambience").GetComponent<OWAudioSource>();
		ambience.AssignAudioLibraryClip(AudioType.EyeAmbience_LP);

		// Props
		GameObject.Find("Scene/Background/PlanetPivot/PlanetRoot/Props/Structure_HEA_PlayerShip_v4_NearProxy").SetActive(false);
		GameObject.Find("Scene/Background/PlanetPivot/PlanetRoot/Traveller_HEA_Riebeck (1)").SetActive(false);
	}

	private void OnMainMenuInit()
	{
		/*
		var startButton = Main.Instance.ModHelper.Menus.MainMenu.ResumeExpeditionButton ?? Main.Instance.ModHelper.Menus.MainMenu.NewExpeditionButton;
		startButton.Duplicate(TranslationHandler.GetTranslation("DREAMSTALKER_STARTBUTTON", TranslationHandler.TextType.UI).ToUpper(), 3);

		_main.ModHelper.Events.Unity.FireOnNextUpdate(() =>
		{
			GameObject.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/MainMenuBlock/MainMenuLayoutGroup/Button-ResumeGame")?.SetActive(false);
			GameObject.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/MainMenuBlock/MainMenuLayoutGroup/Button-NewGame")?.SetActive(false);
		});
		*/
	}
}
