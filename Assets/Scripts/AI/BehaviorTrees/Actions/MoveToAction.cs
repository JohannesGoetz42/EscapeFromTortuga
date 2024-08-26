using UnityEngine;

public class MoveToAction : BehaviorTreeAction
{
    public MoveToAction() : base()
    {
#if UNITY_EDITOR
        nodeName = "Move to";
#endif
    }
}
