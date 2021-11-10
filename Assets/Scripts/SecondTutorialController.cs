using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialContainer1;
    [SerializeField] private GameObject tutorialContainer2;
    [SerializeField] private GameObject tutorialContainer3;
    [SerializeField] private float targetY;

    private Hook firstHook;
    private Hook secondHook;
    private Transform target;
    private bool isCompeteFirstPull = false;
    private LevelController levelController;
    private PullButton pullButton;
    
    private void Start()
    {
        var hooks = FindObjectsOfType<Hook>();
        levelController = FindObjectOfType<LevelController>();
        pullButton = FindObjectOfType<PullButton>();
        target = GameObject.FindWithTag("Target").transform;
        firstHook = hooks[0];
        secondHook = hooks[1];
        firstHook.OnLocked += cable =>
        {
            tutorialContainer1.SetActive(false);
            tutorialContainer2.SetActive(true);
        };
        secondHook.OnLocked += cable =>
        {
            tutorialContainer3.SetActive(false);
            tutorialContainer2.SetActive(true);
        };
        levelController.Fsm.Changed += state =>
        {
            if (state == LevelController.GameState.Win || state == LevelController.GameState.Lose)
            {
                tutorialContainer1.SetActive(false);
                tutorialContainer2.SetActive(false);
                tutorialContainer3.SetActive(false);
            }
        };
    }

    private void Update()
    {
        if (isCompeteFirstPull) return;
        if (Mathf.Abs(target.rotation.eulerAngles.y - targetY) < 1f)
        {
            isCompeteFirstPull = true;
            tutorialContainer2.SetActive(false);
            tutorialContainer3.SetActive(true);
            levelController.StopPullAll();
            pullButton.Release();
        }
    }
}
