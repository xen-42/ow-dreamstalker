using Dreamstalker.Patches;
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
	}
}
