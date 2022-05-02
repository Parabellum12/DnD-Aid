using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class General_ViewportContentItemMapSelector_Script : MonoBehaviour
{
    [SerializeField] TMP_Text mapNameText;
    [SerializeField] Button loadMapButton;
    [SerializeField] Image CachedSelector;
    bool isCached;
    float selectorMoveDist = 20;

    System.Action onChangeChacheValue;
    public void setup(string mapName, bool isChached, System.Action loadMapButtonOnClick, System.Action onChangeChacheValue)
    {
        mapNameText.text = mapName;
        this.onChangeChacheValue = onChangeChacheValue;
        this.isCached = isChached;
        reflectCachedValueValue();
        loadMapButton.onClick.AddListener(() =>
        {
            loadMapButtonOnClick.Invoke();
        });
    }

    public void setCachedSelectorToTrue()
    {
        CachedSelector.transform.localPosition = new Vector2(selectorMoveDist, 0);
    }

    public void setCachedSelectorToFalse()
    {
        CachedSelector.transform.localPosition = new Vector2(-selectorMoveDist, 0);
    }

    public void updateCachedValue()
    {
        isCached = !isCached;
        reflectCachedValueValue();
    }

    public void reflectCachedValueValue()
    {
        if (isCached)
        {
            setCachedSelectorToTrue();
        }
        else
        {
            setCachedSelectorToFalse();
        }
        onChangeChacheValue.Invoke();
    }
}
