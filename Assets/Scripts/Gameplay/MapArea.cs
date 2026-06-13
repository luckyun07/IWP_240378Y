using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Xeomon> wildXeomons;

    public Xeomon GetRandomWildXeomon()
    {
        var wildXeomon =  wildXeomons[Random.Range(0, wildXeomons.Count)];
        wildXeomon.Init();

        return wildXeomon;
    }
}
