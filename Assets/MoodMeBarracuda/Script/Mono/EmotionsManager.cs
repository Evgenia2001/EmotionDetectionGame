using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoodMe;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Completed;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

namespace MoodMe
{

    public class EmotionsManager : MonoBehaviour
    {
        //public MeshRenderer PreviewMR;
        //[Header("ENTER LICENSE HERE")]
        //public string Email = "";
        //public string AndroidLicense = "";
        //public string IosLicense = "";
        //public string OsxLicense = "";
        //public string WindowsLicense = "";

        [Header("Input")]
        public ManageEmotionsNetwork EmotionNetworkManager;
        public FaceDetector FaceDetectorManager;

        [Header("Performance")]
        [Range(1, 60)]
        public int ProcessEveryNFrames = 15;
        [Header("Processing")]
        public bool FilterAllZeros = true;
        [Range(0, 29f)]
        public int Smoothing;
        [Header("Emotions")]
        public bool TestMode = false;
        [Range(0, 1f)]
        public float Angry;
        [Range(0, 1f)]
        public float Disgust;
        [Range(0, 1f)]
        public float Happy;
        [Range(0, 1f)]
        public float Neutral;
        [Range(0, 1f)]
        public float Sad;
        [Range(0, 1f)]
        public float Scared;
        [Range(0, 1f)]
        public float Surprised;
        [Range(0, 1f)]

        public static float EmotionIndex;

        public static MoodMeEmotions.MDMEmotions Emotions;
        private static MoodMeEmotions.MDMEmotions CurrentEmotions;

        private string[] sadPhrases = new string[5];
        private string[] surprisedPhrases = new string[5];
        private string[] neutralPhrases = new string[3];

        //Main buffer texture
        public static WebCamTexture CameraTexture;

        private EmotionsInterface _emotionNN;

        public int lastEmotion = 0;
        public bool analyzeEmotions = true;

        //Main buffer


        private byte[] _buffer;
        private bool _bufferProcessed = false;

        private int NFramePassed;

        private static DateTime timestamp;

        public static EmotionsManager instance = null;

        [Range(0, 0.5f)]
        public float sadTrigger = 0.27f;
        [Range(0, 8f)]
        public float sadTimeout = 5f;
        public bool isSad;

        [Range(0, 1f)]
        public float surprisedTrigger = 0.6f;
        [Range(0, 8f)]
        public float surprisedTimeout = 5f;
        public bool isSurprised;

        [Range(0, 20f)]
        public float neutralTimeout = 20f;
        public bool isNeutral;

        void Awake()
        {
            if (instance == null)

                instance = this;

            else if (instance != this)

                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            EmotionNetworkManager = GameObject.Find("NetworkAgent(Clone)").GetComponent<ManageEmotionsNetwork>();
            FaceDetectorManager = GameObject.Find("NetworkAgent(Clone)").GetComponent<FaceDetector>();
            _emotionNN = new EmotionsInterface(EmotionNetworkManager, FaceDetectorManager);

            setPhrases();

            //int remainingDays = _emotionNN.SetLicense(Email == "" ? null : Email, EnvKey == "" ? null : EnvKey);

            //if (remainingDays == -1)
            //{
            //    Debug.Log("INVALID OR EMPTY LICENSE. The SDK will run in demo mode.");
            //    remainingDays = _emotionNN.SetLicense(null, EnvKey);
            //}

            //if (remainingDays < 0x7ff)
            //{
            //    Debug.Log("Remaining " + remainingDays + " days");
            //    if (remainingDays == 0)
            //    {
            //        Debug.Log("LICENSE EXPIRED. Please contact sales@mood-me.com to extend the license.");
            //    }
            //}
            //else
            //{
            //    Debug.Log("Lifetime license!");
            //}

        }

        void OnDestroy()
        {
            _emotionNN = null;
        }


