using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    [CreateAssetMenu(menuName="LevelSetting")]
    public class Scriptable_Level : ScriptableObject {

        [Header("Basic")]
        public int level;
        public float distance;
        public PossibleBlockInfo[] cubes;

        [Header("Floor Changes")]
        public float nextFloorDropInSecond;
        [Range(0, 1)]
        public float maxFloorRecoverPercentage;
        [Range(0, 1)]
        public float minFloorRecoverPercentage;
        [Range(0, 1)]
        public float startRecoverPercentage;

        [Header("Items")]
        public float nextItemDropInSecond;
    }

    [System.Serializable]
    public class PossibleBlockInfo
    {
        public Colors color;
        [Range(0f, 1f)]
        public float percentage;
    }
}


