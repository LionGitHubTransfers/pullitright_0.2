using DG.Tweening;
using UnityEngine;

public class SecondTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialContainer1;
    [SerializeField] private GameObject tutorialContainer2;
    [SerializeField] private GameObject tutorialContainer3;
    [SerializeField] private float targetY;
    [SerializeField] private RectTransform[] handsTutorialTransform;
    [SerializeField] private Hook firstHook;
    [SerializeField] private Hook secondHook;
    
    private Transform target;
    private bool isCompeteFirstPull = false;
    private LevelController levelController;
    private PullButton pullButton;
    
    private void Start()
    {
        levelController = FindObjectOfType<LevelController>();
        pullButton = FindObjectOfType<PullButton>();
        target = GameObject.FindWithTag("Target").transform;
        foreach (var hand in handsTutorialTransform)
        {
            DOTween.Sequence()
                .Append(hand.DOScale(0.8f, 0.3f).SetEase(Ease.InSine))
                .Append(hand.DOScale(1f, 0.3f).SetEase(Ease.InSine))
                .SetLoops(-1);
        }
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
