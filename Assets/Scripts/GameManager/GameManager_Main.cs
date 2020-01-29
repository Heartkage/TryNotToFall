using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TNTF
{
    public class GameManager_Main : MonoBehaviour
    {

        #region Singleton
        public static GameManager_Main instance;
        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this);
            gameState = GameState.Menu;
        }
        #endregion

        #region Events
        public delegate void Begin(int lv);
        public event Begin StartGame_FirstLevel;
        public event Begin StartLevelEvents;

        public delegate void Stop(GameStopType end);
        public event Stop StopGame;

        #endregion


        [SerializeField]
        GameState gameState;
        public GameState GetGameState()
        {
            return gameState;
        }
        public bool IsItInThisGameState(GameState state)
        {
            return (state == gameState) ? true : false;
        }

        void Start()
        {
            gameState = GameState.Menu;
        }

        public void StartGame()
        {
            if (StartGame_FirstLevel != null)
                StartGame_FirstLevel(0);
            if (StartLevelEvents != null)
                StartLevelEvents(GameManager_LevelSettings.instance.GetCurrentLevelInfoIndex());
            AudioManager.instance.SoundOutput(SoundType.Main, SoundAction.Play);
            gameState = GameState.InGame;
        }

        public void ReloadLevel()
        {
            if (StartLevelEvents != null)
                StartLevelEvents(GameManager_LevelSettings.instance.GetCurrentLevelInfoIndex());
            AudioManager.instance.SoundOutput(SoundType.Main, SoundAction.Play);
            gameState = GameState.InGame;
        }

        public void GamePause(GameStopType type)
        {
            if (type == GameStopType.Die)
            {
                bool stillAlive = Player_Movement.instance.IsPlayerStillAliveAfterThisDeath();
                if (!stillAlive)
                {
                    gameState = GameState.GameOver;
                    if (StopGame != null)
                        StopGame(GameStopType.Finish);
                }
                else
                {
                    gameState = GameState.Menu;
                    if (StopGame != null)
                        StopGame(type);
                    StartCoroutine(DelayRestart());
                }
            }
            else if (type == GameStopType.NextLevel)
            {
                gameState = GameState.Menu;
                if (StopGame != null)
                    StopGame(type);
                StartCoroutine(CurrentLevelComplete()) ;
            }
            else if (type == GameStopType.Finish)
            {
                gameState = GameState.Menu;
                if (StopGame != null)
                    StopGame(type);
            }
        }

        IEnumerator DelayRestart()
        {
            yield return new WaitForSeconds(4f);
            ReloadLevel();
        }

        IEnumerator CurrentLevelComplete()
        {
            //---All Levels Completed---//
            if (!GameManager_LevelSettings.instance.MoveToNextLevelIndex())
            {
                AudioManager.instance.SoundOutput(SoundType.GameComplete, SoundAction.Play);
                GamePause(GameStopType.Finish);    
            }
            else
            {
                AudioManager.instance.SoundOutput(SoundType.LevelComplete, SoundAction.Play);
                yield return new WaitForSeconds(2f);
                ReloadLevel();
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }

    public enum GameState
    {
        Menu = 0,
        InGame = 1,
        GameOver = 2
    }

    public enum GameStopType
    {
        Finish = 0,
        Die = 1,
        NextLevel = 2,
        Pause = 3
    }
}


