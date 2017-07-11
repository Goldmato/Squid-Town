using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum SpawnMethod : byte { Random, InHouses, InJail }

public enum BehaviourType : byte { RandomMovement, SeekDoors }

public enum EnemyType : byte { Squid, Starfish }

///<summary>
/// Container for Alert Data
///</summary>
public struct AlertData
{
    public string message;
    public Sprite icon;

    public AlertData(string message, Sprite icon)
    {
        this.message = message;
        this.icon = icon;
    }
}

///<summary>
/// Container for Alert Color Data
///</summary>
public class AlertColorArray
{
    public Color panelColor;
    public Color textColor;

    public static AlertColorArray Red = new AlertColorArray(Color.red, Color.black);
    public static AlertColorArray Yellow = new AlertColorArray(Color.yellow, Color.black);
    public static AlertColorArray Green = new AlertColorArray(Color.green, Color.black);

    public AlertColorArray(Color panelColor, Color textColor)
    {
        this.panelColor = panelColor;
        this.textColor = textColor;
    }
}
