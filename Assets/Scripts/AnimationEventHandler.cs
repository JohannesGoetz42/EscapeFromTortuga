using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private CharacterSounds _sounds;
    private AudioSource _audioSource;

    public static void AddToObject(GameObject obj, CharacterSounds sounds)
    {
        if (sounds == null || obj == null)
        {
            return;
        }

        AnimationEventHandler createdInstance = obj.AddComponent<AnimationEventHandler>();
        createdInstance._sounds = sounds;

        createdInstance._audioSource = obj.AddComponent<AudioSource>();
        createdInstance._audioSource.spatialBlend = 1.0f;
        createdInstance._audioSource.rolloffMode = AudioRolloffMode.Linear;
        createdInstance._audioSource.maxDistance = 50.0f;
    }



    public void FootstepRun()
    {
        if (_audioSource == null)
        {
            return;
        }

        _audioSource.PlayOneShot(_sounds.footStepRun);
    }

    public void FootstepWalk()
    {
        if (_audioSource == null)
        {
            return;
        }

        _audioSource.PlayOneShot(_sounds.footStepWalk);
    }
}
