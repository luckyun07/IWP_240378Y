using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlingDialogueBox : MonoBehaviour
{
    [SerializeField] Text dialogueText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject attackSelector;
    [SerializeField] GameObject attackDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] List<Text> attackTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text elementText;
    [SerializeField] Text attackInfoText;

    [SerializeField] Color highlightedColor;
    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (var letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialoguetext(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableAttackSelector(bool enabled)
    {
        attackDetails.SetActive(enabled);
        attackSelector.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i =0; i<actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
        ppText.text = $"PP {move.PP}/{move.BaseInformation.PP}";
        elementText.text = move.BaseInformation.Element.ToString();
    }

    public void UpdateAttackSelection(int selectedMove)
    {
        for (int i = 0; i < attackTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                attackTexts[i].color = highlightedColor;
            }
            else
            {
                attackTexts[i].color = Color.black;
            }
        }

        if (selectedMove == 0)
            attackInfoText.text = "physical attack";
        else if (selectedMove == 1)
            attackInfoText.text = "special attack";
        else if (selectedMove == 2)
            attackInfoText.text = "physical attack boosted by your xeomon, it will cause damage to your xeomon";
        else if (selectedMove == 3)
            attackInfoText.text = "special attack boosted by your xeomon, it will cause damage to your xeomon";
        else
            attackInfoText.text = "All out attack, will need to recharge";
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].BaseInformation.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }
}
