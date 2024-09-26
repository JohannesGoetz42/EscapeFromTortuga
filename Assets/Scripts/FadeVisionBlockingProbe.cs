using UnityEngine;

public class FadeVisionBlockingProbe : MonoBehaviour
{
    [field: SerializeField]
    public LayerMask viewCollisionFadeLayer { get; private set; }

    private CapsuleCollider _viewFadeCollider;

    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera == null)
        {
            return;
        }

        FollowPlayer followPlayerComponent = mainCamera.GetComponent<FollowPlayer>();
        if (followPlayerComponent == null)
        {
            return;
        }

        // add the collider to the camera, so it is rotated as it should be
        _viewFadeCollider = mainCamera.AddComponent<CapsuleCollider>();
        _viewFadeCollider.excludeLayers = ~viewCollisionFadeLayer;
        _viewFadeCollider.isTrigger = true;
        _viewFadeCollider.height = followPlayerComponent.cameraDistance;
        _viewFadeCollider.radius = 2.0f;
        _viewFadeCollider.center = new Vector3(0.0f, 0.0f, 0.5f * followPlayerComponent.cameraDistance);
        _viewFadeCollider.direction = 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        FadeWhenPlayerIsClose fader = other.gameObject.GetComponent<FadeWhenPlayerIsClose>();
        if (fader != null)
        {
            fader.Fade(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FadeWhenPlayerIsClose fader = other.gameObject.GetComponent<FadeWhenPlayerIsClose>();
        if (fader != null)
        {
            fader.Fade(true);
        }
    }

    private void OnDrawGizmos()
    {
        if (_viewFadeCollider != null)
        {
            Gizmos.color = Color.magenta;
        }
    }
}
