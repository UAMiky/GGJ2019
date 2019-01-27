using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Level Progress")]
    [SerializeField]
    Image levelProgress;
    [SerializeField]
    float[] fillAmounts;

    float targetLevelProgress;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(levelProgress)
        {
            float level = (targetLevelProgress > 0) ? 
                Mathf.Lerp(levelProgress.fillAmount, targetLevelProgress, Time.deltaTime) : 0f;
            levelProgress.fillAmount = level;
        }
    }

    internal void ShowLogos()
    {

    }

    internal void SetLevelProgress(int nItems)
    {
        targetLevelProgress = 1f;
        if (nItems < fillAmounts.Length)
            targetLevelProgress = fillAmounts[nItems];
    }
}
