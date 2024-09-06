using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EmbeddedBehaviorTreeNodeView : VisualElement, INodeView
{
    public EmbeddedBehaviorTreeNode embeddedNode;

    private BehaviorTreeNodeView _parentView;
    private Button _button;
    private Label _nodeTypeLabel;
    private float _lastStateChange;

    public EmbeddedBehaviorTreeNodeView(EmbeddedBehaviorTreeNode node, BehaviorTreeNodeView parentView) : base()
    {
        embeddedNode = node;
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/EmbeddedBehaviorTreeNode.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(style);

        _parentView = parentView;

        _button = this.Q("button") as Button;

        _nodeTypeLabel = this.Q("nodeTypeLabel") as Label;
        if (_nodeTypeLabel != null)
        {
            _nodeTypeLabel.text = node.nodeTypeName;
        }

        if (_button != null)
        {
            _button.text = embeddedNode.nodeName;
            _button.clicked += SelectNode;
            if (node is DecoratorBase)
            {
                _button.AddToClassList("decorator");
            }
            else if (node is BehaviorTreeServiceBase)
            {
                _button.AddToClassList("service");
            }
        }

        node.evaluated += SetEvaluatedState;
    }

    private void SetEvaluatedState(BehaviorNodeState evaluationResult)
    {
        _lastStateChange = Time.time;
        if (evaluationResult == BehaviorNodeState.Failed)
        {
            _button.AddToClassList("failed");
        }
    }

    public void SelectNode()
    {
        _parentView.BehaviorTreeView.editor.SelectNode(this);
    }

    BehaviorTreeNodeBase INodeView.GetNode() => embeddedNode;

    void INodeView.Update()
    {
        if (_button != null && embeddedNode != null)
        {
            _button.text = embeddedNode.nodeName;
        }
    }

    internal void UpdateNodeState()
    {
        if (Time.time - _lastStateChange > 3.0f)
        {
            _button.RemoveFromClassList("failed");
            _lastStateChange = float.MaxValue;
        }
    }
}
