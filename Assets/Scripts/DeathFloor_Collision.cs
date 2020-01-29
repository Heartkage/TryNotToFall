using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    public class DeathFloor_Collision : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.CompareTo("Player") == 0)
            {
                AudioManager.instance.SoundOutput(SoundType.GameOver, SoundAction.Play);
                GameManager_Main.instance.GamePause(GameStopType.Die);
            }
            else
            {
                Object_BaseScript script = other.gameObject.GetComponent<Object_BaseScript>();
                if (script != null)
                    script.DestoryObject();
                    
            }
        }

    }
}

