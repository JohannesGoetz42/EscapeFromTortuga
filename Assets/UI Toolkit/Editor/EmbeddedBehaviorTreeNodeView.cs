using UnityEditor;
using UnityEngine.UIElements;

public class EmbeddedBehaviorTreeNodeView : VisualElement, INodeView
{
    public EmbeddedBehaviorTreeNode embeddedNode;

    private BehaviorTreeNodeView _parentView;
    private Button _button;

    public EmbeddedBehaviorTreeNodeView(EmbeddedBehaviorTreeNode node, BehaviorTreeNodeView parentView) : base()
    {
        embeddedNode = node;
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/EmbeddedBehaviorTreeNode.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(style);

        _parentView = parentView;

        _button = this.Q("button") as Button;
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
}
