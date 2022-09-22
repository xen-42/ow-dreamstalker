using Dreamstalker.Handlers.EyeScene;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using UnityEngine;

namespace Dreamstalker.Components;

[RequireComponent(typeof(AstroObject))]
internal class EyeRotationController : MonoBehaviour
{
	private GameObject _eye;

	public void Awake()
	{
		var ao = gameObject.GetRequiredComponent<AstroObject>();
		_eye = DetailBuilder.Make(gameObject, ao._rootSector, EyeHandler.EyePrefab, new PropModule.DetailInfo() { keepLoaded = true });

		if (_eye == null)
		{
			enabled = false;
		}
	}

	public void Update()
	{
		var toPlayer = (Locator.GetPlayerTransform().position - _eye.transform.position).normalized;
		_eye.transform.rotation = Quaternion.FromToRotation(Vector3.up, toPlayer);
	}
}
