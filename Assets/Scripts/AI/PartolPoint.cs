using UnityEngine;

public class PartolPoint : MonoBehaviour, IHasWaitTime
{

    [SerializeField]
    float minWaitDuration = 0.0f;
    [SerializeField]
    float maxWaitDuration = 0.0f;

    public float MinWaitDuration => minWaitDuration;
    public float MaxWaitDuration => maxWaitDuration;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
#endif
}
