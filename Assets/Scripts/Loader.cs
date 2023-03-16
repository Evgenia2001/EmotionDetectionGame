using UnityEngine;
using System.Collections;
using MoodMe;

namespace Completed
{

    public class Loader : MonoBehaviour
    {
        public GameObject gameManager;
        public GameObject soundManager;
        public GameObject emotionManager;
        public GameObject cameraManager;
        public GameObject networkAgent;

        void Awake()
        {
            if (CameraManager.instance == null)

                Instantiate(cameraManager);

            if (EmotionsManager.instance == null)

                Instantiate(emotionManager);

            if (FaceDetector.instance == null)

                Instantiate(networkAgent);
            if (GameManager.instance == null)

                Instantiate(gameManager);

            if (SoundManager.instance == null)

                Instantiate(soundManager);


        }
    }
}