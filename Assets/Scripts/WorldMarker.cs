using System.Collections.Generic;
using System.Linq;
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
    ScreenEdgeMarker edgeMarkerPrefab;

    Dictionary<Object, WorldMarkerVisibility> _visibilitySources = new Dictionary<Object, WorldMarkerVisibility>();
    ScreenEdgeMarker _edgeMarker;

    private void Awake()
    {
        enabled = false;
    }

    public void AddMarkerSource(Object sourceObject, WorldMarkerVisibility visibility, Sprite thumbnail)
    {
        _visibilitySources.Add(sourceObject, visibility);
        markerMesh.enabled = visibility != WorldMarkerVisibility.Hidden;

        if (visibility == WorldMarkerVisibility.OverheadAndScreenBorder && edgeMarkerPrefab != null && _edgeMarker == null)
        {
            _edgeMarker = ScreenEdgeMarker.AddToGameObject(markerMesh, edgeMarkerPrefab, thumbnail);
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
        bool screenMarkerActive = false;
        foreach (var source in _visibilitySources)
        {
            if (source.Value == WorldMarkerVisibility.OverheadAndScreenBorder)
            {
                screenMarkerActive = true;
                break;
            }
        }

        if (_edgeMarker != null)
        {
            _edgeMarker.gameObject.SetActive(screenMarkerActive);
        }
    }

    private void OnDestroy()
    {
        if (_edgeMarker != null)
        {
            Destroy(_edgeMarker.gameObject);
        }
    }
}
