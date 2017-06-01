using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    [SerializeField] private Text m_ScoreText;
    [SerializeField] private Text m_GoalText;

    public void UpdateScore(int score, int goal)
    {
        if(score > 999 || score < 0) { throw new UnityException("Score out of range (TextController)"); }

        m_ScoreText.text = score.ToString();
        m_GoalText.text = goal.ToString();
    }
}
