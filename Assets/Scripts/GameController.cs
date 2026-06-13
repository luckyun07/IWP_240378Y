using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattlingSystem battlingSystem;
    [SerializeField] Camera worldCamera;

    GameState state;

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battlingSystem.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battlingSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        
        var playerParty = playerController.GetComponent<XeomonParty>();
        var wildXeomon = FindAnyObjectByType<MapArea>().GetRandomWildXeomon();

        var wildXeomonCopy = new Xeomon(wildXeomon.BaseInformation, wildXeomon.Level);

        battlingSystem.StartBattle(playerParty, wildXeomonCopy);
    }
    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battlingSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate(); 
        }
        else if (state == GameState.Battle)
        {
            battlingSystem.HandleUpdate();
        }
    }
}
