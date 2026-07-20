using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;

public enum BattleState { Start, ActionSelection, MoveSelection, AbilityAttack, PerformMove, Busy, PartyScreen, MoveToForget, BattleOver, RunningTurn }

public class BattlingSystem : MonoBehaviour
{
    [SerializeField] BattlingXeomon playerXeomon;
    [SerializeField] BattlingXeomon enemyXeomon;
    [SerializeField] BattlingDialogueBox dialogueBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] GameObject xorbballSprite;
    [SerializeField] Sprite[] weaponSprite;
    [SerializeField] Image weapon;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentAttack;
    int currentMember;

    bool usedLimitRelease = false;

    XeomonParty playerParty;
    Xeomon wildXeomon;

    MoveBaseInformation moveToLearn;

    [SerializeField] BoolSO instantcatch;

    public void StartBattle(XeomonParty playerParty, Xeomon wildXeomon)
    {
        this.playerParty = playerParty;
        this.wildXeomon = wildXeomon;

        usedLimitRelease = false;
        instantcatch.value = false;
        StartCoroutine(SetUpBattle());
    }

    public IEnumerator SetUpBattle()
    {
        playerXeomon.Setup(playerParty.GetHealthyXeomon());
        enemyXeomon.Setup(wildXeomon);

        partyScreen.Init();

        dialogueBox.SetMoveNames(playerXeomon.Xeomon.Moves);

        if (playerXeomon.Xeomon.BaseInformation.Element1 == XeomonElement.Fire)
        {
            weapon.sprite = weaponSprite[0];
        }
        else if (playerXeomon.Xeomon.BaseInformation.Element1 == XeomonElement.Water)
        {
            weapon.sprite = weaponSprite[1];
        }
        else if (playerXeomon.Xeomon.BaseInformation.Element1 == XeomonElement.Grass)
        {
            weapon.sprite = weaponSprite[2];
        }

        yield return dialogueBox.TypeDialogue("A wild " + enemyXeomon.Xeomon.BaseInformation.Name + " appeared!");

        ActionSelection();
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogueBox.SetDialogue("Choose an action");
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Xeomons);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialoguetext(false);
        dialogueBox.EnableMoveSelector(true);
    }

    void AbilityAttack()
    {
        state = BattleState.AbilityAttack;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialoguetext(false);
        dialogueBox.EnableAttackSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        var move = playerXeomon.Xeomon.Moves[currentMove];
        yield return RunMove(playerXeomon, enemyXeomon, move);
        usedLimitRelease = false;

        // If the battle stat was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }
    IEnumerator PlayerAttack(int num)
    {
        state = BattleState.PerformMove;
        yield return RunAbility(playerXeomon, enemyXeomon, num);

        // If the battle stat was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyXeomon.Xeomon.GetRandomMove();
        yield return RunMove(enemyXeomon, playerXeomon, move);

        // If the battle stat was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(BattlingXeomon sourceXeomon, BattlingXeomon targetXeomon, Move move)
    {

        move.PP--;
        yield return dialogueBox.TypeDialogue(sourceXeomon.Xeomon.BaseInformation.Name + " used " + move.BaseInformation.Name);

        if (move.BaseInformation.Category == MoveCategory.Status)
        {
            var effects = move.BaseInformation.Effects;
            if (effects.Boosts != null)
            {
                if (move.BaseInformation.Target == MoveTarget.Self)
                    sourceXeomon.Xeomon.ApplyBoosts(effects.Boosts);
                else
                    targetXeomon.Xeomon.ApplyBoosts(effects.Boosts);
            }
        }
        else
        {
            var damageDetails = targetXeomon.Xeomon.TakeDamage(move, sourceXeomon.Xeomon);
            yield return targetXeomon.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (targetXeomon.Xeomon.HP <= 0)
        {
            yield return HandleXeomonFainted(targetXeomon);
        }
    }

    IEnumerator RunAbility(BattlingXeomon sourceXeomon, BattlingXeomon targetXeomon, int num)
    {
        if (num == 0)
            yield return dialogueBox.TypeDialogue("You used a " + sourceXeomon.Xeomon.BaseInformation.Element1 + " slash!");
        else if (num == 1)
            yield return dialogueBox.TypeDialogue("You used a " + sourceXeomon.Xeomon.BaseInformation.Element1 + " blast!");
        else if (num == 2)
            yield return dialogueBox.TypeDialogue("You used a " + sourceXeomon.Xeomon.BaseInformation.Element1 + " enhanced slash!");
        else if (num == 3)
            yield return dialogueBox.TypeDialogue("You used a " + sourceXeomon.Xeomon.BaseInformation.Element1 + " enhanced blast!");
        else if (num == 4) {
            usedLimitRelease = true;
            yield return dialogueBox.TypeDialogue("You used a " + sourceXeomon.Xeomon.BaseInformation.Element1 + " limit release!");
        }

        var damageDetails = targetXeomon.Xeomon.TakeAbilityDamage(5f * sourceXeomon.Xeomon.Level, sourceXeomon.Xeomon, num);

        if (num == 2 || num == 3)
        {
            yield return sourceXeomon.Hud.UpdateHP();
        }

        yield return targetXeomon.Hud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (targetXeomon.Xeomon.HP <= 0)
        {
            yield return HandleXeomonFainted(targetXeomon);
        }
    }

    IEnumerator HandleXeomonFainted(BattlingXeomon faintedXeomon)
    {
        yield return dialogueBox.TypeDialogue(faintedXeomon.Xeomon.BaseInformation.Name + " fainted!");
        yield return new WaitForSeconds(2f);

        if (!faintedXeomon.IsPlayerXeomon)
        {
            // Gaining EXP
            int Exp = faintedXeomon.Xeomon.BaseInformation.ExpYield;
            int enemyLevel = faintedXeomon.Xeomon.Level;

            int Expgain = Mathf.FloorToInt((Exp * enemyLevel) / 7f);
            playerXeomon.Xeomon.Exp += Expgain;
            yield return dialogueBox.TypeDialogue(playerXeomon.Xeomon.BaseInformation.Name + " gained " + Expgain + " XP!");
            yield return playerXeomon.Hud.SetExpSmooth();

            // Check for level up

            while (playerXeomon.Xeomon.CheckForLevelUp())
            {
                playerXeomon.Hud.SetLevel();
                yield return dialogueBox.TypeDialogue(playerXeomon.Xeomon.BaseInformation.Name + " leveled up to level " + playerXeomon.Xeomon.Level + "!");

                // Try to learn new moves
                var newMove = playerXeomon.Xeomon.GetLearnableMoveAtCurrentLevel();

                if(newMove != null)
                {
                    if (playerXeomon.Xeomon.Moves.Count < XeomonBaseInformation.MaxNumOfMoves)
                    {
                        playerXeomon.Xeomon.LearnMove(newMove);
                        yield return dialogueBox.TypeDialogue(playerXeomon.Xeomon.BaseInformation.Name + " learned " + newMove.BaseInformation.Name + "!");
                        dialogueBox.SetMoveNames(playerXeomon.Xeomon.Moves);
                    }
                    else
                    {
                        yield return dialogueBox.TypeDialogue(playerXeomon.Xeomon.BaseInformation.Name + " wants to learn " + newMove.BaseInformation.Name + ", but it can't learn more than " + XeomonBaseInformation.MaxNumOfMoves + " moves.");
                        yield return ChooseMoveToForget(playerXeomon.Xeomon, newMove.BaseInformation);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerXeomon.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);

        }

        CheckForBattleOver(faintedXeomon);
    }

    void CheckForBattleOver(BattlingXeomon faintedXeomon) {
        if (faintedXeomon.IsPlayerXeomon)
        {
            var nextXeomon = playerParty.GetHealthyXeomon();
            if (nextXeomon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
    }

    IEnumerator ChooseMoveToForget(Xeomon xeomon, MoveBaseInformation newMove)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue("Choose move to forget");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(xeomon.Moves.Select(x => x.BaseInformation).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    //IEnumerator RunTurns()
    //{

    //}

    IEnumerator ShowDamageDetails(DamageDetails damageDetails) {
        if (damageDetails.Critical > 1f)
            yield return dialogueBox.TypeDialogue("A critical hit!");

        if (damageDetails.Element > 1f)
            yield return dialogueBox.TypeDialogue("It's super effective!");
        else if (damageDetails.Element < 1f)
            yield return dialogueBox.TypeDialogue("It's not very effective...");
    }

    public void HandleUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.pKey.wasPressedThisFrame)
            {
                instantcatch.value = !instantcatch.value;
            }
        }

        if (state == BattleState.ActionSelection)
            HandleActionSelection();
        else if (state == BattleState.MoveSelection)
            HandleMoveSelection();
        else if (state == BattleState.PartyScreen)
            HandlePartySelection();
        else if (state == BattleState.AbilityAttack)
            HandleAbilitySelection();
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == XeomonBaseInformation.MaxNumOfMoves)
                {
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerXeomon.Xeomon.BaseInformation.name} did not learn {moveToLearn.name}"));
                }
                else
                {
                    var selectedMove = playerXeomon.Xeomon.Moves[moveIndex].BaseInformation;
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerXeomon.Xeomon.BaseInformation.name} forgot {selectedMove.name} and learned {moveToLearn.name}"));

                    playerXeomon.Xeomon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    void HandleAbilitySelection()
    {
       var keyboard = Keyboard.current;
       if (keyboard != null)
       {
           if (keyboard.rightArrowKey.wasPressedThisFrame)
               currentAttack++;
           else if (keyboard.leftArrowKey.wasPressedThisFrame)
                currentAttack--;
           else if (keyboard.downArrowKey.wasPressedThisFrame)
                currentAttack += 3;
           else if (keyboard.upArrowKey.wasPressedThisFrame)
               currentAttack -= 3;

           currentAttack = Mathf.Clamp(currentAttack, 0, 4);
           dialogueBox.UpdateAttackSelection(currentAttack);

           if (keyboard.enterKey.wasPressedThisFrame)
           {
               dialogueBox.EnableAttackSelector(false);
               dialogueBox.EnableDialoguetext(true);
               StartCoroutine(PlayerAttack(currentAttack));
           }
           else if (keyboard.escapeKey.wasPressedThisFrame)
           {
               dialogueBox.EnableAttackSelector(false);
               dialogueBox.EnableDialoguetext(true);
               ActionSelection();
           }
       }
    }

    void HandleActionSelection()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.rightArrowKey.wasPressedThisFrame)
                currentAction++;
            else if (keyboard.leftArrowKey.wasPressedThisFrame)
                currentAction--;
            else if (keyboard.downArrowKey.wasPressedThisFrame)
                currentAction += 2;
            else if (keyboard.upArrowKey.wasPressedThisFrame)
                currentAction -= 2;

            currentAction = Mathf.Clamp(currentAction, 0, 3);

            dialogueBox.UpdateActionSelection(currentAction);

            if (keyboard.enterKey.wasPressedThisFrame)
            {
                if (currentAction == 0)// Fight
                {
                    MoveSelection();
                }
                else if (currentAction == 1)// Weapon
                {
                    if (usedLimitRelease)
                    {
                        dialogueBox.SetDialogue("Attack on cooldown.");
                        return;
                    }
                    else
                        AbilityAttack();
                }
                else if (currentAction == 2)// Xeomon
                {
                    OpenPartyScreen();
                }
                else if (currentAction == 3)// Catch
                {
                    if ( playerParty.Xeomons.Count < 6)
                    {
                        StartCoroutine(ThrowXorbball());
                    }
                    else{
                        dialogueBox.SetDialogue("Your party is full");
                    }
                }
            }
        }
    }

    void HandleMoveSelection()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.rightArrowKey.wasPressedThisFrame)
                currentMove++;
            else if (keyboard.leftArrowKey.wasPressedThisFrame)
                currentMove--;
            else if (keyboard.downArrowKey.wasPressedThisFrame)
                currentMove += 2;
            else if (keyboard.upArrowKey.wasPressedThisFrame)
                currentMove -= 2;

            currentMove = Mathf.Clamp(currentMove, 0, playerXeomon.Xeomon.Moves.Count - 1);

            dialogueBox.UpdateMoveSelection(currentMove, playerXeomon.Xeomon.Moves[currentMove]);

            if (keyboard.enterKey.wasPressedThisFrame)
            {
                dialogueBox.EnableMoveSelector(false);
                dialogueBox.EnableDialoguetext(true);
                StartCoroutine(PlayerMove());
            }
            else if (keyboard.escapeKey.wasPressedThisFrame)
            {
                dialogueBox.EnableMoveSelector(false);
                dialogueBox.EnableDialoguetext(true);
                ActionSelection();
            }
        }
    }
    

    void HandlePartySelection()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.rightArrowKey.wasPressedThisFrame)
                currentMember++;
            else if (keyboard.leftArrowKey.wasPressedThisFrame)
                currentMember--;
            else if (keyboard.downArrowKey.wasPressedThisFrame)
                currentMember += 2;
            else if (keyboard.upArrowKey.wasPressedThisFrame)
                currentMember -= 2;

            currentMember = Mathf.Clamp(currentMember, 0, playerParty.Xeomons.Count - 1);

            partyScreen.UpdateMemberSelection(currentMember);

            if (keyboard.enterKey.wasPressedThisFrame)
            {
                var selectedXeomon = playerParty.Xeomons[currentMember];
                if (selectedXeomon.HP <= 0) {
                    partyScreen.SetMessageText("You can't sent out a fainted xeomon.");
                    return;
                }
                if (selectedXeomon == playerXeomon.Xeomon)
                {
                    partyScreen.SetMessageText("This xeomon is already out in battle.");
                    return;
                }
                partyScreen.gameObject.SetActive(false);
                state = BattleState.Busy;
                StartCoroutine(SwitchXeomon(selectedXeomon));
            }
            else if (keyboard.escapeKey.wasPressedThisFrame)
            {
                partyScreen.gameObject.SetActive(false);
                ActionSelection();
            }
        }
    }

    IEnumerator SwitchXeomon(Xeomon newXeomon)
    {
        if(playerXeomon.Xeomon.HP > 0)
        {
            yield return dialogueBox.TypeDialogue("Come back " + playerXeomon.Xeomon.BaseInformation.Name + "!");
            yield return new WaitForSeconds(2f);
        }

        playerXeomon.Setup(newXeomon);
        dialogueBox.SetMoveNames(newXeomon.Moves);
        yield return dialogueBox.TypeDialogue("Go, " + newXeomon.BaseInformation.Name + "!");

        if (playerXeomon.Xeomon.BaseInformation.Element1 == XeomonElement.Fire)
        {
            weapon.sprite = weaponSprite[0];
        }
        else if (playerXeomon.Xeomon.BaseInformation.Element1 == XeomonElement.Water)
        {
            weapon.sprite = weaponSprite[1];
        }
        else if (playerXeomon.Xeomon.BaseInformation.Element1 == XeomonElement.Grass)
        {
            weapon.sprite = weaponSprite[2];
        }

        StartCoroutine(EnemyMove());
        usedLimitRelease = false;
    }

    IEnumerator ThrowXorbball()
    {
        state = BattleState.Busy;
        dialogueBox.EnableActionSelector(false);

        yield return dialogueBox.TypeDialogue("You threw a Xorbball!");

        var xorbballObj = Instantiate(xorbballSprite, playerXeomon.transform.position /*- new Vector3(0, 0.5f)*/, Quaternion.identity);

        int shakeCount = TryToCatchXeomon(enemyXeomon.Xeomon, 1, 1);

        if (shakeCount == 4)
        {
            yield return dialogueBox.TypeDialogue("You caught " + enemyXeomon.Xeomon.BaseInformation.Name + "!");

            playerParty.AddXeomon(enemyXeomon.Xeomon);
            yield return dialogueBox.TypeDialogue(enemyXeomon.Xeomon.BaseInformation.Name + " has been added to your party");

            Destroy(xorbballObj);
            BattleOver(true);
        }
        else
        {
            if (shakeCount < 2)
                yield return dialogueBox.TypeDialogue("Oh no! The " + enemyXeomon.Xeomon.BaseInformation.Name + " broke free!");
            else
                yield return dialogueBox.TypeDialogue("So close! The " + enemyXeomon.Xeomon.BaseInformation.Name + " almost got caught!");

            Destroy(xorbballObj);
            state = BattleState.RunningTurn;
            StartCoroutine(EnemyMove());
        }
        Destroy(xorbballObj);
        usedLimitRelease = false;
    }

    int TryToCatchXeomon(Xeomon xeomon, float ballBonus, float statusBonus)
    {
        float a = ((3 * xeomon.MaxHP - 2 * xeomon.HP) * xeomon.BaseInformation.CatchRate * ballBonus)/ (3 * xeomon.MaxHP);
        a *=  statusBonus;

        if (a >= 255)
            return 4;

        float b = 65536f * Mathf.Pow(a / 255f, 0.25f);

        int shakeCount = 0;
        while (shakeCount < 4) {

            if (UnityEngine.Random.Range(0f, 65536f) >= b)
                break;

            shakeCount++;
        }

        if (instantcatch.value)
            shakeCount = 4;

        return shakeCount;

    }
}
