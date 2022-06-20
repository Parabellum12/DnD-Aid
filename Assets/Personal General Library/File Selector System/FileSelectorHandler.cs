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
    [SerializeField] RectTransform FilePathTextRect;
    [SerializeField] TMP_InputField FileName;

    [SerializeField] Button AcceptButton;
    [SerializeField] Button CancelButton;

    [SerializeField] GameObject FileTypeSelectorGO;

    List<string> filePathHistory = new List<string>();
    int filePathHistorySelectionIndex = 0;

    string[] extentionsToSearchFor;
    System.Action<string[]> callback;

    string selectedFilePath = "";

    private void Awake()
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
    private void Start()
    {
        Debug.Log("START:"+ filePathHistory);
    }

    void addToHistory(string path)
    {
        Debug.Log("addToHistory: " + path);
        if (filePathHistory.Count > 0)
        {
            Debug.Log("addToHistory1: " + path);
            List<string> tempFilePathHistory = new List<string>();
            for (int i = 0; i <= filePathHistorySelectionIndex; i++)
            {
                Debug.Log("addToHistory RECOVER " + filePathHistory[i]);
                tempFilePathHistory.Add(filePathHistory[i]);
            }
            tempFilePathHistory.Add(path);
            filePathHistory.Clear();
            filePathHistory.AddRange(tempFilePathHistory);
            filePathHistorySelectionIndex = filePathHistory.Count - 1;
        }
        else
        {
            Debug.Log("addToHistory2: " + path);
            filePathHistory.Add(path);
            filePathHistorySelectionIndex = filePathHistory.Count - 1;
            Debug.Log("addToHistory2B: " + filePathHistory[filePathHistorySelectionIndex]);
        }
        updateHistorySelectorsVisuals();
    }


    public void GoToPath(string path)
    {
        addToHistory(path);
        loadPath();
        Debug.Log("WHAT IS HAPPENING??????:" + filePathHistory[filePathHistorySelectionIndex]);
    }

    [SerializeField] string toChange;
    bool wantToChange = false;
    private void Update()
    {
        if (wantToChange)
        {
            wantToChange = false;
            FilePath.text = toChange;
        }
        //Debug.Log("WHY:" + filePathHistory.Count);
    }
    void loadPath()
    {
        string path = filePathHistory[filePathHistorySelectionIndex];
        FilePathTextRect.localPosition = new Vector2(0, FilePathTextRect.localPosition.y);
        FilePath.text = path;
        toChange = path;
        wantToChange = true;
        Debug.Log("loadPath:" + path + "    " + FilePath.text);
        if (Directory.Exists(path))
        {
            //handle show files
            Debug.Log("Show Folder COntents:" + path);
            Debug.Log("1: " + filePathHistory[filePathHistorySelectionIndex]);
        }
        else
        {
            if (File.Exists(path))
            {
                //handle select File
                Debug.Log("Select File:" + path);
                Debug.Log("2: " + filePathHistory[filePathHistorySelectionIndex]);
            }
            else
            {
                //show nothing found
                Debug.Log("Nothing Found: " + path);
                Debug.Log("3: " + filePathHistory[filePathHistorySelectionIndex]);
            }
        }
        Debug.Log("4: " + filePathHistory[filePathHistorySelectionIndex]);
    }




    void updateHistorySelectorsVisuals()
    {
        ForwardButton.interactable = filePathHistory.Count > 0 && (filePathHistorySelectionIndex < filePathHistory.Count - 1);
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
        if (FilePath.text.Length > 0)
        {
            if (filePathHistory.Count > 0)
            {
                if (!FilePath.text.Equals(filePathHistory[filePathHistorySelectionIndex]))
                {
                    GoToPath(FilePath.text);
                }
            }
            else
            {
                GoToPath(FilePath.text);
            }
        }
    }
}



