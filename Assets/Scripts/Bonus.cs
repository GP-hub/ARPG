using UnityEngine;


public class Bonus : MonoBehaviour
{
    [SerializeField] private Perk perk;

    public void GetBonus()
    {
        // Call the SelectPerk method from PerksManager
        PerksManager.Instance.SelectPerk(perk);
    }
}
