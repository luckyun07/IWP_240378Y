using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Xeomon> xeomons;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Xeomon> xeomons)
    {
        this.xeomons = xeomons;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < xeomons.Count)
            {
                memberSlots[i].SetData(xeomons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Xeomon";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
