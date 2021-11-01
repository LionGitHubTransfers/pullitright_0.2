using System.Collections.Generic;
using Filo;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public enum GameState
    {
        Init,
        Pull,
        Win,
        Lose
    }

    [SerializeField] private Button pullButton;
    [SerializeField] private CableSolver cableSolver;
    public StateMachine<GameState> Fsm { private set; get; }

    private Hook[] hooks;
    private bool isCanPull = false;
    private List<Cable> cables;

    private void Awake()
    {
        Fsm = new StateMachine<GameState>(this);
        cables = new List<Cable>();
        Fsm.ChangeState(GameState.Init);
    }

    private void Start()
    {
        hooks = FindObjectsOfType<Hook>();
        foreach (var hook in hooks)
        {
            hook.OnLocked += (cable) =>
            {
                pullButton.interactable = true;
                isCanPull = true;
                cables.Add(cable);
                cableSolver.cables = cables.ToArray();
            };
        }

        pullButton.interactable = false;
        isCanPull = false;
    }

    public void Pull()
    {
        if (!isCanPull) return;
        Fsm.ChangeState(GameState.Pull);
        foreach (var hook in hooks)
        {
            hook.SetNeedToPull();
        }
    }

    private void StopPull()
    {
        foreach (var hook in hooks)
        {
            hook.StopPull();
        }
    }

    public void StopPullAll()
    {
        Fsm.ChangeState(GameState.Init);
        StopPull();
    }

    public void WinLevel()
    {
        if (Fsm.State == GameState.Lose) return;
        Fsm.ChangeState(GameState.Win);
        StopPull();
    }

    public void LoseLevel()
    {
        if (Fsm.State == GameState.Win) return;
        Fsm.ChangeState(GameState.Lose);
        StopPull();
    }
}
