using Dreamstalker.Utility;
using UnityEngine;

namespace Dreamstalker.Components.Volumes;

public class CompletionCampfire : MonoBehaviour
{
	private Campfire _campfire;
	private bool _playerSleepingHere;
	public AstroObject.Name destinationPlanet;

	public void Awake()
	{
		_campfire = gameObject.GetComponent<Campfire>();

		GlobalMessenger<bool>.AddListener("StartSleepingAtCampfire", OnStartSleepingAtCampfire);
		GlobalMessenger.AddListener("StopSleepingAtCampfire", OnStopSleepingAtCampfire);
	}

	public void OnDestroy()
	{
		GlobalMessenger<bool>.RemoveListener("StartSleepingAtCampfire", OnStartSleepingAtCampfire);
		GlobalMessenger.RemoveListener("StopSleepingAtCampfire", OnStopSleepingAtCampfire);
	}

	public void OnStopSleepingAtCampfire()
	{
		if (_playerSleepingHere)
		{
			PlayerSpawnUtil.SpawnAt(destinationPlanet);
		}

		_playerSleepingHere = false;
	}

	public void OnStartSleepingAtCampfire(bool _)
	{
		_playerSleepingHere = _campfire._isPlayerSleeping;

		// TODO: start loading in the destination planet
	}
}
