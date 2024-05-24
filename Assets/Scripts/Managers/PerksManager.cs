using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerksManager : Singleton<PerksManager>
{
    public List<Perk> availablePerks = new List<Perk>();
    private List<Perk> selectedPerks = new List<Perk>();

    [SerializeField] private GameObject perksUIPicker;

    [HideInInspector]public GameObject player;

    private Perk perkChoice01;
    private Perk perkChoice02;
    private Perk perkChoice03;

    private GameObject perkUI01;
    private GameObject perkUI02;
    private GameObject perkUI03;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        perkUI01 = perksUIPicker.transform.GetChild(0).Find("Perk_01").gameObject;
        perkUI02 = perksUIPicker.transform.GetChild(0).Find("Perk_02").gameObject;
        perkUI03 = perksUIPicker.transform.GetChild(0).Find("Perk_03").gameObject;
    }

    // Method to select a perk
    public void SelectPerk(int perkChoiceNumber)
    {
        Perk pickePerk = PerkChoiceNumberToPerk(perkChoiceNumber);
        if (!selectedPerks.Contains(pickePerk))
        {
            selectedPerks.Add(pickePerk);
            ApplyPerks(pickePerk);
        }
        perksUIPicker.SetActive(false);
    }

    private Perk PerkChoiceNumberToPerk(int i)
    {
        if (i==1) return perkChoice01;
        if (i==2) return perkChoice02;
        if (i==3) return perkChoice03;
        else return null;
    }

    // Method to apply perks to spells
    public void ApplyPerks(Perk perk)
    {
        if (player != null)
        {
            perk.ApplyEffects();
        }
    }

    public void PerksPickerProcess()
    {
        RollForPerks();
        perksUIPicker.SetActive(true);
    }

    private void RollForPerks()
    {
        List<Perk> resultList = GetRandomEntriesInFirstNotInSecond(availablePerks, selectedPerks, 3);
        if (resultList.Count >= 1 && resultList[0] != null) perkChoice01 = resultList[0];
        if (resultList.Count >= 2 && resultList[1] != null) perkChoice02 = resultList[1];
        if (resultList.Count >= 3 && resultList[2] != null) perkChoice03 = resultList[2];

        UpdatePerkUI(resultList.Count);
    }

    private List<T> GetRandomEntriesInFirstNotInSecond<T>(List<T> firstList, List<T> secondList, int count)
    {
        List<T> result = new List<T>();

        foreach (T item in firstList)
        {
            if (!secondList.Contains(item))
            {
                result.Add(item);
            }
        }

        // Shuffle the result list
        System.Random rand = new System.Random();
        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            T temp = result[i];
            result[i] = result[j];
            result[j] = temp;
        }

        // Take 'count' number of elements from the shuffled list
        if (result.Count > count)
        {
            result = result.GetRange(0, count);
        }

        return result;
    }

    private void UpdatePerkUI(int size)
    {
        if (size >= 1 && perkUI01 != null) 
        {
            perkUI01.GetComponent<PerkUI>().perkTitle.GetComponent<TMP_Text>().text = perkChoice01.perkName;
            perkUI01.GetComponent<PerkUI>().perkDescription.GetComponent<TMP_Text>().text = perkChoice01.perkDescription;
            perkUI01.GetComponent<PerkUI>().perkIcon.GetComponent<Image>().sprite = perkChoice01.perkImage;

            if (size==1)
            {
                perkUI02.SetActive(false);
                perkUI03.SetActive(false);
            }
        }
        if (size >= 2 && perkUI02 != null)
        {
            perkUI02.GetComponent<PerkUI>().perkTitle.GetComponent<TMP_Text>().text = perkChoice02.perkName;
            perkUI02.GetComponent<PerkUI>().perkDescription.GetComponent<TMP_Text>().text = perkChoice02.perkDescription;
            perkUI02.GetComponent<PerkUI>().perkIcon.GetComponent<Image>().sprite = perkChoice02.perkImage;

            if (size == 2)
            {
                perkUI03.SetActive(false);
            }
        }
        if (size >= 3 && perkUI03 != null)
        {
            perkUI03.GetComponent<PerkUI>().perkTitle.GetComponent<TMP_Text>().text = perkChoice03.perkName;
            perkUI03.GetComponent<PerkUI>().perkDescription.GetComponent<TMP_Text>().text = perkChoice03.perkDescription;
            perkUI03.GetComponent<PerkUI>().perkIcon.GetComponent<Image>().sprite = perkChoice03.perkImage;
        }
    }
}
