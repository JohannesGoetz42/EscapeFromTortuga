using System.Collections.Generic;
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

    Dictionary<Object, WorldMarkerVisibility> _visibilitySources = new Dictionary<Object, WorldMarkerVisibility>();

    public void AddMarkerSource(Object sourceObject, WorldMarkerVisibility visibility)
    {
        _visibilitySources.Add(sourceObject, visibility);
        markerMesh.enabled = visibility != WorldMarkerVisibility.Hidden;
    }

    public void RemoveMarkerSource(Object sourceObject)
    {
        _visibilitySources.Remove(sourceObject);
        if (_visibilitySources.Count == 0)
        {
            markerMesh.enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        markerMesh.enabled = _visibilitySources.Count > 0;
    }
}
