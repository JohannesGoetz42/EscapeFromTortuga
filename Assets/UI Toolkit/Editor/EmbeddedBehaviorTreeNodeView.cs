using UnityEditor;
using UnityEngine.UIElements;

public class EmbeddedBehaviorTreeNodeView : VisualElement
{
    public EmbeddedBehaviorTreeNode embeddedNode;

    private BehaviorTreeNodeView _parentView;
    private TextElement _nodeNameText;
    private VisualElement _background;

    public EmbeddedBehaviorTreeNodeView(EmbeddedBehaviorTreeNode node, BehaviorTreeNodeView parentView) : base()
    {
        embeddedNode = node;
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/EmbeddedBehaviorTreeNode.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(style);

        if (node is DecoratorBase)
        {
            AddToClassList("decorator");
        }
        else if (node is BehaviorTreeServiceBase)
        {
            AddToClassList("service");
        }

        _parentView = parentView;
        _background = this.Q("background");
        _nodeNameText = this.Q("nodeNameText") as TextElement;
    }

    void SelectNode()
    {
        _parentView.BehaviorTreeView.editor.SelectNode(embeddedNode);
    }
}
