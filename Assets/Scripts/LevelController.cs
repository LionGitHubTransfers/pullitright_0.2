using System.Collections.Generic;
using DG.Tweening;
using Filo;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float loseCameraShakeDuration = 0.3f;
    [SerializeField] private float loseCameraShakeStrength = 0.2f;
    [SerializeField] private int loseCameraShakes = 50;
    [SerializeField] private LoadLevelManager levelManager;
    
    public StateMachine<GameState, Driver> Fsm { private set; get; }

    private Hook[] hooks;
    private List<Cable> cables;
    private Camera camera;

    private void Awake()
    {
        Fsm = new StateMachine<GameState, Driver>(this);
        cables = new List<Cable>();
        camera = Camera.main;
        Fsm.ChangeState(GameState.Init);
        Vibration.Init();
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
        Fsm.Driver.Update.Invoke();
    }

    private void Init_Update()
    {
        if (Input.GetMouseButton(0))
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

    private void Pull_Update()
    {
        Vibration.VibratePop();
        if (Input.GetMouseButtonUp(0))
        {
            pullButton.Release();
            StopPullAll();
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
        foreach (var hook in hooks)
        {
            hook.IsCanLaunch = false;
        }
        Fsm.ChangeState(GameState.Win);
        Vibration.VibrateNope();
        StopPull();
    }

    public void NextLevel()
    {
        levelManager.LoadNextLevel();
    }

    public void RestartLevel()
    {
        levelManager.LoadCurrentLevel();
    }

    public void LoseLevel()
    {
        if (Fsm.State == GameState.Win) return;
        foreach (var hook in hooks)
        {
            hook.IsCanLaunch = false;
        }
        Vibration.VibratePeek();
        camera.transform.DOShakePosition(loseCameraShakeDuration, loseCameraShakeStrength, loseCameraShakes);
        Fsm.ChangeState(GameState.Lose);
        StopPull();
    }
}
