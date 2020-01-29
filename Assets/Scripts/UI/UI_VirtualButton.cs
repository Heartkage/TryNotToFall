using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TNTF
{
    public class UI_VirtualButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public virtual void OnPointerDown(PointerEventData ped)
        {
            Player_Movement.instance.SetJump();
        }

        public virtual void OnPointerUp(PointerEventData ped)
        {
            Player_Movement.instance.UnsetJump();
        }
    }
}


