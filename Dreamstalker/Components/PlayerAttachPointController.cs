using HarmonyLib;
using UnityEngine;

namespace Dreamstalker.Components;

[HarmonyPatch]
internal class PlayerAttachPointController : MonoBehaviour
{
	public static PlayerAttachPointController Instance { get; private set; }

	private PlayerAttachPoint _currentAttachPoint;

	private void Awake()
	{
		Instance = this;
		GlobalMessenger.AddListener("DetachPlayerFromPoint", OnDetachPlayerFromPoint);
	}

	private void OnDestroy()
	{
		GlobalMessenger.RemoveListener("DetachPlayerFromPoint", OnDetachPlayerFromPoint);
	}

	public void Detatch()
	{
		if (_currentAttachPoint != null)
		{
			var campfire = _currentAttachPoint?.transform?.parent?.GetComponentInChildren<Campfire>();
			if (campfire != null)
			{
				campfire.StopRoasting();
			}
			else
			{
				_currentAttachPoint.DetachPlayer();
			}
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PlayerAttachPoint), nameof(PlayerAttachPoint.AttachPlayer))]
	private static void PlayerAttachPoint_AttachPlayer(PlayerAttachPoint __instance)
	{
		if (Instance != null)
		{
			Instance._currentAttachPoint = __instance;
		}
	}

	private void OnDetachPlayerFromPoint() => _currentAttachPoint = null;

	public PlayerAttachPoint AttachPoint => _currentAttachPoint;
}
