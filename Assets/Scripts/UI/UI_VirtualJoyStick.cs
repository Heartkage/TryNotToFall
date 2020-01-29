using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace TNTF
{
    public class UI_VirtualJoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {

        private Image backgroundImage;
        private Image joystickImage;
        private Vector3 inputVector;

        void Awake()
        {
            backgroundImage = GetComponent<Image>();
            joystickImage = transform.GetChild(0).GetComponent<Image>();
        }

        void Start()
        {
            GameManager_Main.instance.StartLevelEvents += ResetVirtualJoystick;
        }

        void OnDestroy()
        {
            GameManager_Main.instance.StartLevelEvents -= ResetVirtualJoystick;
        }

        void ResetVirtualJoystick(int notUsed)
        {
            inputVector = Vector3.zero;
            joystickImage.rectTransform.anchoredPosition = Vector3.zero;
        }


        public virtual void OnDrag(PointerEventData ped)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundImage.rectTransform, ped.position, ped.pressEventCamera, out pos))
            {
                pos.x = (pos.x / backgroundImage.rectTransform.sizeDelta.x);
                pos.y = (pos.y / backgroundImage.rectTransform.sizeDelta.y);

                inputVector = new Vector3(pos.x * 2 - 1f, 0, pos.y * 2 - 1f);
                inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
                

                joystickImage.rectTransform.anchoredPosition = new Vector3(inputVector.x * backgroundImage.rectTransform.sizeDelta.x / 2.7f,
                                                                           inputVector.z * backgroundImage.rectTransform.sizeDelta.y / 2.7f, 0);

            }

        }

        public virtual void OnPointerDown(PointerEventData ped)
        {
            OnDrag(ped);
        }

        public virtual void OnPointerUp(PointerEventData ped)
        {
            inputVector = Vector3.zero;
            joystickImage.rectTransform.anchoredPosition = Vector3.zero;
        }

        public Vector3 GetHorizontalAndVerticleMovement()
        {
            if (inputVector.magnitude != 0)
                return inputVector;
            else
                return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }


    }
}

