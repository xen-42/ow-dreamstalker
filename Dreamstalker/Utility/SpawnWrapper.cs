using Dreamstalker.Components.Dreamstalker;
using Dreamstalker.Components.Volumes;
using NewHorizons.Utility;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Utility;

internal static class SpawnWrapper
{
	public static string GhostBirdPath => "DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_2/Ghosts_DreamZone_2/GhostNodeMap_HornetHouse/Prefab_IP_GhostBird_Hornet/Ghostbird_IP_ANIM";
	public static string MummyPath => "RingWorld_Body/Sector_RingInterior/Sector_Zone2/Sector_DreamFireLighthouse_Zone2_AnimRoot/Interactibles_DreamFireLighthouse_Zone2/DreamFireChamber/MummyCircle/MummyPivot (HORNET)/Mummy_IP_Anim";
	public static string SkeletonPath => "QuantumMoon_Body/Sector_QuantumMoon/State_DB/Interactables_DBState/QuantumDeadNomaiSuit/State_4/Prefab_NOM_Dead_Suit_GroundC/Character_NOM_Dead_Suit";

	private static GameObject _dreamstalkerPrefab;

	public static GameObject Spawn(AstroObject planet, string path, Vector3 position)
	{
		var go = Main.Instance.NewHorizonsAPI.SpawnObject(planet.gameObject, planet.GetRootSector(), path, position, Vector3.zero, 1f, true);
		AlignToNormal(planet.GetRootSector().gameObject, go);
		return go;
	}

	public static GameObject Spawn(GameObject root, string path, Vector3 position)
	{
		var go = Main.Instance.NewHorizonsAPI.SpawnObject(root, null, path, position, Vector3.zero, 1f, true);
		AlignToNormal(root, go);
		return go;
	}

	public static void SpawnDreamstalker(AstroObject planet, Campfire campfire, CompletionVolume volume, Vector3 position)
	{
		if (_dreamstalkerPrefab == null) InitDreamstalkerPrefab();

		var dreamstalker = _dreamstalkerPrefab.InstantiateInactive();
		dreamstalker.transform.parent = planet.GetRootSector().transform;
		dreamstalker.transform.position = planet.GetRootSector().transform.TransformPoint(position);
		AlignToNormal(planet.GetRootSector().gameObject, dreamstalker);

		var controller = dreamstalker.GetComponent<DreamstalkerController>();
		controller.SetPlanet(planet);
		controller.SetCampfire(campfire);
		controller.SetVolume(volume);
	}

	#region Dreamstalker creation
	private static void InitDreamstalkerPrefab()
	{
		var th = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

		_dreamstalkerPrefab = Spawn(th, GhostBirdPath, Vector3.zero);
		var skeleton = Spawn(th, SkeletonPath, Vector3.zero);

		var originalSkins = _dreamstalkerPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (var originalSkin in originalSkins)
		{
			originalSkin.forceRenderingOff = true;
			originalSkin.enabled = false;
		}

		var root = skeleton.transform.Find("Nomai_Rig_v01:TrajectorySHJnt");
		foreach (var transform in root.transform.GetComponentsInChildren<Transform>().Append(root))
		{
			var name = GhostBirdBoneMap(transform.name);
			if (name != null)
			{
				var newParent = _dreamstalkerPrefab.GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name == name);
				if (newParent != null)
				{
					transform.parent = newParent;

					var adjustment = NomaiBoneAdjustments(transform.name);

					transform.localPosition = adjustment.offset;
					transform.localRotation = Quaternion.Euler(adjustment.rotation);
				}
			}
		}

		foreach (var skin in skeleton.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			skin.transform.parent = _dreamstalkerPrefab.transform;
		}

		// Disables the artifact
		//_dreamstalkerPrefab.transform.Find("Ghostbird_Skin_01:Ghostbird_Rig_V01:Base").gameObject.SetActive(false);

		GameObject.Destroy(skeleton);

		_dreamstalkerPrefab.name = "Dreamstalker_Prefab";

		_dreamstalkerPrefab.SetActive(false);

		_dreamstalkerPrefab.AddComponent<DreamstalkerController>();
		_dreamstalkerPrefab.AddComponent<DreamstalkerEffectsController>();
		var grabAttachPoint = new GameObject("GrabAttachPoint");
		grabAttachPoint.transform.parent = _dreamstalkerPrefab.transform;
		grabAttachPoint.transform.localPosition = Vector3.zero;

