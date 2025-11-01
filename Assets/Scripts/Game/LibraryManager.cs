using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public int money = 100;
    public int reputation = 50; // показатель прогресса библиотеки

    public void EarnMoney(int amount)
    {
        money += amount;
        Debug.Log($"Заработано {amount}. Текущий баланс: {money}");
    }

    public void SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log($"Потрачено {amount}. Текущий баланс: {money}");
        }
        else
        {
            Debug.Log("Недостаточно средств!");
        }
    }

    public void ImproveLibrary(int points)
    {
        reputation += points;
        Debug.Log($"Прогресс библиотеки увеличен на {points}. Текущий рейтинг: {reputation}");
    }
}
