using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip bgm;
    [SerializeField] private bool loop = true;

    private void Start()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlayBgm(bgm, loop);
    }
}
