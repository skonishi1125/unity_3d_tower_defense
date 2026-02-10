using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVfx : MonoBehaviour
{
    private MeshRenderer[] mrs;
    // MeshRendererをkeyにした、Rendererのマテリアル配列
    // originalMaterials[mr] で、元マテリアルを配列で出せるような設計。
    private Dictionary<MeshRenderer, Material[]> originalMaterials;

    [Header("Damage Vfx")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .1f;

    // ダメージ数値
    [Header("Attack Damage Number Vfx")]
    [SerializeField] private GameObject damageNumberVfx;
    [SerializeField] private Color damageNumberVfxColor = Color.white;

    private Coroutine onDamageVfxCo;

    protected virtual void Awake()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Dictionary<MeshRenderer, Material[]>(mrs.Length); // lengthを指定することでメモリの節約になる

        foreach (var mr in mrs)
        {
            // 1つのMeshRendererごとに、マテリアルを格納
            // mrに複数マテリアルが付くケースがあるので（SubMeshと呼ぶ）、クローンして元配列自体を格納
            // クローン結果はobjectで返るので、Material[]でキャストして格納
            originalMaterials[mr] = (Material[])mr.materials.Clone();
        }
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCo != null)
            StopCoroutine(onDamageVfxCo);

        onDamageVfxCo = StartCoroutine(OnDamageVfx());

    }

    private IEnumerator OnDamageVfx()
    {
        // ダメージ
        foreach (var mr in mrs)
        {
            if (mr == null) continue;

            var materials = mr.materials;
            for (int i = 0; i < materials.Length; i++)
                materials[i] = onDamageMaterial;

            mr.materials = materials;
        }

        yield return new WaitForSeconds(onDamageVfxDuration);

        // 元マテリアルに戻す
        foreach (var mr in mrs)
        {
            if (mr == null) continue;

            if (originalMaterials.TryGetValue(mr, out var original))
                mr.materials = original;
        }

        // もうこのCoroutineは走っていないという状態にしておく
        onDamageVfxCo = null;
    }

    public void CreateOnDamageNumberVfx(Transform target, float damage)
    {
        var go = Instantiate(damageNumberVfx, target.position, Quaternion.identity);
        //var vfx = go.GetComponent<DamageNumberVfx>();
        //vfx.Init(damage, damageNumberVfxColor);
    }



}
