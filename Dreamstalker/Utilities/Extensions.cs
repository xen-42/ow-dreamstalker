using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
