using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TNTF
{
    [RequireComponent(typeof(GameManager_Main))]
    public class GameManager_LevelChanger : MonoBehaviour
    {
        #region Singleton
        public static GameManager_LevelChanger instance;
        void Awake()
        {
            instance = this;
        }
        #endregion

        private GameManager_Main gameManagerMain;
        public float offset = 40f;
        public DeathFloor deathGround;

        void OnEnable()
        {
            gameManagerMain = GetComponent<GameManager_Main>();
            gameManagerMain.StartLevelEvents += SetDeathFloorSize;
        }

        void OnDisable()
        {
            gameManagerMain.StartLevelEvents -= SetDeathFloorSize;
        }

        void SetDeathFloorSize(int level)
        {
            deathGround.floorObject.transform.position = deathGround.initialPosition;
        }
 
    }

    [System.Serializable]
    public class DeathFloor
    {
        public GameObject floorObject;
        public Vector3 initialPosition;
        public Vector3 floorBaseSize;
        public float floorScaleSize;
    }
}

