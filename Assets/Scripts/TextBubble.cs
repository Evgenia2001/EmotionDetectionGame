using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour
{
    private TMP_Text bubbleText;

    private void Awake()
    {
        bubbleText = GameObject.FindGameObjectWithTag("TextField").GetComponent<TMP_Text>();
    }

    void Start()
    {
        // Setup("New Textaaaa");

    }

    // Update is called once per frame
    public void Setup(string text)
    {   
        bubbleText.SetText(text);
        bubbleText.ForceMeshUpdate();
    }
}
