using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI hotKey;
    [SerializeField] private Button button;

    // UnitIconsがアイコンを生成するときに走らせる処理
    public void SetInfo(UnitDefinition def, Action<UnitDefinition> onClickAction)
    {
        cost.text = $"¥{def.Cost}";
        image.sprite = def.Icon;
        hotKey.text = $"[{def.HotkeySlot}]";

        // ボタンリスナー設定
        // 一度外して、引数として渡されたメソッドをクリック時に走らせるように登録
        // 以降、クリックされたとき渡されたメソッドが走るようになる。
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickAction(def));
    }

}
