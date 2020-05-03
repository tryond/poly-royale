using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    [SerializeField] private Text scoreTMP;
    [SerializeField] private Text restartTMP;

    public static Canvas instance;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
        // make text clear
        scoreTMP.color = new Color(255f, 255f, 255f, 0f);
        restartTMP.color =new Color(255f, 255f, 255f, 0f);
    }

    public void DisplayScore(int playersRemaining, int playersTotal)
    {
        scoreTMP.text = playersRemaining.ToString() + " / " + playersTotal.ToString();
        StartCoroutine(FadeInText(1f, scoreTMP));
        StartCoroutine(FadeInText(1f, restartTMP));
    }
    
    private IEnumerator FadeInText(float timeSpeed, Text text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }
}
