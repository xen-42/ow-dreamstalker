using System.Collections.Generic;
using UnityEngine;

namespace Dreamstalker.Components.Dreamworld;

internal class CameraLayerCullController : SectoredMonoBehaviour
{
    private SectorDetector _playerSectorDetector;
    private List<OWCamera> _affectedCameras = new();
    private bool _containsPlayer;

    public void Start()
    {
        _playerSectorDetector = Locator.GetPlayerSectorDetector();
        GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);
    }

    private void OnSwitchActiveCamera(OWCamera camera)
    {
        if (_containsPlayer)
        {
            AddCamera(camera);
        }
    }

    private void AddCamera(OWCamera camera)
    {
        if (!_affectedCameras.Contains(camera))
        {
            _affectedCameras.Add(camera);

            var distances = new float[32];
            for (int i = 0; i < 32; i++)
            {
                // We set them to 3000f so we don't see that far, except the sun layer which stays at 0 meaning it takes the default farClipPlane
                distances[i] = i == LayerMask.NameToLayer("Sun") || i == LayerMask.NameToLayer("IgnoreSun") ? 0 : 3000f;
            }
            camera.mainCamera.layerCullDistances = distances;
        }
    }

    public override void OnSectorOccupantAdded(SectorDetector sectorDetector)
    {
        base.OnSectorOccupantAdded(sectorDetector);

        if (sectorDetector == _playerSectorDetector)
        {
            AddCamera(Locator.GetActiveCamera());

            _containsPlayer = true;
        }
    }

    public override void OnSectorOccupantRemoved(SectorDetector sectorDetector)
    {
        base.OnSectorOccupantRemoved(sectorDetector);

        if (sectorDetector == _playerSectorDetector)
        {
            foreach (var cam in _affectedCameras)
            {
                cam.mainCamera.layerCullDistances = new float[32];
            }
            _affectedCameras.Clear();

            _containsPlayer = false;
        }
    }
}
