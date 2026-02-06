using System;
using UnityEngine;

// 現在選択中のユニットを管理するクラス
// ユニットアイコンが選ばれたとき、ここで情報を更新して通知する
public class UnitSelection : MonoBehaviour
{
    public UnitDefinition Selected { get; private set; }
    public event Action<UnitDefinition> SelectedChanged;

    [SerializeField] private UnitDefinition debugHohe;

    public void Select(UnitDefinition def)
    {
        if (def == null) return;
        if (Selected == def) return;

        Selected = def;
        SelectedChanged?.Invoke(def);
    }

    // デバッグ用 ホヘーを差し込んでテストする
    public void DebugSelect()
    {
        Selected = debugHohe;
        SelectedChanged?.Invoke(debugHohe);
    }

}
