using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace BulletMenuVR
{
    public class VrMenu
    {
        public static GameObject menuObject;
        public static List<VrMenuButton> registeredButtons = new List<VrMenuButton>();

        public static void RegisterMainButton(VrMenuButton button)
        {
            MelonLogger.Msg("Registered button with label: " + button.label);
            registeredButtons.Add(button);
        }

        public static void RefreshMenu()
        {
            foreach (VrMenuButton button in registeredButtons)
            {
                AddMainButton(button);
            }
        }

        public static void ShowPage(int page)
        {
            MenuBehavior menuBehavior = menuObject.GetComponent<MenuBehavior>();
            menuBehavior.pageIndex = 0;
            menuBehavior.ShowPage();
        }

        private static void AddMainButton(VrMenuButton vrMenuButton)
        {
            MenuBehavior menuBehavior = menuObject.GetComponent<MenuBehavior>();
            menuBehavior.AddButton(vrMenuButton.label, vrMenuButton.buttonAction);
        }

        public static void DisplayButtonsManual(List<VrMenuButton> buttons)
        {
            List<GameObject> buttonObjects = new List<GameObject>();
            MenuBehavior menuBehavior = menuObject.GetComponent<MenuBehavior>();
            foreach (var menuButton in buttons)
            {
                buttonObjects.Add(menuBehavior.MakeButton(menuButton.label, menuButton.buttonAction));
            }
            menuBehavior.ShowButtonList(buttonObjects);
        }
    }
}