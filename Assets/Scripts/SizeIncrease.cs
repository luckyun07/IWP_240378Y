using UnityEngine;

public class SizeIncrease : MonoBehaviour
{
    [SerializeField] GameObject backgroundsprite;
    [SerializeField] GameObject[] fingersprites;
    float sizeIncrease = 0.01f;
    float timeToRevealFingers = 0f;
    int fingerspriteIndex = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (backgroundsprite.transform.localScale.x < 20f)
            backgroundsprite.transform.localScale += new Vector3(sizeIncrease, sizeIncrease, sizeIncrease);
        else if (fingerspriteIndex < fingersprites.Length)
        {
            timeToRevealFingers += Time.deltaTime;

            if (timeToRevealFingers >= 5f)
            {
                fingersprites[fingerspriteIndex].SetActive(true);
                timeToRevealFingers = 0f;
                fingerspriteIndex++;
            }
        }

    }
}
