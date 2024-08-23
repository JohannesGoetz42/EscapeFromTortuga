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
    BehaviorTreeView treeView;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    public static void Open(BehaviorTree tree)
    {
        BehaviorTreeEditor editor = GetWindow<BehaviorTreeEditor>();
        editor.treeView.SetBehaviorTree(tree);
    }

    [MenuItem("Window/AI/BehaviorTreeEditor")]
    public static void ShowExample()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        m_VisualTreeAsset.CloneTree(rootVisualElement);


        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI Toolkit/Editor/BehaviorTreeEditor.uss");
        root.styleSheets.Add(style);

        treeView = root.Q<BehaviorTreeView>();
    }

    private void OnSelectionChange()
    {
        BehaviorTree selectedTree = Selection.activeObject as BehaviorTree;
        if(selectedTree)
        {
            treeView.SetBehaviorTree(selectedTree);
        }
    }
}
