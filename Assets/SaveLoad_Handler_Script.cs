using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad_Handler_Script : MonoBehaviour
{
    string CachedFile;
    [SerializeField] StraightLine_Handler_ScriptV2 straightLineHandler;
    [SerializeField] CurvedLine_Handler_ScriptV2 curvedLineHandler;

    //file data standard: <string data>;<string data>
    //string data standard: <overall tag>[<data>|<data>
    //data standard: <data value tag>:<value>,<value>

    private void loadFile(string fileName)
    {

    }

    public void loadFromFile(string fileName)
    {

    }

    public void loadFromCache()
    {
        string[] data = CachedFile.Split('|');
        for (int i = 0; i < data.Length; i++)
        {
            string[] tagAndValues = data[i].Split(':');
        }
    }


}
