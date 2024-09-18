using UnityEngine;
using System;
using UnityEngine.Profiling;
using System.Linq;

public enum ViewConeMode
{
    Idle,
    Searching,
    Hidden // while the player is seen, hide the view cone
}

[Serializable]
struct ViewConeModeData
{
    public ViewConeMode coneMode;
    public Material coneMaterial;
}

public class ViewCone : MonoBehaviour
{
    [SerializeField]
    private int visualizationSubdivisions = 10;
    [SerializeField]
    private float visualizationGradiendEndPos = 0.9f;
    [SerializeField]
    private ViewConeModeData[] viewConeModeData;
    [SerializeField]
    private MeshFilter viewConeFilter;
    [SerializeField]
    private MeshRenderer viewConeRenderer;

    public float viewAngle { get; set; }
    public float viewRange { get; set; }

    private Mesh viewConeVisualization;
    private NPCController npcController;
    private PlayerController playerController;

    public void SetViewConeMode(ViewConeMode newMode)
    {
        if (viewConeRenderer == null)
        {
            return;
        }

        if (newMode == ViewConeMode.Hidden)
        {
            viewConeRenderer.enabled = false;
            return;
        }

        viewConeRenderer.enabled = true;
        ViewConeModeData modeData = viewConeModeData.FirstOrDefault(x => x.coneMode == newMode);
        if (modeData.coneMaterial != null)
        {
            if (viewConeRenderer != null)
            {
                viewConeRenderer.material = modeData.coneMaterial;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (viewConeFilter != null)
        {
            viewConeVisualization = new Mesh();
            viewConeVisualization.name = "View cone visualization";

            viewConeFilter.mesh = viewConeVisualization;
        }
        if (viewConeFilter == null || viewConeVisualization == null)
        {
            SetViewConeMode(ViewConeMode.Hidden);
            enabled = false;
        }
        else
        {
            SetViewConeMode(ViewConeMode.Idle);
            UpdateVisualization();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateVisualization();
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
        if (Physics.Raycast(transform.position, rayDirection, out hit, viewRange, Constants.npcViewLayer) && !hit.collider.gameObject.CompareTag("Player"))
        {
            refConePosition = Quaternion.Inverse(transform.rotation) * (hit.point - transform.position);
        }
        else
        {
            refConePosition = sourceLocation + stepDirection;
        }
    }
}

