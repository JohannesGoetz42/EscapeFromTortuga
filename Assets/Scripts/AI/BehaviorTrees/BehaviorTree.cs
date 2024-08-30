using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System;
#endif

/** Abstraction for users of a behavior tree (most likely a monobehavior) */
public interface IBehaviorTreeUser
{
    public Blackboard GetBlackboard();
    public Transform GetBehaviorUser();
}

public class BehaviorTreeUpdater : MonoBehaviour
{
    public static void RegisterBehaviorTree(BehaviorTree behaviorTree)
    {
        // if there is no instance, initialize it!
        if (_instance == null)
        {
            GameObject gameObject = new GameObject("BehaviorTreeUpdater");
            gameObject.AddComponent<BehaviorTreeUpdater>();
        }

        _instance._activeBehaviors.Add(behaviorTree);
    }

    public static void UnregisterBehaviorTree(BehaviorTree behaviorTree)
    {
        if (_instance != null)
        {
            _instance._activeBehaviors.Remove(behaviorTree);
        }
    }

    private static BehaviorTreeUpdater _instance;
    private HashSet<BehaviorTree> _activeBehaviors = new HashSet<BehaviorTree>();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _instance = this;
    }

    private void Update()
    {
        foreach (BehaviorTree behaviorTree in _activeBehaviors)
        {
            behaviorTree.Update();
        }
    }
}

[CreateAssetMenu(fileName = "BehaviorTree", menuName = "Scriptable Objects/Behavior/BehaviorTree")]
public class BehaviorTree : ScriptableObject
{
    public BehaviorTreeRoot root;
    public Blackboard blackboard;

    /** The transforms currently using this behavior tree */
    private HashSet<IBehaviorTreeUser> _activeUsers = new HashSet<IBehaviorTreeUser>();
    public HashSet<IBehaviorTreeUser> ActiveUsers { get => _activeUsers; }

    public Blackboard RunBehavior(IBehaviorTreeUser user)
    {
        if (blackboard == null)
        {
            Debug.LogWarning(string.Format("Behavior {0} has no blackbboard assigned and can't be run!", name));
            return null;
        }

        _activeUsers.Add(user);
        Blackboard blackboardInstance = CreateInstance(blackboard.GetType()) as Blackboard;
        blackboardInstance.InitializeValues();

        BehaviorTreeUpdater.RegisterBehaviorTree(this);

        return blackboardInstance;
    }

    public void StopBehavior(IBehaviorTreeUser user)
    {
        _activeUsers.Remove(user);
        if(_activeUsers.Count == 0)
        {
            BehaviorTreeUpdater.UnregisterBehaviorTree(this);
        }
    }

    public void Update()
    {
        foreach(IBehaviorTreeUser user in _activeUsers)
        {
            root.Update(user);
        }
    }

#if UNITY_EDITOR
    public List<BehaviorTreeNode> nodes = new List<BehaviorTreeNode>();
    public List<EmbeddedBehaviorTreeNode> embeddedNodes = new List<EmbeddedBehaviorTreeNode>();
#endif

#if UNITY_EDITOR
    public BehaviorTreeNodeBase CreateNode(Type type)
    {
        BehaviorTreeNodeBase node = CreateInstance(type) as BehaviorTreeNodeBase;
        node.name = type.Name;
        node.id = GUID.Generate();
        node.behaviorTree = this;

        BehaviorTreeNode standaloneNode = node as BehaviorTreeNode;
        if (standaloneNode != null)
        {
            nodes.Add(standaloneNode);
        }

        EmbeddedBehaviorTreeNode embeddedNode = node as EmbeddedBehaviorTreeNode;
        if (embeddedNode != null)
        {
            embeddedNodes.Add(embeddedNode);
        }

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(BehaviorTreeNodeBase node)
    {
        // remove node as standalone node
        BehaviorTreeNode standaloneNode = node as BehaviorTreeNode;
        if (standaloneNode != null)
        {
            // remove services and decorators
            foreach (EmbeddedBehaviorTreeNode service in standaloneNode.services.ToArray())
            {
                DeleteNode(service);
            }
            foreach (EmbeddedBehaviorTreeNode decorator in standaloneNode.decorators.ToArray())
            {
                DeleteNode(decorator);
            }

            nodes.Remove(standaloneNode);
        }

        // remove decorator node
        EmbeddedBehaviorTreeNode embeddedNode = node as EmbeddedBehaviorTreeNode;
        if (embeddedNode != null)
        {
            // remove embedded node from parent
            BehaviorTreeServiceBase service = node as BehaviorTreeServiceBase;
            if (service != null)
            {
                embeddedNode.parent.services.Remove(service);
            }
            DecoratorBase decorator = node as DecoratorBase;
            if (decorator != null)
            {
                embeddedNode.parent.decorators.Remove(decorator);
            }

            embeddedNodes.Remove(embeddedNode);
        }

        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

#endif
}
