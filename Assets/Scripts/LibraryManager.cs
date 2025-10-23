using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public int money = 100;
    public int reputation = 50; // ���������� ��������� ����������

    public void EarnMoney(int amount)
    {
        money += amount;
        Debug.Log($"���������� {amount}. ������� ������: {money}");
    }

    public void SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log($"��������� {amount}. ������� ������: {money}");
        }
        else
        {
            Debug.Log("������������ �������!");
        }
    }

    public void ImproveLibrary(int points)
    {
        reputation += points;
        Debug.Log($"�������� ���������� �������� �� {points}. ������� �������: {reputation}");
    }
}
