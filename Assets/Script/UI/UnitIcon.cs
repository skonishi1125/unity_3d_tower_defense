using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI hotKey;

    public void SetInfo(UnitDefinition def)
    {
        image.sprite = def.Icon;
        hotKey.text = $"{def.HotkeySlot}";
    }

}
