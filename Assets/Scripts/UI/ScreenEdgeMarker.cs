using Unity.VisualScripting;
using UnityEngine;

public class ScreenEdgeMarker : MonoBehaviour
{
    MeshRenderer _targetMesh;
    RectTransform _screenBorderMarker;

    public static ScreenEdgeMarker AddToGameObject(MeshRenderer targetMesh, ScreenEdgeMarker markerPrefab)
    {
        GameObject gameObject = Instantiate(markerPrefab.gameObject);
        ScreenEdgeMarker createdInstance = gameObject.GetComponent<ScreenEdgeMarker>();

        createdInstance._screenBorderMarker = gameObject.transform as RectTransform;
        createdInstance._targetMesh = targetMesh;
        gameObject.transform.SetParent(PlayerController.Instance.OverlayCanvas.transform);

        return createdInstance;
    }

    private void LateUpdate()
    {
        ProfilingUtility.BeginSample("WorldMarkerUpdate");

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(PlayerController.Instance.MainCamera);

        // disable view border marker if object is visible
        if (GeometryUtility.TestPlanesAABB(frustumPlanes, _targetMesh.bounds))
        {
            // TODO: HIDE MARKER!!!
        }
        // update view border marker position
        else
        {
            UpdateBorderMarker(frustumPlanes);
        }

        ProfilingUtility.EndSample();
    }

    private void UpdateBorderMarker(Plane[] frustumPlanes)
    {
        Vector3 direction = _targetMesh.transform.position - PlayerController.Instance.transform.position;

        // rotate in target direction
        Vector2 direction2D = new Vector2(direction.x, direction.z);
        float angle = Mathf.Acos(Vector2.Dot(direction2D.normalized, Vector2.up)) * Mathf.Rad2Deg + 180;
        angle = direction.x > 0.0f ? -angle : angle;

        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        _screenBorderMarker.transform.rotation = rotation;

        // update position
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

        _screenBorderMarker.anchoredPosition = PlayerController.Instance.MainCamera.WorldToScreenPoint(ray.GetPoint(closestDistance));
    }
}
