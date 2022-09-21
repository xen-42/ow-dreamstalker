using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Utility;

internal static class StreamingGroups
{
	public static StreamingGroup Get(AstroObject.Name name)
	{
		if (name == AstroObject.Name.CaveTwin || name == AstroObject.Name.TowerTwin)
		{
			return GameObject.Find("FocalBody/StreamingGroup_HGT").GetComponent<StreamingGroup>();
		}
		else
		{
			return Locator.GetAstroObject(name).GetComponentInChildren<StreamingGroup>();
		}
	}
}
