using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject expBar;

    Xeomon _xeomon;

    public void SetData(Xeomon xeomon)
    {
        _xeomon = xeomon;
        nameText.text = xeomon.BaseInformation.Name;
        levelText.text = "Lvl " + xeomon.Level;
        SetLevel();
        healthBar.SetHP((float)xeomon.HP / xeomon.MaxHP);
        SetExp();
    }

    public IEnumerator UpdateHP()
    {
        yield return healthBar.SetHPSmooth((float)_xeomon.HP / _xeomon.MaxHP);
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _xeomon.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp()
    {
        int currLevelExp = _xeomon.BaseInformation.GetExpForLevel(_xeomon.Level);
        int nextLevelExp = _xeomon.BaseInformation.GetExpForLevel(_xeomon.Level + 1);

        float normalizedExp = (float)(_xeomon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
}
