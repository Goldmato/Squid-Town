using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    [SerializeField] private Text m_ScoreText;
    [SerializeField] private Text m_GoalText;
    [SerializeField] private Alert m_AlertPrefab;
    [SerializeField] private RectTransform m_AlertPanel;

    private int m_MaxAlerts;
    private int m_ActiveAlerts;

    const float ALERT_TIMEOUT = 2f;
    const float ALERT_HANG_DELAY = 1.5f;
    const float ALPHA_THRESHOLD = 0.15f;

    void Awake()
    {
        var vertLayout = m_AlertPanel.GetComponent<VerticalLayoutGroup>();
        m_MaxAlerts = Mathf.FloorToInt((m_AlertPanel.rect.height - vertLayout.padding.bottom - vertLayout.padding.top) /
            (m_AlertPrefab.GetComponent<RectTransform>().rect.height + vertLayout.spacing));
        Debug.Log("Max number of alerts: " + m_MaxAlerts);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            CreateAlert(new AlertData("TEST ALERT", IconRegistry.GetIcon(IconRegistry.WarningIcons, 0)), AlertColorArray.Green);
    }

    public void UpdateScore(int score, int goal)
    {
        if(score > 999 || score < 0) { throw new UnityException("Score out of range (TextController)"); }

        m_ScoreText.text = score.ToString();
        m_GoalText.text = goal.ToString();
    }

    public bool CreateAlert(AlertData alertData, AlertColorArray colorArray = null)
    {
        if(m_ActiveAlerts >= m_MaxAlerts)
            return false;

        // TODO: Implement method for populating an alert panel grid with self-destructing alerts
        // TODO: Create alert specific class that controls the alert text + icon

        var alert = Instantiate(m_AlertPrefab, m_AlertPanel);
        m_ActiveAlerts++;

        // Set the appropriate fields/values
        if(colorArray != null)
            alert.SetColorArray(colorArray);
        alert.Text.text = alertData.message;
        alert.Icon.sprite = alertData.icon;

        // Destroy the alert after a predefined length of time
        StartCoroutine(FadeAlert(alert));

        return true;
    }

    public void DeleteAlert(Alert alert)
    {
        if(m_ActiveAlerts > 0)
            m_ActiveAlerts--;

        Destroy(alert.gameObject);
    }

    IEnumerator FadeAlert(Alert alert)
    {
        yield return new WaitForSeconds(ALERT_HANG_DELAY);
        foreach(var cur in TweenAlertAlphas(alert)) { yield return null; }
        yield return new WaitForSeconds(ALERT_HANG_DELAY);

        if(alert != null)
            DeleteAlert(alert);
    }

    IEnumerable TweenAlertAlphas(Alert alert, float delay = ALERT_TIMEOUT)
    {
        delay /= 1 - ALPHA_THRESHOLD;

        while(alert != null && alert.Panel.color.a > ALPHA_THRESHOLD)
        {
            float alphaInterval = (Time.deltaTime / delay);

            alert.Panel.color = new Color(alert.Panel.color.r, alert.Panel.color.g, alert.Panel.color.b,
                alert.Panel.color.a - alphaInterval);
            alert.Icon.color = new Color(alert.Icon.color.r, alert.Icon.color.g, alert.Icon.color.b,
                alert.Icon.color.a - alphaInterval);
            alert.Text.color = new Color(alert.Text.color.r, alert.Text.color.g, alert.Text.color.b,
                alert.Text.color.a - alphaInterval);

            yield return null;
        }
    }
}
