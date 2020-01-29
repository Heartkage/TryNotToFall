using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TNTF
{
    [RequireComponent(typeof(CharacterController))]
    public class Player_Movement : MonoBehaviour
    {

        #region Singleton
        public static Player_Movement instance;
        void Awake()
        {
            instance = this;
        }
        #endregion 

        private MeshRenderer body;

        public float jumpForce;
        public float forwardSpeed;
        public float groundForce;

        [SerializeField]
        private GameManager_Main gameManagerMain;
        [SerializeField]
        private RunningBar_Moving runningBar;
        [SerializeField]
        private UI_VirtualJoyStick virtualJoyStick;
        [SerializeField]
        private Text runningBar_DistanceText;


        private CharacterController controller;
        private Vector3 moveDir;

        private float requiredDistance;
        private float movedDistance;

        private int playerLife;

        bool shouldJump;

        void OnEnable()
        {            
            SetInitialReferences();
            gameManagerMain.StartGame_FirstLevel += SetPlayerStartingLifeAmount;
            gameManagerMain.StartLevelEvents += SetPlayerInitialPosition;
        }

        void OnDisable()
        {
            gameManagerMain.StartGame_FirstLevel -= SetPlayerStartingLifeAmount;
            gameManagerMain.StartLevelEvents -= SetPlayerInitialPosition;
        }

        void SetPlayerStartingLifeAmount(int notUsed)
        {
            playerLife = 3;
        }

        public bool IsPlayerStillAliveAfterThisDeath()
        {
            playerLife--;
            return (playerLife < 1) ? false : true;
        }

        public int GetPlayerRemainingLife()
        {
            return playerLife;
        }

        void SetPlayerInitialPosition(int lv)
        {
            body.enabled = true;
            float x = GameManager_LevelChanger.instance.deathGround.floorBaseSize.x + lv * GameManager_LevelChanger.instance.deathGround.floorScaleSize;
            float y = GameManager_LevelChanger.instance.deathGround.floorBaseSize.y;
            float z = GameManager_LevelChanger.instance.deathGround.floorBaseSize.z + lv * GameManager_LevelChanger.instance.deathGround.floorScaleSize;
            Vector3 pos = new Vector3(x/2, y, -z/2);
            this.transform.position = pos;       
            DistanceReset();
            runningBar.SetBar(0);
        }

        public void SetInitialReferences()
        {
            controller = GetComponent<CharacterController>();
            body = GetComponent<MeshRenderer>();
            body.enabled = false;
            shouldJump = false;
        }

        public void SetPlayerProperHeight()
        {
            float x = transform.position.x;
            float y = GameManager_LevelChanger.instance.deathGround.floorBaseSize.y;
            float z = transform.position.z;
            Vector3 pos = new Vector3(x, y, z);
            this.transform.position = pos;   
        }

        void DistanceReset()
        {
            movedDistance = 0;
            requiredDistance = GameManager_LevelSettings.instance.GetCurrentLevelDistance();
        }

        void FixedUpdate()
        {
            if (movedDistance < requiredDistance)
                CheckInput();
            else if (GameManager_Main.instance.IsItInThisGameState(GameState.InGame))
                GameManager_Main.instance.GamePause(GameStopType.NextLevel);                    
        }

        public void SetJump()
        {
            shouldJump = true;
        }
        public void UnsetJump()
        {
            shouldJump = false;
        }

        void CheckInput()
        {
            if (controller.isGrounded)
            {
                moveDir = virtualJoyStick.GetHorizontalAndVerticleMovement();
                moveDir = transform.TransformDirection(moveDir) * forwardSpeed;

                if (((Input.GetAxis("Jump") > 0) || shouldJump) && gameManagerMain.IsItInThisGameState(GameState.InGame))
                {
                    moveDir.y = jumpForce;
                    AudioManager.instance.SoundOutput(SoundType.Jump, SoundAction.Play);
                }
            }
            else
            {
                Vector3 dir = virtualJoyStick.GetHorizontalAndVerticleMovement();
                dir = new Vector3(dir.x * forwardSpeed, moveDir.y, dir.z * forwardSpeed);
                moveDir = transform.TransformDirection(dir);
            }

            if (gameManagerMain.IsItInThisGameState(GameState.InGame))
            {
                movedDistance += new Vector3(moveDir.x, 0, moveDir.z).magnitude * Time.deltaTime;
                runningBar.SetBar(Mathf.Clamp((movedDistance / requiredDistance), 0, 1));
                UpdateDistanceText();
            }
            else
            {
                moveDir.x = 0;
                moveDir.z = 0;
            }
        
            moveDir.y -= groundForce * Time.deltaTime;
            controller.Move(moveDir * Time.deltaTime);
        }

        void UpdateDistanceText()
        {
            runningBar_DistanceText.text = string.Concat("Level: " + GameManager_LevelSettings.instance.GetCurrentLevel().ToString() +
                                                         "       " + movedDistance.ToString("F2") + " / " + requiredDistance.ToString("F2") +
                                                         " m");
        }

    }
}

