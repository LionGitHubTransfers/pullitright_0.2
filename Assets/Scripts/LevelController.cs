using System;
using MonsterLove.StateMachine;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public enum GameState
    {
        Init,
        Pull,
        Win,
        Lose
    }
    
    public StateMachine<GameState> Fsm { private set; get; }

    private Hook[] hooks;

    private void Awake()
    {
        Fsm = new StateMachine<GameState>(this);
        Fsm.ChangeState(GameState.Init);
    }

    private void Start()
    {
        hooks = FindObjectsOfType<Hook>();   
    }

    public void Pull()
    {
        Fsm.ChangeState(GameState.Pull);
    }

    private void StopPullAll()
    {
        foreach (var hook in hooks)
        {
            hook.StopPull();
        }
    }

    private void Pull_Enter()
    {
        foreach (var hook in hooks)
        {
            hook.SetNeedToPull();
        }
    }

    private void Pull_Exit()
    {
        StopPullAll();
    }

    public void WinLevel()
    {
        Fsm.ChangeState(GameState.Win);
    }

    public void LoseLevel()
    {
        Fsm.ChangeState(GameState.Lose);
    }
}
