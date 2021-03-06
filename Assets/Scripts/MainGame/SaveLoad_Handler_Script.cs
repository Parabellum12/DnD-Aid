using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoad_Handler_Script : MonoBehaviour
{
    /*
     * handles saving and loading to and from save files
     */
    [SerializeField] StraightLine_Handler_ScriptV2 straightLineHandler;
    [SerializeField] CurvedLine_Handler_ScriptV2 curvedLineHandler;
    List<saveClass> CachedSaveData = new List<saveClass>();
    List<string> errorFileNames = new List<string>();
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
        UtilClass.GetFileNames(Application.persistentDataPath);
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
        for (int i = 0; i < returner.Length; i++)
        {
            saveClass temp = null;
            try
            {
                string persistentDataPath = Application.persistentDataPath + "/" + returner[i] + "." + fileType;
                BinaryFormatter bf = new BinaryFormatter();
                FileStream sr = new FileStream(persistentDataPath, FileMode.Open);
                try
                {
                    Debug.Log("Deserializing " + returner[i]);
                    temp = bf.Deserialize(sr) as saveClass;
                    sr.Close();
                }
                catch
                {
                    Debug.Log("Error In Deserializing " + returner[i]);
                    errorFileNames.Add(returner[i]);
                    sr.Close();
                }
            }
            catch
            {
                temp = null;
            }
            if (temp == null)
            {
                handleErroredFiles();
            }
        }
        return returner;
    }

    public string[] getSaveFileMapDataNames()
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

        string[] nameReturner = new string[returner.Length];
        for (int i = 0; i < returner.Length; i++)
        {
            saveClass temp = getMapData(returner[i]);
            if (temp != null)
            {
                nameReturner[i] = temp.MapName;
            }
            else
            {
                return getSaveFileMapDataNames();
            }
        }
        return nameReturner;
    }


    public string[] getSaveFiles()
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
        Debug.Log("loadFromFile:"+fileName);
        if (doesCacheContainFileViaId(fileName) != null)
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
        Debug.Log("Open FIleStream:" + fileName);
        string persistentDataPath = Application.persistentDataPath + "/" + fileName + "." + fileType;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream sr = new FileStream(persistentDataPath, FileMode.Open);
        saveClass temp = null;
        try
        {
            Debug.Log("Deserializing " + fileName);
            temp = bf.Deserialize(sr) as saveClass;
        }
        catch
        {
            Debug.Log("Error In Deserializing " + fileName);
            errorFileNames.Add(fileName);
            sr.Close();
            return;
        }
        CachedSaveData.Add(temp);
        sr.Close();

        Debug.Log("close FIleStream:" + fileName);
    }

    public void removeFromCache(string mapId)
    {
        foreach (saveClass sc in CachedSaveData)
        {
            if (sc.MapID.Equals(mapId))
            {
                CachedSaveData.Remove(sc);
                return;
            }
        }
    }

    public saveClass getMapData(string mapId)
    {
        if (errorFileNames.Contains(mapId))
        {
            handleErroredFiles();
            return null;
        }
        foreach (saveClass sc in CachedSaveData)
        {
            if (sc == null)
            {
                Debug.LogError("saveclass is null");
            }
            if (sc.MapID.Equals(mapId))
            {
                return sc;
            }
        }
        bool avalible = false;
        foreach (string s in getSaveFileNames())
        {
            if (s.Equals(mapId))
            {
                avalible = true;
            }
        }
        if (!avalible)
        {
            return null;
        }
        addToCache(mapId);
        return getMapData(mapId);
    }


    public void saveToFile(string fileName)
    {
        saveClass test = doesCacheContainFileViaName(fileName);
        if (test != null)
        {
            string tempName = test.MapName;
            string tempID = test.MapID;
            Debug.Log("get New MapData");
            test = getSaveData(fileName);
            test.MapName = tempName;
            test.MapID = tempID;
            if (test.MapID.Length == 0)
            {
                test.MapID = createMapId(fileName);
            }

        }
        else
        {

            test = getSaveData(fileName);
        }



        Debug.Log("Save To File: " + fileName);
        BinaryFormatter bf = new BinaryFormatter();
        string persistentDataPath = Application.persistentDataPath + "/" + test.MapID + "." + fileType;
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
        if (test == null)
        {
            bf.Serialize(fs, test);
        }
        else
        {
            bf.Serialize(fs, test);
        }
        fs.Close();
        removeFromCache(test.MapID);
        loadFromFile(test.MapID);
    }

    public void saveToFile(saveClass test, bool loadToFileAlso)
    {
         
        BinaryFormatter bf = new BinaryFormatter();
        string persistentDataPath = Application.persistentDataPath + "/" + test.MapID + "." + fileType;
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
        if (test == null)
        {
            bf.Serialize(fs, test);
        }
        else
        {
            bf.Serialize(fs, test);
        }
        fs.Close();
        if (loadToFileAlso)
        {
            loadFromFile(test.MapID);
        }
    }

    public void loadFromObjectCache(string fileName)
    {
        Debug.Log("CachedSaveData:" + CachedSaveData.Count + " loadFromObjectCache:" + fileName);
        foreach (saveClass sc in CachedSaveData)
        {
            Debug.Log("AAA:"+sc.MapName);
            if (sc != null && sc.MapID.Equals(fileName))
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
            Debug.Log("Map To Load Was Null");
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

    public string getCurrentlyLoadedMapID()
    {
        if (CurrentlyLoadedSaveData == null)
        {
            return "";
        }
        else
        {
            return CurrentlyLoadedSaveData.MapID;
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
        else
        {
            Debug.Log("CurrentlyLoadedSaveData is null");
        }
    }

    saveClass doesCacheContainFileViaId(string mapId)
    {
        foreach (saveClass data in CachedSaveData)
        {
            if (data.MapID.Equals(mapId))
            {
                return data;
            }
        }
        return null;
    }

    saveClass doesCacheContainFileViaName(string mapName)
    {
        foreach (saveClass data in CachedSaveData)
        {
            if (data.MapName.Equals(mapName))
            {
                return data;
            }
        }
        return null;
    }




    private saveClass getSaveData(string fileName)
    {
        return new saveClass(fileName, createMapId(fileName), curvedLineHandler.getCurvedLinesAsSaveData(), straightLineHandler.getLinePoints());
    }

    string createMapId(string mapName)
    {
        return UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString() + mapName.GetHashCode();
    }



    [System.Serializable]
    public class saveClass
    {
        public float3[][] CurvedLines = new float3[0][];
        public float2[][] StraightLinePoints = new float2[0][];
        public string MapName = "";
        public string MapID = "";
        public saveClass(string fileName, string MapID, float3[][] CurvedLines, float2[][] StraightLinePoints)
        {
            this.MapID = MapID;
            this.CurvedLines = CurvedLines;
            this.StraightLinePoints = StraightLinePoints;
            this.MapName = fileName;
        }

    }


    void handleErroredFiles()
    {
        string[] folderNames = Directory.GetDirectories(Application.persistentDataPath);
        string errorFolderPath = Application.persistentDataPath + "/" + "ErroredFiles";
        bool errorFolderExist = false;
        foreach (string s in folderNames)
        {
            if (s.Equals("ErroredFiles"))
            {
                errorFolderExist = true;
                break;
            }
        }
        if (!errorFolderExist)
        { 
            DirectoryInfo temp = Directory.CreateDirectory(errorFolderPath);
        }
        foreach (string s in errorFileNames)
        {
            string errorFilePath = Application.persistentDataPath + "/" + s + "." + fileType;
            string newErrorFilePath = errorFolderPath + "/" + s + "." + fileType;
            File.Move(errorFilePath, newErrorFilePath);
        }
        errorFileNames.Clear();
    }



}
