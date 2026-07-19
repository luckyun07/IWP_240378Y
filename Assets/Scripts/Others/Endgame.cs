using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Endgame : MonoBehaviour
{
    [SerializeField] BoolSO isGameOver;
    [SerializeField] Sprite winScreen;
    [SerializeField] Sprite loseScreen;
    [SerializeField] TMP_Text winLoseText;

    private void Update()
    {
        if (isGameOver.value)
        {
            winLoseText.text = "DED"; 
            this.gameObject.GetComponent<Image>().sprite = loseScreen;
        }
        else
        {
            winLoseText.text = "Omg You Did It!"; 
            this.gameObject.GetComponent<Image>().sprite = winScreen;
        }
    }
}
