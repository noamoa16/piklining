using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class Button : MonoBehaviour
    {
        public bool isPressed { get; private set; } = false;
        public void OnPushDown()
        {
            isPressed = true;
        }

        public void OnPushUp()
        {
            isPressed = false;
        }
    }
}
