using MelonLoader;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using FirearmSystem;
using HurricaneVR.Framework.ControllerInput;
using InteractionSystem;
using BulletMenuVR;
using HurricaneVR.Framework.Shared;

namespace HBMF
{
    public static class BuildInfo
    {
        public const string Name = "HBMF";
        public const string Author = null;
        public const string Company = null;
        public const string Version = "0.0.1";
        public const string DownloadLink = null;
    }

    public class HBMF : MelonMod
    {
        public bool isUIactive = false;
        public static bool iscustomitemmenuactive = false;
        public GameObject playerhand;
        public GameObject menu;
        public bool HasFoundMenuItems = false;
        public bool hasAddedCollider = false;
        private GameObject rHand;
        private GameObject lHand;
        private GameObject head;
        private GameObject addedCollider;
        public bool thefunnyisdone = false;
        private HVRInputManager inputManager;
        private bool debounce = false;

        /*private VrMenuPage testPage;*/

        public override void OnApplicationStart()
        {        
            hasAddedCollider = false;

            // Examples: 
           /* testPage = VrMenuPageBuilder.Builder()
                .AddButton(new VrMenuButton("Test 1", () =>
                {
                    MelonLogger.Msg("test button pressed");
                }))
                .AddButton(new VrMenuButton("Test 2", () =>
                {
                    MelonLogger.Msg("test button 2 pressed");
                }))
                .AddButton(new VrMenuButton("Test 3", () =>
                {
                    MelonLogger.Msg("test button 3 pressed");
                }))
                .AddButton(new VrMenuButton("Test 4", () =>
                {
                    MelonLogger.Msg("test button 4 pressed");
                }))
                .AddButton(new VrMenuButton("Test 5", () =>
                {
                    MelonLogger.Msg("test button 5 pressed");
                }))
                .Build();*/
/*
            VrMenu.RegisterMainButton(new VrMenuButton("Example Page", () =>
            {
               testPage.Open();
            }));*/
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            inputManager = GameObject.Find("[REQUIRED COMPONENTS]/HVRGlobal").GetComponent<HVRInputManager>();

            head = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/Pelvis/CameraRig");

            AssetLoader.SpawnMenu(1);
            VrMenu.menuObject = AssetLoader.menu.transform.Find("MenuHolder").Find("VRMenu").gameObject;
            MenuBehavior menuBehavior = VrMenu.menuObject.GetComponent<MenuBehavior>();
            menuBehavior.button = AssetLoader.menu.transform.Find("MenuHolder").Find("Button").gameObject;
            VrMenu.RefreshMenu();
            VrMenu.ShowPage(0);
            rHand = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/RightArm/Hand/");
            lHand = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/LeftArm/Hand/");

            addedCollider = AssetLoader.menu.transform.Find("MenuHolder").Find("PlayerCollider").gameObject;
            GameObject instanciatedCollider = GameObject.Instantiate(addedCollider);
            instanciatedCollider.transform.parent = rHand.transform;
            instanciatedCollider.transform.localPosition = new Vector3(0, 0, 0);

            AssetLoader.menu.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            r.FindTheStuff();
            Scene currentscene = SceneManager.GetActiveScene();
            if (currentscene.name == "EnemyTesting AUGUST")
            {
                CustomItems.knifespawner();
            }
            thefunnyisdone = true;
        }
        public override void OnUpdate()
        {
            if (thefunnyisdone)
            {
                if (VrMenu.menuObject != null)
                {
                    var from = VrMenu.menuObject.transform.position;
                    var to = head.gameObject.transform.position;
                    to.Set(to.x, from.y, to.z);
                    Vector3 dir = (from - to).normalized;
                    VrMenu.menuObject.transform.LookAt(from + dir);
                }              
                if (inputManager.LeftController.JoystickClicked)
                {
                    if (debounce == false)
                    {
                        debounce = true;
                        if (isUIactive)
                        {
                            isUIactive = false;
                        }
                        else
                        {
                            isUIactive = true;

                        }
                    } 
                }
                else
                {
                    debounce = false;
                }
                if(isUIactive)
                {
                    VrMenu.menuObject.transform.position = lHand.transform.position + new Vector3(0, 0.3f, 0);
                } else
                {
                    VrMenu.menuObject.transform.position = new Vector3(0, 3000, 0);
                }
                if (addedCollider != null && isUIactive)
                {
                    addedCollider.transform.position = rHand.transform.position;
                } else
                {
                    addedCollider.transform.position = new Vector3(0, 999, 0);
                }
            }
        }
    
    public class CustomItems : MonoBehaviour
        {
            public bool yes = false;

