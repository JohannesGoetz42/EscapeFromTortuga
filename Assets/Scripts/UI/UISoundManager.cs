using UnityEngine;
using UnityEngine.UIElements;

public class UISoundManager : MonoBehaviour
{
    [SerializeField]
    AudioClip buttonPressSound;
    [SerializeField]
    AudioSource audioSource;

    private static UISoundManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;
    }

    public static void OnClick(EventBase pressEvent)
    {
        ClickEvent clickEvent = pressEvent as ClickEvent;
        if (clickEvent != null)
        {
            _instance.audioSource.PlayOneShot(_instance.buttonPressSound);
        }
    }
}
