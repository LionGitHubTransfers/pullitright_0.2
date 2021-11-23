using System;
using System.Collections.Generic;
using DG.Tweening;
using Filo;
using LionStudios.Suite.Analytics;
using MonsterLove.StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private int vibrationFrameInterval = 10;
    [SerializeField] private int minTargetToWin = 0;
    [SerializeField] private TMP_Text targetCounter;
    
    public StateMachine<GameState, Driver> Fsm { private set; get; }

    private Hook[] hooks;
    private WinController[] winControllers;
    private List<Cable> cables;
    private Camera camera;
    private int targetsCount;
    private int currentTargetsWin = 0;

    private void Awake()
    {
        Fsm = new StateMachine<GameState, Driver>(this);
        cables = new List<Cable>();
        camera = Camera.main;
        Fsm.ChangeState(GameState.Init);
        LionAnalytics.LevelStart(levelManager.CurrentLevelNumber, levelManager.CurrentAttempt);
        Vibration.Init();
    }

    private void OnChangeWinCount(bool isWin)
    {
        if (Fsm.State == GameState.Win || Fsm.State == GameState.Lose) return;
        currentTargetsWin += isWin ? 1 : -1;
        FillTargetCounter();
        if (currentTargetsWin < targetsCount) return;
        WinLevel();
        foreach (var winController in winControllers)
        {
            winController.PlayWinParticle();
        }
    }

    private void FillTargetCounter()
    {
        if (targetCounter != null && minTargetToWin > 0)
        {
            targetCounter.SetText($"{currentTargetsWin} / {targetsCount}");    
        }
        
    }

    private void Start()
    {
        hooks = FindObjectsOfType<Hook>();
        winControllers = FindObjectsOfType<WinController>();
        targetsCount = minTargetToWin == 0 ? GameObject.FindGameObjectsWithTag("Target").Length : minTargetToWin;
        FillTargetCounter();
        Debug.Log($"Total target objects: {targetsCount}");
        foreach (var winController in winControllers)
        {
            winController.OnChangedWin += OnChangeWinCount;
        }
        foreach (var hook in hooks)
        {
            hook.OnLocked += (cable) =>
            {
                cables.Add(cable);
                cableSolver.cables = cables.ToArray();
            };
            hook.OnUnlocked += (cable) =>
            {
                cables.Remove(cable);
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
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
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
        if (Time.frameCount % vibrationFrameInterval == 0)
        {
            Vibration.VibratePop();   
        }
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
        LionAnalytics.LevelComplete(levelManager.CurrentLevelNumber, levelManager.CurrentAttempt);
        Vibration.VibratePop();
        StopPull();
    }

    public void NextLevel()
    {
        levelManager.CurrentAttempt = 0;
        levelManager.LoadNextLevel();
    }

    public void RestartLevel()
    {
        levelManager.CurrentAttempt++;
        LionAnalytics.LevelRestart(levelManager.CurrentLevelNumber, levelManager.CurrentAttempt);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseLevel()
    {
        if (Fsm.State == GameState.Win) return;
        foreach (var hook in hooks)
        {
            hook.IsCanLaunch = false;
        }
        Vibration.VibratePop();
        camera.transform.DOShakePosition(loseCameraShakeDuration, loseCameraShakeStrength, loseCameraShakes);
        Fsm.ChangeState(GameState.Lose);
        LionAnalytics.LevelFail(levelManager.CurrentLevelNumber, levelManager.CurrentAttempt);
        StopPull();
    }

    private void OnDestroy()
    {
        foreach (var winController in winControllers)
        {
            winController.OnChangedWin -= OnChangeWinCount;
        }
    }
}
