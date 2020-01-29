using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TNTF
{
    public class HintControl : MonoBehaviour
    {
        #region Singleton
        public static HintControl instance;
        void Awake()
        {
            instance = this;
        }
        #endregion 

        [SerializeField]
        private Image timerBackground;
        [SerializeField]
        private Image fillBar;
        [SerializeField]
        private Image hintColor;
        [SerializeField]
        private Sprite unknownImage;
        [SerializeField]
        private Sprite trollFace;

        private Color invisibleColor;
        private Color halfInvisibleColor;

        private bool timerIsOn;
        private bool hintIsOn;
        
        void Start()
        {
            invisibleColor = new Color(1f, 1f, 1f, 0);
            halfInvisibleColor = new Color(1f, 1f, 1f, 0.4f);
            timerIsOn = false;
            hintIsOn = false;
            DisableTimer();
            DisableHintImage();
        }

        public void HintSetup(bool useTimer, bool useHint)
        {
            if(useTimer)
                EnableTimer();
            else
                DisableTimer();

            if(useHint)
                EnableHintImage();
            else
                DisableHintImage();
        }

        void EnableTimer()
        {
            if(!timerIsOn)
            {
                fillBar.color = Color.white;
                timerBackground.color = Color.white;
                timerIsOn = true;
            }
        }

        void DisableTimer()
        {
            if(timerIsOn)
            {
                fillBar.color = invisibleColor;
                timerBackground.color = halfInvisibleColor;
                timerIsOn = false;
            }    
        }

        void EnableHintImage()
        {
            if(!hintIsOn)
            {
                hintColor.sprite = null;
                hintColor.color = Color.white;
                hintIsOn = true;
            } 
        }

        void DisableHintImage()
        {
            if(hintIsOn)
            {
                hintColor.sprite = unknownImage;
                hintColor.color = halfInvisibleColor;
                hintIsOn = false;
            } 
        }

        public void SetFillPercent(float amount)
        {
            if (timerIsOn)
                fillBar.fillAmount = amount;
        }

        public void SetHintColors(Color c)
        {
            if (timerIsOn)
                fillBar.color = c;

            if (hintIsOn)
                hintColor.color = c;
            else
            {
                fillBar.color = halfInvisibleColor;
            }     
        }
        

    }
}


