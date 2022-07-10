using System;
using UnityEngine;

namespace BulletMenuVR
{
    public class VrMenuButton
    {
        public string label;
        public Action buttonAction;

        public VrMenuButton(string label, Action buttonAction)
        {
            this.label = label;
            this.buttonAction = buttonAction;
        }
    }
}