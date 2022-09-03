using OWML.ModHelper;
using OWML.Common;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

namespace NewHorizonsTemplate
{
    public class Main : ModBehaviour
    {
        public INewHorizons NewHorizonsAPI;

        private void Start()
        {
            NewHorizonsAPI = ModHelper.Interaction.GetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizonsAPI.LoadConfigs(this);
        }
    }
}
