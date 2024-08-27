using UnityEditor;
using UnityEngine.UIElements;

public class EmbeddedBehaviorTreeNodeView : VisualElement
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

        _button = this.Q("button") as Button;

        if (node is DecoratorBase)
        {
            _button.AddToClassList("decorator");
        }
        else if (node is BehaviorTreeServiceBase)
        {
            _button.AddToClassList("service");
        }

        _button.text = embeddedNode.nodeName;
        _button.clicked += SelectNode;
        _parentView = parentView;
    }

    void SelectNode()
    {
        _parentView.BehaviorTreeView.editor.SelectNode(embeddedNode);
    }
}
