using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    public class Cube_Script : Object_BaseScript
    {
        bool startDropping = false;
        bool startRepairing = false;

        private Vector3 currentPosition;
        private Vector3 toPosition;

        private Material originalMaterial;
        private Material fadedMaterial;
        private Color fadedColor;
        Renderer rend;

        float time;
        float lerp;
        float dropDuration = 5f;
        float repairDuration = 1f;

        void OnEnable()
        {
            rend = GetComponent<Renderer>();
            startDropping = false;
            startRepairing = false;
        }

        public void AddCubeToDropEvent()
        {
            GameManager_FloorGenerator.instance.DropCubes += DropCube;
        }

        public void RemoveCubeEvent()
        {
            GameManager_FloorGenerator.instance.DropCubes -= DropCube;
        }

        void SetMaterials()
        {
            originalMaterial = rend.material;
            fadedColor = rend.material.color;
            fadedColor.a = 0;
            fadedMaterial = new Material(originalMaterial);
            fadedMaterial.color = fadedColor;
        }

        void DropCube()
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().constraints = (RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<BoxCollider>().isTrigger = true;
            SetMaterials();
            

            time = 0;
            startDropping = true;
            
        }

        void FixedUpdate()
        {
            if (startDropping)
            {
                time += Time.deltaTime;
                lerp = time / dropDuration;
                
                rend.material.Lerp(originalMaterial, fadedMaterial, lerp);
            }
            else if (startRepairing)
            {
                time += Time.deltaTime;
                lerp = time / repairDuration;

                rend.material.Lerp(fadedMaterial, originalMaterial, lerp);
                this.gameObject.transform.position = Vector3.Lerp(currentPosition, toPosition, lerp);

                if (lerp >= 1f)
                    startRepairing = false;
            }

        }

        public void RespawnCube(Vector3 toPos)
        {
            time = 0;
            currentPosition = this.transform.position;
            toPosition = toPos;
            SetMaterials();
            startRepairing = true;
            rend.material = fadedMaterial;
        }

        public override void DestoryObject()
        {
            base.DestoryObject();

            RemoveCubeEvent();
            startDropping = false;
            GetComponent<BoxCollider>().isTrigger = false;
            GameObjectPool.instance.CubeBackToPool(this.transform.gameObject);

            //StartCoroutine(DestroyBlock());
        }

        /*IEnumerator DestroyBlock()
        {
            yield return new WaitForSeconds(0.5f);

            startDropping = false;
            GetComponent<BoxCollider>().isTrigger = false;
            GameObjectPool.instance.CubeBackToPool(this.transform.gameObject);
        }*/
    }
}

