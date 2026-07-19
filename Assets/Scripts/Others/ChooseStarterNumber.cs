using UnityEngine;

public class ChooseStarterNumber : MonoBehaviour
{
    [SerializeField] FloatSO starterNumber;

    public void SetStarterNumber(float number)
    {
        starterNumber.value = number;
    }
}