		grabAttachPoint.AddComponent<DreamstalkerGrabController>();
	}

	private static string GhostBirdBoneMap(string nomaiBone) => nomaiBone switch
	{
		"Nomai_Rig_v01:TrajectorySHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Base",
		"Nomai_Rig_v01:Spine_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01",
		"Nomai_Rig_v01:Spine_02SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03",
		"Nomai_Rig_v01:Spine_TopSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04",
		// Left arm
		"Nomai_Rig_v01:LF_Arm_ClavicleSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleL",
		"Nomai_Rig_v01:LF_Arm_ShoulderSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderL",
		"Nomai_Rig_v01:LF_Arm_ElbowSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowL",
		"Nomai_Rig_v01:LF_Arm_WristSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:WristL",
		// Index finger
		"Nomai_Rig_v01:LF_Finger_01_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:IndexL01",
		"Nomai_Rig_v01:LF_Finger_01_02SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:IndexL02",
		"Nomai_Rig_v01:LF_Finger_01_03SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:IndexL03",
		// Middle finger
		"Nomai_Rig_v01:LF_Finger_02_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:MiddleL01",
		"Nomai_Rig_v01:LF_Finger_02_02SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:MiddleL02",
		"Nomai_Rig_v01:LF_Finger_02_03SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:MiddleL03",
		// Thumb
		"Nomai_Rig_v01:LF_Thumb_01_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbL00",
		"Nomai_Rig_v01:LF_Thumb_01_02SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbL01",
		"Nomai_Rig_v01:LF_Thumb_01_03SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbL02",
		"Nomai_Rig_v01:LF_Thumb_01_04SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbL03",

		// Right arm
		"Nomai_Rig_v01:RT_Arm_ClavicleSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleR",
		"Nomai_Rig_v01:RT_Arm_ShoulderSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderR",
		"Nomai_Rig_v01:RT_Arm_ElbowSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowR",
		"Nomai_Rig_v01:RT_Arm_WristSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:WristR",
		// Index finger
		"Nomai_Rig_v01:RT_Finger_01_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:IndexR01",
		"Nomai_Rig_v01:RT_Finger_01_02SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:IndexR02",
		"Nomai_Rig_v01:RT_Finger_01_03SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:IndexR03",
		// Middle finger
		"Nomai_Rig_v01:RT_Finger_02_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:MiddleR01",
		"Nomai_Rig_v01:RT_Finger_02_02SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:MiddleR02",
		"Nomai_Rig_v01:RT_Finger_02_03SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:MiddleR03",
		// Thumb
		"Nomai_Rig_v01:RT_Thumb_01_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbR00",
		"Nomai_Rig_v01:RT_Thumb_01_02SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbR01",
		"Nomai_Rig_v01:RT_Thumb_01_03SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbR02",
		"Nomai_Rig_v01:RT_Thumb_01_04SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:ThumbR03",

		// Neck
		"Nomai_Rig_v01:Neck_01SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck01",
		"Nomai_Rig_v01:Neck_TopSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck02",
		"Nomai_Rig_v01:Head_JawSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Head",
		"Nomai_Rig_v01:Head_JawEndSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:Jaw",
		"Nomai_Rig_v01:Head_TopSHJnt" => "SIM_GhostBirdHead",

		// Left leg
		"Nomai_Rig_v01:LF_Leg_HipSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:HipL",
		"Nomai_Rig_v01:LF_Leg_Knee1SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:KneeL",
		"Nomai_Rig_v01:LF_Leg_Knee2SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:AnkleL",
		"Nomai_Rig_v01:LF_Leg_AnkleSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:FootL",

		// Right leg
		"Nomai_Rig_v01:RT_Leg_HipSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:HipR",
		"Nomai_Rig_v01:RT_Leg_Knee1SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:KneeR",
		"Nomai_Rig_v01:RT_Leg_Knee2SHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:AnkleR",
		"Nomai_Rig_v01:RT_Leg_AnkleSHJnt" => "Ghostbird_Skin_01:Ghostbird_Rig_V01:FootR",

		_ => null
	};

	private static (Vector3 offset, Vector3 rotation) NomaiBoneAdjustments(string nomaiBone) => nomaiBone switch
	{
		"Nomai_Rig_v01:TrajectorySHJnt" => (new Vector3(0, 2.5f, -0.5f), new Vector3(60, 180, 180)),

		"Nomai_Rig_v01:LF_Arm_WristSHJnt" => (new Vector3(-0.1f, -0.12f, 0.05f), new Vector3(90, 300, 160)),
		"Nomai_Rig_v01:RT_Arm_WristSHJnt" => (new Vector3(-0.1f, -0.12f, 0.05f), new Vector3(90, 300, 160)),

		"Nomai_Rig_v01:LF_Leg_HipSHJnt" => (new Vector3(-0.04f, 0.03f, 0.17f), new Vector3(0, 65, 355)),
		"Nomai_Rig_v01:RT_Leg_HipSHJnt" => (new Vector3(0.04f, -0.03f, -0.17f), new Vector3(0, -65, -355)),

		"Nomai_Rig_v01:LF_Leg_Knee1SHJnt" => (new Vector3(0.5f, -0.2f, 0.4f), Vector3.zero),
		"Nomai_Rig_v01:RT_Leg_Knee1SHJnt" => (new Vector3(-0.5f, 0.2f, -0.4f), Vector3.zero),

		"Nomai_Rig_v01:Spine_TopSHJnt" => (new Vector3(1f, 0f, 0f), Vector3.zero),
		"Nomai_Rig_v01:Neck_TopSHJnt" => (new Vector3(0.1f, -0.5f, 0f), Vector3.zero),

		_ => (Vector3.zero, Vector3.zero)
	};
	#endregion

	private static void AlignToNormal(GameObject root, GameObject go)
	{
		var globalUp = (go.transform.position - root.transform.position).normalized;
		go.transform.rotation = Quaternion.FromToRotation(Vector3.up, globalUp);
	}
}
