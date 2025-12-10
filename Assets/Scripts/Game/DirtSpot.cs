using UnityEngine;

public class DirtSpot : MonoBehaviour
{
    [Tooltip("”ровень гр€зи (1 = маленький мусор, 3 = большое загр€знение)")]
    public int dirtLevel = 1;

    public int GetDirtLevel()
    {
        return Mathf.Max(0, dirtLevel);
    }

    public void Clean(int amount = 1)
    {
        dirtLevel -= amount;
        if (dirtLevel < 0) dirtLevel = 0;
    }
}
