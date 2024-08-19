using UnityEngine;
using UnityEngine.Profiling;

public class Vision : MonoBehaviour
{
    public float viewAngle = 90.0f;
    public float viewRange = 10.0f;

    [SerializeField]
    int visualizationSubdivisions = 10;

    [SerializeField]
    private MeshFilter viewConeFilter;
    private Mesh viewConeVisualization;
    private NPCController npcController;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcController = GetComponent<NPCController>();
        if (viewConeFilter != null)
        {
            viewConeVisualization = new Mesh();
            viewConeVisualization.name = "View cone visualization";

            viewConeFilter.mesh = viewConeVisualization;
            UpdateVisualization();
        }
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
        if (direction.sqrMagnitude > System.Math.Pow(viewRange, 2))
        {
            return false;
        }

        float dotProduct = Vector3.Dot(transform.forward, direction.normalized);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return viewAngle > angle;
    }

    void UpdateVisualization()
    {
        Profiler.BeginSample("Update vision cone");
        int requiredVectorCount = visualizationSubdivisions + 2;
        Vector3[] vertices = new Vector3[requiredVectorCount];
        Vector2[] uvs = new Vector2[requiredVectorCount];

        int[] triangles = new int[visualizationSubdivisions * 3];

        Vector3 stepDirection = Vector3.forward * viewRange;
        stepDirection = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up) * stepDirection;
        Quaternion stepRotation = Quaternion.AngleAxis(viewAngle / visualizationSubdivisions, Vector3.up);

        // add the first two vertices at the source position
        vertices[0] = new Vector3(0.0f, 0.1f, 0.0f);
        vertices[1] = vertices[0] + stepDirection;
        uvs[0] = Vector2.zero;
        uvs[1] = Vector2.one;

        // create all triangles
        for (int i = 2; i < requiredVectorCount; i++)
        {
            stepDirection = stepRotation * stepDirection;

            vertices[i] = vertices[0] + stepDirection;
            uvs[i] = Vector2.one;

            int trianglesStartIndex = (i - 2) * 3;
            triangles[trianglesStartIndex] = 0;
            triangles[trianglesStartIndex + 1] = i - 1;
            triangles[trianglesStartIndex + 2] = i;
        }

        viewConeVisualization.vertices = vertices;
        viewConeVisualization.triangles = triangles; // triangles must be set AFTER vertices!
        viewConeVisualization.uv = uvs;
        Profiler.EndSample();
    }
}
