using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EmbeddedBehaviorTreeNode : BehaviorTreeNodeBase
{
#if UNITY_EDITOR
    /** Allows UI elements to react to evaluation results */
    public delegate void EvaluationEvent(BehaviorNodeState evaluationResult);
    public EvaluationEvent evaluated;
    public virtual string[] GetRefreshProperties() => new string[0];
    public virtual void OnPropertyChanged(SerializedProperty property) { }
#endif

}
