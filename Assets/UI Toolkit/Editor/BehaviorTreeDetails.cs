using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

[UxmlElement]
public partial class BehaviorTreeDetails : VisualElement
{
    public BehaviorTreeEditor editor;

    DropdownField _behaviorTreeSelection;
    DropdownField _blackboardSelection;
    DropdownField _debugInstanceSelection;

    List<BehaviorTree> _behaviorTreeAssets = new List<BehaviorTree>();
    List<Blackboard> _blackboardAssets = new List<Blackboard>();
    List<IBehaviorTreeUser> _behaviorUsers = new List<IBehaviorTreeUser>();

    public BehaviorTreeDetails() : base()
    {
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/BehaviorTreeDetails.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        _behaviorTreeSelection = this.Q("behaviorTreeSelection") as DropdownField;
        _behaviorTreeSelection.RegisterValueChangedCallback(x => OnBehaviorTreeSelected());

        _blackboardSelection = this.Q("blackboardSelection") as DropdownField;
        _blackboardSelection.RegisterValueChangedCallback(x => OnBlackboardSelected());

        _debugInstanceSelection = this.Q("debugInstanceSelection") as DropdownField;
        _debugInstanceSelection.RegisterValueChangedCallback(x => OnDebugUserSelected());
    }

    /** updates the view without changing the underlaying state */
    public void Update()
    {
        if (!editor || !editor.CurrentTree)
        {
            return;
        }

        // update behavior tree selection dropdown
        _behaviorTreeAssets.Clear();
        string[] behaviorTreeAssets = AssetDatabase.FindAssets("t:" + nameof(BehaviorTree));
        foreach (string behaviorTreeID in behaviorTreeAssets)
        {
            BehaviorTree behaviorTree = AssetDatabase.LoadAssetAtPath<BehaviorTree>(AssetDatabase.GUIDToAssetPath(behaviorTreeID));
            if (behaviorTree != null)
            {
                _behaviorTreeSelection.choices.Add(behaviorTree.name);
                _behaviorTreeAssets.Add(behaviorTree);
            }
        }
        _behaviorTreeSelection.SetValueWithoutNotify(editor.CurrentTree.name);

        // update blackboard selection dropdown
        _blackboardAssets.Clear();
        string[] blackboardAssets = AssetDatabase.FindAssets("t:" + nameof(Blackboard));
        foreach (string blackboardID in blackboardAssets)
        {
            Blackboard blackboard = AssetDatabase.LoadAssetAtPath<Blackboard>(AssetDatabase.GUIDToAssetPath(blackboardID));
            if (blackboard != null)
            {
                _blackboardSelection.choices.Add(blackboard.name);
                _blackboardAssets.Add(blackboard);
            }
        }

        _blackboardSelection.SetValueWithoutNotify(editor.CurrentTree.Blackboard.name);
    }

    internal void UpdateUsers()
    {
        // TODO: implement a cleaner solution to user updates
        // if the child count is the same, assume thi user count is the same
        if (editor.CurrentTree.ActiveUsers.Count == _debugInstanceSelection.childCount)
        {
            return;
        }

        _behaviorUsers = editor.CurrentTree.ActiveUsers.ToList();
        _debugInstanceSelection.choices.Clear();
        foreach (IBehaviorTreeUser user in editor.CurrentTree.ActiveUsers)
        {
            _debugInstanceSelection.choices.Add(user.GetBehaviorUser().name);
        }
    }

    void OnDebugUserSelected()
    {
        int selectedIndex = _debugInstanceSelection.index;
        if (_behaviorUsers.Count > selectedIndex)
        {
            editor.debugUser = _behaviorUsers[selectedIndex];
        }
    }

    void OnBehaviorTreeSelected()
    {
        int selectedIndex = _behaviorTreeSelection.index;
        if (_behaviorTreeAssets.Count > selectedIndex)
        {
            editor.SetTree(_behaviorTreeAssets[selectedIndex]);
        }
    }

    void OnBlackboardSelected()
    {
        int selectedIndex = _blackboardSelection.index;
        if (_blackboardAssets.Count > selectedIndex)
        {
            editor.SetBlackboard(_blackboardAssets[selectedIndex]);
        }
    }
}
