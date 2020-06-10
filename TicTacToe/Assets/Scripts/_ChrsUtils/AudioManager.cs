using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clips { TAP, PLACE, WOOSH,TITLE_SONG, MATCH_SONG, WIN, TIE }

public class AudioManager : MonoBehaviour
{
    public Dictionary<Clips, AudioClip> audioLibrary { get; private set; }
    private List<AudioSource> audioSources = new List<AudioSource>();
    public AudioSource primaryAudioSource;
    private AudioClip audioClip;

    private bool fadeAudio = false;

    private float _volume
    {
        get
        {
            if (muted) return 0;
            else return 1;
        }
    }
    public bool muted { get; private set; }

    private TaskManager _tm = new TaskManager();
	// Use this for initialization
	public void Init ()
    {
        audioLibrary = new Dictionary<Clips, AudioClip>();
        primaryAudioSource = GetComponentInChildren<AudioSource>();
        if(primaryAudioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
            primaryAudioSource = GetComponent<AudioSource>();
        }
        audioSources.Add(primaryAudioSource);
        muted = false;
        primaryAudioSource.volume = _volume;
        LoadLibrary();
	}

    private void LoadLibrary()
    {
        audioLibrary.Add(Clips.TAP, Resources.Load<AudioClip>("Audio/hint"));
        audioLibrary.Add(Clips.PLACE, Resources.Load<AudioClip>("Audio/drum"));
        audioLibrary.Add(Clips.WOOSH, Resources.Load<AudioClip>("Audio/woosh"));
        audioLibrary.Add(Clips.TITLE_SONG, Resources.Load<AudioClip>("Audio/titleSong"));
        audioLibrary.Add(Clips.MATCH_SONG, Resources.Load<AudioClip>("Audio/matchSong"));
        audioLibrary.Add(Clips.WIN, Resources.Load<AudioClip>("Audio/winSound"));
        audioLibrary.Add(Clips.TIE, Resources.Load<AudioClip>("Audio/clack"));


    }

    public void CreateTrackAndPlay(Clips clip, float pitch = -1)
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        float v = 0.5f;
        if(muted) v = 0;
        audioSources.Add(newSource);
        if(pitch == -1)
            PlayClip(clip, 1, v, newSource);
        else
            PlayClip(clip, pitch, v, newSource);
    }

    public void PlayClipVaryPitch(Clips clip, AudioSource source = null)
    {
        if(source == null) source = primaryAudioSource;
        float pitch = Random.Range(0.8f, 1.2f);
        PlayClip(clip, pitch, 0.3f, source);
    }

    public void PlayClip(Clips clip, AudioSource source = null)
    {
        if(source == null) source = primaryAudioSource;
        PlayClip(clip, 1.0f, 0.3f, source);
    }

    public void PlayClip(Clips clip, float pitch, float volume, AudioSource source = null)
    {
        if(source == null) source = primaryAudioSource;
        audioClip = audioLibrary[clip];
        source.pitch = pitch;
        source.clip = audioClip;
        source.PlayOneShot(audioClip, volume);
    }

    public void StopClip(AudioSource source = null)
    {
        if(source == null) source = primaryAudioSource;
        source.Stop();
    }

    public void FadeAudio(AudioSource source = null)
    {
        if(source == null) source = primaryAudioSource;
        Task fadeAudio = new FadeAudio(source, source.volume, 0, 0.5f);
        _tm.Do(fadeAudio);
    }

    public void ToggleMute(AudioSource source = null)
    {
        if(source == null) source = primaryAudioSource;
        muted = !muted;
        if(muted) source.volume = 0;
        else source.volume = 0.3f;
    }

    public void SetVolume(float volume, AudioSource source = null)
    {
        if(source == null) source = primaryAudioSource;
        source.volume = volume;
    }

    List<AudioSource> sourcesToBeRemoved = new List<AudioSource>();
    private void Update()
    {
        _tm.Update();
        foreach(AudioSource source in audioSources)
        {
            if(source != primaryAudioSource && !source.isPlaying)
            {
                sourcesToBeRemoved.Add(source);
            }
        }

        foreach(AudioSource source in sourcesToBeRemoved)
        {
            audioSources.Remove(source);
            Destroy(source);
        }

    }
}
