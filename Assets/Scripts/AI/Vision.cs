using UnityEngine;

public class Vision : MonoBehaviour
{
    public float ViewAngle;
    public float ViewRange;

    private NPCController npcController;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcController = GetComponent<NPCController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (npcController != null)
        {
            npcController.canSeePlayer = IsVisible(PlayerController.Instance.transform);
        }
    }

    bool IsVisible(Transform target)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 direction = target.position - transform.position;
        // target is out of view range
        if (direction.sqrMagnitude > System.Math.Pow(ViewRange, 2))
        {
            return false;
        }

        float dotProduct = Vector3.Dot(transform.forward, direction.normalized);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return ViewAngle > angle;
    }
}
