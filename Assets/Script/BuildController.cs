using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private GameObject towerPrefab;

    private void Awake()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Build();

    }

    private void Build()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 p = hit.point;
            float s = .2f;

            // 十字マーカー（赤）
            Debug.DrawLine(p - Vector3.right * s, p + Vector3.right * s, Color.red, 10f);
            Debug.DrawLine(p - Vector3.forward * s, p + Vector3.forward * s, Color.red, 10f);
            Debug.DrawLine(p - Vector3.up * s, p + Vector3.up * s, Color.red, 10f);

            // Ray 自体も可視化
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 10f);

            Instantiate(towerPrefab, hit.point, Quaternion.identity);
        }
    }

}
