using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    [SerializeField] private Text m_ScoreText;

    public void UpdateScore(int newScore)
    {
        if(newScore > 999 || newScore < 0) { throw new UnityException("Score out of range (TextController)"); }

        m_ScoreText.text = newScore.ToString();
    }
}
