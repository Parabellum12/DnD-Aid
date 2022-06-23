using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadHandler : MonoBehaviour
{
    [SerializeField] StraightLineHandler_Script straightLineHandler;
    [SerializeField] CurvedLineHandler_Script curvedLineHandler;
    string saveFileType = "MapData";

    public void LoadSave(string fileName)
    {
        SaveData saveDat = UtilClass.LoadFromFile<SaveData>(Application.persistentDataPath, fileName, saveFileType, true);
        if (saveDat == null)
        {
            Debug.LogWarning("Load File: " + fileName + " Failed");
            return;
        }

        straightLineHandler.LoadSaveData(saveDat.GetStraightLinePoints());
        curvedLineHandler.LoadSaveData(saveDat.GetCurvedLineLines());
    }

    public void Save(string fileName)
    {
        SaveData saveDat = new SaveData(straightLineHandler.GetSaveData(), curvedLineHandler.GetSaveData(), createMapId());
        UtilClass.SaveToFile(Application.persistentDataPath, fileName, saveFileType, saveDat);
    }

    string createMapId()
    {
        string id = Random.Range(int.MinValue, int.MaxValue).ToString() + Random.Range(int.MinValue, int.MaxValue).ToString();
        return id.GetHashCode().ToString() + id;
    }



    public class SaveData
    {
        List<StraightLineHandler_Script.Point> straightLinePoints;
        List<CurvedLineHandler_Script.CurvedLine> curvedLineLines;

        string MapId;

        public SaveData(List<StraightLineHandler_Script.Point> straightLinePoints, List<CurvedLineHandler_Script.CurvedLine> curvedLineLines, string mapId)
        {
            this.straightLinePoints = straightLinePoints;
            this.curvedLineLines = curvedLineLines;
            MapId = mapId;
        }


        public List<StraightLineHandler_Script.Point> GetStraightLinePoints()
        {
            return straightLinePoints;
        }

        public List<CurvedLineHandler_Script.CurvedLine> GetCurvedLineLines()
        {
            return curvedLineLines;
        }

        public string GetMapId()
        {
            return MapId;
        }
    }
} 
