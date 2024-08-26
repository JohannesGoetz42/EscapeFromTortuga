using UnityEditor;
using UnityEngine.UIElements;

public class EmbeddedBehaviorTreeNodeView : VisualElement
{
    public EmbeddedBehaviorTreeNode embeddedNode;
    private VisualElement _titleContainer;
    private TextElement _title;

    public EmbeddedBehaviorTreeNodeView(EmbeddedBehaviorTreeNode node) : base()
    {
        embeddedNode = node;
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/EmbeddedBehaviorTreeNode.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/Editor/BehaviorTreeEditor.uss");
        styleSheets.Add(style);

        _titleContainer = this.Q("titleContainer");
        _title = this.Q("title") as TextElement;

        if (node is DecoratorBase)
        {
            _titleContainer.AddToClassList("decorator");
        }
        else if (node is BehaviorTreeServiceBase)
        {
            _titleContainer.AddToClassList("service");
        }

        _title.text = embeddedNode.nodeName;
    }
}
