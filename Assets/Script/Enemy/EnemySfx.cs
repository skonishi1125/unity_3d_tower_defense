using UnityEngine;

public class EnemySfx : MonoBehaviour
{
    [Header("SE Clips")]
    [SerializeField] private AudioClip hittedSfx;

    public void PlayHitted() => AudioManager.Instance?.PlaySfx(hittedSfx);

}
