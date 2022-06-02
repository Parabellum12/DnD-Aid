using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectButton_Handler : MonoBehaviour
{
    [SerializeField] Image selected;
    [SerializeField] Button button;
    [SerializeField] Main_Handler_Script.tools tool;

    public void deSelect()
    {
        selected.enabled = false;
    }

    public void select()
    {
        selected.enabled = true;
    }

    public void setup(System.Action<Main_Handler_Script.tools> callback)
    {
        deSelect();
        button.onClick.AddListener(() =>
        {
            callback.Invoke(tool);
        });
    }
}
