using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Unity.VisualScripting;


struct PropertyDetailData
{
    public VisualElement element;
    public SerializedProperty property;
}

[UxmlElement]
public partial class BehaviorNodeDetails : VisualElement
{
    static readonly string[] ignoredProperties = {
        "m_Script",
        nameof(BehaviorTreeNode.id),
        nameof(BehaviorTreeNode.parent),
        nameof(BehaviorTreeNode.behaviorTree),
        nameof(BehaviorTreeNode.position),
        nameof(BehaviorTreeNode.decorators),
        nameof(BehaviorTreeNode.services),
        nameof(BehaviorTreeNode.nodeTypeName),
        nameof(BehaviorTreeCompositeNode.children)
    };

    public BehaviorTreeEditor editor;

    private INodeView _nodeView;
    private VisualElement _propertyContainer;
    private TextField _nodeName;
    private string[] refreshProperties = new string[0];

    public BehaviorNodeDetails() : base()
    {
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/BehaviorNodeDetails.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        _nodeName = this.Q("nodeName") as TextField;
        _propertyContainer = this.Q("propertyContainer");
    }

    public void UpdateContent()
    {
        // clear previous properties
        _propertyContainer.Clear();
        _nodeName.Unbind();

        SerializedObject serializedObject = new SerializedObject(_nodeView.GetNode());
        SerializedProperty property = serializedObject.GetIterator();
        bool bHasAdditionalProperty = property.NextVisible(true);
        while (bHasAdditionalProperty)
        {
            if (ignoredProperties.Contains(property.name))
            {
                bHasAdditionalProperty = property.NextVisible(false);
                continue;
            }

            // the node name is at the top of the detail window and will be handled seperately
            if (property.name == nameof(BehaviorTreeNodeBase.nodeName))
            {
                if (_nodeName != null)
                {
                    _nodeName.TrackPropertyValue(property, x => _nodeView.Update());
                    _nodeName.BindProperty(property);
                }
            }
            else
            {
                AddProperty(serializedObject, property);
            }

            bHasAdditionalProperty = property.NextVisible(false);
        }
    }

    public void SetNode(INodeView nodeView)
    {
        if (nodeView == null || nodeView.GetNode() == null || nodeView == _nodeView)
        {
            return;
        }

        EmbeddedBehaviorTreeNode embeddedNode = nodeView.GetNode() as EmbeddedBehaviorTreeNode;
        if (embeddedNode == null)
        {
            refreshProperties = new string[0];
        }
        else
        {
            refreshProperties = embeddedNode.GetRefreshProperties();
        }

        _nodeView = nodeView;
        UpdateContent();
    }

    void AddProperty(SerializedObject serializedObject, SerializedProperty property)
    {
        BindableElement propertyElement = null;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Boolean:
                Toggle toggle = new Toggle(property.displayName);
                propertyElement = toggle;
                break;
            case SerializedPropertyType.Float:
                FloatField floatField = new FloatField(property.displayName);
                floatField.value = property.floatValue;
                propertyElement = floatField;
                break;
            case SerializedPropertyType.Integer:
                IntegerField intField = new IntegerField(property.displayName);
                intField.value = property.intValue;
                propertyElement = intField;
                break;
            case SerializedPropertyType.Enum:
                EnumField enumField = new EnumField(property.displayName);
                propertyElement = enumField;
                break;
            case SerializedPropertyType.LayerMask:
                LayerMaskField layerMaskField = new LayerMaskField(property.displayName);
                propertyElement = layerMaskField;
                break;
            case SerializedPropertyType.Generic:
                if (!TryAddGenericProperty(serializedObject, property))
                {
                    Debug.LogWarning(string.Format("decorator properties of type '{0}' are not supported: {1}!", property.propertyType.ToString(), property.name));
                }
                return;
            default:
                Debug.LogWarning(string.Format("decorator properties of type '{0}' are not supported: {1}!", property.propertyType.ToString(), property.name));
                return;
        }

        if (propertyElement != null)
        {
            if (refreshProperties.Contains(property.name))
            {
                propertyElement.TrackPropertyValue(property, x => OnPropertyUpdated(x));
            }

            propertyElement.BindProperty(property);
            _propertyContainer.Add(propertyElement);
        }
    }

    void OnPropertyUpdated(SerializedProperty property)
    {
        EmbeddedBehaviorTreeNode embeddedNode = _nodeView as EmbeddedBehaviorTreeNode;
        if (embeddedNode != null)
        {
            embeddedNode.OnPropertyChanged(property);
            UpdateContent();
        }
    }

    bool TryAddGenericProperty(SerializedObject serializedObject, SerializedProperty property)
    {
        if (property.type == nameof(BlackboardKeySelector))
        {
            BlackboardKeySelectorField propertyDetails = new BlackboardKeySelectorField();
            propertyDetails.SetProperty(property, _nodeView.GetNode().behaviorTree.Blackboard);
            _propertyContainer.Add(propertyDetails);
            return true;

        }

        return false;
    }
}
