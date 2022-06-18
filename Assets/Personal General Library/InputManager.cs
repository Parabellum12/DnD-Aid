using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] KeyCode modifierKey = KeyCode.LeftAlt;
    Dictionary<string, KeyCode> actionNameToKey = new Dictionary<string, KeyCode>();
    Dictionary<KeyCode, List<string>> keyToActionName = new Dictionary<KeyCode, List<string>>();
    Dictionary<string, System.Action> actionNameToFunctionCall = new Dictionary<string, System.Action>();
    Dictionary<string, KeyActionType> actionCallType = new Dictionary<string, KeyActionType>();
    Dictionary<string, bool> ActionToRequiresModifier = new Dictionary<string, bool>();
    [SerializeField]List<string> KeyBindings = new List<string>();
    public enum KeyActionType
    {
        None,
        Down,
        Pressed,
        Up,
    }


    public IEnumerator ReturnKeyDown(System.Action<KeyCode, bool> callback)
    {
        while (true)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key) && key != modifierKey)
                {
                    callback.Invoke(key, Input.GetKey(modifierKey));
                    yield break;
                }
            }
            yield return null;
        }
    }

    void handleInspectorVisuals()
    {
        KeyBindings.Clear();
        foreach (KeyCode key in keyToActionName.Keys)
        {
            keyToActionName.TryGetValue(key, out List<string> s);
            foreach (string s2 in s)
            {
                actionCallType.TryGetValue(s2, out KeyActionType type);
                KeyBindings.Add(key.ToString() + ":" + type.ToString() + "  " + s2);
            }
        }
    }



    public void AddKeyBinding(KeyCode key, bool requiresModifier, KeyActionType actionType, string ActionNameOrID, System.Action action)
    {
        if (actionNameToKey.ContainsKey(ActionNameOrID))
        {
            RemoveKeyBinding(ActionNameOrID);
        }
        actionCallType.Add(ActionNameOrID, actionType);
        actionNameToKey.Add(ActionNameOrID, key);
        ActionToRequiresModifier.Add(ActionNameOrID, requiresModifier);
        if (keyToActionName.ContainsKey(key))
        {
            keyToActionName.TryGetValue(key, out List<string> temp);
            temp.Add(ActionNameOrID);
            keyToActionName.Remove(key);
            keyToActionName.Add(key, temp);
        }
        else
        {
            List<string> temp = new List<string>();
            temp.Add(ActionNameOrID);
            keyToActionName.Add(key, temp);
        }
        actionNameToFunctionCall.Add(ActionNameOrID, action);


        keyToActionName.TryGetValue(key, out List<string> temp2);
        handleInspectorVisuals();
        //Debug.Log(ActionNameOrID + ":"+temp2.Count);
    }

    public void RemoveKeyBinding(KeyCode key)
    {
        keyToActionName.TryGetValue(key, out List<string> actionNames);
        if (keyToActionName.ContainsKey(key))
        {
            keyToActionName.Remove(key);
            foreach (string actionName in actionNames)
            {
                actionNameToKey.Remove(actionName);
                actionNameToFunctionCall.Remove(actionName);
                actionCallType.Remove(actionName);
            }
        }
        handleInspectorVisuals();
    }
    public void RemoveKeyBinding(string actionName)
    {
        if (actionNameToKey.ContainsKey(actionName))
        {
            actionNameToKey.TryGetValue(actionName, out KeyCode key);
            actionNameToFunctionCall.Remove(actionName);
            actionCallType.Remove(actionName);


            keyToActionName.TryGetValue(key, out List<string> temp);
            temp.Remove(actionName);
            keyToActionName.Remove(key);
            keyToActionName.Add(key, temp);
            actionNameToKey.Remove(actionName);
            ActionToRequiresModifier.Remove(actionName);
        }
        handleInspectorVisuals();
    }


    private void LateUpdate()
    {
        CallKeyBindingHandling();
    }
    public void CallKeyBindingHandling()
    {
        foreach (KeyCode key in keyToActionName.Keys)
        {
            HandleKeyBindingActionCall(key);
        }
    }

    void HandleKeyBindingActionCall(KeyCode key)
    {
        keyToActionName.TryGetValue(key, out List<string> actionNames);
        foreach (string actionName in actionNames)
        {


            actionCallType.TryGetValue(actionName, out KeyActionType type);
            bool modifierActive = true;
            ActionToRequiresModifier.TryGetValue(actionName, out bool result);
            if (result)
            {
                modifierActive = Input.GetKey(modifierKey);
            }
            switch (type)
            {
                case KeyActionType.None:
                    //Debug.Log("Call " + actionName + " Error");
                    return;
                case KeyActionType.Down:


                    if (Input.GetKeyDown(key) && modifierActive)
                    {
                        actionNameToFunctionCall.TryGetValue(actionName, out System.Action callback);
                        //Debug.Log("Call " + actionName + " Down");
                        callback?.Invoke();
                    }
                    //Debug.Log("Test Call " + actionName + " Down");
                    break;
                case KeyActionType.Up:
                    if (Input.GetKeyUp(key) && modifierActive)
                    {
                        actionNameToFunctionCall.TryGetValue(actionName, out System.Action callback);
                        //Debug.Log("Call " + actionName + " Up");
                        callback?.Invoke();
                    }
                    //Debug.Log("Test Call " + actionName + " Up");
                    break;
                case KeyActionType.Pressed:
                    if (Input.GetKey(key) && modifierActive)
                    {
                        actionNameToFunctionCall.TryGetValue(actionName, out System.Action callback);
                        //Debug.Log("Call " + actionName +" pressed");
                        callback?.Invoke();
                    }
                    //Debug.Log("Test Call " + actionName + " Pressed");
                    break;
            }
        }
    }
}
