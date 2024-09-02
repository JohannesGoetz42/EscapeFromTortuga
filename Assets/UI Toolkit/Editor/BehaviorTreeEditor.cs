using System;
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
    public BehaviorTree CurrentTree { get; private set; }
    public BehaviorTreeView TreeView { get; private set; }
    public BehaviorNodeDetails NodeDetails { get; private set; }
    public BehaviorTreeDetails TreeDetails { get; private set; }

    public IBehaviorTreeUser debugUser;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    public static void Open(BehaviorTree tree)
    {
        BehaviorTreeEditor editor = GetWindow<BehaviorTreeEditor>();
        editor.TreeView.editor = editor;
        editor.SetTree(tree);
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

        TreeDetails = root.Q<BehaviorTreeDetails>();
        TreeDetails.editor = this;
        TreeDetails.UpdateDetails();
    }

    internal void SetTree(BehaviorTree tree)
    {
        CurrentTree = tree;

        TreeView.OnBehaviorTreeChanged();
        TreeDetails.UpdateDetails();
    }

    internal void SelectNode(INodeView nodeView)
    {
        if (NodeDetails != null && nodeView != null)
        {
            NodeDetails.SetNode(nodeView);
        }
    }

    internal void SetBlackboard(Blackboard blackboard)
    {
        CurrentTree.Blackboard = blackboard;
        NodeDetails.UpdateContent();
    }

    private void OnSelectionChange()
    {
        BehaviorTree selectedTree = Selection.activeObject as BehaviorTree;
        if (selectedTree != null)
        {
            CurrentTree = selectedTree;
            TreeView.OnBehaviorTreeChanged();
            TreeDetails.UpdateDetails();
        }
    }

    private void OnInspectorUpdate()
    {
        if (!Application.isPlaying || TreeDetails == null)
        {
            return;
        }

        TreeDetails.UpdateUsers();
        if (TreeView != null)
        {
            TreeView.UpdateActiveNodes();
        }
    }
}
