using UnityEngine;

public class SwitchSOValue : MonoBehaviour
{
    [SerializeField] BoolSO boolSO;

    public void SwitchValue()
    {
        boolSO.value = !boolSO.value;
    }
}
