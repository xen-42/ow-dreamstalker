using Dreamstalker.Utility;

namespace Dreamstalker.Components;

internal class VanishController : VisibilityObject
{
	private bool _shouldVanish;

	public override void Awake()
	{
		base.Awake();

		_visibilityTrackers = new VisibilityTracker[]
		{
			VisibilityUtility.AddVisibilityTracker(transform, 1f)
		};
	}

	public override void Update()
	{
		base.Update();

		if (_shouldVanish && !IsVisible())
		{
			gameObject.SetActive(false);

			// In case something re-enables it
			_shouldVanish = false;
		}
	}

	public void QueueVanish()
	{
		_shouldVanish = true;
	}
}
