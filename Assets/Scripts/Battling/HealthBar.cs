using System.Collections;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float HPnormalised)
    {
        health.transform.localScale = new Vector3(HPnormalised, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        float currentHP = health.transform.localScale.x;
        float changeAmt = currentHP - newHP;

        while (currentHP - newHP > Mathf.Epsilon)
        {
            currentHP -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHP, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHP, 1f);
    }
}
