using UnityEngine;

namespace Dreamstalker.Handlers.Credits;

internal class DreamstalkerCreditsHandler : BaseHandler
{
	public static bool flagWon;

	protected override void Awake()
	{
		base.Awake();

		_main.FastCredits.AddListener(OnFastCredits);
	}

	private void OnDestroy()
	{
		if (_main != null)
		{
			_main.FastCredits.RemoveListener(OnFastCredits);
		}
	}

	private void OnFastCredits()
	{
		if (flagWon)
		{
			GameObject.Find("AudioSource").GetComponent<OWAudioSource>().PlayOneShot(AudioType.SadNomaiTheme, 1f);
		}

		flagWon = false;
	}
}
