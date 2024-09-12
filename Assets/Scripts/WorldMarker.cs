using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WorldMarkerVisibility
{
    Hidden,
    Overhead,
    OverheadAndScreenBorder
}

public class WorldMarker : MonoBehaviour
{
    [SerializeField]
    MeshRenderer markerMesh;
    [SerializeField]
    RectTransform screenBorderMarker;

    Dictionary<Object, WorldMarkerVisibility> _visibilitySources = new Dictionary<Object, WorldMarkerVisibility>();

    public void AddMarkerSource(Object sourceObject, WorldMarkerVisibility visibility)
    {
        _visibilitySources.Add(sourceObject, visibility);
        markerMesh.enabled = visibility != WorldMarkerVisibility.Hidden;

        enabled = screenBorderMarker != null;
        if (visibility == WorldMarkerVisibility.OverheadAndScreenBorder)
        {
            enabled = screenBorderMarker != null;
        }
    }

    public void RemoveMarkerSource(Object sourceObject)
    {
        _visibilitySources.Remove(sourceObject);
        if (_visibilitySources.Count == 0)
        {
            markerMesh.enabled = false;
        }

        // update screen border marker visibility
        enabled = false;
        foreach (var source in _visibilitySources)
        {
            if (source.Value == WorldMarkerVisibility.OverheadAndScreenBorder)
            {
                enabled = screenBorderMarker != null;
                break;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        markerMesh.enabled = _visibilitySources.Count > 0;
        enabled = false;

        if (screenBorderMarker == null)
        {
            Debug.LogWarningFormat("WorldMarker {0} has no screen border marker assigned!", name);
        }
    }

    private void LateUpdate()
    {
        ProfilingUtility.BeginSample("WorldMarkerUpdate");

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(PlayerController.Instance.MainCamera);
        // disable view border marker if object is visible
        if (GeometryUtility.TestPlanesAABB(frustumPlanes, markerMesh.bounds))
        {
            screenBorderMarker.gameObject.SetActive(false);
        }
        // update view border marker position
        else
        {
            screenBorderMarker.gameObject.SetActive(true);
            Vector3 screenBorderPosition = GetScreenBorderPosition(frustumPlanes);
            
            screenBorderMarker.transform.position = screenBorderPosition;
        }

        ProfilingUtility.EndSample();
    }

    Vector3 GetScreenBorderPosition(Plane[] frustumPlanes)
    {
        Vector3 direction = transform.position - PlayerController.Instance.transform.position;

        //// face player
        if (PlayerController.Instance != null && PlayerController.Instance.MainCamera != null)
        {
            transform.rotation = PlayerController.Instance.MainCamera.transform.rotation;
        }

        // rotate in parent direction
        Vector2 direction2D = new Vector2(direction.x, direction.z);
        float angle = Mathf.Acos(Vector2.Dot(direction2D.normalized, Vector2.up)) * Mathf.Rad2Deg + 180;
        angle = direction.x > 0.0f ? -angle : angle;

        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        screenBorderMarker.transform.rotation *= rotation;


        Ray ray = new Ray(PlayerController.Instance.transform.position, direction);
        // ignore near and far planes (indices 4 and 5)
        float closestDistance = float.MaxValue;
        for (int i = 0; i < 4; i++)
        {
            float enter;
            if (frustumPlanes[i].Raycast(ray, out enter))
            {
                closestDistance = Mathf.Min(closestDistance, enter);
            }
        }

        return ray.GetPoint(closestDistance);
    }
}
