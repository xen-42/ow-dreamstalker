using UnityEngine;

namespace Dreamstalker.Handlers;

[RequireComponent(typeof(Main))]
public class BaseHandler : MonoBehaviour
{
	protected Main _main;

	protected virtual void Awake()
	{
		_main = gameObject.GetRequiredComponent<Main>();
	}
}
