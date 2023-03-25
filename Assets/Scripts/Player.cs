using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MoodMe;

namespace Completed
{
    public class Player : MovingObject
    {
        public float restartLevelDelay = 1f;
        public int pointsPerFood = 10;
        public int pointsPerSoda = 20;
        public int wallDamage = 1;
        public Text foodText;
        public AudioClip moveSound1;
        public AudioClip moveSound2;
        public AudioClip eatSound1;
        public AudioClip eatSound2;
        public AudioClip drinkSound1;
        public AudioClip drinkSound2;
        public AudioClip gameOverSound;

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        public GameObject companion;

        private bool hasSaidFood;

        private int food;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	
#endif



        protected override void Start()
        {

            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            food = GameManager.instance.playerFoodPoints;

            foodText.text = "Stamina: " + food;

            spriteRenderer.sortingOrder = 8;
            base.Start();
        }


        private void OnDisable()
        {
            GameManager.instance.playerFoodPoints = food;
        }


        private void Update()
        {
            if (!GameManager.instance.playersTurn) return;

            int horizontal = 0;
            int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER

            horizontal = (int)(Input.GetAxisRaw("Horizontal"));

            vertical = (int)(Input.GetAxisRaw("Vertical"));

            if (horizontal != 0)
            {
                vertical = 0;
            }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			if (Input.touchCount > 0)
			{
				Touch myTouch = Input.touches[0];
				
				if (myTouch.phase == TouchPhase.Began)
				{
					touchOrigin = myTouch.position;
				}
				
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					Vector2 touchEnd = myTouch.position;
					
					float x = touchEnd.x - touchOrigin.x;
					
					float y = touchEnd.y - touchOrigin.y;
					
					touchOrigin.x = -1;
					
					if (Mathf.Abs(x) > Mathf.Abs(y))
						horizontal = x > 0 ? 1 : -1;
					else
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif
            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove<Wall>(horizontal, vertical);
            }
        }

        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            food--;
            if (food < 20 && !hasSaidFood)
            {
                if (Companion.instance)
                {
                    Companion.instance.Say("We're low on food");
                    hasSaidFood = true;
                }
            }
            else if (hasSaidFood && food > 30)
                hasSaidFood = false;

            foodText.text = "Stamina: " + food;

            base.AttemptMove<T>(xDir, yDir);

            RaycastHit2D hit;
            spriteRenderer.sortingOrder = 8 - (int)transform.position[1];

            if (Move(xDir, yDir, out hit))
            {
                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            }

            CheckIfGameOver();

            GameManager.instance.playersTurn = false;
        }


        protected override void OnCantMove<T>(T component)
        {
            Wall hitWall = component as Wall;

            hitWall.DamageWall(wallDamage);

            animator.SetTrigger("playerChop");

            if (Companion.instance)
                Companion.instance.Say("Great hit");
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Exit")
            {
                Invoke("Restart", restartLevelDelay);

                enabled = false;
            }

            else if (other.tag == "Food")
            {
                food += pointsPerFood;

                foodText.text = "+" + pointsPerFood + " Stamina: " + food;

                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

                Statistics.LogStat("Player picked food");
                other.gameObject.SetActive(false);
                if (Companion.instance)
                    Companion.instance.Say("Mmm, tasty!");
            }

            else if (other.tag == "Soda")
            {
                food += pointsPerSoda;

                foodText.text = "+" + pointsPerSoda + " Stamina: " + food;

                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

                other.gameObject.SetActive(false);

                Statistics.LogStat("Player picked food");
                if (Companion.instance)
                    Companion.instance.Say("Bubbles!");
            }
        }


        // private void Restart()
        // {
        //     foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        //     {
        //         go.SetActive(false);
        //     }
        //     SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Additive);

        // }

        void Restart()
        {
            EmotionsManager.instance.lastEmotion = 0;
            EmotionsManager.instance.isSad = false;
            EmotionsManager.instance.isSurprised = false;
            CameraManager.instance.CameraTexture.Stop();
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
            Time.timeScale = 1;
        }

        public void LoseFood(int loss)
        {
            animator.SetTrigger("playerChop");
            if (Random.Range(0, 2) == 1)
            {
                if (Companion.instance)
                    Companion.instance.Say("Careful");
            }

            food -= loss;

            foodText.text = "-" + loss + " Stamina: " + food;

            CheckIfGameOver();
        }


        private void CheckIfGameOver()
        {
            if (food <= 0)
            {
                SoundManager.instance.PlaySingle(gameOverSound);

                SoundManager.instance.musicSource.Stop();

                GameManager.instance.GameOver();
            }
        }
    }
}

