using MelonLoader;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using System.Reflection;
using HurricaneVR.Framework.ControllerInput;
using BulletMenuVR;

namespace HBMF
{
    public static class BuildInfo
    {
        public const string Name = "HBMF";
        public const string Author = null;
        public const string Company = null;
        public const string Version = "0.2.0";
        public const string DownloadLink = null;
    }

    public class HBMF : MelonMod
    {
        public bool isUIactive = false;
        public GameObject playerhand;
        public GameObject menu;
        public bool HasFoundMenuItems = false;
        private GameObject rHand;
        private GameObject lHand;
        private GameObject head;
        private GameObject addedCollider;
        public bool thefunnyisdone = false;
        private HVRInputManager inputManager;
        private bool debounce = false;
        public static int tempCI = 0;
        public static float CurrentItemType = 0f;
        public static MelonPreferences_Category HBMFcat;
        public static MelonPreferences_Entry<bool> ConfigHBMF;


        public override void OnApplicationStart()
        {
            HBMFcat = MelonPreferences.CreateCategory("HBMFcat");
            ConfigHBMF = HBMFcat.CreateEntry("Config", false);


            VrMenuPageBuilder builder = VrMenuPageBuilder.Builder();
            VrMenuPageBuilder Main = VrMenuPageBuilder.Builder();
            VrMenuPageBuilder CustomItems = VrMenuPageBuilder.Builder();
            VrMenuPageBuilder Inpuit = VrMenuPageBuilder.Builder();


            Inpuit.AddButton(new VrMenuButton("Joystick", () =>
            {
                ConfigHBMF.Value = false;
            }));
            Inpuit.AddButton(new VrMenuButton("LeftSecButton", () =>
            {
                ConfigHBMF.Value = true;
            }));

            builder.AddButton(new VrMenuButton("MainMenu", () =>
            {
                SceneManager.LoadScene("MainMenu2021");
            }
            ));
            builder.AddButton(new VrMenuButton("Sandbox", () =>
            {
                SceneManager.LoadScene("EnemyTesting AUGUST");
            }
            ));
            builder.AddButton(new VrMenuButton("Stairs", () =>
            {
                SceneManager.LoadScene("Stairs Fight");
            }
           ));

            builder.AddButton(new VrMenuButton("Action", () =>
            {
                SceneManager.LoadScene("Action SEPTEMBER 2021");
            }
            ));
            builder.AddButton(new VrMenuButton("Baths", () =>
            {
                SceneManager.LoadScene("PoolDay SEPTEMBER 2021");
            }
          ));

            builder.AddButton(new VrMenuButton("Kowloon", () =>
            {
                SceneManager.LoadScene("Kowloon");
            }
            ));
            builder.AddButton(new VrMenuButton("Market", () =>
            {
                SceneManager.LoadScene("Kowloon 2");
            }
           ));

            builder.AddButton(new VrMenuButton("Rooftop", () =>
            {
                SceneManager.LoadScene("RoofTop Level");
            }
            ));
            builder.AddButton(new VrMenuButton("Basement", () =>
            {
                SceneManager.LoadScene("Basement");
            }
          ));
            VrMenuPage myPage = builder.Build();
            VrMenuPage main = Main.Build();
            VrMenuPage CustomitemsPage = CustomItems.Build();
            VrMenuPage InputCon = Inpuit.Build();

            Main.AddButton(new VrMenuButton("Scene select", () =>
            {
                myPage.Open();
            }, Color.blue
            ));
            Main.AddButton(new VrMenuButton("InputConfig", () =>
            {
                InputCon.Open();
            }, Color.green
            ));

            VrMenu.RegisterMainButton(new VrMenuButton("HBMF", () =>
            {
                main.Open();
            }
            ));

            Directory.CreateDirectory(MelonUtils.UserDataDirectory + "\\CustomItems");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            inputManager = GameObject.Find("[REQUIRED COMPONENTS]/HVRGlobal").GetComponent<HVRInputManager>();
            head = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/Pelvis/CameraRig/FloorOffset/Scaler/Camera");

            AssetLoader.SpawnMenu(1);
            AssetLoader.SpawnNotification(1);

            VrMenu.menuObject = AssetLoader.menu.transform.Find("MenuHolder").Find("VRMenu").gameObject;
            MenuBehavior menuBehavior = VrMenu.menuObject.GetComponent<MenuBehavior>();
            menuBehavior.button = AssetLoader.menu.transform.Find("MenuHolder").Find("Button").gameObject;
            VrMenu.RefreshMenu();
            VrMenu.ShowPage(0);
            rHand = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/RightArm/Hand/");
            lHand = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/LeftArm/Hand/");
            addedCollider = AssetLoader.menu.transform.Find("MenuHolder").Find("PlayerCollider").gameObject;
            if (rHand.transform.Find("PlayerCollider") == null)
            {
                GameObject instanciatedCollider = GameObject.Instantiate(addedCollider);
                instanciatedCollider.transform.parent = rHand.transform;
                instanciatedCollider.transform.localPosition = new Vector3(0, 0, 0);
            }
            AssetLoader.menu.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            r.DontTouch();
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

                    VrMenu.menuObject.transform.eulerAngles = new Vector3(0f,
                        Quaternion.LookRotation(from - to).normalized.eulerAngles.y, 0f);
                }


