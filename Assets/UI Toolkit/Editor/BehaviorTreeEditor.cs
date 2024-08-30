using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        BehaviorTree tree = EditorUtility.InstanceIDToObject(instanceId) as BehaviorTree;
        if (tree != null)
        {
            BehaviorTreeEditor.Open(tree);
            return true;
        }

        return false;
    }
}

public class BehaviorTreeEditor : EditorWindow
{
    public BehaviorTreeView TreeView {  get; private set; }
    public BehaviorNodeDetails NodeDetails { get; private set; }

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    public static void Open(BehaviorTree tree)
    {
        BehaviorTreeEditor editor = GetWindow<BehaviorTreeEditor>();
        editor.TreeView.SetBehaviorTree(tree);
        editor.TreeView.editor = editor;
    }

    public void SelectNode(INodeView nodeView)
    {
        if (NodeDetails != null && nodeView != null)
        {
            NodeDetails.SetNode(nodeView);
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        m_VisualTreeAsset.CloneTree(rootVisualElement);


        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/Editor/BehaviorTreeEditor.uss");
        root.styleSheets.Add(style);

        TreeView = root.Q<BehaviorTreeView>();
        NodeDetails = root.Q<BehaviorNodeDetails>();
        NodeDetails.editor = this;
    }

    private void OnSelectionChange()
    {
        BehaviorTree selectedTree = Selection.activeObject as BehaviorTree;
        if(selectedTree)
        {
            TreeView.SetBehaviorTree(selectedTree);
        }
    }
}
