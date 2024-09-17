using Unity.VisualScripting;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    BoxCollider wallCollider;

    protected void OnDrawGizmos()
    {
        if (wallCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);
            Gizmos.DrawSphere(transform.position + Vector3.up * wallCollider.size.y * transform.localScale.y, 0.5f);

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(wallCollider.center, wallCollider.size);
        }
    }
#endif
}
