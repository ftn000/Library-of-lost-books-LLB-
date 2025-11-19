using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinqView : MonoBehaviour
{
    [SerializeField] TMP_Text _cellIndex;
    [SerializeField] TMP_Text _cellCount;
    [SerializeField] Image _cellIcon;
    public void FillView(int cellIndex, Sprite image, int cellCount)
    {
        _cellIndex.text = cellIndex.ToString();
        _cellCount.text = cellCount.ToString();
        _cellIcon.sprite = image;
    }
}
