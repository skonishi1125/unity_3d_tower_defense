using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVfx : MonoBehaviour
{
    [SerializeField] private EnemyHealth health;

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
    [SerializeField] private Vector3 damagePopupOffset = new Vector3(0, 2f, 0);
    [SerializeField] private float randomRange = 0.5f;

    [Header("Display Money Number Vfx")]
    [SerializeField] private GameObject moneyNumberVfx;
    [SerializeField] private Color moneyNumberColor = Color.yellow;
    [SerializeField] private Vector3 moneyPopupOffset = new Vector3(0, 2f, 0);

    private Coroutine onDamageVfxCo;

    protected virtual void Awake()
    {
        if (health == null)
            health = GetComponent<EnemyHealth>();

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

    public void CreateDamagePopup(float damage)
    {
        if (damageNumberVfx == null) return;

        // 生成位置を決定（敵の少し上 + ランダムなズレ）
        Vector3 spawnPosition = transform.position + damagePopupOffset;
        spawnPosition.x += Random.Range(-randomRange, randomRange);
        spawnPosition.z += Random.Range(-randomRange, randomRange);

        // プレハブを生成
        GameObject popupObj = Instantiate(damageNumberVfx, spawnPosition, Quaternion.identity);

        // スクリプトを取得してセットアップ
        DamagePopUp popupScript = popupObj.GetComponent<DamagePopUp>();
        if (popupScript != null)
            popupScript.Setup(damage, damageNumberVfxColor);
    }

    public void CreateMoneyPopup(int amount)
    {
        if (moneyNumberVfx == null) return;

        Vector3 spawnPosition = transform.position + moneyPopupOffset;

        GameObject popupObj = Instantiate(moneyNumberVfx, spawnPosition, Quaternion.identity);

        MoneyPopUp script = popupObj.GetComponent<MoneyPopUp>();
        script.Setup(amount, moneyNumberColor);
    }



}
