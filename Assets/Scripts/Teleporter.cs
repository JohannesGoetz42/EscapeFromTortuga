using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    SphereCollider triggerSphere;
    [SerializeField]
    /** The maximum facing angle at which the player is teleported */
    float maxTeleportFacingAngle = 45.0f;

    Vector2 _facingDirection;
    List<GameObject> _objectsInRange = new List<GameObject>();

    private void Awake()
    {
        if (triggerSphere != null)
        {
            Vector2 center2D = new Vector2(triggerSphere.center.x, triggerSphere.center.z);
            _facingDirection = (transform.rotation * -center2D).normalized;
        }

        enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _objectsInRange.Contains(other.gameObject))
        {
            return;
        }

        if (TryTeleport(other.gameObject))
        {
            return;
        }

        _objectsInRange.Add(other.gameObject);
        enabled = true;
    }

    private void Update()
    {
        if (_objectsInRange.Count < 1)
        {
            enabled = false;
            return;
        }

        foreach (GameObject obj in _objectsInRange)
        {
            if (TryTeleport(obj))
            {
                _objectsInRange.Remove(gameObject.gameObject);
            }
        }
    }

    bool TryTeleport(GameObject gameObject)
    {
        Vector2 playerForward = new Vector2(gameObject.transform.forward.x, gameObject.transform.forward.z);
        if (playerForward != _facingDirection)
        {
            // to avoid NaN, return early if facing in opposite direction
            if (playerForward == -_facingDirection)
            {
                return false;
            }

            float angle = Mathf.Acos(Vector2.Dot(_facingDirection, playerForward)) * Mathf.Rad2Deg;
            if (angle > maxTeleportFacingAngle)
            {
                return false;
            }
        }

        gameObject.gameObject.transform.position = transform.position;
        return true;
    }

    private void OnTriggerExit(Collider other)
    {
        _objectsInRange.Remove(other.gameObject);
        if (_objectsInRange.Count < 1)
        {
            enabled = false;
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawLine(transform.position + triggerSphere.center, transform.position);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + triggerSphere.center, triggerSphere.radius);
    }
}
