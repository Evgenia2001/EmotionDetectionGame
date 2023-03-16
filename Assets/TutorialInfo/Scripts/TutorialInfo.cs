using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TutorialInfo : MonoBehaviour
{

    public bool showAtStart = true;

    public string url;

    public GameObject overlay;
    public GameObject emotionManager;

    public AudioListener mainListener;

    public static string showAtStartPrefsKey = "showLaunchScreen";

    private static bool alreadyShownThisSession = false;


    void Awake()
    {
        if (alreadyShownThisSession)
        {
            StartGame();
        }
        else
        {
            alreadyShownThisSession = true;

            if (PlayerPrefs.HasKey(showAtStartPrefsKey))
            {
                showAtStart = PlayerPrefs.GetInt(showAtStartPrefsKey) == 1;
            }

            if (showAtStart)
            {
                ShowLaunchScreen();
            }
            else
            {
                StartGame();
            }
        }
    }

    public void ShowLaunchScreen()
    {
        Time.timeScale = 0f;
        mainListener.enabled = true;
        overlay.SetActive(true);
        // emotionManager.SetActive(false);
    }

    public void StartGame()
    {
        overlay.SetActive(false);
        // emotionManager.SetActive(true);
        mainListener.enabled = true;
        Time.timeScale = 1f;
    }

    public void ToggleShowAtLaunch()
    {
        showAtStart = true;
        PlayerPrefs.SetInt(showAtStartPrefsKey, showAtStart ? 1 : 0);
    }
    public void Restart(){
        alreadyShownThisSession = false;
    }
}
