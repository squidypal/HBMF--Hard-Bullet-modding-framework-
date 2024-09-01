using HBMF.GameResources.Internal;
using HBMF.ModMenu;
using HBMF.ModMenu.Internal;
using HBMF.SpawnGun.Internal;
using Il2CppInterop.Runtime;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(HBMF.Mod), "HBMF", "1.4.0", "korbykob")]
[assembly: MelonGame("GexagonVR", "Hard Bullet")]

namespace HBMF
{
    public class Mod : MelonMod
    {
        private bool loadedAssets = false;

        public override void OnInitializeMelon()
        {
            SpawnerManager.Setup(Menu.CreateCategory("HBMF"));
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            GameResourcesManager.GetResources();
            MenuManager.Setup();
            if (!loadedAssets)
            {
                SpawnerManager.pool = new GameObject().transform;
                Object.DontDestroyOnLoad(SpawnerManager.pool.gameObject);
                SpawnerManager.pool.gameObject.SetActive(false);
                foreach (Object obj in Resources.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()))
                {
                    GameObject gameObject = obj.Cast<GameObject>();
                    if (gameObject.transform.parent == null && !gameObject.scene.IsValid() && gameObject.name == "M1911")
                    {
                        SpawnerManager.m1911 = gameObject;
                    }
                }
                loadedAssets = true;
            }
        }
    }
}
