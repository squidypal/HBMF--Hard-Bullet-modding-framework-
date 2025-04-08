using System.Collections.Generic;
using System;
using Il2CppInteractionSystem;
using UnityEngine.Events;
using UnityEngine;
using Il2CppHardBullet.UI.WindowsSystem.Data;
using UnityEngine.UI;
using Il2CppHardBullet.UI.WindowsSystem.Windows;
using Il2CppHardBullet.UI.WindowsSystem;
using System.Linq;
using Object = UnityEngine.Object;
using HBMF.ModMenu.Internal;
using Il2Cpp;
using EnumInfo = HBMF.ModMenu.Internal.EnumInfo;
using MelonLoader;
using Il2CppHardBullet.UI;
using Il2CppGameSettings;
using System.Collections;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Attributes;

namespace HBMF.ModMenu
{
    public class Category
    {
        public string Name { get; internal set; }
        internal List<KeyValuePair<string, object>> options = new();

        public Category CreateAction(string name, string buttonText, Action action)
        {
            options.Add(new(name, new ActionInfo(buttonText, action)));
            return this;
        }

        public Category CreateBool(string name, bool state, Action<bool> onChanged)
        {
            options.Add(new(name, new BoolInfo(state, onChanged)));
            return this;
        }

        public Category CreateInt(string name, int state, int min, int max, int step, string units, Action<int> onChanged)
        {
            options.Add(new(name, new IntInfo(state, min, max, step, units, onChanged)));
            return this;
        }

        public Category CreateFloat(string name, float state, float min, float max, float step, string units, Action<float> onChanged)
        {
            options.Add(new(name, new FloatInfo(state, min, max, step, units, onChanged)));
            return this;
        }

        public Category CreateEnum(string name, int state, string[] states, Action<int> onChanged)
        {
            options.Add(new(name, new EnumInfo(state, states, onChanged)));
            return this;
        }

        public Category CreateSlider(string name, float state, float min, float max, int steps, string units, Action<float> onChanged)
        {
            options.Add(new(name, new SliderInfo((state - min) / (max - min), min, max, steps + 1, units, onChanged)));
            return this;
        }
    }

    public class Menu
    {
        public static Category CreateCategory(string name)
        {
            Category category = new()
            {
                Name = name
            };
            MenuManager.categories.Add(category);
            return category;
        }
    }
}

namespace HBMF.ModMenu.Internal
{
    internal class MenuManager
    {
        private static bool createdNormal = false;
        private static bool createdRide = false;
        internal static WindowsManager manager;
        internal static readonly List<Category> categories = new();
        internal static readonly Dictionary<string, object> properties = new();

