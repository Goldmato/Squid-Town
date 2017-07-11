using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class IconRegistry
{
    public static string WarningIcons { get { return m_WarningKey; } }
    public static string AlertIcons { get { return m_AlertKey; } }

    private static Dictionary<string, List<Sprite>> m_IconRegistry = new Dictionary<string, List<Sprite>>();
    private static List<string> m_IconKeys = new List<string>();

    private static string m_WarningKey;
    private static string m_AlertKey;

    const string RESOURCE_PATH = "/Resources/";
    const string UI_PATH = "sprites/ui/";

    public static void LoadIcons()
    {
        // First get a list of all the folder path names within the UI folder
        var folders = Directory.GetDirectories(Application.dataPath + RESOURCE_PATH + UI_PATH);

        for(int i = 0; i < folders.Length; i++)
        {
            string folderName = Path.GetFileName(folders[i]);

            m_IconRegistry.Add(folderName, Resources.LoadAll<Sprite>(UI_PATH + folderName).ToList());
            m_IconKeys.Add(folderName);
        }

        // Set up predefined key variables for easy public access
        m_WarningKey = m_IconKeys.Find(x => x.Contains("warn"));
        m_AlertKey = m_IconKeys.Find(x => x.Contains("alert"));
    }

    public static Sprite GetIcon(string key, int index)
    {
        if(m_IconRegistry[key][index] == null)
            throw new UnityException("Sprite does not exist, please check to make sure " +
                "that the correct key is being used or isn't misspelled");

        return m_IconRegistry[key][index];
    }
}
