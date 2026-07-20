using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Xeomon> wildXeomons;
    [SerializeField] XeomonParty playerXeomons;
    [SerializeField] GameObject lockedArea;

    private bool[] conditionMet = new bool[3];
    private int[] encounterCounts;

    private void Awake()
    {
        encounterCounts = new int[wildXeomons.Count];
    }

    public Xeomon GetRandomWildXeomon()
    {
        int totalWeight = 0;

        for (int i = 0; i < wildXeomons.Count; i++)
        {
            int weight = Mathf.Max(1, 10 - encounterCounts[i]);
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);

        for (int i = 0; i < wildXeomons.Count; i++)
        {
            int weight = Mathf.Max(1, 10 - encounterCounts[i]);

            if (randomValue < weight)
            {
                encounterCounts[i]++;

                Xeomon wildXeomon = wildXeomons[i];
                wildXeomon.Init();

                return wildXeomon;
            }

            randomValue -= weight;
        }

        return null;
    }

    private void Update()
    {
        for (int i = 0; i < playerXeomons.Xeomons.Count; i++)
        {
            string xeomonName = playerXeomons.Xeomons[i].BaseInformation.name;

            if (xeomonName == "Beage")
                conditionMet[0] = true;

            if (xeomonName == "Sparkityr")
                conditionMet[1] = true;

            if (xeomonName == "Tulcub")
                conditionMet[2] = true;
        }

        if (conditionMet[0] && conditionMet[1] && conditionMet[2])
        {
            lockedArea.SetActive(false);
        }
    }
}