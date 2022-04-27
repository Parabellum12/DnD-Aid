using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoad_Handler_Script : MonoBehaviour
{
    string CachedFile;
    [SerializeField] StraightLine_Handler_ScriptV2 straightLineHandler;
    [SerializeField] CurvedLine_Handler_ScriptV2 curvedLineHandler;

    //file data standard: <string data>;<string data>
    //string data standard: <overall tag>[<data>|<data>
    //data standard: <data value tag>:<value>,<value>

    private void Start()
    {
        getSaveFiles();
    }

    public void loadFromCache()
    {
        string[] data = CachedFile.Split(';');
        if (data.Length == 0)
        {
            return;
        }
        for (int i = 0; i < data.Length; i++)
        {
            Debug.Log("LoadTagSys:" + data[i]);
            string[] tagAndValues = data[i].Split('[');
            if (tagAndValues[0].Equals("SLTD"))
            {
                //straight line
                straightLineHandler.LoadFromSavePointsAsString(data[i]);
            }
            else if (tagAndValues[0].Equals("TCLD"))
            {
                //curve line data
                curvedLineHandler.loadFromString(data[i]);
            }
        }
        straightLineHandler.drawLines();
        curvedLineHandler.drawLines();
    }
    public void saveToCache()
    {
        CachedFile = straightLineHandler.ReturnSavePointsAsString() + ";" + curvedLineHandler.getCurveLineSaveData();
    }

    public void saveToFile(string fileName)
    {
        Debug.Log("Save To File: " + fileName);
        BinaryFormatter bf = new BinaryFormatter();
        string persistentDataPath = Application.persistentDataPath + "/" + fileName + "." + fileType;
        FileStream fs;
        Debug.Log("FilePath:" + persistentDataPath);
        if (File.Exists(persistentDataPath))
        {
            Debug.Log("replace F");
            fs = new FileStream(persistentDataPath, FileMode.Truncate);
        }
        else
        {
            Debug.Log("new F");
            fs = new FileStream(persistentDataPath, FileMode.CreateNew);
        }
        bf.Serialize(fs, getSaveData());
        fs.Close();
    }

    public string fileNamePathSeperator = ":_|-|_:";
    public string fileType = "SaveData";
    public string[] getSaveFileNames()
    {
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*." + fileType);
        string[] returner = new string[files.Length];
        int index = 0;
        foreach (string s in files)
        {
            int startIndex = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s.ToCharArray()[i] == '/' || s.ToCharArray()[i] == '\\')
                {
                    startIndex = i;
                    break;
                }
            }
            string mapName = "";
            for (int i = startIndex + 1; i < s.Length; i++)
            {
                if (s.ToCharArray()[i] == '.')
                {
                    break;
                }
                mapName += s.ToCharArray()[i];
            }
            returner[index] = mapName;
            Debug.Log(returner[index]);
            index++;
        }
        return returner;
    }

    public string[] getSaveFiles()
    {
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*." + fileType);
        string[] returner = new string[files.Length];
        int index = 0;
        foreach (string s in files)
        {
            bool active = false;
            int startIndex = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s.ToCharArray()[i] == '/' || s.ToCharArray()[i] == '\\')
                {
                    startIndex = i;
                    break;
                }
            }
            string mapName = "";
            for (int i = startIndex+1; i < s.Length; i++)
            {
                if (s.ToCharArray()[i] == '.')
                {
                    break;
                }
                mapName += s.ToCharArray()[i];
            }
            returner[index] = mapName + fileNamePathSeperator + s;

            Debug.Log(returner[index]);
            index++;
        }
        return returner;
    }

    public void loadFromFile(string fileName)
    {
        Debug.Log(fileName);
    }


    public void saveToObjectCache()
    {
        objectCashe = getSaveData();
    }

    public void loadFromObjectCache()
    {
        if (objectCashe != null)
        {
            curvedLineHandler.setDataFromSaveData(objectCashe.CurvedLines);
            straightLineHandler.setDataFromSaveData(objectCashe.StraightLinePoints);
        }
    }


    saveClass objectCashe;


    private saveClass getSaveData()
    {
        return new saveClass(curvedLineHandler.getCurvedLinesAsSaveData(), straightLineHandler.getLinePoints());
    }

    [System.Serializable]
    public class saveClass
    {
        public float3[][] CurvedLines;
        public float2[][] StraightLinePoints;

        public saveClass(float3[][] CurvedLines, float2[][] StraightLinePoints)
        {
            this.CurvedLines = CurvedLines;
            this.StraightLinePoints = StraightLinePoints;
        }
    }

}
