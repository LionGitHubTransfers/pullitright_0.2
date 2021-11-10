using DG.Tweening;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject firstTutorialController;
    [SerializeField] private GameObject secondTutorialController;
    [SerializeField] private RectTransform handTutorialTransform;
    [SerializeField] private RectTransform secondHandTutorialTransform;

    private Sequence ropeTutorial;

    private Hook hook;

    private void Start()
    {
        hook = FindObjectOfType<Hook>();
        DOTween.Sequence()
            .Append(secondHandTutorialTransform.DOScale(0.8f, 0.3f).SetEase(Ease.InSine))
            .Append(secondHandTutorialTransform.DOScale(1f, 0.3f).SetEase(Ease.InSine))
            .SetLoops(-1);
        hook.OnLocked += cable =>
        {
            ropeTutorial.Pause();
            handTutorialTransform.anchoredPosition = new Vector2(139, -490);
            firstTutorialController.SetActive(false);
            secondTutorialController.SetActive(true);
            handTutorialTransform.gameObject.SetActive(false);
        };

        ropeTutorial = DOTween.Sequence()
            .Append(handTutorialTransform.DOScale(0.8f, 0.3f).SetEase(Ease.InSine))
            .Append(handTutorialTransform.DOLocalMoveY(230f, 1f).SetEase(Ease.InQuad))
            .Append(handTutorialTransform.DOScale(1f, 0.3f).SetEase(Ease.InSine))
            .Append(handTutorialTransform.DOLocalMoveY(-227f, 1f).SetEase(Ease.InQuad))
            .SetLoops(-1);

        FindObjectOfType<LevelController>().Fsm.Changed += state =>
        {
            if (state == LevelController.GameState.Lose || state == LevelController.GameState.Win)
            {
                secondTutorialController.SetActive(false);
            }
        };
    }
}
