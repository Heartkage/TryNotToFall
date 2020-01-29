using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    public class Player_CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private GameObject player;
        private Vector3 cameraInitialOffset;
        private Vector3 nextPos;
        private Vector3 initialPos;

        [SerializeField]
        private GameManager_Main gameManagerMain;

        [Range(0.01f, 1f)]
        public float smoothFactor = 0.5f;

        void OnEnable()
        {
            initialPos = transform.position;
            cameraInitialOffset = initialPos - player.transform.position;
            gameManagerMain.StartLevelEvents += StartCameraPosition;
        }

        void OnDisable()
        {
            gameManagerMain.StartLevelEvents -= StartCameraPosition;
        }

        void StartCameraPosition(int level)
        {
            transform.position = initialPos;
        }

        void LateUpdate()
        {
            nextPos = player.transform.position + cameraInitialOffset;

            transform.position = Vector3.Slerp(transform.position, nextPos, smoothFactor);
        }


    }
}


