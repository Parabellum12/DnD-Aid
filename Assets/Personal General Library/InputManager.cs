using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Dictionary<KeyCode, System.Action> keyCodeToActionDown = new Dictionary<KeyCode, System.Action>();
    Dictionary<KeyCode, System.Action> keyCodeToActionHeld = new Dictionary<KeyCode, System.Action>();
    public enum keyInputType
    {
        Replace,
        Add
    }
    keyInputType DuplicateInputSettings = keyInputType.Replace;



    private void Update()
    {
        handleDownCalls();
        handleHeldCalls();
    }


    public void AddKeyToActionDown(KeyCode key, System.Action action)
    {
        if (DuplicateInputSettings == keyInputType.Replace)
        {
            RemoveKeyInput(key);
        }
        keyCodeToActionDown.Add(key, action);
    }

    public void AddKeyToActionHeld(KeyCode key, System.Action action)
    {
        if (DuplicateInputSettings == keyInputType.Replace)
        {
            RemoveKeyInput(key);
        }
        keyCodeToActionHeld.Add(key, action);
    }

    public void RemoveKeyInput(KeyCode key)
    {
        keyCodeToActionDown.Remove(key);
        keyCodeToActionHeld.Remove(key);
    }


    public IEnumerator ReturnKeyPressed(System.Action<KeyCode> callback)
    {
        while (true)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    callback.Invoke(key);
                    yield break;
                }
            }
            yield return null;
        }
    }


    void handleDownCalls()
    {
        foreach (KeyCode key in keyCodeToActionDown.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                keyCodeToActionDown.TryGetValue(key, out System.Action action);
                action?.Invoke();
            }
        }
    }

    void handleHeldCalls()
    {
        foreach (KeyCode key in keyCodeToActionHeld.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                keyCodeToActionHeld.TryGetValue(key, out System.Action action);
                action?.Invoke();
            }
        }
    }
}
