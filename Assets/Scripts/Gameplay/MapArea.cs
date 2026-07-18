using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Xeomon> wildXeomons;
    [SerializeField] XeomonParty playerXeomons;
    [SerializeField] GameObject lockedArea;

    bool[] conditionMet = new bool[3];
    public void Init()
    {
        if (conditionMet == null)
        {
            for (int i = 0; i < 3; i++)
            {
                conditionMet[i] = false;
            }
        }
    }

    public Xeomon GetRandomWildXeomon()
    {
        var wildXeomon =  wildXeomons[Random.Range(0, wildXeomons.Count)];
        wildXeomon.Init();

        return wildXeomon;
    }
    
    public void Update()
    {
        for (int i = 0; i < playerXeomons.Xeomons.Count; i++)
        {
            if (playerXeomons.Xeomons[i].BaseInformation.name == "Beage")
            {
                conditionMet[0] = true;
            }
            if (playerXeomons.Xeomons[i].BaseInformation.name == "Sparkityr")
            {
                conditionMet[1] = true;
            }
            if (playerXeomons.Xeomons[i].BaseInformation.name == "Tulcub")
            {
                conditionMet[2] = true;
            }
        }

        if (conditionMet[0] && conditionMet[1] && conditionMet[2])
        {
            lockedArea.SetActive(false);
        }
    }

}
