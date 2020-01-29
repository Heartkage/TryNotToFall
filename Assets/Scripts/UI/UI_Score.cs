using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TNTF
{
    public class UI_Score : MonoBehaviour
    {
        #region Singleton
        public static UI_Score instance;
        void Awake()
        {
            instance = this;
        }
        #endregion


        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private int basePoint;
        [SerializeField]
        private int digitAmount;
        private long score;

        private float timeInOneSec;

        void Update()
        {
            if (GameManager_Main.instance.IsItInThisGameState(GameState.InGame))
            {
                timeInOneSec += Time.deltaTime;
 
                if (timeInOneSec >= 1f)
                {
                    timeInOneSec = 0;
                    score += (int)(basePoint * GameManager_LevelSettings.instance.GetCurrentLevel());
                    string scoreInText = score.ToString();

                    for (int i = scoreInText.Length; i < digitAmount; i++)
                        scoreInText = scoreInText.Insert(0, "0");

                    scoreText.text = scoreInText;
                }
            }
        }

        void Start()
        {
            GameManager_Main.instance.StartGame_FirstLevel += ResetScore;
        }

        void OnDestroy()
        {
            GameManager_Main.instance.StartGame_FirstLevel -= ResetScore;
        }

        void ResetScore(int notUsed)
        {
            score = 0;
            timeInOneSec = 0;
            scoreText.text = "";
            for (int i = 0; i < digitAmount; i++)
                scoreText.text = scoreText.text.Insert(0, "0");  
        }

        
    }
}


