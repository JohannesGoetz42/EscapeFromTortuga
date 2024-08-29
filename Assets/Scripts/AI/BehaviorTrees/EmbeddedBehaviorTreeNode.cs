using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EmbeddedBehaviorTreeNode : BehaviorTreeNodeBase
{
#if UNITY_EDITOR
    public virtual string[] GetRefreshProperties() => new string[0];
    public virtual void OnPropertyChanged(SerializedProperty property) { }
#endif

}
