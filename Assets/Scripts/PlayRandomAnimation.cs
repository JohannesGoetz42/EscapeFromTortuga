using UnityEngine;

public class PlayRandomAnimation : MonoBehaviour
{
    [SerializeField]
    string[] availableAnimations = new string[0];
    [SerializeField]
    float MinAnimationInterval = 3.0f;
    [SerializeField]
    float MaxAnimationInterval = 6.0f;

    private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator != null && availableAnimations.Length > 0)
        {
            Invoke(nameof(PlayRandom), Random.Range(MinAnimationInterval, MaxAnimationInterval));
        }
    }

    void PlayRandom()
    {
        int selectedIndex = Random.Range(0, availableAnimations.Length);
        _animator.SetTrigger(availableAnimations[selectedIndex]);

        Invoke(nameof(PlayRandom), Random.Range(MinAnimationInterval, MaxAnimationInterval));
    }
}
