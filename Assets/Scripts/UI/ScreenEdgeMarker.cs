using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEdgeMarker : MonoBehaviour
{
    [SerializeField]
    Image thumbnailImage;
    [SerializeField]
    Image backgroundImage;

    MeshRenderer _targetMesh;
    RectTransform _screenBorderMarker;
    RectTransform _thumbnail;

    public static ScreenEdgeMarker AddToGameObject(MeshRenderer targetMesh, ScreenEdgeMarker markerPrefab, Sprite thumbnailSprite, Color backgroundColor)
    {
        GameObject gameObject = Instantiate(markerPrefab.gameObject);
        ScreenEdgeMarker createdInstance = gameObject.GetComponent<ScreenEdgeMarker>();

        createdInstance._screenBorderMarker = gameObject.transform as RectTransform;
        createdInstance._targetMesh = targetMesh;
        gameObject.transform.SetParent(PlayerController.Instance.OverlayCanvas.transform);

        // initialize thumbnail
        if (createdInstance.thumbnailImage != null)
        {
            createdInstance.thumbnailImage.sprite = thumbnailSprite;
            createdInstance._thumbnail = createdInstance.thumbnailImage.gameObject.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarningFormat("Screen edge marker {0} has no thumbnail image!", markerPrefab.name);
        }

        // initialize background color
        if (createdInstance.backgroundImage!= null)
        {
            createdInstance.backgroundImage.color = backgroundColor;
        }
        else
        {
            Debug.LogWarningFormat("Screen edge marker {0} has no background image!", markerPrefab.name);
        }

        return createdInstance;
    }

    private void LateUpdate()
    {
        ProfilingUtility.BeginSample("WorldMarkerUpdate");

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(PlayerController.Instance.MainCamera);

        // disable view border marker if object is visible
        if (GeometryUtility.TestPlanesAABB(frustumPlanes, _targetMesh.bounds))
        {
            _screenBorderMarker.localScale = Vector3.zero;
        }
        // update view border marker position
        else
        {
            _screenBorderMarker.localScale = Vector3.one;
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

        if (_thumbnail != null)
        {
            _thumbnail.transform.rotation = Quaternion.Inverse(rotation);
        }

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
