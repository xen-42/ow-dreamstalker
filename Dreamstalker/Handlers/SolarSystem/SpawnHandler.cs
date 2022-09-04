using NewHorizons.Utility;
using System.Linq;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class SpawnHandler : SolarSystemHandler
{
    private const string ghostBirdPath = "DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_2/Ghosts_DreamZone_2/GhostNodeMap_HornetHouse/Prefab_IP_GhostBird_Hornet/Ghostbird_IP_ANIM";
    private const string mummyPath = "RingWorld_Body/Sector_RingInterior/Sector_Zone2/Sector_DreamFireLighthouse_Zone2_AnimRoot/Interactibles_DreamFireLighthouse_Zone2/DreamFireChamber/MummyCircle/MummyPivot (HORNET)/Mummy_IP_Anim";
    private const string skeletonPath = "QuantumMoon_Body/Sector_QuantumMoon/State_DB/Interactables_DBState/QuantumDeadNomaiSuit/State_4/Prefab_NOM_Dead_Suit_GroundC/Character_NOM_Dead_Suit";

    private GameObject zombiePrefab;

    protected override void OnSolarSystemAwake() { }

    protected override void OnSolarSystemStart()
    {
        InitZombiePrefab();

        var th = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

        SpawnZombie(th, new Vector3(28.1f, -43.8f, 183.6f));
        Spawn(th, ghostBirdPath, new Vector3(25.6f, -43.6f, 184f));
        Spawn(th, mummyPath, new Vector3(30.6f, -42.9f, 183.5f));
    }

    public void SpawnZombie(AstroObject planet, Vector3 position)
    {
        var zombie = zombiePrefab.InstantiateInactive();
        zombie.transform.parent = planet.GetRootSector().transform;
        zombie.transform.position = planet.GetRootSector().transform.TransformPoint(position);
        AlignToNormal(planet.GetRootSector().gameObject, zombie);
        zombie.SetActive(true);
    }

    private void AlignToNormal(GameObject root, GameObject go)
    {
        var globalUp = (go.transform.position - root.transform.position).normalized;
        go.transform.rotation = Quaternion.FromToRotation(Vector3.up, globalUp);

    }

    public void InitZombiePrefab()
    {
        var th = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

        zombiePrefab = Spawn(th, ghostBirdPath, Vector3.zero);
        var skeleton = Spawn(th, skeletonPath, Vector3.zero);

        var originalSkins = zombiePrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
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
                var newParent = zombiePrefab.GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name == name);
                if (newParent != null)
                {
                    var localPosition = transform.localPosition;
                    var localRotation = transform.localRotation;
                    transform.parent = newParent;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }
        }

        foreach (var skin in skeleton.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skin.transform.parent = zombiePrefab.transform;
        }

        // Disables the artifact
        zombiePrefab.transform.Find("Ghostbird_Skin_01:Ghostbird_Rig_V01:Base").gameObject.SetActive(false);

        Destroy(skeleton);

        zombiePrefab.SetActive(false);
    }

    public GameObject Spawn(AstroObject planet, string path, Vector3 position)
    {
        var go = _main.NewHorizonsAPI.SpawnObject(planet.gameObject, planet.GetRootSector(), path, position, Vector3.zero, 1f, true);
        AlignToNormal(planet.GetRootSector().gameObject, go);
        return go;
    }

    public GameObject Spawn(GameObject root, string path, Vector3 position)
    {
        var go = _main.NewHorizonsAPI.SpawnObject(root, null, path, position, Vector3.zero, 1f, true);
        AlignToNormal(root, go);
        return go;
    }

    public string GhostBirdBoneMap(string nomaiBone) => nomaiBone switch
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

}
