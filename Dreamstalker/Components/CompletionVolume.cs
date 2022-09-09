using Dreamstalker.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Components;

internal class CompletionVolume : MonoBehaviour
{
	public AstroObject.Name NextPlanet { set; private get; }

	private Campfire _campfire;

	public UnityEvent OnPlayerEnter = new();

	public void SetCampfire(Campfire campfire)
	{
		_campfire = campfire;

		campfire.OnCampfireStateChange += OnCampfireStateChange;
	}

	public void OnDestroy()
	{
		if (_campfire != null)
		{
			_campfire.OnCampfireStateChange -= OnCampfireStateChange;
		}
	}

	private void OnCampfireStateChange(Campfire campfire) =>
		enabled = campfire.GetState() == Campfire.State.LIT;

	public void OnTriggerEnter(Collider hitCollider)
	{
		if (hitCollider.attachedRigidbody.CompareTag("Player"))
		{
			OnPlayerEnter?.Invoke();
			PlayerEffectController.Instance.Blink(0f, 10f);
			PlayerSpawnUtil.SpawnAt(NextPlanet);
		}
	}
}
