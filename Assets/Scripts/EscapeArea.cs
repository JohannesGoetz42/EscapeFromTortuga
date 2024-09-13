using UnityEngine;

public class EscapeArea : MonoBehaviour
{
    /** The display name i.E. used in dialog texts */
    [field: SerializeField]
    public string DisplayName { get; private set; }
    [SerializeField]
    Sprite thumbnail;
    [SerializeField]
    WorldMarker marker;

    bool isReadyToDepart;

    public void SetReadyToDepart()
    {
        isReadyToDepart = true;
        if (marker != null)
        {
            marker.AddMarkerSource(this, WorldMarkerVisibility.OverheadAndScreenBorder, thumbnail);
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
