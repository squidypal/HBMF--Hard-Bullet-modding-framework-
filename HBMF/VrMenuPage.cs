using System.Collections.Generic;

namespace BulletMenuVR
{
    public class VrMenuPage
    {
        private List<VrMenuButton> buttons;

        public VrMenuPage(List<VrMenuButton> vrMenuButtons)
        {
            buttons = new List<VrMenuButton>();
            buttons = vrMenuButtons;
        }

        public void Open()
        {
            VrMenu.DisplayButtonsManual(buttons);
        }
    }

    public class VrMenuPageBuilder
    {

        private List<VrMenuButton> buttons;

        public VrMenuPageBuilder()
        {
            buttons = new List<VrMenuButton>();
        }

        public VrMenuPageBuilder AddButton(VrMenuButton button)
        {
            buttons.Add(button);
            return this;
        }

        public static VrMenuPageBuilder Builder()
        {
            return new VrMenuPageBuilder();
        }

        public VrMenuPage Build()
        {
            return new VrMenuPage(buttons);
        }
    }
}