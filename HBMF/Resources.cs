using HarmonyLib;
using Il2Cpp;
using Il2CppHurricaneVR.Framework.ControllerInput;
using Il2CppInteractionSystem;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HBMF.GameResources
{
    public class GameResources
    {
        public static Action ResourcesReady { get; set; }
        public static Transform HexaBody { get; internal set; }
        public static Rigidbody Locoball { get; internal set; }
        public static Transform Head { get; internal set; }
        public static Transform Camera { get; internal set; }
        public static Grabber LeftGrabber { get; internal set; }
        public static Grabber RightGrabber { get; internal set; }
        public static Transform TrueLeftHand { get; internal set; }
        public static Transform TrueRightHand { get; internal set; }
        public static HVRInputManager InputManager { get; internal set; }
        public static Transform Enemies { get; internal set; }
    }
}

namespace HBMF.GameResources.Internal
{
    [HarmonyPatch]
    internal class GameResourcesManager
    {
        internal static void GetResources()
        {
            GameResources.Enemies = Object.FindObjectOfType<EnemyPool>()._activeRoot;
            CheckForEvent();
        }

        [HarmonyPatch(typeof(PlayerRoot), nameof(PlayerRoot.Awake))]
        [HarmonyPostfix]
        public static void GetHexaBody(PlayerRoot __instance)
        {
            GameResources.HexaBody = __instance.transform.Find("HexaBody");
            GameResources.Locoball = __instance.transform.Find("HexaBody/LocoBall").GetComponent<Rigidbody>();
            GameResources.Head = __instance.transform.Find("HexaBody/Head");
            GameResources.Camera = __instance.transform.Find("HexaBody/Pelvis/CameraRig/FloorOffset/Scaler/Camera");
            GameResources.TrueLeftHand = __instance.transform.Find("HexaBody/Pelvis/CameraRig/FloorOffset/Scaler/LeftController");
            GameResources.LeftGrabber = __instance.transform.Find("HexaBody/LeftArm/Hand").GetComponent<Grabber>();
            GameResources.TrueRightHand = __instance.transform.Find("HexaBody/Pelvis/CameraRig/FloorOffset/Scaler/RightController");
            GameResources.RightGrabber = __instance.transform.Find("HexaBody/RightArm/Hand").GetComponent<Grabber>();
            CheckForEvent();
        }

        [HarmonyPatch(typeof(HVRInputManager), nameof(HVRInputManager.Awake))]
        [HarmonyPostfix]
        public static void GetInputs(HVRInputManager __instance)
        {
            GameResources.InputManager = __instance;
            CheckForEvent();
        }

        private static void CheckForEvent()
        {
            if (GameResources.Enemies != null && GameResources.HexaBody != null && GameResources.InputManager != null)
            {
                GameResources.ResourcesReady();
            }
        }
    }
}
