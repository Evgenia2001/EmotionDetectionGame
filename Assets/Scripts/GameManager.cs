using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
    using System.Collections.Generic;
    using MoodMe;
    using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
        public bool storyMode = true;
        public float levelStartDelay = 7f;
        public float turnDelay = 0.1f;
        public int playerFoodPoints = 100;
        public static GameManager instance = null;
        [HideInInspector] public bool playersTurn = true;

        private GameObject emotionManager;
        private GameObject tutorialInfo;

        private int lastEmotion = 0;


        private Text levelText;
        private GameObject levelImage;
        public BoardManager boardScript;
        private int level = 1;
        private List<Enemy> enemies;
        private bool enemiesMoving;
        private bool doingSetup = true;

        private StoryLevel currentStoryLevel;

        public bool turnEmotionsOff = false;


        void Awake()
        {
            Statistics.LogStat("");
            Statistics.LogStat("##########################################");
            Statistics.LogStat("New Game");
            Statistics.LogStat("Emotion Off: " + turnEmotionsOff);
            Statistics.LogStat("");
            if (instance == null)

                instance = this;

            else if (instance != this)

                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();
            boardScript = GetComponent<BoardManager>();
            if (storyMode)
            {
                CreateStory();
            }
            InitGame();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;
            instance.InitGame();
        }

        private void Start()
        {

            emotionManager = GameObject.Find("MoodMeEmotionManager");
            tutorialInfo = GameObject.Find("TutorialInfo");

        }

        private void CreateStory()
        {
            StoryLevel surprisedAfterCompanion = new StoryLevel("More monsters apperaed in the park. It looks like the fight will be difficult");
            surprisedAfterCompanion.setAnswers("Once more? Maybe on other path are less monsters",
            "Okay, come on!",
            "Yeah!! Once more!");
            surprisedAfterCompanion.enemyCount = 3;
            surprisedAfterCompanion.increaseEnemies = true;
            surprisedAfterCompanion.companion = true;
            surprisedAfterCompanion.neutral = surprisedAfterCompanion;
            surprisedAfterCompanion.surprised = surprisedAfterCompanion;
            surprisedAfterCompanion.name = "surprisedAfterCompanion";

            StoryLevel sadAfterCompanion = new StoryLevel("More monsters apperaed in the park. It looks like the fight will be difficult");
            sadAfterCompanion.setAnswers("Once more? Maybe on other path are less monsters",
            "Okay, come on!",
            "Yeah!! Once more!");
            sadAfterCompanion.enemyCount = 0;
            sadAfterCompanion.emotion = 1;
            sadAfterCompanion.companion = true;
            sadAfterCompanion.increaseEnemies = true;
            sadAfterCompanion.sad = surprisedAfterCompanion;
            sadAfterCompanion.neutral = sadAfterCompanion;
            sadAfterCompanion.surprised = sadAfterCompanion;
            surprisedAfterCompanion.sad = sadAfterCompanion;
            sadAfterCompanion.name = "sadAfterCompanion";

            StoryLevel parkAfterCompanion = new StoryLevel("More monsters apperaed in the park. It looks like the fight will be difficult");
            parkAfterCompanion.setAnswers("Once more?",
            "Okay, come on!",
            "Yeah!! Once more!");
            parkAfterCompanion.companion = true;
            parkAfterCompanion.sad = sadAfterCompanion;
            parkAfterCompanion.neutral = surprisedAfterCompanion;
            parkAfterCompanion.surprised = surprisedAfterCompanion;
            parkAfterCompanion.name = "parkAfterCompanion";

            StoryLevel parkWithoutEnemies = new StoryLevel("You walk through the park. Suddenly, you see some magical bird trapped by monster");
            parkWithoutEnemies.setAnswers("You are sad! You don't want to save some bird, walk in the forest is better.",
            "Let's help that small bird",
            "Magical bird and monsters! That's interesting. Let's fight them!");
            parkWithoutEnemies.name = "parkWithoutEnemies";
            // parkWithoutEnemies.companion = true;

            StoryLevel parkCompanion = new StoryLevel("You found that magic bird. Now it will be you companion");
            parkCompanion.setAnswers("That's all?",
            "Good. I was glad to help her",
            "Hi, bird! We will be friends!");
            parkCompanion.enemyCount = 3;
            parkCompanion.sad = parkAfterCompanion;
            parkCompanion.neutral = parkAfterCompanion;
            parkCompanion.surprised = parkAfterCompanion;
            parkCompanion.name = "parkCompanion";

            StoryLevel parkWithBird2 = new StoryLevel("More monsters apperaed in the park. It looks like the fight will be difficult");
            parkWithBird2.setAnswers("Once more?",
            "Okay, come on!",
            "Yeah!! Once more!");
            parkWithBird2.enemyCount = 2;
            parkWithBird2.sad = parkCompanion;
            parkWithBird2.neutral = parkCompanion;
            parkWithBird2.surprised = parkCompanion;
            parkWithBird2.name = "parkWithBird2";

            StoryLevel parkWithBird1 = new StoryLevel("You see few monsters in the park. It's time to fight them");
            parkWithBird1.setAnswers("You are not in the mood for a fight, but okay",
            "Okay, I am ready!",
            "Yeah!! Let's fignt, monsters!");
            parkWithBird1.enemyCount = 1;
            parkWithBird1.sad = parkWithBird2;
            parkWithBird1.neutral = parkWithBird2;
            parkWithBird1.surprised = parkWithBird2;
            parkWithoutEnemies.neutral = parkWithBird1;
            parkWithoutEnemies.surprised = parkWithBird1;
            parkWithBird1.name = "parkWithBird1";

            StoryLevel forestWithEnemies = new StoryLevel("Forest is very dark here. You even can't see sun. You here some more zombies.");
            forestWithEnemies.setAnswers("You are tired! You want back to the castle.",
            "You are thinking about your father death while going deeper in the forest. Soon zombies attack you.",
            "Zombies! That's interesting. Let's fight them!");
            forestWithEnemies.sad = parkWithoutEnemies;
            forestWithEnemies.neutral = forestWithEnemies;
            forestWithEnemies.surprised = forestWithEnemies;
            forestWithEnemies.emotion = 1;
            forestWithEnemies.enemyCount = 2;
            forestWithEnemies.increaseEnemies = true;
            forestWithEnemies.name = "forestWithEnemies";

            StoryLevel darkForest = new StoryLevel("You walked into the dark forest. Strange noised are coming from around you");
            darkForest.setAnswers("You don't want to find out what are these noises. Start walking back to castle",
            "You are thinking about your father death while going deeper in the forest. Soon you found zombies that attacked you.",
            "Strange noises? Let's check from where they are coming");
            darkForest.sad = parkWithoutEnemies;
            darkForest.neutral = forestWithEnemies;
            darkForest.surprised = forestWithEnemies;
            darkForest.emotion = 1;
            parkWithoutEnemies.sad = darkForest;
            darkForest.name = "darkForest";

            StoryLevel parkWithEnemies = new StoryLevel("You won this battle, but new monsters are coming.");
            parkWithEnemies.setAnswers("You don't want to fight anymore. So you decided to retreat.",
            "Okay, let's fight",
            "Yeah. I can beat one more level");
            parkWithEnemies.sad = parkWithoutEnemies;
            parkWithEnemies.neutral = parkWithEnemies;
            parkWithEnemies.surprised = parkWithEnemies;
            parkWithEnemies.enemyCount = 2;
            parkWithEnemies.increaseEnemies = true;
            parkWithEnemies.name = "parkWithEnemies";

            StoryLevel deathOfKing = new StoryLevel("You are the mighty knight, you fought many monsters. But you were not ready for the next twist of fate. The King is Dead! Your father is Dead!");
            deathOfKing.setAnswers("Ohh,no! You are sad about king's death, so you decided to walk in the forest alone.",
            "You start walking towards the castle.",
            "You are pleasantly surprised by this occasion. You run through the park, not noticing that you are surrounded by enemies");
            deathOfKing.sad = darkForest;
            deathOfKing.neutral = parkWithoutEnemies;
            deathOfKing.surprised = parkWithEnemies;
            deathOfKing.name = "deathOfKing";

            currentStoryLevel = deathOfKing;
        }


        void InitGame()
        {
            doingSetup = true;

            tutorialInfo = GameObject.Find("Tutorial Info");
            Statistics.LogStat("***************************************");
            Statistics.LogStat("Level: " + instance.level);

            levelImage = GameObject.Find("LevelImage");
            levelText = GameObject.Find("LevelText").GetComponent<Text>();
            if (currentStoryLevel == null)
            {
                storyMode = false;
            }

            if (storyMode)
            {
                levelText.text = currentStoryLevel.lvlDescription;
            }
            else
            {
                levelText.text = "Level " + level;
            }
            levelImage.SetActive(true);
            EmotionsManager.instance.lastEmotion = 0;
            Invoke("ChangeText", 9f);


        }
        void ChangeText()
        {
            if (!emotionManager)
            {
                emotionManager = GameObject.Find("MoodMeEmotionManager(Clone)");

            }
            lastEmotion = emotionManager.GetComponent<EmotionsManager>().lastEmotion;
            string emotion;
            string text = "";
            switch (lastEmotion)
            {
                case 1:
                    emotion = "Sad";
                    if (storyMode)
                    {
                        text = currentStoryLevel.sadAnswer;
                        currentStoryLevel = currentStoryLevel.sad;
                    }
                    break;
                case 2:
                    emotion = "Surprised";
                    if (storyMode)
                    {
                        text = currentStoryLevel.surprisedAnswer;
                        currentStoryLevel = currentStoryLevel.surprised;
                    }
                    break;
                default:
                    emotion = "Neutral";
                    if (storyMode)
                    {
                        text = currentStoryLevel.neutralAnswer;
                        currentStoryLevel = currentStoryLevel.neutral;
                    }
                    break;

            }

            Debug.Log("Level Emotion: " + emotion);
            Statistics.LogStat("Level emotion: " + emotion);
            Statistics.LogStat("Name: " + instance.currentStoryLevel.name);
            levelText.text = "Level: " + instance.level + "\t" + emotion + "\n\n" + text;
            Invoke("HideLevelImage", levelStartDelay);
        }

        void HideLevelImage()
        {
            levelImage.SetActive(false);
            enemies.Clear();
            if (storyMode)
            {
                boardScript.SetupStoryScene(currentStoryLevel);
            }
            else
                boardScript.SetupScene(level, lastEmotion);

            doingSetup = false;
        }

        void Update()
        {
            if (Input.GetKey("escape"))
            {
                Statistics.LogStat("");
                Statistics.LogStat("Exit");
                Statistics.LogStat("");
                int scene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.UnloadScene(scene);
                Application.Quit();
            }
            if (playersTurn || enemiesMoving || doingSetup)

                return;

            StartCoroutine(MoveEnemies());
        }

        public void AddEnemyToList(Enemy script)
        {
            enemies.Add(script);
        }

        public void RemoveEnemyFromList(Enemy script)
        {
            enemies.Remove(script);
        }


        public void GameOver()
        {
            levelText.text = "After " + level + " days, you starved.";
            levelImage.SetActive(true);
            enabled = false;
            Statistics.LogStat("");
            Statistics.LogStat("GAME OVER");
            Statistics.LogStat("Level: " + level);
            Statistics.LogStat("");
            Invoke("RestartGame", 8f);
        }

        IEnumerator MoveEnemies()
        {
            enemiesMoving = true;

            yield return new WaitForSeconds(turnDelay);

            if (Companion.instance)
                Companion.instance.MoveCompanion();

            if (enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].MoveEnemy();
                if (i < enemies.Count)
                    yield return new WaitForSeconds(enemies[i].moveTime);
            }
            playersTurn = true;

            enemiesMoving = false;
        }
        void RestartGame()
        {

            CameraManager.instance.CameraTexture.Stop();
            if (storyMode)
                CreateStory();
            instance.level = 0;
            tutorialInfo.GetComponent<TutorialInfo>().Restart();
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
            Time.timeScale = 1;
        }
    }
}

