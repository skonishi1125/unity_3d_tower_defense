using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float master = 1f;
    [Range(0f, 1f)] public float bgm = 1f;
    [Range(0f, 1f)] public float ui = 1f;
    [Range(0f, 1f)] public float sfx = 1f;

    // Sceneをまたいで使うので、その処理
    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip == null || uiSource == null)
            return;

        uiSource.PlayOneShot(clip, master * ui);
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        Debug.Log("playsfx!");
        if (clip == null || sfxSource == null)
            return;

        Debug.Log($"SFX {clip?.name} frame={Time.frameCount} vol={volume}");
        sfxSource.PlayOneShot(clip, master * sfx * Mathf.Clamp01(volume));
    }

    public void PlayBgm(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null)
            return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.loop = loop;
        bgmSource.clip = clip;
        bgmSource.volume = master * bgm;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        if (bgmSource == null)
            return;

        bgmSource.Stop();
        bgmSource.clip = null;
    }


}
