using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BlackboardKeySelector
{
    public BlackboardKeySelector(System.Type type) : this()
    {
        this.type = type;
    }

    public string selectedKey;
    public System.Type type;

    public string[] GetBlackboardKeys(Blackboard blackboard)
    {
        if (blackboard == null)
        {
            return new string[0];
        }

        if (type == null)
        {
            List<string> keys = new List<string>();
            keys.AddRange(blackboard.BoolKeys);
            keys.AddRange(blackboard.ObjectKeys);
            return keys.ToArray();
        }

        if (type == typeof(bool))
        {
            return blackboard.BoolKeys;
        }

        if (type == typeof(Object))
        {
            return blackboard.ObjectKeys;
        }

        return new string[0];
    }
}

[CreateAssetMenu(fileName = "Blackboard", menuName = "Scriptable Objects/Behavior/Blackboard")]
public class Blackboard : ScriptableObject
{
    public string[] BoolKeys { get => boolKeys; }
    public string[] ObjectKeys { get => objectKeys; }

    [SerializeField] string[] boolKeys;
    [SerializeField] string[] objectKeys;

    Dictionary<string, bool> _boolValues;
    Dictionary<string, Object> _objectValues;

    /** initializes the dictionaries with default values. Use this after CreateInstance to ensure everything is set up */
    public void InitializeValues()
    {
        _boolValues = new Dictionary<string, bool>(boolKeys.Length);
        foreach (string key in boolKeys)
        {
            _boolValues.Add(key, false);
        }

        _objectValues = new Dictionary<string, Object>(objectKeys.Length);
        foreach (string key in objectKeys)
        {
            _objectValues.Add(key, null);
        }
    }

    public void SetValueAsBool(string key, bool value)
    {
        if (_boolValues.ContainsKey(key))
        {
            _boolValues[key] = value;
        }
    }
    public bool GetValueAsBool(string key) => _boolValues.ContainsKey(key) ? _boolValues[key] : false;
    
    public void SetValueAsObject(string key, Object value)
    {
        if (_objectValues.ContainsKey(key))
        {
            _objectValues[key] = value;
        }
    }
    public bool GetValueAsObject(string key) => _objectValues.ContainsKey(key) ? _objectValues[key] : null;
}
