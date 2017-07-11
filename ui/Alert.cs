using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Alert : MonoBehaviour
{
    public Image Panel { get { return m_AlertBackground; } }
    public Image Icon { get { return m_AlertIcon; } }
    public Text Text { get { return m_AlertText; } }

    [SerializeField] protected Image m_AlertIcon;
    [SerializeField] protected Text m_AlertText;

    protected Image m_AlertBackground;

    void Awake()
    {
        m_AlertBackground = GetComponent<Image>();
    }

    public void SetColorArray(AlertColorArray colorArray)
    {
        this.Panel.color = colorArray.panelColor;
        this.Text.color = colorArray.textColor;
    }

    public void DeleteAlert()
    {
        GameController.Current.TC.DeleteAlert(this);
    }
}
