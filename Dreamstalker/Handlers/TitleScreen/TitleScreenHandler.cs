using Dreamstalker.Patches;
using NewHorizons.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Handlers.TitleScreen;

internal class TitleScreenHandler : BaseHandler
{
	protected override void Awake()
	{
		base.Awake();

		_main.TitleScreenAwake.AddListener(OnTitleScreenAwake);
	}

	protected void OnDestroy()
	{
		if (_main != null)
		{
			_main.TitleScreenAwake.RemoveListener(OnTitleScreenAwake);
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

		// Menu
		// When loading in we go to the Eye first to grab props
		Main.Instance.ModHelper.Events.Unity.RunWhen(PlayerData.IsLoaded, () =>
		{
			var newGame = SearchUtilities.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/MainMenuBlock/MainMenuLayoutGroup/Button-NewGame")?.GetComponent<SubmitActionLoadScene>();
			var resumeGame = SearchUtilities.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/MainMenuBlock/MainMenuLayoutGroup/Button-ResumeGame")?.GetComponent<SubmitActionLoadScene>();
			if (newGame != null) newGame._sceneToLoad = SubmitActionLoadScene.LoadableScenes.EYE;
			if (resumeGame != null) resumeGame._sceneToLoad = SubmitActionLoadScene.LoadableScenes.EYE;
		});
	}
}
