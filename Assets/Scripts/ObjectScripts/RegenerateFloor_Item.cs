using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    public class RegenerateFloor_Item : Item_Script
    {
        public override void Interact()
        {
            base.Interact();
            GameManager_FloorGenerator.instance.RegenerateMissingFloor();
        }

        

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.CompareTo("Player") == 0)
            {
                Interact();
                AudioManager.instance.SoundOutput(SoundType.Collect, SoundAction.Play);
                GameObjectPool.instance.ItemBackToPool(this.gameObject);
            }
        }
    }

}

