using System.Collections;
using UnityEngine;

public class EnemyVfx : MonoBehaviour
{
    private MeshRenderer mr;

    [Header("Damage Vfx")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .1f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCo;

    protected virtual void Awake()
    {
        mr = GetComponentInChildren<MeshRenderer>();
        originalMaterial = mr.material;
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCo != null)
            StopCoroutine(onDamageVfxCo);

        onDamageVfxCo = StartCoroutine(OnDamageVfx());



    }

    private IEnumerator OnDamageVfx()
    {
        mr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        mr.material = originalMaterial;
    }



}
