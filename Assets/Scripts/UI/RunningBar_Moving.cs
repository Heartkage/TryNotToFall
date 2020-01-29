using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TNTF
{
    [RequireComponent(typeof(RectTransform))]
    public class RunningBar_Moving : MonoBehaviour
    {
        public float fillUpPercent;

        private RectTransform rectTransform;
        private float width;
        private float startXpos;
        private float currentXpos;

        void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
            width = rectTransform.rect.width;
            startXpos = -width;
            fillUpPercent = 0f;
            SetBar(fillUpPercent);
        }

        public void SetBar(float percent)
        {
            fillUpPercent = percent;
            currentXpos = startXpos + width * fillUpPercent;
            rectTransform.localPosition = new Vector3(currentXpos, 0, 0);
        }

    }
}

