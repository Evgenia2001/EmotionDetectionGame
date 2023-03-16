using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Completed

{

    public class BoardManager : MonoBehaviour
    {

        [Serializable]
        public class Count
        {
            public int minimum;
            public int maximum;



            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }


        public int columns = 8;
        public int rows = 8;
        public Count wallCount = new Count(5, 9);
        public Count foodCount = new Count(1, 5);
        public GameObject exit;
        public GameObject companion;
        public GameObject[] outerWallCorners;
        public GameObject[] floorTiles;
        public GameObject[] surprisedWallTiles;
        public GameObject[] foodTiles;
        public GameObject[] enemyTiles;
        public GameObject[] outerWallTiles;
        public GameObject[] sadOuterWalls;
        public GameObject[] sadFloorTiles;
        public GameObject[] sadWallTiles;

        public GameObject textBubble;

        private Transform boardHolder;
        private List<Vector3> gridPositions = new List<Vector3>();


        void InitialiseList()
        {
            gridPositions.Clear();

            for (int x = 1; x < columns - 1; x++)
            {
                for (int y = 1; y < rows - 1; y++)
                {
                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }


        void BoardSetup(int emotion)
        {
            boardHolder = new GameObject("Board").transform;

            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    if (emotion == 1)
                    {
                        toInstantiate = sadFloorTiles[Random.Range(0, sadFloorTiles.Length)];

                        if (x == -1 || x == columns || y == -1 || y == rows)
                            toInstantiate = sadOuterWalls[Random.Range(0, sadOuterWalls.Length)];
                    }
                    else
                    {
                        if (x == -1)
                        {
                            if (y == rows)
                                toInstantiate = outerWallCorners[0];

                            else if (y == -1)
                                toInstantiate = outerWallCorners[1];
                            else
                                toInstantiate = outerWallTiles[0];
                        }

                        else if (x == columns)
                        {
                            if (y == -1)
                                toInstantiate = outerWallCorners[2];

                            else if (y == rows)
                                toInstantiate = outerWallCorners[3];
                            else
                                toInstantiate = outerWallTiles[2];

                        }

                        else
                        {
                            if (y == -1)
                                toInstantiate = outerWallTiles[1];
                            else if (y == rows)
                                toInstantiate = outerWallTiles[3];
                        }
                    }

                    GameObject instance =
                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(boardHolder);
                }
            }
        }


        Vector3 RandomPosition()
        {
            int randomIndex = Random.Range(0, gridPositions.Count);

            Vector3 randomPosition = gridPositions[randomIndex];

            gridPositions.RemoveAt(randomIndex);

            return randomPosition;
        }


        void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();

                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

                SpriteRenderer rend = tileChoice.GetComponent<SpriteRenderer>();
                rend.sortingOrder = rows - (int)randomPosition[1];

                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        void LayoutObjectAtRandom(GameObject[] tileArray, int objectCount)
        {
            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();

                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

                SpriteRenderer rend = tileChoice.GetComponent<SpriteRenderer>();
                rend.sortingOrder = rows - (int)randomPosition[1];

                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }


        public void SetupScene(int level, int emotion)
        {
            BoardSetup(emotion);

            InitialiseList();
            int enemyCount = (int)Mathf.Log(level + 1, 2f); ;
            if (emotion == 1)
            {
                LayoutObjectAtRandom(sadWallTiles, wallCount.minimum, wallCount.maximum);
                enemyCount = 7;
                int objectCount = Random.Range(foodCount.minimum, foodCount.maximum + 1);
                LayoutObjectAtRandom(foodTiles, objectCount);
                Statistics.LogStat("Food amount: " + objectCount);
            }
            else if (emotion == 2)
            {
                LayoutObjectAtRandom(surprisedWallTiles, wallCount.minimum, wallCount.maximum);
                enemyCount = 0;
                LayoutObjectAtRandom(foodTiles, foodCount.maximum);
                Statistics.LogStat("Food amount: " + foodCount.maximum);
            }
            else
            {
                LayoutObjectAtRandom(surprisedWallTiles, wallCount.minimum, wallCount.maximum);
                int objectCount = Random.Range(foodCount.minimum, foodCount.maximum + 1);
                LayoutObjectAtRandom(foodTiles, objectCount);
                Statistics.LogStat("Food amount: " + objectCount);
            }

            Statistics.LogStat("Enemy number: " + enemyCount);
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }

        public void SetupStoryScene(StoryLevel currentStoryLevel)
        {
            BoardSetup(currentStoryLevel.emotion);

            InitialiseList();
            if (currentStoryLevel.emotion == 1)
            {
                LayoutObjectAtRandom(sadWallTiles, wallCount.minimum, wallCount.maximum);
            }
            else
            {
                LayoutObjectAtRandom(surprisedWallTiles, wallCount.minimum, wallCount.maximum);
            }

            LayoutObjectAtRandom(foodTiles, currentStoryLevel.foodAmount);
            Statistics.LogStat("Food amount: " + currentStoryLevel.foodAmount);
            LayoutObjectAtRandom(enemyTiles, currentStoryLevel.enemyCount);
            Statistics.LogStat("Enemy amount: " + currentStoryLevel.enemyCount);
            if (currentStoryLevel.increaseEnemies && currentStoryLevel.enemyCount < 8) currentStoryLevel.enemyCount++;
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
            if (currentStoryLevel.companion)
            {
                Instantiate(companion, new Vector3(1, 1, 0f), Quaternion.identity).GetComponent<Companion>();
                Statistics.LogStat("Companion instantiated");
            }
        }
    }
}
