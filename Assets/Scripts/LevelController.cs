using System.Collections.Generic;
using Filo;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private GameObject disablePullButtonObject;
    [SerializeField] private GameObject pullButtonObject;
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
                disablePullButtonObject.SetActive(false);
                pullButtonObject.SetActive(true);
                isCanPull = true;
                cables.Add(cable);
                cableSolver.cables = cables.ToArray();
            };
        }

        disablePullButtonObject.SetActive(true);
        pullButtonObject.SetActive(false);
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

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseLevel()
    {
        if (Fsm.State == GameState.Win) return;
        Fsm.ChangeState(GameState.Lose);
        StopPull();
    }
}
