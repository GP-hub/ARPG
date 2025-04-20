using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityValues : MonoBehaviour
{
    [SerializeField] private int damage;

    public List<GameObject> playersToDamage = new List<GameObject>();
    public int Damage { get => damage; set => damage = value; }


    public void DoDamage(int damage)
    {
        foreach (GameObject players in playersToDamage)
        {
            EventManager.PlayerTakeDamage(damage);
        }
    }

    private void OnDisable()
    {
        playersToDamage.Clear();
    }
}
