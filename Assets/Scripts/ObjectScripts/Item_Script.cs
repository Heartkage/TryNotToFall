using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    public class Item_Script : Object_BaseScript
    {
        public virtual void Interact()
        {

        }

        void OnEnable()
        {
            GameManager_Main.instance.StopGame += BackToPool;
        }

        void OnDisable()
        {
            GameManager_Main.instance.StopGame -= BackToPool;
        }

        void BackToPool(GameStopType end)
        {
            GameObjectPool.instance.ItemBackToPool(this.gameObject);
        }

        public override void DestoryObject()
        {
            base.DestoryObject();
            GameObjectPool.instance.ItemBackToPool(this.transform.gameObject);
        }
    }
}

