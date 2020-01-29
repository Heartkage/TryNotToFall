using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TNTF
{
    [RequireComponent(typeof(Animator))]
    public class UI_CanvasTransition : MonoBehaviour
    {
        #region Singleton
        public static UI_CanvasTransition instance;
        void Awake()
        {
            instance = this;
            anime = GetComponent<Animator>();
        }
        #endregion


        [SerializeField]
        Text stopGameTitle;


        private Animator anime;

        void OnDisable()
        {
            GameManager_Main.instance.StartLevelEvents -= MenuTransitionToInGame;
            GameManager_Main.instance.StopGame -= BringUpStopGameCanvas;
        }

        void Start()
        {
            GameManager_Main.instance.StartLevelEvents += MenuTransitionToInGame;
            GameManager_Main.instance.StopGame += BringUpStopGameCanvas;
        }

        void MenuTransitionToInGame(int notUsed)
        {
            anime.SetBool("InGame", true);
        }

        void BringUpStopGameCanvas(GameStopType type)
        {
            anime.SetBool("InGame", false);
            if (type == GameStopType.Die)
            {
                stopGameTitle.text = string.Concat(Player_Movement.instance.GetPlayerRemainingLife().ToString() + " life remaining");
                anime.SetTrigger("ShowTitle");
            }
            else if (type == GameStopType.NextLevel)
            {
                stopGameTitle.text = string.Concat("Congratulations, moving on to Level " + (GameManager_LevelSettings.instance.GetCurrentLevel() + 1).ToString());
                anime.SetTrigger("ShowTitle");
            }
            else if (type == GameStopType.Finish)
            {
                stopGameTitle.text = "Game Over";
                anime.SetTrigger("StopGame");
            }


        }

        public void BackToMenu()
        {
            AudioManager.instance.SoundOutput(SoundType.Main, SoundAction.Stop);
            Player_Movement.instance.SetInitialReferences();
            anime.SetTrigger("BackToMenu");
        }

    }
}
