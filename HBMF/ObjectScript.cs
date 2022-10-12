using System;
using System.Collections;
using System.Collections.Generic;
using HBMF;
using MelonLoader;
using TMPro;
using UnityEngine;
using AudioImporter;

namespace BulletMenuVR
{
    public class MenuBehavior : MonoBehaviour
    {

        public GameObject button;
        public int pageIndex = 0;
        private float scale = 0.5f;
        private List<GameObject> activeButtons = new List<GameObject>();

        private List<GameObject> totalButtons = new List<GameObject>();

        public bool isShowingCustomPage = false;

        private List<GameObject> temporaryButtons = new List<GameObject>();
        private List<GameObject> allTemporaryButtons = new List<GameObject>();

        public float lastSelect;

        public void IncreasePage()
        {
            pageIndex++;

            if (!isShowingCustomPage)
            {
                if (pageIndex > Math.Ceiling((double)totalButtons.Count / 4) - 1)
                {
                    pageIndex--;
                }
            }
            else
            {
                if (pageIndex > Math.Ceiling((double)allTemporaryButtons.Count / 4) - 1)
                {
                    pageIndex--;
                }
            }

            ShowPage();
        }

        public void DecreasePage()
        {
            pageIndex--;

            if (!isShowingCustomPage)
            {
                if (pageIndex < 0)
                {
                    pageIndex++;
                }
            }
            else
            {
                if (pageIndex < 0)
                {
                    DestroyTempPage();
                    isShowingCustomPage = false;
                }
            }

            ShowPage();
        }

        private void FixRotationTemp()
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void FixScaleGrow()
        {
            AssetLoader.menu.transform.localScale = new Vector3(1, 1, 1);
        }

        private void FixScaleShrink()
        {
            AssetLoader.menu.transform.localScale = new Vector3(scale, scale, scale);
        }

        public void ShowButtonList(List<GameObject> buttons)
        {
            if (isShowingCustomPage)
            {
                DestroyTempPage();
            }

            isShowingCustomPage = true;
            pageIndex = 0;
            FixRotationTemp();
            foreach (GameObject activeButton in activeButtons)
            {
                activeButton.SetActive(false);
            }

            activeButtons.Clear();
            allTemporaryButtons = buttons;
            for (int i = 0; i < buttons.Count; i++)
            {
                if (activeButtons.Count == 4)
                {
                    break;
                }

                GameObject buttonToShow = buttons[i];
                Display(buttonToShow);
                temporaryButtons.Add(buttonToShow);
            }
        }

        private void DestroyTempPage()
        {
            foreach (GameObject activeButton in temporaryButtons)
            {
                if (activeButtons.Contains(activeButton))
                {
                    activeButtons.Remove(activeButton);
                }
                Destroy(activeButton);
            }

            temporaryButtons.Clear();
        }

        public void ShowPage()
        {
            FixRotationTemp();
            foreach (GameObject activeButton in activeButtons)
            {
                activeButton.SetActive(false);
            }

            activeButtons.Clear();

            if (!isShowingCustomPage)
            {
                for (int i = 0; i < totalButtons.Count; i++)
                {
                    if (i >= (pageIndex * 4))
                    {
                        if (activeButtons.Count == 4)
                        {
                            break;
                        }

                        GameObject buttonToShow = totalButtons[i];
                        Display(buttonToShow);
                    }
                }
            }
            else
            {
                for (int i = 0; i < allTemporaryButtons.Count; i++)
                {
                    if (i >= (pageIndex * 4))
                    {
                        if (activeButtons.Count == 4)
                        {
                            break;
                        }

                        GameObject buttonToShow = allTemporaryButtons[i];
                        Display(buttonToShow);
                        temporaryButtons.Add(buttonToShow);
                    }
                }
            }
        }
        
        public GameObject MakeButton(string label, Action buttonAction, Color color)
        {
            FixScaleGrow();
            GameObject buttonInstance = Instantiate(button);
            buttonInstance.transform.parent = gameObject.transform.parent;
            buttonInstance.transform.position = gameObject.transform.position;
            buttonInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
            ButtonScript buttonScript = buttonInstance.GetComponent<ButtonScript>();
            buttonScript.SetAction(buttonAction);
            buttonInstance.transform.Find("Text (TMP)").GetComponent<TMP_Text>().SetText(label);
            buttonInstance.transform.Find("Text (TMP)").GetComponent<TMP_Text>().color = color;
            buttonInstance.gameObject.SetActive(false);
            FixScaleShrink();
            return buttonInstance;
        }

        public void AddButton(string label, Action buttonAction, Color color)
        {
            GameObject buttonInstance = MakeButton(label, buttonAction, color);
            totalButtons.Add(buttonInstance);
        }

        private void Display(GameObject button)
        {
            button.gameObject.SetActive(true);
            button.gameObject.transform.parent = gameObject.transform;
            button.transform.localPosition = new Vector3(0, 0.333f - (activeButtons.Count * 0.2f), 0);
            activeButtons.Add(button);
        }
    }

    public class ButtonScript : MonoBehaviour
    {
        private Action buttonAction = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetAction(Action action)
        {
            buttonAction = action;
        }

        public void OnTriggerEnter(Collider other)
        {
            MenuBehavior menuBehavior = GetComponentInParent<MenuBehavior>();
            if (Time.unscaledTime - menuBehavior.lastSelect >= 0.5f)
            {
                if (other.name.ToLower().Contains("playercollider"))
                {
                    MelonCoroutines.Start(GrayifyButton(gameObject));
                    GetComponentInParent<AudioSource>().Play(new PlaySettings()
                    {
                        Use2D = true,
                        UseSlowMotion = false
                    });
                    buttonAction.Invoke();
                    menuBehavior.lastSelect = Time.unscaledTime;
                }
            }
        }

        public static IEnumerator GrayifyButton(GameObject gameObject)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            var material = renderer.material;
            Color originalColor = material.color;
            material.color = Color.gray;
            yield return new WaitForSecondsRealtime(0.2f);
            if (gameObject != null)
            {
                if (!gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(true);
                    renderer.material.color = originalColor;
                    gameObject.SetActive(false);
                }
                else
                {
                    renderer.material.color = originalColor;
                }
            }
            yield break;
        }
    }

    public class ChangePageButton : MonoBehaviour
    {
        public bool isAdd;
        // Start is called before the first frame update
        void Start()
        {
            if (gameObject.name.Equals("PrevPageButton"))
            {
                isAdd = false;
            }
            else
            {
                isAdd = true;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTriggerEnter(Collider other)
        {
            MenuBehavior menuBehavior = GetComponentInParent<MenuBehavior>();
            if (Time.unscaledTime - menuBehavior.lastSelect >= 0.5f)
            {
                if (other.name.ToLower().Contains("playercollider"))
                {
                    GetComponentInParent<AudioSource>().Play(new PlaySettings()
                    {
                        Use2D = true,
                        UseSlowMotion = false
                    });

                    if (isAdd)
                    {
                        menuBehavior.IncreasePage();
                    }
                    else
                    {
                        menuBehavior.DecreasePage();
                    }

                    menuBehavior.lastSelect = Time.unscaledTime;
                }
            }
        }
    }
}