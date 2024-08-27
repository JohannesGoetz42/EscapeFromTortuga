using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


struct PropertyDetailData
{
    public VisualElement element;
    public SerializedProperty property;

}

[UxmlElement]
public partial class BehaviorNodeDetails : VisualElement
{
    static readonly string[] ignoredProperties = { "id", "m_Script", "parent" };

    private BehaviorTreeNodeBase _node;
    private VisualElement _propertyContainer;
    private TextField _nodeName;

    public BehaviorNodeDetails() : base()
    {
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/BehaviorNodeDetails.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        _nodeName = this.Q("nodeName") as TextField;
        _propertyContainer = this.Q("propertyContainer");
    }

    public void SetNode(BehaviorTreeNodeBase node)
    {
        if (node == null || node == _node)
        {
            return;
        }

        _node = node;

        // clear previous properties
        _propertyContainer.Clear();

        SerializedObject serializedObject = new SerializedObject(node);
        SerializedProperty property = serializedObject.GetIterator();
        bool bHasAdditionalProperty = property.NextVisible(true);
        while (bHasAdditionalProperty)
        {
            if (ignoredProperties.Contains(property.name))
            {
                bHasAdditionalProperty = property.NextVisible(true);
                continue;
            }

            // the node name is at the top of the detail window and will be handled seperately
            if (property.name == nameof(BehaviorTreeNodeBase.nodeName))
            {
                if (_nodeName != null)
                {
                    _nodeName.BindProperty(property);
                }
            }
            else
            {
                AddProperty(property);
            }

            bHasAdditionalProperty = property.NextVisible(true);
        }

    }

    void AddProperty(SerializedProperty property)
    {
        BindableElement propertyElement = null;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Boolean:
                Toggle toggle = new Toggle(property.name);
                propertyElement = toggle;
                break;
            case SerializedPropertyType.Float:
                FloatField floatField = new FloatField(property.name);
                floatField.value = property.floatValue;
                propertyElement = floatField;
                break;
            case SerializedPropertyType.Integer:
                IntegerField intField = new IntegerField(property.name);
                intField.value = property.intValue;
                propertyElement = intField;
                break;
            default:
                Debug.LogWarning(string.Format("decorator properties of type '{0}' are not supported: {1}!", property.propertyType.ToString(), property.name));
                return;
        }

        if (propertyElement != null)
        {
            propertyElement.BindProperty(property);
            _propertyContainer.Add(propertyElement);
        }
    }
}
