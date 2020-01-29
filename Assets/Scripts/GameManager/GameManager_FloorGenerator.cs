using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNTF
{
    [RequireComponent(typeof(GameManager_Main))]
    [RequireComponent(typeof(GameManager_LevelSettings))]
    [RequireComponent(typeof(GameManager_LevelChanger))]
    public class GameManager_FloorGenerator : MonoBehaviour
    {
        #region Singleton
        public static GameManager_FloorGenerator instance;
        void Awake()
        {
            instance = this;
        }

        struct Pair
        {
            public int firstValue;
            public int secondValue;
            public void Set(int first, int second)
            {
                firstValue = first;
                secondValue = second;
            }
        }

        #endregion

        private GameManager_Main gameManagerMain;
        private GameManager_LevelSettings levelSetting;
        private GameManager_LevelChanger levelChanger;

        public GameObject floorParent;
        public GameObject itemParent;
        [Tooltip("Color MUST be in same order as the Enum")]
        public CubeTypes[] cubeStyle;
        int floorScale = 2;

        #region In Game Variables
        Vector3 pos;
        Floor[,] floor;
        int floorLength;
        int floorWidth;

        float percentOfFloorRemaining;

        FloorManipulator floorManipulator;
        Colors nextDropColor;
        float nextCollapseTime;
        float nextDropTime;
        bool hasCollapseEvent;
        List<Pair> stillFlyingTileIndex;
        List<Pair> emptyTileIndex;
        Pair pair;

        #endregion

        public delegate void InGameEvent();
        public event InGameEvent DropCubes;


        void OnEnable()
        {
            gameManagerMain = GetComponent<GameManager_Main>();
            levelSetting = GetComponent<GameManager_LevelSettings>();
            levelChanger = GetComponent<GameManager_LevelChanger>();
            floorLength = (int)(levelChanger.deathGround.floorBaseSize.x / floorScale);
            floorWidth = (int)(levelChanger.deathGround.floorBaseSize.z / floorScale);
            floor = new Floor[floorLength, floorWidth];
            floorManipulator = new FloorManipulator();
            stillFlyingTileIndex = new List<Pair>();
            emptyTileIndex = new List<Pair>();

            //---Add Events---//
            gameManagerMain.StartLevelEvents += SetupFloor;
        }

        void OnDisable()
        {
            gameManagerMain.StartLevelEvents -= SetupFloor;
        }

        void SetupFloor(int level)
        {
            foreach (Transform child in floorParent.transform)
                GameObjectPool.instance.CubeBackToPool(child.gameObject);

            stillFlyingTileIndex.Clear();
            floorManipulator.InitializeList(levelSetting.levelInfo[level]);

            for (int i = 0; i < floorWidth; i++)
            {
                for (int j = 0; j < floorLength; j++)
                {
                    pos = new Vector3(transform.position.x + (j * floorScale), transform.position.y, -(transform.position.z + (i * floorScale)));
                    GameObject go = GameObjectPool.instance.InstanceCubeFromPool(floorParent.transform, pos);
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    go.GetComponent<Rigidbody>().isKinematic = true;
                    int colorIndex = floorManipulator.GetRandomColorIndex();
                    if (colorIndex < 0)
                    {
                        Debug.Log("[Error] Color Index value went to negative value.");
                        continue;
                    }
                    Floor newFloor = new Floor(go, pos, true, cubeStyle[colorIndex]);
                    floor[i, j] = newFloor;
                    pair.Set(i, j);
                    stillFlyingTileIndex.Add(pair);
                }
            }

            //---Clear Events---//
            DropCubes = null;
            hasCollapseEvent = false;

            nextCollapseTime = floorManipulator.nextDropInterval + Time.time + 1f;
            nextDropTime = 0;

            percentOfFloorRemaining = 1f;

            HintControl.instance.HintSetup(true, true);
        }

        void ResetFloorColor()
        {
            int remainFloorCounter = 0;
            for (int i = 0; i < floorWidth; i++)
            {
                for (int j = 0; j < floorLength; j++)
                {
                    if ((floor[i, j].Tile != null) && (floor[i, j].isFlying))
                    {
                        //---Only change 50% of the tiles---//
                        int randomValue = Random.Range(0, 100);
                        if (randomValue < 50)
                        {
                            int colorIndex = floorManipulator.GetRandomColorIndex(nextDropColor);
                            floor[i, j].ResetColor(cubeStyle[colorIndex]);
                        }
                        remainFloorCounter++;
                    }
                }
            }

            percentOfFloorRemaining = ((float)(remainFloorCounter) / (float)(floorWidth * floorLength));
            //Debug.Log(percentOfFloorRemaining);
        }

        void SetupNextDropEvent()
        {
            stillFlyingTileIndex.Clear();
            emptyTileIndex.Clear();
            nextDropColor = floorManipulator.SelectRandomColorToDrop();

            HintControl.instance.SetHintColors(cubeStyle[(int)nextDropColor].colorMaterial.color);

            //Debug.Log("[System] Next Drop color: " + nextDropColor);
            for (int i = 0; i < floorWidth; i++)
            {
                for (int j = 0; j < floorLength; j++)
                {
                    if ((floor[i, j].Tile != null) && (floor[i, j].isFlying))
                    {
                        if (floor[i, j].GetFloorColor() == nextDropColor)
                        {
                            Cube_Script script = floor[i, j].Tile.GetComponent<Cube_Script>();
                            script.AddCubeToDropEvent();

                            floor[i, j].isFlying = false;
                        }
                        else
                        {
                            if (!floor[i, j].hasItem)
                            {
                                pair.Set(i, j);
                                stillFlyingTileIndex.Add(pair);
                            }
                        }
                    }
                    else
                    {
                        pair.Set(i, j);
                        emptyTileIndex.Add(pair);
                    }
                }
            }
            hasCollapseEvent = true;
        }

        void SpawnRepairFloorItem()
        {
            int randomIndex = Random.Range(0, stillFlyingTileIndex.Count);

            pair = stillFlyingTileIndex[randomIndex];
            floor[pair.firstValue, pair.secondValue].hasItem = true;
            Vector3 pos = floor[pair.firstValue, pair.secondValue].position;
            pos.y += 7;

            GameObject go = GameObjectPool.instance.InstanceItemFromPool(itemParent.transform, pos);
            RegenerateFloor_Item script =  go.GetComponent<RegenerateFloor_Item>();
            if(script == null)
                go.AddComponent<RegenerateFloor_Item>();
        }

        void LateUpdate()
        {
            if (gameManagerMain.GetGameState() == GameState.InGame)
            {
                if (!hasCollapseEvent)
                    SetupNextDropEvent();
                else if (Time.time >= nextCollapseTime)
                {
                    nextCollapseTime = Time.time + floorManipulator.nextDropInterval;

                    if (DropCubes != null)
                        DropCubes();

                    DropCubes = null;
                    hasCollapseEvent = false;
                    ResetFloorColor();
                }

                if (percentOfFloorRemaining <= floorManipulator.startRecoverPercentage)
                {
                    if (Time.time >= nextDropTime)
                    {
                        nextDropTime = Time.time + floorManipulator.nextDropInterval;
                        SpawnRepairFloorItem();
                    }
                }

                HintControl.instance.SetFillPercent((nextCollapseTime - Time.time) / floorManipulator.nextDropInterval);
            }
        }


        public void RegenerateMissingFloor()
        {
            lock (emptyTileIndex)
            {
                //Debug.Log("[System] -Start regenerating floor...-");
                float regeneratingPercent = Random.Range(0f, (floorManipulator.maxRecoverPercentage - floorManipulator.minRecoverPercentage)) + floorManipulator.minRecoverPercentage;
                int regeneratingAmount = (int)((float)emptyTileIndex.Count * regeneratingPercent);

                /*Debug.Log(regeneratingPercent + ", Amount = " + regeneratingAmount);
                Debug.Log("Total: " + emptyTileIndex.Count);*/

                while ((regeneratingAmount > 0) && (emptyTileIndex.Count > 0))
                {
                    int randomIndex = Random.Range(0, emptyTileIndex.Count);
                    int y = emptyTileIndex[randomIndex].firstValue;
                    int x = emptyTileIndex[randomIndex].secondValue;

                    if (!floor[y, x].isFlying)
                    {
                        Vector3 pos = floor[y, x].position;
                        pos.y -= 3f;
                        GameObject go = GameObjectPool.instance.InstanceCubeFromPool(floorParent.transform, pos);
                        go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        go.GetComponent<Rigidbody>().isKinematic = true;
                        floor[y, x].Tile = go;

                        int colorIndex = floorManipulator.GetRandomColorIndex(nextDropColor);
                        floor[y, x].ResetColor(cubeStyle[colorIndex]);
                        floor[y, x].isFlying = true;
                        floor[y, x].hasItem = false;

                        floor[y, x].Tile.GetComponent<Cube_Script>().RespawnCube(floor[y, x].position);

                        emptyTileIndex.RemoveAt(randomIndex);
                        regeneratingAmount--;
                    }
                    else
                    {
                        Debug.Log("[Error] Current Tile is not null, but it's in empty list");
                    }
                }
            }
        }

    }


    [System.Serializable]
    public class CubeTypes
    {
        public string name;
        public Material colorMaterial;
        public Colors color;
    }

    public class Floor
    {
        private CubeTypes cubeType;
        private GameObject tile;
        public GameObject Tile
        {
            get { return tile; }
            set { tile = value; }
        }
        public bool isFlying;
        public bool hasItem;
        public Vector3 position;

        public Floor(GameObject obj, Vector3 pos, bool fly, CubeTypes style)
        {
            tile = obj;
            position = pos;
            isFlying = fly;
            cubeType = style;
            hasItem = false;
            tile.GetComponent<Renderer>().material = style.colorMaterial;
        }

        public void ResetColor(CubeTypes style)
        {
            cubeType = style;
            tile.GetComponent<Renderer>().material = style.colorMaterial;
        }

        public Colors GetFloorColor()
        {
            return cubeType.color;
        }
    }

    public class FloorManipulator
    {
        #region SetUp
        struct PossibleFloorType {
            public Colors color;
            public int minOccurRange;
            public int maxOccurRange;

            public void Set(Colors c, int min, int max)
            {
                color = c;
                minOccurRange = min;
                maxOccurRange = max;
            }
        }

        public FloorManipulator()
        {
            existingTile = new List<PossibleFloorType>();
            floorTyle = new PossibleFloorType();
        }
        #endregion

        //---Local Variables---//
        List<PossibleFloorType> existingTile;
        PossibleFloorType floorTyle;
        int occurRange;
   
        //---Variables For outside---//
        public float nextDropInterval;
        public float maxRecoverPercentage;
        public float minRecoverPercentage;
        public float startRecoverPercentage;

        #region UtilityFunction
        public void InitializeList(Scriptable_Level currentLevel)
        {
            //--Reset Variables--//
            occurRange = 0;
            existingTile.Clear();

            int lastValue = 0;
            foreach (var possibleCube in currentLevel.cubes)
            {
                occurRange += (int)(possibleCube.percentage * 100f)-1;
                //Debug.Log(lastValue + ", " + occurRange);
                floorTyle.Set(possibleCube.color, lastValue, occurRange);
                lastValue = occurRange + 1;
                occurRange = lastValue;
                existingTile.Add(floorTyle);
            }

            nextDropInterval = currentLevel.nextFloorDropInSecond;
            maxRecoverPercentage = currentLevel.maxFloorRecoverPercentage;
            minRecoverPercentage = currentLevel.minFloorRecoverPercentage;
            startRecoverPercentage = currentLevel.startRecoverPercentage;
        }

        public int GetRandomColorIndex()
        {
            int colorIndex = -1;
            int randomValue = Random.Range(0, occurRange);
            
            foreach (PossibleFloorType t in existingTile)
            {
                if (randomValue >= t.minOccurRange && randomValue <= t.maxOccurRange)
                {
                    //---Change colors to int type in order to get the index in color list---//
                    colorIndex = (int)t.color;
                    break;
                }
            }
            return colorIndex;
        }

        public int GetRandomColorIndex(Colors notColor)
        {
            int colorIndex = -1;
            int randomValue = Random.Range(0, occurRange);

            for (int i = 0; i < existingTile.Count; i++)
            {
                if (randomValue >= existingTile[i].minOccurRange && randomValue <= existingTile[i].maxOccurRange)
                {
                    if (existingTile[i].color == notColor)
                    {
                        int nextIndex = (i + 1) % existingTile.Count;
                        colorIndex = (int)existingTile[nextIndex].color;
                    }
                    else
                        colorIndex = (int)existingTile[i].color;
                    break;
                }
            }
 
            return colorIndex;
        }

        public Colors SelectRandomColorToDrop()
        {
            int index = Random.Range(0, existingTile.Count);
            return existingTile[index].color;
        }

        #endregion
    }

    public enum Colors
    {
        White = 0,
        Red = 1,
        Orange = 2,
        Yellow = 3,
        Green = 4,
        Blue = 5,
        Purple = 6,
        Brown = 7,
        Pink = 8,
        Black = 9,
        None = 10
    }
}

