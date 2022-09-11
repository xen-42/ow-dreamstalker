using Dreamstalker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dreamstalker.Components;

internal class DebugCommands : MonoBehaviour
{
	public void Update()
	{
		if (Main.DebugMode)
		{
			if (Keyboard.current[Key.L].isPressed)
			{
				if (Keyboard.current[Key.Numpad1].wasReleasedThisFrame) PlayerSpawnUtil.SpawnAt(AstroObject.Name.TimberHearth);
				if (Keyboard.current[Key.Numpad2].wasReleasedThisFrame) PlayerSpawnUtil.SpawnAt(AstroObject.Name.TowerTwin);
				if (Keyboard.current[Key.Numpad3].wasReleasedThisFrame) PlayerSpawnUtil.SpawnAt(AstroObject.Name.CaveTwin);
				if (Keyboard.current[Key.Numpad4].wasReleasedThisFrame) PlayerSpawnUtil.SpawnAt(AstroObject.Name.BrittleHollow);
				if (Keyboard.current[Key.Numpad5].wasReleasedThisFrame) PlayerSpawnUtil.SpawnAt(AstroObject.Name.GiantsDeep);
				if (Keyboard.current[Key.Numpad6].wasReleasedThisFrame) PlayerSpawnUtil.SpawnAt(AstroObject.Name.DarkBramble);
				if (Keyboard.current[Key.Numpad7].wasReleasedThisFrame) PlayerSpawnUtil.SpawnAt(AstroObject.Name.DreamWorld);
			} 
		}
	}
}
