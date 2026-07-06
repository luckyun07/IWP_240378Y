using UnityEngine;

public class SizeIncrease : MonoBehaviour
{
    [SerializeField] GameObject backgroundsprite;
    [SerializeField] GameObject fingersprite;
    float sizeIncrease = 0.01f;
    void Start()
    {
        
    }

    void Update()
    {
        if (backgroundsprite.transform.localScale.x < 25)
            backgroundsprite.transform.localScale += new Vector3(sizeIncrease, sizeIncrease, sizeIncrease);
        else
            fingersprite.SetActive(true);
    }
}
