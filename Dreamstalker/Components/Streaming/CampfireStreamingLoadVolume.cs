namespace Dreamstalker.Components.Streaming;

internal class CampfireStreamingLoadVolume : StreamingLoadVolume
{
	public Campfire campfire;

	protected override bool ShouldLoad => campfire?._state == Campfire.State.LIT;

	public void Start()
	{
		if (campfire != null) campfire.OnCampfireStateChange += OnCampfireStateChange;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (campfire != null) campfire.OnCampfireStateChange -= OnCampfireStateChange;
	}

	private void OnCampfireStateChange(Campfire campfire)
	{
		if (ShouldLoad) _playerInVolume = true;
	}
}
