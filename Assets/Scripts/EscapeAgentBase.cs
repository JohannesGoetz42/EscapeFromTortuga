using Unity.VisualScripting;
using UnityEngine;

public enum EscapeAgentState
{
    Initial,
    WaitingForTaskCompletion,
    WaitingForTaskReturn,
    ReadyToDepart
}

public abstract class EscapeAgentBase : RunBehaviorTree, IHasThumbnail
{
    public const string EscapeAgentStateKey = "EscapeAgentState";
    public const string EscapeAreaKey = "EscapeArea";

    [SerializeField]
    protected Sprite thumbnail;
    [SerializeField]
    protected float interactionRange = 4.0f;
    [SerializeField]
    protected DialogueText dialogueText;
    [SerializeField, TextArea(10, 100)]
    protected string readyToDepartText;
    [SerializeField]
    public Color markerColor = Color.white;
    public Color BackgroundColor => markerColor;

    SphereCollider _trigger;
    public Sprite Thumbnail => thumbnail;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        _trigger = transform.AddComponent<SphereCollider>();
        _trigger.radius = interactionRange;
        _trigger.isTrigger = true;
    }
}