                if (ConfigHBMF.Value == false)
                {
                    if (r.LeftJoystickClick)
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
                    //
                }
                else
                {
                    if (r.LeftSecButton)
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
                }

                if (isUIactive)
                {
                    VrMenu.menuObject.transform.position = lHand.transform.position + new Vector3(0, 0.3f, 0);
                }
                else
                {
                    VrMenu.menuObject.transform.position = new Vector3(0, 3000, 0);
                }

                if (addedCollider != null && isUIactive)
                {
                    addedCollider.transform.position = rHand.transform.position;
                }
                else
                {
                    addedCollider.transform.position = new Vector3(0, 999, 0);
                }

                // Input
                r.LeftJoystickClick = inputManager.LeftController.JoystickClicked;
                r.RightJoystickClick = inputManager.RightController.JoystickClicked;
                r.LeftPrimButton = inputManager.LeftController.PrimaryButton;
                r.RightPrimButton = inputManager.RightController.PrimaryButton;
                r.LeftMenuButton = inputManager.LeftController.MenuButton;
                r.RightMenuButton = inputManager.RightController.MenuButton;
                r.LeftSecButton = inputManager.LeftController.SecondaryButton;
                r.RightSecButton = inputManager.RightController.SecondaryButton;
                r.LeftTrackpad = inputManager.LeftController.TrackPadClicked;
                r.RightTrackPad = inputManager.RightController.TrackPadClicked;
                r.LeftTriggerPress = inputManager.LeftController.TriggerButton;
                r.RightTriggerPress = inputManager.RightController.TriggerButton;
                r.LeftVelocity = inputManager.LeftController.Velocity;
                r.RightVelocity = inputManager.RightController.Velocity;
                r.LeftJoyAxis = inputManager.LeftController.JoystickAxis;
                r.RightJoyAxis = inputManager.RightController.JoystickAxis;
                r.LeftTrigger = inputManager.LeftController.Trigger;
                r.RightTrigger = inputManager.RightController.Trigger;
                r.LeftGrip = inputManager.LeftController.Grip;
                r.RightGrip = inputManager.RightController.Grip;
                r.LeftGripPress = inputManager.LeftController.GripButton;
                r.RightGripPress = inputManager.RightController.GripButton;
                r.playerloc = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root").transform.position;
            }
        }
        public static void startstuff()
        {
            CustomItemLoad.SpawnItem(tempCI);
            Transform spawnedItem = GameObject.Find("CustomItem(Clone)").transform;
            if (spawnedItem.GetChild(0).name.Contains("Pistol"))
            {
                CurrentItemType = 1f;
                MelonCoroutines.Start(CustomItems.PistolSetup());
            }
            else if (spawnedItem.GetChild(0).name.Contains("Blade"))
            {
                CurrentItemType = 2f;
                MelonCoroutines.Start(CustomItems.Knifesetup());
            }
            else if (spawnedItem.GetChild(0).name.Contains("Blunt"))
            {
                CurrentItemType = 3f;
            }
            else if (spawnedItem.GetChild(0).name.Contains("AR"))
            {
                CurrentItemType = 4f;
            }
            else if (spawnedItem.GetChild(0).name.Contains("SMG"))
            {
                CurrentItemType = 5f;
            }
        }
    }

    public class CustomItems : MonoBehaviour
    {
        public static bool yes = false;
        public static int Itemtoload = 0;

        public static IEnumerator PistolSetup()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject InstatiatedWeapon = GameObject.Instantiate(GameObject.Find("SCENE]/Environment/Interactive/WeaponsStand/WeaponSet[DEFAULT]/Socket[Beretta]/SocketForFirearmStand/Beretta(Clone)"));

            Transform Visual = InstatiatedWeapon.transform.GetChild(2);
            Visual.gameObject.SetActive(false);
            Transform HandleOne = InstatiatedWeapon.transform.GetChild(0).GetChild(2);
            Transform HandleTwo = InstatiatedWeapon.transform.GetChild(0).GetChild(3);
            Transform slideModel = InstatiatedWeapon.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            Transform slide = InstatiatedWeapon.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            GameObject CIhandleOne = GameObject.Find("CustomItem(Clone)/Grip");
            GameObject CIhandleTwo = GameObject.Find("CustomItem(Clone)/Grip2");
            GameObject CIslide = GameObject.Find("CustomItem(Clone)/slide");
            GameObject CImain = GameObject.Find("CustomItem(Clone)");
            slideModel.gameObject.SetActive(false);
        }
        public static IEnumerator Knifesetup()
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
            Instantiate(asset, new Vector3(0, 0, 0), Quaternion.identity);
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

        // Input
        public static bool LeftJoystickClick = false;
        public static bool RightJoystickClick = false;
        public static bool LeftPrimButton = false;
        public static bool RightPrimButton = false;
        public static bool LeftMenuButton = false;
        public static bool RightMenuButton = false;
        public static bool LeftSecButton = false;
        public static bool RightSecButton = false;
        public static bool LeftTrackpad = false;
        public static bool RightTrackPad = false;
        public static bool LeftTriggerPress = false;
        public static bool RightTriggerPress = false;
        public static Vector3 LeftVelocity;
        public static Vector3 RightVelocity;
        public static Vector2 LeftJoyAxis;
        public static Vector2 RightJoyAxis;
        public static float LeftTrigger;
        public static float RightTrigger;
        public static float LeftGrip;
        public static float RightGrip;
        public static bool LeftGripPress;
        public static bool RightGripPress;

        public static void DontTouch()
        {
            playerHEAD = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/neck_01/head");
            playerhandL = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l/lowerarm_l/hand_l");
            playerhandR = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root/pelvis/spine_01/spine_02/spine_03/clavicle_l/upperarm_l/lowerarm_l/hand_r");
            player = GameObject.Find("[HARD BULLET PLAYER]");
            playerloc = GameObject.Find("[HARD BULLET PLAYER]/HexaBody/PlayerModel/PlayerModel/root").transform.position;
            Currentscene = SceneManager.GetActiveScene();
        }
    }
}
public static class EmbeddedAssetBundle
{
    public static byte[] LoadFromAssembly(Assembly assembly, string name)
    {
        string[] manifestResources = assembly.GetManifestResourceNames();

        if (manifestResources.Contains(name))
        {
            using (Stream str = assembly.GetManifestResourceStream(name))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                str.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        return null;
    }
}
public class Notifications : MonoBehaviour
{
    public static float notitime = 0f;
    public static TMP_Text notitext;
    public static GameObject notitextGO;
    public static bool isnotiactive = false;

