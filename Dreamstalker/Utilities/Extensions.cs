using UnityEngine;

namespace Dreamstalker.Utilities;

public static class Extensions
{
	public static GameObject InstantiateInactive(this GameObject original)
	{
		if (!original.activeSelf)
		{
			return UnityEngine.Object.Instantiate(original);
		}

		original.SetActive(false);
		var copy = UnityEngine.Object.Instantiate(original);
		original.SetActive(true);
		return copy;
	}

	public static string GetPath(this Transform current)
	{
		if (current.parent == null) return current.name;
		return current.parent.GetPath() + "/" + current.name;
	}
}
