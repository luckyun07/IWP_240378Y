using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HealthBar healthBar;

    [SerializeField] Color highlightedColor;

    Xeomon _xeomon;

    public void SetData(Xeomon xeomon)
    {
        _xeomon = xeomon;
        nameText.text = xeomon.BaseInformation.Name;
        levelText.text = "Lvl " + xeomon.Level;
        healthBar.SetHP((float)xeomon.HP / xeomon.MaxHP);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }
}
