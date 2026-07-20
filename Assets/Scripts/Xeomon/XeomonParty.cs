using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XeomonParty : MonoBehaviour
{
    [SerializeField] List<Xeomon> xeomons;

    public List<Xeomon> Xeomons {
        get
        {
            return xeomons;
        }
    }

    private void Start()
    {
        foreach (var xeomon in xeomons)
        {
            xeomon.Init();
        }
    }

    public void Reset()
    {
        xeomons.Clear();
    }

    public Xeomon GetHealthyXeomon()
    {
        return xeomons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddXeomon(Xeomon newXeomon)
    {
        if (xeomons.Count < 6)
        {
            xeomons.Add(newXeomon);
        }
        else { 
            // TODO: add pc system
        }
    }
}
