using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberTileView : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private TextMeshProUGUI numberText;

    public void SetNumber(int num)
    {
        numberText.SetText(num.ToString());
        numberText.ForceMeshUpdate();
    }

    public void Apply(TileColor tileColor)
    {
        bg.color = tileColor.backgroundColor;
        numberText.color = tileColor.numberColor;
    }
}
