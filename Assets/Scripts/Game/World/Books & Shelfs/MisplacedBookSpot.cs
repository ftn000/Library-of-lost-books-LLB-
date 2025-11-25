using UnityEngine;

public class MisplacedBookSpot : MonoBehaviour
{
    public bool hasBook = false;

    public void PlaceBook()
    {
        if (!hasBook)
        {
            hasBook = true;
            Debug.Log(" нига оставлена не на полке!");
        }
    }

    public void TakeBook()
    {
        if (hasBook)
        {
            hasBook = false;
            Debug.Log(" нига убрана с неправильного места.");
        }
    }
}
