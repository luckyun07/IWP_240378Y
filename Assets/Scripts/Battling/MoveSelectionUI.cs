using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Color highlightedColor;

    int currentSelectedMoveIndex = 0;

    public void SetMoveData(List<MoveBaseInformation> currentMoves, MoveBaseInformation newMove)
    {
        for (int i = 0; i<currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {   
            if (keyboard.downArrowKey.wasPressedThisFrame)
                currentSelectedMoveIndex++;

            else if (keyboard.upArrowKey.wasPressedThisFrame)
                currentSelectedMoveIndex--;

            currentSelectedMoveIndex = Mathf.Clamp(currentSelectedMoveIndex, 0, XeomonBaseInformation.MaxNumOfMoves);

            UpdateMoveSelection(currentSelectedMoveIndex);

            if (keyboard.enterKey.wasPressedThisFrame)
                onSelected?.Invoke(currentSelectedMoveIndex);
        }
            

    }

    public void UpdateMoveSelection(int selectedMoveIndex)
    {
        for (int i = 0; i < XeomonBaseInformation.MaxNumOfMoves + 1; i++)
        {
            if (i == selectedMoveIndex)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
    }

}
