using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BackgroundMusicManager : MonoBehaviour
{
    private const string Track1Path = "Assets/Sounds/soundTrack1Sound.mp3";
    private const string Track2Path = "Assets/Sounds/soundTrack2Sound.mp3";
    private const float MusicVolume = 0.03f;

    public AudioClip soundTrack1Sound;
    public AudioClip soundTrack2Sound;

    private AudioSource musicSource;
    private int nextTrackIndex;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        BackgroundMusicManager existing = FindObjectOfType<BackgroundMusicManager>();
        if (existing != null) return;

        GameObject managerObject = new GameObject("BackgroundMusicManager");
        managerObject.AddComponent<BackgroundMusicManager>();
        DontDestroyOnLoad(managerObject);
    }

    void Awake()
    {
        BackgroundMusicManager[] managers = FindObjectsOfType<BackgroundMusicManager>();
        if (managers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        musicSource.loop = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = MusicVolume;

        AssignTracksIfNeeded();
    }

    void Update()
    {
        if (musicSource == null || musicSource.isPlaying) return;

        AudioClip nextTrack = GetNextTrack();
        if (nextTrack == null) return;

        musicSource.clip = nextTrack;
        musicSource.Play();
    }

    private AudioClip GetNextTrack()
    {
        AudioClip[] tracks = { soundTrack1Sound, soundTrack2Sound };
        if (tracks[0] == null && tracks[1] == null)
        {
            return null;
        }

        for (int i = 0; i < tracks.Length; i++)
        {
            AudioClip candidate = tracks[nextTrackIndex];
            nextTrackIndex = (nextTrackIndex + 1) % tracks.Length;
            if (candidate != null)
            {
                return candidate;
            }
        }

        return null;
    }

    private void AssignTracksIfNeeded()
    {
#if UNITY_EDITOR
        if (soundTrack1Sound == null)
        {
            soundTrack1Sound = AssetDatabase.LoadAssetAtPath<AudioClip>(Track1Path);
        }

        if (soundTrack2Sound == null)
        {
            soundTrack2Sound = AssetDatabase.LoadAssetAtPath<AudioClip>(Track2Path);
        }
#endif

        if (soundTrack1Sound == null && soundTrack2Sound == null)
        {
            Debug.LogWarning("[BackgroundMusicManager] Music tracks are not assigned.");
        }
    }
}
