using System;
using UnityEngine;

namespace BulletMenuVR
{
    public class VrMenuButton
    {
        public string label;
        public Action buttonAction;
        public Color color;

        public VrMenuButton(string label, Action buttonAction)
        {
            this.label = label;
            this.buttonAction = buttonAction;
            this.color = Color.white;
        }
        
        public VrMenuButton(string label, Action buttonAction, Color color)
        {
            this.label = label;
            this.buttonAction = buttonAction;
            this.color = color;
        }
    }
}