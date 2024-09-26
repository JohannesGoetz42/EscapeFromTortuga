using UnityEngine;

public class EscapeArea : MonoBehaviour, IHasThumbnail
{
    /** The display name i.E. used in dialog texts */
    [field: SerializeField]
    public string DisplayName { get; private set; }

    Sprite IHasThumbnail.Thumbnail => thumbnail;
    public Color BackgroundColor => color;

    [SerializeField]
    Sprite thumbnail;
    [SerializeField]
    WorldMarker marker;
    [SerializeField]
    SphereCollider triggerSphere;

    Color color = Color.white;
    bool isReadyToDepart;

    public void SetReadyToDepart(Color markerColor)
    {
        color = markerColor;
        isReadyToDepart = true;
        if (marker != null)
        {
            marker.AddMarkerSource(this, WorldMarkerVisibility.OverheadAndScreenBorder);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReadyToDepart && other.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.TryEscape();
        }
    }
}
