using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
public class FileSelectorHandler : MonoBehaviour
{
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 FolderSelector;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 FileTypeSelector;
    [SerializeField] General_UI_DropDown_Handler_ScriptV2 FileSeclector;

    [SerializeField] Button BackButton;
    [SerializeField] Button ForwardButton;

    [SerializeField] TMP_InputField FilePath;
    [SerializeField] TMP_InputField FileName;

    [SerializeField] Button AcceptButton;
    [SerializeField] Button CancelButton;

    [SerializeField] GameObject FileTypeSelectorGO;

    List<string> filePathHistory = new List<string>();
    int filePathHistorySelectionIndex = 0;

    string[] extentionsToSearchFor;
    System.Action<string[]> callback;

    string selectedFilePath = "";

    private void Start()
    {
        FileTypeSelectorGO.SetActive(false);
        BackButton.onClick.AddListener(() =>
        {
            handleBackwardHistory();
        });
        ForwardButton.onClick.AddListener(() =>
        {
            handleForwardHistory();
        });
        updateHistorySelectorsVisuals();

        AcceptButton.onClick.AddListener(() =>
        {
            handleAcceptClick();
        });
        CancelButton.onClick.AddListener(() =>
        {
            handleCancelClick();
        });
    }
    public void OpenFileSelector(string initialPath, string[] extentionsToSearchFor, bool lockSearchToInitialPath, System.Action<string[]> callback)
    {
        this.callback = callback;
        this.extentionsToSearchFor = extentionsToSearchFor;
        GoToPath(initialPath);
    }

    void addToHistory(string path)
    {
        if (filePathHistory.Count > 0)
        {
            List<string> tempFilePathHistory = new List<string>();
            for (int i = 0; i <= filePathHistorySelectionIndex; i++)
            {
                tempFilePathHistory.Add(filePathHistory[i]);
                tempFilePathHistory.Add(path);
            }
            filePathHistory.Clear();
            filePathHistory.AddRange(tempFilePathHistory);
            filePathHistorySelectionIndex = filePathHistory.Count - 1;
        }
        else
        {
            filePathHistory.Add(path);
            filePathHistorySelectionIndex = 0;
        }
    }


    public void GoToPath(string path)
    {
        addToHistory(path);
        loadPath();
    }
    void loadPath()
    {
        string path = filePathHistory[filePathHistorySelectionIndex];
        FilePath.text = path;
        Debug.Log("loadPath:" + path + "    " + FilePath.text);
        if (Directory.Exists(path))
        {
            //handle show files
            Debug.Log("Show Folder COntents:" + path);
        }
        else
        {
            if (File.Exists(path))
            {
                //handle select File
                Debug.Log("Select File:" + path);
            }
            else
            {
                //show nothing found
                Debug.Log("Nothing Found: " + path);
            }
        }
    }




    void updateHistorySelectorsVisuals()
    {
        ForwardButton.interactable = (filePathHistorySelectionIndex < filePathHistory.Count - 1) && filePathHistory.Count > 0;
        BackButton.interactable = filePathHistorySelectionIndex > 0 && filePathHistory.Count > 1;
    }

    public void handleForwardHistory()
    {
        filePathHistorySelectionIndex++;
        updateHistorySelectorsVisuals();
        loadPath();
    }

    public void handleBackwardHistory()
    {
        filePathHistorySelectionIndex--;
        updateHistorySelectorsVisuals();
        loadPath();
    }




    public void handleAcceptClick()
    {
        if (!File.Exists(selectedFilePath) || selectedFilePath.Length == 0)
        {
            return;
        }
        else
        {
            callback?.Invoke(new string[] { selectedFilePath });
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    public void handleCancelClick()
    {
        callback?.Invoke(new string[] { });
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void handleFileTypeSelectorClick()
    {
        FileTypeSelectorGO.SetActive(!FileTypeSelectorGO.activeSelf);
    }


    public void HandleFilePathEndEdit()
    {
        GoToPath(FilePath.text);
    }
}



