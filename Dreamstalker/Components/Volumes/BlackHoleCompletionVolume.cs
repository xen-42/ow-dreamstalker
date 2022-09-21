using Dreamstalker.Components.Player;
using UnityEngine;

namespace Dreamstalker.Components.Volumes;

internal class BlackHoleCompletionVolume : CompletionVolume
{
	public override void OnTriggerEnter(Collider collider)
	{
		if (!enabled)
		{
			// If not enabled, kill player when they enter the black hole
			Locator.GetDeathManager().KillPlayer(DeathType.Impact);
			PlayerEffectController.Instance.PlayOneShot(AudioType.BH_BlackHoleEmission);
		}
		else
		{
			base.OnTriggerEnter(collider);
		}
	}
}
