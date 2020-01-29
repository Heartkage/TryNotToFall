using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    public class GameObjectPool : MonoBehaviour
    {
        public static GameObjectPool instance;

        
        public GameObject _cubePrefab;
        [SerializeField]
        int initialCubeAmount = 10;

        public GameObject _itemPrefab;
        [SerializeField]
        int initialItemAmount = 10;

        List<GameObject> availableCubes;
        List<GameObject> availableItems;

        void Awake()
        {
            instance = this;
            availableCubes = new List<GameObject>();
            availableItems = new List<GameObject>();

            for (int i = 0; i < initialCubeAmount; i++)
            {
                GameObject go = Instantiate<GameObject>(_cubePrefab, this.transform);
                availableCubes.Add(go);
                go.SetActive(false);
            }

            for (int i = 0; i < initialItemAmount; i++)
            {
                GameObject go = Instantiate<GameObject>(_itemPrefab, this.transform);
                availableItems.Add(go);
                go.SetActive(false);
            }
        }

        public GameObject InstanceCubeFromPool(Transform parent, Vector3 pos)
        {
            lock (availableCubes)
            {
                int lastIndex = availableCubes.Count - 1;

                if (lastIndex >= 0)
                {
                    GameObject go = availableCubes[lastIndex];
                    if (go.transform.parent != parent)
                        go.transform.SetParent(parent);
                    

                    go.transform.position = pos;
                    availableCubes.RemoveAt(lastIndex);
                    go.SetActive(true);
                    
                    return go;

                }
                else
                {
                    return Instantiate<GameObject>(_cubePrefab, pos, Quaternion.identity, parent);
                }
            }
        }

        public void CubeBackToPool(GameObject go)
        {
            lock (availableCubes)
            {
                availableCubes.Add(go);
                go.SetActive(false);
            }
        }

        public GameObject InstanceItemFromPool(Transform parent, Vector3 pos)
        {
            lock (availableItems)
            {
                int lastIndex = availableItems.Count - 1;
                if (lastIndex >= 0)
                {
                    GameObject go = availableItems[lastIndex];
                    if (go.transform.parent != parent)
                        go.transform.SetParent(parent);

                    go.transform.position = pos;
                    availableItems.RemoveAt(lastIndex);
                    go.SetActive(true);
                    return go;
                }
                else
                {
                    return Instantiate<GameObject>(_itemPrefab, pos, Quaternion.identity, parent);
                }
            }
        }

        public void ItemBackToPool(GameObject go)
        {
            lock (availableItems)
            {
                availableItems.Add(go);
                go.SetActive(false);
            }
        }
    }
}


