using System.Linq;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[UxmlElement]
public partial class BlackboardKeySelectorField : VisualElement
{
    Blackboard blackboard;
    SerializedObject ownerNode;
    BlackboardKeySelector selector;
    PropertyInfo propertyInfo;

    private Label label;
    private DropdownField typeSelector;
    private DropdownField keySelector;

    public BlackboardKeySelectorField() : base()
    {
        label = new Label("");
        Add(label);
        typeSelector = new DropdownField("Value type");
        Add(typeSelector);
        keySelector = new DropdownField("Blackboard key");
        Add(keySelector);

        typeSelector.RegisterValueChangedCallback(OnSelectedTypeChanged);
        keySelector.RegisterValueChangedCallback(OnSelectedKeyChanged);
    }

    public void SetProperty(SerializedProperty property, Blackboard newBlackboard)
    {
        label.text = property.displayName;

        ownerNode = property.serializedObject;
        blackboard = newBlackboard;

        // ensure the name does not contain any pre- / suffixes
        propertyInfo = ownerNode.targetObject.GetType().GetProperties().Where(x => x.PropertyType == typeof(BlackboardKeySelector))
            .FirstOrDefault(x => property.name .Contains(x.Name.TrimStart(nameof(BlackboardKeySelector) + " ")));

        if (propertyInfo == null)
        {
            Debug.LogError(string.Format("The nodetype '{0}' has no property for the BlackboarKeySelector '{1}'.\n" +
                "Ensure BlackboardKeySelectors are declared with auto properties.", ownerNode.targetObject.GetType().ToString(), name));
            return;
        }

        selector = (BlackboardKeySelector)propertyInfo.GetValue(ownerNode.targetObject);

        InitializeValueTypeSelection();
        InitializeKeySelection();
    }

    private void InitializeValueTypeSelection()
    {
        typeSelector.choices.Clear();

        // initialize value type selection
        foreach (int i in Enum.GetValues(typeof(BlackboardValueType)))
        {
            BlackboardValueType currentType = (BlackboardValueType)i;
            if (selector.IsSupportedValueType(currentType))
            {
                typeSelector.choices.Add(currentType.ToString());
            }
        }

        if (typeSelector.choices.Count > 0)
        {
            // if current type is valid, set it in the dropdown
            if (selector.IsSupportedValueType(selector.type))
            {
                typeSelector.SetValueWithoutNotify(selector.type.ToString());
            }
            // otherwise set the selector type to the first valid type
            else
            {
                string initialValue = typeSelector.choices[0].ToString();
                typeSelector.SetValueWithoutNotify(initialValue);
                selector.type = (BlackboardValueType)Enum.Parse(typeof(BlackboardValueType), initialValue);
                propertyInfo.SetValue(ownerNode.targetObject, selector);
            }
        }
    }

    private void InitializeKeySelection()
    {
        keySelector.choices.Clear();

        string[] availableKeys = selector.GetBlackboardKeys(blackboard);
        foreach (string availableKey in availableKeys)
        {
            keySelector.choices.Add(availableKey);
        }

        // set the selected key if it is available for the type
        if (availableKeys.Contains(selector.selectedKey))
        {
            keySelector.SetValueWithoutNotify(selector.selectedKey);
        }
        // set the first available key to the selector
        else
        {
            string initialValue = availableKeys.Length == 0 ? "" : availableKeys[0];
            keySelector.SetValueWithoutNotify(initialValue);
            selector.selectedKey = initialValue;
            propertyInfo.SetValue(ownerNode.targetObject, selector);
        }
    }

    private void OnSelectedTypeChanged(ChangeEvent<string> evt)
    {
        selector.type = (BlackboardValueType)Enum.Parse(typeof(BlackboardValueType), evt.newValue);
        propertyInfo.SetValue(ownerNode.targetObject, selector);
        InitializeKeySelection();
    }

    private void OnSelectedKeyChanged(ChangeEvent<string> evt)
    {
        selector.selectedKey = evt.newValue;
        propertyInfo.SetValue(ownerNode.targetObject, selector);
    }
}

