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
    saveClass CurrentlyLoadedSaveData = null;
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
            //Debug.Log(returner[index]);
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

            //Debug.Log(returner[index]);
            index++;
        }
        return returner;
    }

    public void loadFromFile(string fileName)
    {
        //Debug.Log(fileName);
        if (doesCacheContainFile(fileName) != null)
        {
            Debug.Log("loadFromFile Cache");
            loadFromObjectCache(fileName);
        }
        else 
        {
            Debug.Log("loadFromFile File");
            //not currently loaded in cache
            addToCache(fileName);
            loadFromObjectCache(fileName);
        }

    }

    public void addToCache(string fileName)
    {
        string persistentDataPath = Application.persistentDataPath + "/" + fileName + "." + fileType;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream sr = new FileStream(persistentDataPath, FileMode.Open);
        saveClass temp = bf.Deserialize(sr) as saveClass;
        CachedSaveData.Add(temp);
    }

    public void removeFromCache(string fileName)
    {
        foreach (saveClass sc in CachedSaveData)
        {
            if (sc.MapName.Equals(fileName))
            {
                CachedSaveData.Remove(sc);
                return;
            }
        }
    }

    public saveClass getMapData(string mapName)
    {
        foreach (saveClass sc in CachedSaveData)
        {
            if (sc.MapName.Equals(mapName))
            {
                return sc;
            }
        }
        bool avalible = false;
        foreach (string s in getSaveFileNames())
        {
            if (s.Equals(mapName))
            {
                avalible = true;
            }
        }
        if (!avalible)
        {
            return null;
        }
        addToCache(mapName);
        return getMapData(mapName);
    }


    public void saveToFile(string fileName)
    {
        saveClass test = doesCacheContainFile(fileName);
        if (test != null)
        {
            CachedSaveData.Remove(test);
        }


        //Debug.Log("Save To File: " + fileName);
        BinaryFormatter bf = new BinaryFormatter();
        string persistentDataPath = Application.persistentDataPath + "/" + fileName + "." + fileType;
        FileStream fs;
        //Debug.Log("FilePath:" + persistentDataPath);
        if (File.Exists(persistentDataPath))
        {
            //Debug.Log("replace F");
            fs = new FileStream(persistentDataPath, FileMode.Truncate);
        }
        else
        {
            //Debug.Log("new F");
            fs = new FileStream(persistentDataPath, FileMode.CreateNew);
        }
        bf.Serialize(fs, getSaveData(fileName));
        fs.Close();
        loadFromFile(fileName);
    }

    public void loadFromObjectCache(string fileName)
    {
        //Debug.Log("CachedSaveData:" + CachedSaveData.Count + " loadFromObjectCache:" + fileName);
        foreach (saveClass sc in CachedSaveData)
        {
            Debug.Log("AAA:"+sc.MapName);
            if (sc != null && sc.MapName.Equals(fileName))
            {
                CurrentlyLoadedSaveData = sc;
                LoadCurrentObjectCache();
                return;
            }
        }
        //Debug.LogError("No Cached Obejct Found");
    }

    public void loadMap(saveClass mapToLoad)
    {
        if (mapToLoad == null)
        {
            return;
        }
        CurrentlyLoadedSaveData = mapToLoad;
        LoadCurrentObjectCache();
    }

    public List<saveClass> getLocalMapCache()
    {
        return CachedSaveData;
    }

    public string getCurrentlyLoadedMapName()
    {
        if (CurrentlyLoadedSaveData == null)
        {
            return "";
        }
        else
        {
            return CurrentlyLoadedSaveData.MapName;
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
        public float3[][] CurvedLines = new float3[0][];
        public float2[][] StraightLinePoints = new float2[0][];
        public string MapName = "";

        public saveClass(string fileName, float3[][] CurvedLines, float2[][] StraightLinePoints)
        {
            this.CurvedLines = CurvedLines;
            this.StraightLinePoints = StraightLinePoints;
            this.MapName = fileName;
        }

    }


    public byte[] ObjectToByteArray(saveClass obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    public saveClass ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        saveClass obj = (saveClass)binForm.Deserialize(memStream);

        return obj;
    }

}
