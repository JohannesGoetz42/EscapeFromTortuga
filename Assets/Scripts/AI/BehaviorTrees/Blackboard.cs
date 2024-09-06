using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum BlackboardValueType
{
    Any,
    Bool,
    Float,
    Vector3,
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

            case BlackboardValueType.Float:
                return blackboard.FloatKeys;

            case BlackboardValueType.Object:
                return blackboard.ObjectKeys;

            case BlackboardValueType.Vector3:
                return blackboard.Vector3Keys;

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
    [field: SerializeField] public string[] BoolKeys { get; private set; }
    [field:SerializeField] public string[] FloatKeys { get; private set; }
    [field: SerializeField] public string[] ObjectKeys { get; private set; }
    [field: SerializeField] public string[] Vector3Keys { get; private set; }

    Dictionary<string, bool> _boolValues;
    Dictionary<string, float> _floatValues;
    Dictionary<string, Object> _objectValues;
    Dictionary<string, Vector3> _vector3Values;

    /** initializes the dictionaries with default values. Use this after CreateInstance to ensure everything is set up */
    public void InitializeValues()
    {
        _boolValues = new Dictionary<string, bool>(BoolKeys.Length);
        foreach (string key in BoolKeys)
        {
            _boolValues.Add(key, false);
        }

        _floatValues = new Dictionary<string, float>(FloatKeys.Length);
        foreach (string key in FloatKeys)
        {
            _floatValues.Add(key, 0.0f);
        }

        _objectValues = new Dictionary<string, Object>(ObjectKeys.Length);
        foreach (string key in ObjectKeys)
        {
            _objectValues.Add(key, null);
        }

        _vector3Values = new Dictionary<string, Vector3>(Vector3Keys.Length);
        foreach (string key in Vector3Keys)
        {
            _vector3Values.Add(key, Vector3.zero);
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

    public void SetValueAsFloat(string key, float value)
    {
        if (_floatValues.ContainsKey(key))
        {
            _floatValues[key] = value;
        }
    }
    public float GetValueAsFloat(string key) => _floatValues.ContainsKey(key) ? _floatValues[key] : 0.0f;

    public void SetValueAsObject(string key, Object value)
    {
        if (_objectValues.ContainsKey(key))
        {
            _objectValues[key] = value;
        }
    }
    public Object GetValueAsObject(string key) => _objectValues.ContainsKey(key) ? _objectValues[key] : null;

    public void SetValueAsVector3(string key, Vector3 value)
    {
        if (_vector3Values.ContainsKey(key))
        {
            _vector3Values[key] = value;
        }
    }
    public Vector3 GetValueAsVector3(string key) => _vector3Values.ContainsKey(key) ? _vector3Values[key] : Vector3.zero;
}