        internal static void Setup()
        {
            manager = GameObject.Find("[REQUIRED COMPONENTS]/WindowsManager").GetComponent<WindowsManager>();
            if ((manager._data.name == "PopUpWindowsData" && !createdNormal) || (manager._data.name == "PopUpWindowsRideData" && !createdRide))
            {
                Transform settingsWindow = null;
                foreach (WindowData windowData in manager._data.Windows)
                {
                    if (windowData.WindowType == WindowType.Pause)
                    {
                        Transform button = Object.Instantiate(windowData.Window.transform.Find("Canvas/PauseMenuPanel/PauseButtons/Button[ReturnToGame]"));
                        button.SetParent(windowData.Window.transform.Find("Canvas/PauseMenuPanel/PauseButtons"), false);
                        Object.Destroy(button.GetComponent<UITextFieldLocalisator>());
                        button.Find("Text[Standart]").GetComponent<Text>().text = "HBMF";
                        button.Find("Text[Hovered]").GetComponent<Text>().text = "HBMF";
                        button.gameObject.AddComponent<HBMFButton>();
                    }
                    if (windowData.WindowType == WindowType.Settings)
                    {
                        settingsWindow = windowData.Window.transform;
                    }
                }
                SettingsWindow modWindow = Object.Instantiate(settingsWindow).GetComponent<SettingsWindow>();
                modWindow.gameObject.SetActive(false);
                Object.DontDestroyOnLoad(modWindow.gameObject);
                Transform panel = modWindow.transform.Find("Conatainer/Panels");
                for (int i = 0; i < panel.childCount; i++)
                {
                    Transform child = panel.GetChild(i);
                    child.gameObject.SetActive(child.name == "SettingsParameterPanel");
                }
                Transform options = modWindow.transform.Find("Conatainer/Panels/SettingsParameterPanel");
                bool keep = true;
                for (int i = 0; i < options.childCount; i++)
                {
                    if (keep)
                    {
                        options.GetChild(i).gameObject.SetActive(false);
                        keep = false;
                    }
                    else
                    {
                        Object.Destroy(options.GetChild(i).gameObject);
                    }
                }
                Transform main = options.GetChild(0);
                PanelsManagerUI panelManager = modWindow.GetComponent<PanelsManagerUI>();
                panelManager.panelUIs.Clear();
                Transform categoryMenu = modWindow.transform.Find("Conatainer/Panels/PlayerParametersPanel");
                bool first = true;
                foreach (Category category in categories)
                {
                    Transform newMenu = Object.Instantiate(categoryMenu, categoryMenu.parent);
                    Object.Destroy(newMenu.GetComponent<ActivatorByController>());
                    Transform content = newMenu.Find("Scroll/Viewport/Content");
                    for (int i = 0; i < content.childCount; i++)
                    {
                        content.GetChild(i).gameObject.SetActive(false);
                    }
                    Transform newAction = content.Find("PlayerHeightParameterPanel");
                    Transform newBool = content.Find("StrafeSetting");
                    Transform newNumber = content.Find("FloorOffsetParameterPanel");
                    Transform newSlider = content.Find("VibrationScale");
                    foreach (KeyValuePair<string, object> option in category.options)
                    {
                        Type type = option.Value.GetType();
                        if (type == typeof(ActionInfo))
                        {
                            Transform newOption = Object.Instantiate(newAction, newAction.parent);
                            Object.Destroy(newOption.GetComponent<SettingsParameterUI>());
                            newOption.Find("Panel/ParamenerNameText").GetComponent<Text>().text = option.Key;
                            Transform button = newOption.Find("Panel/ChangeOffsetsButton");
                            button.gameObject.name = categories.IndexOf(category) + "." + content.childCount;
                            Object.Destroy(button.GetComponent<UITextFieldLocalisator>());
                            button.Find("StandartText").GetComponent<Text>().text = ((ActionInfo)option.Value).text;
                            button.Find("ChoosedText").GetComponent<Text>().text = ((ActionInfo)option.Value).text;
                            button.gameObject.AddComponent<MenuAction>();
                            if (!properties.ContainsKey(button.gameObject.name))
                            {
                                properties.Add(button.gameObject.name, option.Value);
                            }
                            newOption.gameObject.SetActive(true);
                        }
                        else if (type == typeof(BoolInfo))
                        {
                            Transform newOption = Object.Instantiate(newBool, newBool.parent);
                            Object.Destroy(newOption.GetComponent<UITextFieldLocalisator>());
                            Object.Destroy(newOption.GetComponent<ScriptableSettingUIChangerCheckbox>());
                            newOption.Find("Panel/ParamenerNameText").GetComponent<Text>().text = option.Key;
                            Transform button = newOption.Find("Panel/ButtonsPanel/ChangeValue");
                            button.gameObject.name = categories.IndexOf(category) + "." + content.childCount;
                            button.gameObject.AddComponent<MenuBool>();
                            if (!properties.ContainsKey(button.gameObject.name))
                            {
                                properties.Add(button.gameObject.name, option.Value);
                            }
                            newOption.gameObject.SetActive(true);
                        }
                        else if (type == typeof(IntInfo))
                        {
                            Transform newOption = Object.Instantiate(newNumber, newNumber.parent);
                            Object.Destroy(newOption.GetComponent<SettingsParameterUI>());
                            Object.Destroy(newOption.GetComponent<SettingsParameterChanger>());
                            newOption.Find("Panel/ParamenerNameText").GetComponent<Text>().text = option.Key;
                            Transform button = newOption.Find("Panel/ButtonsPanel");
                            button.gameObject.name = categories.IndexOf(category) + "." + content.childCount;
                            button.gameObject.AddComponent<MenuInt>();
                            if (!properties.ContainsKey(button.gameObject.name))
                            {
                                properties.Add(button.gameObject.name, option.Value);
                            }
                            newOption.gameObject.SetActive(true);
                        }
                        else if (type == typeof(FloatInfo))
                        {
                            Transform newOption = Object.Instantiate(newNumber, newNumber.parent);
                            Object.Destroy(newOption.GetComponent<SettingsParameterUI>());
                            Object.Destroy(newOption.GetComponent<SettingsParameterChanger>());
                            newOption.Find("Panel/ParamenerNameText").GetComponent<Text>().text = option.Key;
                            Transform button = newOption.Find("Panel/ButtonsPanel");
                            button.gameObject.name = categories.IndexOf(category) + "." + content.childCount;
                            button.gameObject.AddComponent<MenuFloat>();
                            if (!properties.ContainsKey(button.gameObject.name))
                            {
                                properties.Add(button.gameObject.name, option.Value);
                            }
                            newOption.gameObject.SetActive(true);
                        }
                        else if (type == typeof(EnumInfo))
                        {
                            Transform newOption = Object.Instantiate(newNumber, newNumber.parent);
                            Object.Destroy(newOption.GetComponent<SettingsParameterUI>());
                            Object.Destroy(newOption.GetComponent<SettingsParameterChanger>());
                            newOption.Find("Panel/ParamenerNameText").GetComponent<Text>().text = option.Key;
                            Transform button = newOption.Find("Panel/ButtonsPanel");
                            button.gameObject.name = categories.IndexOf(category) + "." + content.childCount;
                            button.gameObject.AddComponent<MenuEnum>();
                            if (!properties.ContainsKey(button.gameObject.name))
                            {
                                properties.Add(button.gameObject.name, option.Value);
                            }
                            newOption.gameObject.SetActive(true);
                        }
                        else if (type == typeof(SliderInfo))
                        {
                            Transform newOption = Object.Instantiate(newSlider, newSlider.parent);
                            Object.Destroy(newOption.GetComponent<SettingsParameterUI>());
                            Object.Destroy(newOption.GetComponent<SettingsParameterChangerScroll>());
                            newOption.Find("Panel/ParamenerNameText").GetComponent<Text>().text = option.Key;
                            Transform button = newOption.Find("Panel/ButtonsPanel");
                            button.gameObject.name = categories.IndexOf(category) + "." + content.childCount;
                            button.gameObject.AddComponent<MenuSlider>();
                            if (!properties.ContainsKey(button.gameObject.name))
                            {
                                properties.Add(button.gameObject.name, option.Value);
                            }
                            newOption.gameObject.SetActive(true);
                        }
                    }
                    Transform newPanelButton = Object.Instantiate(main, main.parent);
                    newPanelButton.gameObject.name = categories.IndexOf(category).ToString();
                    Object.Destroy(newPanelButton.GetComponent<UITextFieldLocalisator>());
                    newPanelButton.GetComponent<Text>().text = category.Name;
                    GameObject choosen = newPanelButton.Find("Choose").gameObject;
                    panelManager.panelUIs.Add(new()
                    {
                        name = newPanelButton.gameObject.name,
                        gameObjects = new GameObject[] { newMenu.gameObject, choosen }
                    });
                    choosen.SetActive(first);
                    newPanelButton.gameObject.AddComponent<CategoryButton>();
                    newPanelButton.gameObject.SetActive(true);
                    newMenu.gameObject.SetActive(first);
                    first = false;
                }
                List<WindowData> newWindows = manager._data.Windows.ToList();
                newWindows.Add(new()
                {
                    WindowType = (WindowType)20,
                    Window = modWindow
                });
                manager._data.Windows = newWindows.ToArray();
                if (manager._data.name == "PopUpWindowsData")
                {
                    createdNormal = true;
                }
                else
                {
                    manager.Awake();
                    createdRide = true;
                }
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    public class HBMFButton : MonoBehaviour
    {
        public HBMFButton(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            UnityEvent interactEvent = new();
            interactEvent.AddListener(new Action(() =>
            {
                MenuManager.manager.OpenWindow((WindowType)20);
            }));
            GetComponent<UIElement>().DoWhenInteract = interactEvent;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class CategoryButton : MonoBehaviour
    {
        public CategoryButton(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            PanelsManagerUI panelManager = transform.parent.parent.parent.parent.GetComponent<PanelsManagerUI>();
            UnityEvent interactEvent = new();
            interactEvent.AddListener(new Action(() =>
            {
                panelManager.SetActivePanelByName(gameObject.name);
            }));
            GetComponent<UIElement>().DoWhenInteract = interactEvent;
            MelonCoroutines.Start(FixCollider());
        }

        [HideFromIl2Cpp]
        private IEnumerator FixCollider()
        {
            RectTransform rect = GetComponent<RectTransform>();
            yield return new WaitForAutoScale(rect);
            BoxCollider collider = GetComponent<BoxCollider>();
            collider.size = new Vector3(rect.sizeDelta.x, collider.size.y, collider.size.z);
            collider.center = new Vector3(collider.size.x / 2f, collider.center.y, collider.center.z);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class WaitForAutoScale : CustomYieldInstruction
    {
        public readonly RectTransform rect;

        public override bool keepWaiting
        {
            get
            {
                return rect.sizeDelta == Vector2.zero;
            }
        }

        public WaitForAutoScale(IntPtr ptr) : base(ptr) { }

        public WaitForAutoScale(RectTransform newRect) : base(ClassInjector.DerivedConstructorPointer<WaitForAutoScale>())
        {
            ClassInjector.DerivedConstructorBody(this);
            rect = newRect;
        }
    }

    internal class ActionInfo
    {
        internal string text;
        internal Action action;

        internal ActionInfo(string newText, Action newAction)
        {
            text = newText;
            action = newAction;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class MenuAction : MonoBehaviour
    {
        public MenuAction(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            UnityEvent interactEvent = new();
            interactEvent.AddListener(((ActionInfo)MenuManager.properties[gameObject.name]).action);
            GetComponent<UIElement>().DoWhenInteract = interactEvent;
        }
    }

    internal class BoolInfo
    {
        internal bool state;
        internal Action<bool> onChange;

        internal BoolInfo(bool newState, Action<bool> newOnChange)
        {
            state = newState;
            onChange = newOnChange;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class MenuBool : MonoBehaviour
    {
        public MenuBool(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            BoolInfo properties = (BoolInfo)MenuManager.properties[gameObject.name];
            GameObject normal = transform.Find("Default/Filled").gameObject;
            normal.SetActive(properties.state);
            GameObject highlighted = transform.Find("Highlighted/Filled").gameObject;
            highlighted.SetActive(properties.state);
            UnityEvent interactEvent = new();
            interactEvent.AddListener(new Action(() =>
            {
                properties.state = !properties.state;
                normal.SetActive(properties.state);
                highlighted.SetActive(properties.state);
                properties.onChange.Invoke(properties.state);
            }));
            GetComponent<UIElement>().DoWhenStopInteract = interactEvent;
        }
    }

    internal class IntInfo
    {
        internal int state;
        internal int min;
        internal int max;
        internal int step;
        internal string units;
        internal Action<int> onChange;

        internal IntInfo(int newState, int newMin, int newMax, int newStep, string newUnits, Action<int> newOnChange)
        {
            state = newState;
            min = newMin;
            max = newMax;
            step = newStep;
            units = newUnits;
            onChange = newOnChange;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class MenuInt : MonoBehaviour
    {
        public MenuInt(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            IntInfo properties = (IntInfo)MenuManager.properties[gameObject.name];
            Text text = transform.Find("ParameterValueText").GetComponent<Text>();
            text.text = properties.state + properties.units;
            UnityEvent increaseEvent = new();
            increaseEvent.AddListener(new Action(() =>
            {
                properties.state = Mathf.Min(properties.state + properties.step, properties.max);
                text.text = properties.state + properties.units;
                properties.onChange.Invoke(properties.state);
            }));
            transform.GetChild(2).GetComponent<UIElement>().DoWhenStopInteract = increaseEvent;
            UnityEvent decreaseEvent = new();
            decreaseEvent.AddListener(new Action(() =>
            {
                properties.state = Mathf.Max(properties.state - properties.step, properties.min);
                text.text = properties.state + properties.units;
                properties.onChange.Invoke(properties.state);
            }));
            transform.GetChild(0).GetComponent<UIElement>().DoWhenStopInteract = decreaseEvent;
        }
    }

    internal class FloatInfo
    {
        internal float state;
        internal float min;
        internal float max;
        internal float step;
        internal string units;
        internal Action<float> onChange;

        internal FloatInfo(float newState, float newMin, float newMax, float newStep, string newUnits, Action<float> newOnChange)
        {
            state = newState;
            min = newMin;
            max = newMax;
            step = newStep;
            units = newUnits;
            onChange = newOnChange;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class MenuFloat : MonoBehaviour
    {
        public MenuFloat(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            FloatInfo properties = (FloatInfo)MenuManager.properties[gameObject.name];
            Text text = transform.Find("ParameterValueText").GetComponent<Text>();
            text.text = properties.state + properties.units;
            UnityEvent increaseEvent = new();
            increaseEvent.AddListener(new Action(() =>
            {
                properties.state = Mathf.Min(properties.state + properties.step, properties.max);
                text.text = properties.state + properties.units;
                properties.onChange.Invoke(properties.state);
            }));
            transform.GetChild(2).GetComponent<UIElement>().DoWhenStopInteract = increaseEvent;
            UnityEvent decreaseEvent = new();
            decreaseEvent.AddListener(new Action(() =>
            {
                properties.state = Mathf.Max(properties.state - properties.step, properties.min);
                text.text = properties.state + properties.units;
                properties.onChange.Invoke(properties.state);
            }));
            transform.GetChild(0).GetComponent<UIElement>().DoWhenStopInteract = decreaseEvent;
        }
    }

    internal class EnumInfo
    {
        internal int state;
        internal string[] states;
        internal Action<int> onChange;

        internal EnumInfo(int newState, string[] newStates, Action<int> newOnChange)
        {
            state = newState;
            states = newStates;
            onChange = newOnChange;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class MenuEnum : MonoBehaviour
    {
        public MenuEnum(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            EnumInfo properties = (EnumInfo)MenuManager.properties[gameObject.name];
            Text text = transform.Find("ParameterValueText").GetComponent<Text>();
            text.text = properties.states[properties.state];
            UnityEvent increaseEvent = new();
            increaseEvent.AddListener(new Action(() =>
            {
                properties.state++;
                if (properties.state == properties.states.Length)
                {
                    properties.state = 0;
                }
                text.text = properties.states[properties.state].ToString();
                properties.onChange.Invoke(properties.state);
            }));
            transform.GetChild(2).GetComponent<UIElement>().DoWhenStopInteract = increaseEvent;
            UnityEvent decreaseEvent = new();
            decreaseEvent.AddListener(new Action(() =>
            {
                properties.state--;
                if (properties.state == -1)
                {
                    properties.state = properties.states.Length - 1;
                }
                text.text = properties.states[properties.state].ToString();
                properties.onChange.Invoke(properties.state);
            }));
            transform.GetChild(0).GetComponent<UIElement>().DoWhenStopInteract = decreaseEvent;
        }
    }

    internal class SliderInfo
    {
        internal float state;
        internal float min;
        internal float max;
        internal int steps;
        internal string units;
        internal Action<float> onChange;

        internal SliderInfo(float newState, float newMin, float newMax, int newSteps, string newUnits, Action<float> newOnChange)
        {
            state = newState;
            min = newMin;
            max = newMax;
            steps = newSteps;
            units = newUnits;
            onChange = newOnChange;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class MenuSlider : MonoBehaviour
    {
        public MenuSlider(IntPtr ptr) : base(ptr) { }

        internal void OnEnable()
        {
            HorizontalVRScrollbar scrollbar = transform.Find("Scrollbar Horizontal").GetComponent<HorizontalVRScrollbar>();
            SliderInfo properties = (SliderInfo)MenuManager.properties[gameObject.name];
            scrollbar._scrollbar.numberOfSteps = properties.steps;
            Text text = transform.Find("SliderControlContainer/ParameterValueSliderText").GetComponent<Text>();
            text.text = properties.min + (properties.max - properties.min) * properties.state + properties.units;
            Scrollbar.ScrollEvent scrollEvent = new();
            bool fire = false;
            scrollEvent.AddListener(new Action<float>((float value) =>
            {
                properties.state = value;
                float newValue = properties.min + (properties.max - properties.min) * value;
                text.text = newValue + properties.units;
                if (fire)
                {
                    properties.onChange.Invoke(newValue);
                }
            }));
            scrollbar._scrollbar.onValueChanged = scrollEvent;
            scrollbar._scrollbar.value = properties.state;
            fire = true;
            UnityEvent increaseEvent = new();
            increaseEvent.AddListener(new Action(() =>
            {
                scrollbar._scrollbar.value = Mathf.Min(scrollbar._scrollbar.value + (1f / properties.steps), 1f);
                scrollbar.EndDrag();
            }));
            transform.Find("Scrollbar Horizontal/Sliding Area/Handle/ButtonHolder/IncreaseValue").GetComponent<UIElement>().DoWhenInteract = increaseEvent;
            UnityEvent decreaseEvent = new();
            decreaseEvent.AddListener(new Action(() =>
            {
                scrollbar._scrollbar.value = Mathf.Max(scrollbar._scrollbar.value - (1f / properties.steps), 0f);
                scrollbar.EndDrag();
            }));
            transform.Find("Scrollbar Horizontal/Sliding Area/Handle/ButtonHolder/DecreaseValue").GetComponent<UIElement>().DoWhenInteract = decreaseEvent;
        }
    }
}
