using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

static class ScoreConstants
{
    public const float TIMER_TRANSITION_FACTOR = 2f;
}

public class ScoreManager : MonoBehaviour
{
    public float mCurrScore = 0;
    public float mTargetScore = 0;
    public float mTimer = 0.0f;
    public TextMeshProUGUI ScoreText;
    void Start()
    {
        
    }

    void Update()
    {
        mTimer += Time.deltaTime;
        mCurrScore = (int)Mathf.Lerp(mCurrScore, mTargetScore, Mathf.Clamp(mTimer / ScoreConstants.TIMER_TRANSITION_FACTOR, 0.0f, 1.0f));
        ScoreText.text = "SCORE: " + mCurrScore;
    }

    public void AddScore(int score)
    {
        mCurrScore = mTargetScore;
        mTimer = 0.0f;
        mTargetScore += score;
    }
}
