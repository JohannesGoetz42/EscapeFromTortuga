using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class BehaviorDecoratorDetails : VisualElement
{
    private DecoratorBase _decorator;
    private VisualElement propertyContainer;

    void SetDecorator(DecoratorBase decorator)
    {
        _decorator = decorator;
    }

    void UpdateProperties()
    {
        // clear previous properties
        foreach (VisualElement property in propertyContainer.Children())
        {
            Remove(property);
        }

        using (var serializedObject = new SerializedObject(_decorator))
        {
            SerializedProperty property = serializedObject.GetIterator();
            bool bHasAdditionalProperty = property != null;
            while (bHasAdditionalProperty)
            {
                AddProperty(property);
                property.Next(true);
            }
        }
    }

    void AddProperty(SerializedProperty property)
    {
        VisualElement propertyElement = null;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Boolean:
                Toggle toggle = new Toggle();
                propertyElement = toggle;
                break;
            default:
                Debug.LogError(string.Format("decorator properties of type '{0}' are not supported!", property.propertyType.ToString()));
                return;
        }

        if (propertyElement != null)
        {
            propertyContainer.Add(propertyElement);
        }
    }
}
