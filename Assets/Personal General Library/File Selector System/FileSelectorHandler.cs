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

    string currentlySelectedExtentionType;

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
        for (int i = 0; i < extentionsToSearchFor.Length; i++)
        {
            extentionsToSearchFor[i] = extentionsToSearchFor[i].ToLower();
            if (extentionsToSearchFor[i].ToCharArray()[0] != '.')
            {
                extentionsToSearchFor[i] = "." + extentionsToSearchFor[i];
            }
        }
        this.extentionsToSearchFor = extentionsToSearchFor;
        currentlySelectedExtentionType = extentionsToSearchFor[0];

        fileExtentionToSearchForText.text = currentlySelectedExtentionType + " (*" + currentlySelectedExtentionType + ")";




        GoToPath(initialPath);
    }
    private void Start()
    {
        //Debug.Log("START:"+ filePathHistory);
    }

    void addToHistory(string path)
    {
        //Debug.Log("addToHistory: " + path);
        if (filePathHistory.Count > 0)
        {
            //Debug.Log("addToHistory1: " + path);
            List<string> tempFilePathHistory = new List<string>();
            for (int i = 0; i <= filePathHistorySelectionIndex; i++)
            {
                // Debug.Log("addToHistory RECOVER " + filePathHistory[i]);
                tempFilePathHistory.Add(filePathHistory[i]);
            }
            tempFilePathHistory.Add(path);
            filePathHistory.Clear();
            filePathHistory.AddRange(tempFilePathHistory);
            filePathHistorySelectionIndex = filePathHistory.Count - 1;
        }
        else
        {
            //Debug.Log("addToHistory2: " + path);
            filePathHistory.Add(path);
            filePathHistorySelectionIndex = filePathHistory.Count - 1;
            //Debug.Log("addToHistory2B: " + filePathHistory[filePathHistorySelectionIndex]);
        }
        updateHistorySelectorsVisuals();
    }


    public void GoToPath(string path)
    {
        addToHistory(path);
        loadPath();
        //Debug.Log("WHAT IS HAPPENING??????:" + filePathHistory[filePathHistorySelectionIndex]);
    }




    public void loadPath()
    {
        string path = filePathHistory[filePathHistorySelectionIndex];
        FilePathTextRect.localPosition = new Vector2(0, FilePathTextRect.localPosition.y);
        FilePath.text = path;
        //Debug.Log("loadPath:" + path + "    " + FilePath.text);
        if (Directory.Exists(path))
        {
            //handle show files
            //Debug.Log("Show Folder COntents:" + path);
            //Debug.Log("1: " + filePathHistory[filePathHistorySelectionIndex]);
            loadDirectory();
        }
        else
        {
            if (File.Exists(path))
            {
                //handle select File
               Debug.Log("Select File:" + path);
               // Debug.Log("2: " + filePathHistory[filePathHistorySelectionIndex]);
            }
            else
            {
                //show nothing found
                Debug.Log("Nothing Found: " + path);
                //Debug.Log("3: " + filePathHistory[filePathHistorySelectionIndex]);
            }
        }
       // Debug.Log("4: " + filePathHistory[filePathHistorySelectionIndex]);
    }

    List<FileFolderSelector_Handler> fileFolderSelector_Handlers = new List<FileFolderSelector_Handler>();
    [SerializeField] GameObject FileFolderSelectorPrefab;
    [SerializeField] GameObject NothingFoundText;
    void loadDirectory()
    {
        foreach (FileFolderSelector_Handler ff in fileFolderSelector_Handlers)
        {
            Destroy(ff.gameObject);
        }
        fileFolderSelector_Handlers.Clear();
        FileSeclector.clearChildDropDowns();

        string path = filePathHistory[filePathHistorySelectionIndex];
        string[] folderNames = Directory.GetDirectories(path);
        string[] fileNames = UtilClass.GetFileNamesWithExtension(path, currentlySelectedExtentionType.Substring(1));

        if (FileName.text.Length  == 0)
        {

            for (int i = 0; i < folderNames.Length; i++)
            {
                GameObject temp = Instantiate(FileFolderSelectorPrefab, FileSeclector.ChildrenObjectHolder.transform);
                General_UI_DropDown_Handler_ScriptV2 tempUIHandler = temp.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
                FileFolderSelector_Handler tempHandler = temp.GetComponent<FileFolderSelector_Handler>();
                fileFolderSelector_Handlers.Add(tempHandler);
                FileSeclector.addToChildDropDowns(tempUIHandler);
                string originPath = folderNames[i];
                string[] temps = folderNames[i].Split(Path.DirectorySeparatorChar);
                tempHandler.setup(true, temps[temps.Length - 1], "", (name) =>
                {
                //need to handleClick
                    //Debug.Log("Handler Folder Click");
                    GoToPath(originPath);
                });
            }
            for (int i = 0; i < fileNames.Length; i++)
            {
                GameObject temp = Instantiate(FileFolderSelectorPrefab, FileSeclector.ChildrenObjectHolder.transform);
                General_UI_DropDown_Handler_ScriptV2 tempUIHandler = temp.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
                FileFolderSelector_Handler tempHandler = temp.GetComponent<FileFolderSelector_Handler>();
                fileFolderSelector_Handlers.Add(tempHandler);
                FileSeclector.addToChildDropDowns(tempUIHandler);
                string[] temps = fileNames[i].Split(".");
                tempHandler.setup(false, fileNames[i], temps[temps.Length - 1], (name) =>
                {
                    //need to handleClick
                    //Debug.Log("Handler File Click");
                    HandleFileSelect(name);
                });
            }
        }
        else
        {
            for (int i = 0; i < folderNames.Length; i++)
            {
                if (!folderNames[i].Split(".")[0].ToLower().Contains(FileName.text.ToLower()))
                {
                    continue;
                }
                GameObject temp = Instantiate(FileFolderSelectorPrefab, FileSeclector.ChildrenObjectHolder.transform);
                General_UI_DropDown_Handler_ScriptV2 tempUIHandler = temp.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
                FileFolderSelector_Handler tempHandler = temp.GetComponent<FileFolderSelector_Handler>();
                fileFolderSelector_Handlers.Add(tempHandler);
                FileSeclector.addToChildDropDowns(tempUIHandler);
                string originPath = folderNames[i];
                string[] temps = folderNames[i].Split(Path.DirectorySeparatorChar);
                tempHandler.setup(true, temps[temps.Length - 1], "", (name) =>
                {
                    //need to handleClick
                    //Debug.Log("Handler Folder Click");
                    GoToPath(originPath);
                });
            }
            for (int i = 0; i < fileNames.Length; i++)
            {                      
                if (!fileNames[i].Split(".")[0].ToLower().Contains(FileName.text.ToLower()))
                {
                    continue;
                }
                GameObject temp = Instantiate(FileFolderSelectorPrefab, FileSeclector.ChildrenObjectHolder.transform);
                General_UI_DropDown_Handler_ScriptV2 tempUIHandler = temp.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
                FileFolderSelector_Handler tempHandler = temp.GetComponent<FileFolderSelector_Handler>();
                fileFolderSelector_Handlers.Add(tempHandler);
                FileSeclector.addToChildDropDowns(tempUIHandler);
                string[] temps = fileNames[i].Split(".");
                tempHandler.setup(false, fileNames[i], temps[temps.Length-1], (name) =>
                {
                    //need to handleClick
                    //Debug.Log("Handler File Click");
                    HandleFileSelect(name);
                });
            }

        }

        if (fileFolderSelector_Handlers.Count == 0)
        {
            //show nothing found text
            NothingFoundText.SetActive(true);
        }
        else
        {
            //hide nothing found text
            NothingFoundText.SetActive(false);
        }


        FileSeclector.setUIPositionsNoCallback();
        FileSeclector.setUIPositions();
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

    List<Button> typeButtons = new List<Button>();
    [SerializeField] GameObject typeSelectorPreFab;
    [SerializeField] TMP_Text fileExtentionToSearchForText;
    public void handleFileTypeSelectorClick()
    {
        FileTypeSelectorGO.SetActive(!FileTypeSelectorGO.activeSelf);
        if (FileTypeSelectorGO.activeSelf)
        {
            foreach (Button b in typeButtons)
            {
                Destroy(b.gameObject);
            }
            typeButtons.Clear();
            FileTypeSelector.clearChildDropDowns();
            for (int i = 0; i < extentionsToSearchFor.Length; i++)
            {
                if (currentlySelectedExtentionType.Equals(extentionsToSearchFor[i]))
                {
                    continue;
                }
                GameObject temp = Instantiate(typeSelectorPreFab, FileTypeSelector.gameObject.transform);
                General_UI_DropDown_Handler_ScriptV2 tempHandler = temp.GetComponent<General_UI_DropDown_Handler_ScriptV2>();
                TMP_Text tempText = temp.GetComponentInChildren<TMP_Text>();

                string extentionName = extentionsToSearchFor[i].Substring(1);
                if (extentionName.Equals("*"))
                {
                    extentionName = "All Files";
                }

                int index = i;
                tempText.text = extentionName + " (*" + extentionsToSearchFor[index] + ")";
                Button button = temp.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    handleTypeSelectorSelected(extentionsToSearchFor[index]);
                });
                typeButtons.Add(button);
                FileTypeSelector.addToChildDropDowns(tempHandler);
            }
            FileTypeSelector.setUIPositionsNoCallback();
            FileTypeSelector.setUIPositions();
        }
    }

    public void handleTypeSelectorSelected(string extention)
    {
        for (int i = 0; i < extentionsToSearchFor.Length; i++)
        {
            if (extentionsToSearchFor[i].Equals(extention))
            {
                currentlySelectedExtentionType = extention;


                string extentionName = extentionsToSearchFor[i].Substring(1);
                if (extentionName.Equals("*"))
                {
                    extentionName = "All Files";
                }
                fileExtentionToSearchForText.text = extentionName + " (*" + extentionsToSearchFor[i] + ")";
                handleFileTypeSelectorClick();
                loadPath();
            }
        }
    }

    void HandleFileSelect(string fileName)
    {
        string path = Path.Combine(FilePath.text, fileName);
        //Debug.Log("HandleFileSelect:" + path);
        callback.Invoke(new string[] { path});
        Destroy(gameObject.transform.parent.gameObject);
    }




    public void HandleFilePathEndEdit()
    {
        if (FilePath.text.Length > 0 && Directory.Exists(FilePath.text) || File.Exists(FilePath.text))
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
        else
        {
            FilePath.text = filePathHistory[filePathHistorySelectionIndex];
        }

    }
}



