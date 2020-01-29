using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    [RequireComponent(typeof(GameManager_Main))]
    public class GameManager_LevelSettings : MonoBehaviour
    {
        public Scriptable_Level[] levelInfo;
        private int currentLevelInfoIndex;
        private int currentLevel;

        #region Singleton
        public static GameManager_LevelSettings instance;
        void Awake()
        {
            instance = this;
        }
        #endregion

        void Start()
        {
            GameManager_Main.instance.StartGame_FirstLevel += SetStartLevel;
        }

        void OnDisable()
        {
            GameManager_Main.instance.StartGame_FirstLevel -= SetStartLevel;
        }

        void SetStartLevel(int notUsed)
        {
            currentLevelInfoIndex = 0;
            currentLevel = 1;
        }

        public bool MoveToNextLevelIndex()
        {
            currentLevelInfoIndex++;
            currentLevel++;

            if (currentLevelInfoIndex >= levelInfo.Length)
            {
                currentLevelInfoIndex = 0;
            }

            return true; 
        }


        public float GetCurrentLevelDistance()
        {
            return levelInfo[currentLevelInfoIndex].distance;
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public int GetCurrentLevelInfoIndex()
        {
            return currentLevelInfoIndex;
        }
        public int GetCurrentLevelInfo()
        {
            return levelInfo[currentLevelInfoIndex].level;
        }
        public string GetCurrentLevelInString()
        {
            return levelInfo[currentLevelInfoIndex].level.ToString();
        }
    }

}

