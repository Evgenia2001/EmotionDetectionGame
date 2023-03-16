using UnityEngine;
using System.Collections;

namespace Completed
{
    public class Companion : MovingObject
    {

        public GameObject textBubble;
        private Animator animator;
        private Transform target;

        private bool skipMove;

        private GameObject textRef;

        private bool shouldSay = false;
        private string text = "Hi!";

        public static Companion instance;

        void Awake()
        {
            if (instance == null)

                instance = this;

            else if (instance != this)

                Destroy(gameObject);
        }
        protected override void Start()
        {

            animator = GetComponent<Animator>();

            target = GameObject.FindGameObjectWithTag("Player").transform;

            base.Start();
        }
        protected override void AttemptMove<T>(int xDir, int yDir)
        {

            // if(skipMove)
            // {
            // 	skipMove = false;
            // 	return;
            // }
            base.AttemptMove<T>(xDir, yDir);
            if (shouldSay)
            {
                Destroy(textRef);
                textRef = Instantiate(textBubble, new Vector3(transform.position.x - 1, transform.position.y, 0f), Quaternion.identity);
                textRef.GetComponent<TextBubble>().Setup(text);
                shouldSay = false;
            }
            else
            {
                Destroy(textRef);
            }
            // skipMove = true;
        }

        public void MoveCompanion()
        {
            int xDir = 0;
            int yDir = 0;

            if (Mathf.Abs(target.position.x - 1 - transform.position.x) < float.Epsilon)

                yDir = target.position.y - 1 > transform.position.y ? 1 : -1;


            else
                xDir = target.position.x - 1 > transform.position.x ? 1 : -1;

            AttemptMove<Player>(xDir, yDir);
        }

        public void Say(string t)
        {
            shouldSay = true;
            text = t;
        }


        protected override void OnCantMove<T>(T component)
        {

        }
    }
}
