using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[UxmlElement]
public partial class BehaviorTreeDetails : VisualElement
{
    public BehaviorTreeEditor editor;

    BehaviorTree behaviorTree;
    DropdownField _behaviorTreeSelection;
    DropdownField _blackboardSelection;

    List<BehaviorTree> _behaviorTreeAssets = new List<BehaviorTree>();
    List<Blackboard> _blackboardAssets = new List<Blackboard>();

    public BehaviorTreeDetails() : base()
    {
        VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/UI Toolkit/Editor/BehaviorTreeDetails.uxml") as VisualTreeAsset;
        visualTreeAsset.CloneTree(this);

        _behaviorTreeSelection = this.Q("behaviorTreeSelection") as DropdownField;
        _behaviorTreeSelection.RegisterValueChangedCallback(x => OnBehaviorTreeSelected());

        _blackboardSelection = this.Q("blackboardSelection") as DropdownField;
        _blackboardSelection.RegisterValueChangedCallback(x => OnBlackboardSelected());
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
        _blackboardSelection.SetValueWithoutNotify(editor.CurrentTree.blackboard.name);
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
