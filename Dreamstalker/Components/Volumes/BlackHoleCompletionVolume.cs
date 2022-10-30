namespace Dreamstalker.Components.Volumes;

internal class BlackHoleCompletionVolume : CompletionVolume
{
	public override void Start()
	{
		base.Start();
		if (_streamingSphere != null)
		{
			_streamingSphere.radius = 100f;
		}
		killWithoutLitCampfire = true;
	}

	public override void OnEnterEarly()
	{
		base.OnEnterEarly();
		PlayerEffectController.Instance.PlayOneShot(AudioType.BH_BlackHoleEmission);
	}
}