        // Update is called once per frame
        void LateUpdate()
        {
            //If a Render Texture is provided in the VideoTexture (or just a still image), Webcam image will be ignored

            if (!TestMode)
            {
                if (CameraManager.WebcamReady)
                {

                    NFramePassed = (NFramePassed + 1) % ProcessEveryNFrames;
                    if (NFramePassed == 0)
                    {

                        try
                        {

                            _emotionNN.ProcessFrame();
                            _bufferProcessed = true;

                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.Message);
                            _bufferProcessed = false;
                        }

                        if (_bufferProcessed)
                        {
                            _bufferProcessed = false;
                            if (!(_emotionNN.DetectedEmotions.AllZero && FilterAllZeros))
                            {
                                CurrentEmotions = _emotionNN.DetectedEmotions;
                                Emotions = Filter(Emotions, CurrentEmotions, Smoothing);
                                //Debug.Log("angry " + Emotions.angry);
                                //Debug.Log("disgust " + Emotions.disgust);
                                //Debug.Log("happy " + Emotions.happy);
                                //Debug.Log("neutral " + Emotions.neutral);
                                //Debug.Log("sad " + Emotions.sad);
                                //Debug.Log("scared " + Emotions.scared);
                                //Debug.Log("surprised " + Emotions.surprised);
                                if (analyzeEmotions)
                                {
                                    Angry = Emotions.angry;
                                    Disgust = Emotions.disgust;
                                    Happy = Emotions.happy;
                                    Neutral = Emotions.neutral;
                                    Sad = Emotions.sad;
                                    if (Sad > sadTrigger)
                                    {
                                        Debug.Log("SAD");
                                        lastEmotion = 1;
                                        if (!isSad)
                                        {

                                            Statistics.LogStat("Player is sad");
                                            if (Companion.instance)
                                            {
                                                int random = Random.Range(0, 5);
                                                string phrase = sadPhrases[random];
                                                Companion.instance.Say(phrase);
                                                Statistics.LogStat("Companion reacted: " + phrase);
                                            }
                                        }
                                        isSad = true;
                                        Invoke("sadReset", sadTimeout);
                                    }
                                    Scared = Emotions.scared;
                                    Surprised = Emotions.surprised;
                                    if (Surprised > surprisedTrigger)
                                    {
                                        Debug.Log("SURPRISED");
                                        lastEmotion = 2;
                                        if (!isSurprised)
                                        {

                                            Statistics.LogStat("Player is surprised");
                                            if (Companion.instance)
                                            {
                                                int random = Random.Range(0, 5);
                                                string phrase = surprisedPhrases[random];
                                                Companion.instance.Say(phrase);
                                                Statistics.LogStat("Companion reacted: " + phrase);
                                            }
                                        }
                                        isSurprised = true;
                                        Invoke("surprisedReset", surprisedTimeout);

                                    }
                                }

                            }
                            else if (GameManager.instance && GameManager.instance.turnEmotionsOff)
                            {
                                Debug.Log("Neutral");
                                if (!isNeutral)
                                {
                                    Statistics.LogStat("Player is neutral");
                                    if (Companion.instance)
                                    {
                                        int random = Random.Range(0, 3);
                                        string phrase = neutralPhrases[random];
                                        Companion.instance.Say(phrase);
                                        Statistics.LogStat("Companion reacted: " + phrase);
                                    }
                                    isNeutral = true;
                                    Invoke("surprisedReset", neutralTimeout);
                                }

                            }

                        }
                        else
                        {
                            Emotions.Error = true;
                        }
                    }
                }

            }
            else
            {
                Emotions.angry = Angry;
                Emotions.disgust = Disgust;
                Emotions.happy = Happy;
                Emotions.neutral = Neutral;
                Emotions.sad = Sad;
                Emotions.scared = Scared;
                Emotions.surprised = Surprised;
            }
            EmotionIndex = (((3f * Happy + Surprised - (Sad + Scared + Disgust + Angry)) / 3f) + 1f) / 2f;

        }

        private void sadReset()
        {
            isSad = false;
        }
        private void surprisedReset()
        {
            isSurprised = false;
        }
        private void neutralReset()
        {
            isNeutral = false;
        }

        // Smoothing function
        MoodMeEmotions.MDMEmotions Filter(MoodMeEmotions.MDMEmotions target, MoodMeEmotions.MDMEmotions source, int SmoothingGrade)
        {
            float targetFactor = SmoothingGrade / 30f;
            float sourceFactor = (30 - SmoothingGrade) / 30f;
            target.angry = target.angry * targetFactor + source.angry * sourceFactor;
            target.disgust = target.disgust * targetFactor + source.disgust * sourceFactor;
            target.happy = target.happy * targetFactor + source.happy * sourceFactor;
            target.neutral = target.neutral * targetFactor + source.neutral * sourceFactor;
            target.sad = target.sad * targetFactor + source.sad * sourceFactor;
            target.scared = target.scared * targetFactor + source.scared * sourceFactor;
            target.surprised = target.surprised * targetFactor + source.surprised * sourceFactor;

            return target;
        }

        private void setPhrases()
        {
            sadPhrases[0] = "So sad(";
            sadPhrases[1] = "I feel you…";
            sadPhrases[2] = "Keep trying!";
            sadPhrases[3] = "Tough luck!";
            sadPhrases[4] = "Don’t worry! It will get better!";
            surprisedPhrases[0] = "Wow!";
            surprisedPhrases[1] = "You didn’t expect that, right?";
            surprisedPhrases[2] = "I can’t believe my eyes!";
            surprisedPhrases[3] = "I must say it surprised me!";
            surprisedPhrases[4] = "Unbelievable!";
            neutralPhrases[0] = "Hey there! Let’s get faster out of here!";
            neutralPhrases[1] = "Get excited! I feel like something is going to happen soon!";
            neutralPhrases[2] = "It looks like this is too easy for you!";
        }
    }

}