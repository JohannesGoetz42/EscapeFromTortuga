using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public enum ViewConeMode
{
    Idle,
    Searching,
    Hidden // while the player is seen, hide the view cone
}

[System.Serializable]
struct ViewConeModeData
{
    public ViewConeMode coneMode;
    public Material coneMaterial;
}

public class Vision : MonoBehaviour
{
    /** 
    * The view angle to each side of the character.
    * viewAngle 90deg results in a 180deg field of view
    */
    public float viewAngle = 90.0f;
    public float viewRange = 10.0f;
    public bool canSeePlayer { get; private set; }
    public float secondsSinceLastSeen { get; private set; }
    public Vector3 lastSeenPosition { get; private set; }


    [SerializeField]
    private int visualizationSubdivisions = 10;
    [SerializeField]
    private float visualizationGradiendEndPos = 0.9f;
    [SerializeField]
    private ViewConeModeData[] viewConeModeData;
    [SerializeField]
    private MeshFilter viewConeFilter;

    private Mesh viewConeVisualization;
    private NPCController npcController;
    private PlayerController playerController;

    public void SetViewConeMode(ViewConeMode newMode)
    {
        MeshRenderer renderer = viewConeFilter.gameObject.GetComponent<MeshRenderer>();
        if (newMode == ViewConeMode.Hidden)
        {
            renderer.enabled = false;
            return;
        }

        renderer.enabled = true;
        ViewConeModeData modeData = viewConeModeData.FirstOrDefault(x => x.coneMode == newMode);
        if (modeData.coneMaterial != null)
        {
            if (renderer != null)
            {
                renderer.material = modeData.coneMaterial;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondsSinceLastSeen = float.MaxValue;

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
    void LateUpdate()
    {
        if (PlayerController.Instance != null)
        {
            if (IsVisible(PlayerController.Instance.transform))
            {
                canSeePlayer = true;
                secondsSinceLastSeen = 0.0f;
                lastSeenPosition = PlayerController.Instance.transform.position;
            }
            else
            {
                canSeePlayer = false;
                secondsSinceLastSeen += Time.deltaTime;
            }
        }

        UpdateVisualization();
    }

    bool IsVisible(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        // target is out of view range
        if (direction.sqrMagnitude > System.Math.Pow(viewRange, 2))
        {
            return false;
        }

        float dotProduct = Vector3.Dot(transform.forward, direction.normalized);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
        if (angle > viewAngle)
        {
            return false;
        }

        RaycastHit hit;
        Vector3 rayDirection = target.position - transform.position;
        if (Physics.Raycast(transform.position, rayDirection, out hit, viewRange))
        {
            return hit.collider.gameObject.CompareTag("Player");
        }

        return false;
    }

    void UpdateVisualization()
    {
        Profiler.BeginSample("Update vision cone");

        // initialize polygon data arrays
        int requiredVectorCount = visualizationSubdivisions + 2;
        Vector3[] vertices = new Vector3[requiredVectorCount];
        Vector2[] uvs = new Vector2[requiredVectorCount];
        int[] triangles = new int[visualizationSubdivisions * 3];

        Vector3 stepDirection = Vector3.forward * viewRange;
        stepDirection = Quaternion.AngleAxis(-viewAngle, Vector3.up) * stepDirection;
        Quaternion stepRotation = Quaternion.AngleAxis((viewAngle * 2) / visualizationSubdivisions, Vector3.up);

        // since three vertices are required for a triangle, draw the first two vertices outside the loop
        vertices[0] = new Vector3(0.0f, 0.1f, 0.0f);
        GetConePositionWithCollision(stepDirection, vertices[0], ref vertices[1]);

        uvs[0] = Vector2.zero;
        uvs[1] = new Vector2(visualizationGradiendEndPos, visualizationGradiendEndPos);

        // create all triangles
        for (int i = 2; i < requiredVectorCount; i++)
        {
            stepDirection = stepRotation * stepDirection;

            GetConePositionWithCollision(stepDirection, vertices[0], ref vertices[i]);

            uvs[i] = new Vector2(visualizationGradiendEndPos, visualizationGradiendEndPos);

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

    void GetConePositionWithCollision(in Vector3 stepDirection, in Vector3 sourceLocation, ref Vector3 refConePosition)
    {
        RaycastHit hit;
        Vector3 rayDirection = transform.rotation * stepDirection;
        if (Physics.Raycast(transform.position, rayDirection, out hit, viewRange) && !hit.collider.gameObject.CompareTag("Player"))
        {
            refConePosition = Quaternion.Inverse(transform.rotation) * (hit.point - transform.position);
        }
        else
        {
            refConePosition = sourceLocation + stepDirection;
        }
    }
}
