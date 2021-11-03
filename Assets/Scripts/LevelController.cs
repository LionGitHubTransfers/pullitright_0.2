using System;
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

    [SerializeField] private CableSolver cableSolver;
    [SerializeField] private PullButton pullButton;
    
    public StateMachine<GameState> Fsm { private set; get; }

    private Hook[] hooks;
    private List<Cable> cables;
    private Camera camera;

    private void Awake()
    {
        Fsm = new StateMachine<GameState>(this);
        cables = new List<Cable>();
        camera = Camera.main;
        Fsm.ChangeState(GameState.Init);
    }

    private void Start()
    {
        hooks = FindObjectsOfType<Hook>();
        foreach (var hook in hooks)
        {
            hook.OnLocked += (cable) =>
            {
                cables.Add(cable);
                cableSolver.cables = cables.ToArray();
            };
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Fsm.State == GameState.Init)
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.collider.gameObject == pullButton.gameObject)
                    {
                        pullButton.Push();
                        Pull();
                    }
                }    
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Fsm.State == GameState.Pull)
            {
                pullButton.Release();
                StopPullAll();    
            }
        }
    }

    public void Pull()
    {
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
