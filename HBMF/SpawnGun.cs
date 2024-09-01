using UnityEngine;
using FirearmSystem;
using InteractionSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using AudioImporter;

namespace SpawnGun
{
    public class Spawner : MonoBehaviour
    {
        private Text label;
        public static Spawnables spawnables;
        private int spawnable;
        private AudioSource source;
        public static AudioClip clip;

        private void Start()
        {
            gameObject.GetComponentInChildren<Firearm>().enabled = false;
            Transform chamber = gameObject.GetComponentInChildren<Chamber>().transform;
            Usable usable = gameObject.GetComponent<Usable>();
            label = Instantiate(AssetLoader.spawnGun, transform).GetComponentInChildren<Text>();
            source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            if (spawnables.spawnables.Count > 0)
            {
                usable.OnUsed += (Usable currentUsable) =>
                {
                    Physics.Raycast(chamber.position + (chamber.forward * 0.1f), chamber.forward, out RaycastHit hit);
                    Instantiate(spawnables.spawnables[spawnable].Key, hit.point, Quaternion.identity);
                    source.Play();
                };
                usable.OnDropClipButtonPressed += (Usable currentUsable) =>
                {
                    spawnable++;
                    if (spawnable >= spawnables.spawnables.Count)
                    {
                        spawnable = 0;
                    }
                    label.text = spawnables.spawnables[spawnable].Value;
                };
                label.text = spawnables.spawnables[spawnable].Value;
            }
        }
    }

    public class Spawnables
    {
        public static Transform pool;
        public List<KeyValuePair<GameObject, string>> spawnables = new List<KeyValuePair<GameObject, string>>();

        public void Add(GameObject spawnable, string name)
        {
            GameObject instantiated = Object.Instantiate(spawnable);
            instantiated.transform.SetParent(pool);
            spawnables.Add(new KeyValuePair<GameObject, string>(instantiated, name));
        }
    }
}