            public void OnUpdate()
            {
                GameObject GrabDectect = GameObject.Find("Socket[Dagger]/SocketForStand/Model");

                if (GrabDectect.activeSelf)
                {
                    MelonCoroutines.Start(Knifesetup());
                }
            }
            public IEnumerator Knifesetup()
            {
                yield return new WaitForSeconds(0.01f);
                yes = false;
                GameObject HBdagger = GameObject.Find("Dagger(Clone)");
                GameObject HBphysics = GameObject.Find("Dagger(Clone)/Physics");
                GameObject HBgrabpoint = GameObject.Find("Dagger(Clone)/Interaction/GrabPoint");
                GameObject HBmodel = GameObject.Find("Dagger(Clone)/Visual");
                GameObject CIstabpoint = GameObject.Find("CustomItem(Clone)/Blade");
                GameObject CIgrabpoint = GameObject.Find("CustomItem(Clone)/GrabPoint");
                GameObject CIKnife = GameObject.Find("CustomItem(Clone)");
                GameObject HBstabpoint = GameObject.Find("Dagger(Clone)/Physics/Colliders[Blade][Pierce]");
                GameObject HBstabpoint2 = GameObject.Find("Dagger(Clone)/Physics/Colliders[Base]");
                GameObject detect = GameObject.Find("CustomItem(Clone)/Knife");

                if (detect != null)
                {
                    CIKnife.transform.rotation = HBdagger.transform.rotation;
                    CIKnife.transform.position = HBdagger.transform.position;
                    HBgrabpoint.transform.rotation = CIgrabpoint.transform.rotation;
                    HBgrabpoint.transform.position = CIgrabpoint.transform.position;
                    HBphysics.transform.position = CIstabpoint.transform.position;
                    HBstabpoint.transform.position = CIstabpoint.transform.position;
                    HBstabpoint2.SetActive(false);
                    HBstabpoint.transform.rotation = Quaternion.Euler(new Vector3(52.2076f, 0, 0));

                    CIKnife.transform.parent = HBdagger.transform;
                    HBmodel.SetActive(false);

                }
                yield return new WaitForSeconds(4);

                yes = true;

                yield break;
            }
            public static void knifespawner()
            {
                GameObject knife = GameObject.Find("[SCENE]/Environment/Interactive/MeleeWeaponsStand/Socket[Dagger]");
                knife.transform.parent = null;
                knife.transform.position = new Vector3(0.0241f, 1.662f, 9.925f);
            }
        }

        public class CustomItemLoad : MonoBehaviour
        {
            // Loads the item
            public static Transform spawnTrans;
            public static bool itemSpawned = false;
            public static int prevLoaded = 100;
            public static void SpawnItem(int itemNum)
            {
                string[] dirs = Directory.GetFiles(MelonUtils.UserDataDirectory + "\\CustomItems", "*.item");
                string itemName = dirs[itemNum - 1];
                AssetBundle localAssetBundle = AssetBundle.LoadFromFile(itemName);
                if (localAssetBundle == null)
                {
                    MelonLogger.Error("Failed");
                    return;
                }
                GameObject asset = localAssetBundle.LoadAsset<GameObject>("CustomItem");
                Instantiate(asset, new Vector3(0, 1000, 0), Quaternion.identity);
                localAssetBundle.Unload(false);
            }
        }

        public class r : MonoBehaviour
        {         
            public static GameObject playerhandL = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l/lowerarm_l/hand_l");
            public static GameObject playerhandR = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l/lowerarm_l/hand_r");
            public static GameObject player = GameObject.Find("[HARD BULLET PLAYER]");
            public static Vector3 playerloc;
            public static GameObject playerHEAD;
            public static Scene Currentscene = SceneManager.GetActiveScene();
            public static void FindTheStuff()
            {
                playerHEAD = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/neck_01/head");
                playerhandL = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l/lowerarm_l/hand_l");
                playerhandR = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l/lowerarm_l/hand_r");
                player = GameObject.Find("[HARD BULLET PLAYER]");
                playerloc = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root").transform.position;
                Currentscene = SceneManager.GetActiveScene();
            }
            public void Update()
            {
                  playerloc = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root").transform.position;
        }
        }
    }
    class AssetLoader
    {
        public static GameObject menu;
        public static void SpawnMenu(int num)
        {
            string[] dirs = Directory.GetFiles(MelonUtils.UserDataDirectory + "\\HBMF", "*.vm");
            string pmName = dirs[num - 1];
            AssetBundle localAssetBundle = AssetBundle.LoadFromFile(pmName);
            if (localAssetBundle == null)
            {
                MelonLogger.Msg("Failed");
                return;
            }
            GameObject asset = localAssetBundle.LoadAsset<GameObject>("MenuPrefab");
            menu = GameObject.Instantiate(asset, new Vector3(0, 2000, 0), Quaternion.identity);
            menu.gameObject.transform.Find("MenuHolder").Find("VRMenu").gameObject.AddComponent<MenuBehavior>();
            GameObject.Destroy(menu.gameObject.transform.Find("MenuHolder").Find("VRMenu").Find("Background").gameObject.GetComponent<BoxCollider>());
            menu.gameObject.transform.Find("MenuHolder").Find("VRMenu").Find("PrevPageButton").gameObject.AddComponent<ChangePageButton>();
            menu.gameObject.transform.Find("MenuHolder").Find("VRMenu").Find("NextPageButton").gameObject.AddComponent<ChangePageButton>();
            menu.gameObject.transform.Find("MenuHolder").Find("Button").gameObject.AddComponent<ButtonScript>();
            localAssetBundle.Unload(false);
        }
    }
}