    public static void NewNotification()
    {
        notitext.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        notitextGO = GameObject.Find("notification(Clone)");
        notitext = notitextGO.GetComponent<TMP_Text>();
        MelonCoroutines.Start(Time());
        isnotiactive = true;
    }
    public static IEnumerator Time()
    {
        yield return new WaitForSeconds(notitime);
        notitext.text = "";
        isnotiactive = false;
        yield break;
    }
    public void Update()
    {
        if (isnotiactive == true)
        {
            notitextGO.transform.position = HBMF.r.playerhandL.transform.position + new Vector3(0, 0.1f, 0);
            notitextGO.transform.eulerAngles = HBMF.r.playerhandL.transform.eulerAngles + Quaternion.Euler(new Vector3(20, 100, 0)).eulerAngles;
        }
        else
        {
            notitextGO.transform.position = new Vector3(0, 9999, 0);
        }
    }
}
class AssetLoader
{
    public static GameObject menu;
    public static GameObject noti;
    public static void SpawnMenu(int num)
    {
        AssetBundle localAssetBundle = AssetBundle.LoadFromMemory(EmbeddedAssetBundle.LoadFromAssembly(Assembly.GetExecutingAssembly(), "HBMF.Resources.vrmenu.vm"));

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
    public static void SpawnNotification(int num)
    {

        AssetBundle localAssetBundle = AssetBundle.LoadFromMemory(EmbeddedAssetBundle.LoadFromAssembly(Assembly.GetExecutingAssembly(), "HBMF.Resources.notification.nt"));
        if (localAssetBundle == null)
        {
            MelonLogger.Msg("Failed");
            return;
        }
        GameObject asset = localAssetBundle.LoadAsset<GameObject>("notification");
        noti = GameObject.Instantiate(asset, new Vector3(0, 2000, 0), Quaternion.identity);
        localAssetBundle.Unload(false);
    }
}















