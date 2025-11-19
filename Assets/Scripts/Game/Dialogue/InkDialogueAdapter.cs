using Ink.Runtime;
using UnityEngine;

public class InkDialogueAdapter : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;

    public TextAsset GetInkJSON() => inkJSON;
}
