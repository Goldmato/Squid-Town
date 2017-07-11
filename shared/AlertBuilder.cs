using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class AlertBuilder
{
    public static void EnemyEscapedAlert(string enemyName, int quantity = 1)
    {
        GameController.Current.TC.CreateAlert(new AlertData(quantity + " " + enemyName + "(s) have escaped from prison!",
            IconRegistry.GetIcon(IconRegistry.AlertIcons, 0)), AlertColorArray.Red);
    }

    public static void StarfishBreakoutAlert()
    {
        GameController.Current.TC.CreateAlert(new AlertData("Starfish is going to break an enemy out of prison!",
            IconRegistry.GetIcon(IconRegistry.WarningIcons, 0)), AlertColorArray.Yellow);
    }

    public static void StarfishEscapeWarningAlert(int seconds)
    {
        GameController.Current.TC.CreateAlert(new AlertData("Starfish will escape from jail in " + seconds + " seconds!",
            IconRegistry.GetIcon(IconRegistry.WarningIcons, 0)), AlertColorArray.Yellow);
    }

    public static void GameWonAlert()
    {
        // TODO: Create game winning icon
        GameController.Current.TC.CreateAlert(new AlertData("You've won the game!", null), AlertColorArray.Green);
    }
}
