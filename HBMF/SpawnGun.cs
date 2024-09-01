using HBMF.AudioImporter;
using HBMF.ModMenu;
using HBMF.SpawnGun.Internal;
using HBMF.Utilities;
using Il2CppFirearmSystem;
using Il2CppInteractionSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HBMF.SpawnGun
{
    public static class Spawnables
    {
        public static void AddSpawnable(string name, GameObject spawnable)
        {
            SpawnerManager.spawnables.Add(new(name, Object.Instantiate(spawnable, SpawnerManager.pool)));
        }
    }
}

namespace HBMF.SpawnGun.Internal
{
    internal class SpawnerManager
    {
        internal static GameObject m1911;
        internal static List<KeyValuePair<string, GameObject>> spawnables = new();
        internal static Transform pool;

        internal static void Setup(Category category)
        {
            AudioClip changeClip = Audio.Import(Utils.GetResource(Assembly.GetExecutingAssembly(), "HBMF.SpawnGunChange.wav"));
            AudioClip shootClip = Audio.Import(Utils.GetResource(Assembly.GetExecutingAssembly(), "HBMF.SpawnGunShoot.wav"));
            category.CreateAction("SPAWN GUN", "SPAWN", () =>
            {
                Transform gun = Object.Instantiate(m1911).transform;
                gun.position = GameObject.Find("[HARD BULLET PLAYER](Clone)/HexaBody/LeftArm/Hand").transform.position;
                gun.Find("Firearm").GetComponent<Firearm>().enabled = false;
                Transform shootPosition = gun.Find("Firearm/Chambers/Chamber/ProjectileInstantiatePosition");
                Transform label = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
                label.SetParent(gun);
                label.localPosition = new(0f, 0.1f, -0.01f);
                label.localEulerAngles = new(-45f, 0f, 0f);
                label.localScale = Vector3.one * 0.01f;
                label.GetComponent<MeshRenderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                CanvasScaler scaler = new GameObject().AddComponent<CanvasScaler>();
                scaler.transform.SetParent(label);
                scaler.transform.localPosition = new(0f, 0.1f, 0f);
                scaler.transform.localEulerAngles = new(90f, 0f, 0f);
                scaler.transform.localScale = Vector3.one;
                scaler.dynamicPixelsPerUnit = 100f;
                RectTransform scalerRect = scaler.GetComponent<RectTransform>();
                scalerRect.anchorMin = Vector2.zero;
                scalerRect.anchorMax = Vector2.zero;
                scalerRect.offsetMin = new(-5f, -4.9f);
                scalerRect.offsetMax = new(5f, 5.1f);
                Text text = new GameObject().AddComponent<Text>();
                text.transform.SetParent(scalerRect.transform);
                text.transform.localPosition = Vector3.zero;
                text.transform.localEulerAngles = Vector3.zero;
                text.transform.localScale = Vector3.one;
                text.alignment = TextAnchor.MiddleCenter;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                text.text = "None";
                text.color = Color.black;
                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                int spawnable = 0;
                AudioSource changeSound = gun.Find("Audio/HandleAudioSource").gameObject.AddComponent<AudioSource>();
                changeSound.clip = changeClip;
                AudioSource shootSound = gun.Find("Audio/ShotAudioSource").gameObject.AddComponent<AudioSource>();
                shootSound.clip = shootClip;
                Usable usable = gun.GetComponent<Usable>();
                usable.add_OnDropClipButtonPressed(new Action<Usable>((Usable currentUsable) =>
                {
                    if (spawnables.Count > 0)
                    {
                        spawnable++;
                        if (spawnable >= spawnables.Count)
                        {
                            spawnable = 0;
                        }
                        text.text = spawnables[spawnable].Key;
                    }
                    changeSound.Play();
                }));
                usable.add_OnUsed(new Action<Usable>((Usable currentUsable) =>
                {
                    if (spawnables.Count > 0)
                    {
                        Physics.Raycast(shootPosition.position, shootPosition.forward, out RaycastHit hit);
                        Object.Instantiate(spawnables[spawnable].Value, hit.point, Quaternion.identity);
                        shootSound.Play();
                    }
                }));
                if (spawnables.Count > 0)
                {
                    text.text = spawnables[spawnable].Key;
                }
            });
        }
    }
}
