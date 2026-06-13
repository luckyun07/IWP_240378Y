using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HealthBar healthBar;

    Xeomon _xeomon;

    public void SetData(Xeomon xeomon)
    {
        _xeomon = xeomon;
        nameText.text = xeomon.BaseInformation.Name;
        levelText.text = "Lvl " + xeomon.Level;
        healthBar.SetHP((float)xeomon.HP / xeomon.MaxHP);
    }

    public IEnumerator UpdateHP()
    {
        yield return healthBar.SetHPSmooth((float)_xeomon.HP / _xeomon.MaxHP);
    }
}
