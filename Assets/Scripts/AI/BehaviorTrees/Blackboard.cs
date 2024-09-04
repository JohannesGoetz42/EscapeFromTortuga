using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum BlackboardValueType
{
    Any,
    Bool,
    Object
}

[System.Serializable]
/**
 * Blackboard key selector for behavior tree nodes
 * IMPORTANT: to enable the editor to set the property correctly (using reflection), nodes need to add an auto property for BlackboardKeySelector variables
 */
public struct BlackboardKeySelector
{
    public BlackboardKeySelector(BlackboardValueType type, BlackboardValueType[] supportedTypes) : this()
    {
        this.type = type;

#if UNITY_EDITOR
        this.supportedTypes = supportedTypes;
#endif
    }

    public string selectedKey;
    public BlackboardValueType type;

#if UNITY_EDITOR
    /** 
     * the supported value types for this selector 
     * set this in your node's constructor to limit user selection to supported types in your node
     * i.e. to avoid selecting a blackboard key for a bool value when your node expects an object
     */
    public readonly BlackboardValueType[] supportedTypes;

    public bool IsSupportedValueType(BlackboardValueType keyType)
    {
        return supportedTypes == null || supportedTypes.Contains(BlackboardValueType.Any) || supportedTypes.Contains(keyType);
    }
#endif

    public string[] GetBlackboardKeys(Blackboard blackboard)
    {
        if (blackboard == null)
        {
            return new string[0];
        }

        switch (type)
        {
            case BlackboardValueType.Any:
                List<string> keys = new List<string>();
                keys.AddRange(blackboard.BoolKeys);
                keys.AddRange(blackboard.ObjectKeys);
                return keys.ToArray();

            case BlackboardValueType.Bool:
                return blackboard.BoolKeys;

            case BlackboardValueType.Object:
                return blackboard.ObjectKeys;

            default:
                Debug.LogError(string.Format("BlackboardValueType {0} is not implemented!", type));
                break;

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
    public Object GetValueAsObject(string key) => _objectValues.ContainsKey(key) ? _objectValues[key] : null;
}
