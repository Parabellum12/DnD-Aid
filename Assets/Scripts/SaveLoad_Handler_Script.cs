using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoad_Handler_Script : MonoBehaviour
{
    [SerializeField] StraightLine_Handler_ScriptV2 straightLineHandler;
    [SerializeField] CurvedLine_Handler_ScriptV2 curvedLineHandler;
    List<saveClass> CachedSaveData = new List<saveClass>();
    saveClass CurrentlyLoadedSaveData;
    //file data standard: <string data>;<string data>
    //string data standard: <overall tag>[<data>|<data>
    //data standard: <data value tag>:<value>,<value>

    private void Start()
    {
        getSaveFiles();
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
        if (doesCacheContainFile(fileName) != null)
        {
            loadFromObjectCache(fileName);
        }
        else 
        {
            //not currently loaded in cache
            string persistentDataPath = Application.persistentDataPath + "/" + fileName + "." + fileType;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream sr = new FileStream(persistentDataPath, FileMode.Open);
            saveClass temp = bf.Deserialize(sr) as saveClass;
            CachedSaveData.Add(temp);
            loadFromObjectCache(fileName);
        }

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
        bf.Serialize(fs, getSaveData(fileName));
        fs.Close();
    }

    public void loadFromObjectCache(string fileName)
    {
        foreach (saveClass sc in CachedSaveData)
        {
            if (sc.MapName.Equals(fileName))
            {
                CurrentlyLoadedSaveData = sc;
                LoadCurrentObjectCache();
                break;
            }
        }
    }

    private void LoadCurrentObjectCache()
    {
        if (CurrentlyLoadedSaveData != null)
        {
            curvedLineHandler.setDataFromSaveData(CurrentlyLoadedSaveData.CurvedLines);
            curvedLineHandler.drawLines();
            straightLineHandler.setDataFromSaveData(CurrentlyLoadedSaveData.StraightLinePoints);
            straightLineHandler.drawLines();
        }
    }

    saveClass doesCacheContainFile(string fileName)
    {
        foreach (saveClass data in CachedSaveData)
        {
            if (data.MapName.Equals(fileName))
            {
                return data;
            }
        }
        return null;
    }




    private saveClass getSaveData(string fileName)
    {
        return new saveClass(fileName, curvedLineHandler.getCurvedLinesAsSaveData(), straightLineHandler.getLinePoints());
    }

    [System.Serializable]
    public class saveClass
    {
        public float3[][] CurvedLines;
        public float2[][] StraightLinePoints;
        public string MapName;

        public saveClass(string fileName, float3[][] CurvedLines, float2[][] StraightLinePoints)
        {
            this.CurvedLines = CurvedLines;
            this.StraightLinePoints = StraightLinePoints;
            this.MapName = fileName;
        }

    }

}
