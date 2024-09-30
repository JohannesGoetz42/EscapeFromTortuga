using System.Collections;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] musicTracks;
    [SerializeField]
    private AudioSource soundSource;
    [SerializeField]
    private float volume = 0.3f;

    private int currentTrack;

    private void Start()
    {
        if (musicTracks == null || soundSource == null)
        {
            return;
        }

        float trackDuration = PlayRandomTrack();
        StartCoroutine(StartNextTrack(trackDuration));
    }

    float PlayRandomTrack()
    {
        // do not play the same track twice
        int newTrack = Random.Range(0, musicTracks.Length - 1);
        currentTrack = newTrack < currentTrack ? newTrack : newTrack + 1;

        soundSource.clip = musicTracks[currentTrack];
        soundSource.volume = volume;
        soundSource.Play();
        return musicTracks[currentTrack].length;
    }

    IEnumerator StartNextTrack(float trackDuration)
    {
        yield return new WaitForSeconds(trackDuration);

        trackDuration = PlayRandomTrack();
        StartCoroutine(StartNextTrack(trackDuration));
    }
}
