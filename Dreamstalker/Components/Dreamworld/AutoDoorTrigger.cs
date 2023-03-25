﻿using UnityEngine;

namespace Dreamstalker.Components.Dreamworld;

internal class AutoDoorTrigger : MonoBehaviour
{
	private OWTriggerVolume _trigger;
	private RotatingDoor _door;

	public void Start()
	{
		_trigger = gameObject.GetComponent<OWTriggerVolume>();
		_door = gameObject.GetComponentInParent<RotatingDoor>();

		_trigger.OnEntry += OnEntry;
		_trigger.OnExit += OnExit;
	}

	private void OnDestroy()
	{
		if (_trigger != null)
		{
			_trigger.OnEntry -= OnEntry;
			_trigger.OnExit -= OnExit;
		}
	}

	private void OnEntry(GameObject hitObject)
	{
		if (hitObject.CompareTag("PlayerDetector"))
		{
			Main.Log("Player entered auto door");
			_door.Open();
		}
	}

	private void OnExit(GameObject hitObject)
	{
		if (hitObject.CompareTag("PlayerDetector"))
		{
			Main.Log("Player exited auto door");
			_door.Close();
		}
	}
}
