using DG.Tweening;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private RectTransform handTutorialTransform;

    private Sequence ropeTutorial;

    private Hook hook;

    private void Start()
    {
        hook = FindObjectOfType<Hook>();
        hook.OnLocked += cable =>
        {
            ropeTutorial.Pause();
            handTutorialTransform.anchoredPosition = new Vector2(139, -490);
            ropeTutorial = DOTween.Sequence()
                .Append(handTutorialTransform.DOScale(0.8f, 0.3f).SetEase(Ease.InSine))
                .Append(handTutorialTransform.DOScale(1f, 0.3f).SetEase(Ease.InSine).SetDelay(3f))
                .SetLoops(-1);
        };

        ropeTutorial = DOTween.Sequence()
            .Append(handTutorialTransform.DOScale(0.8f, 0.3f).SetEase(Ease.InSine))
            .Append(handTutorialTransform.DOLocalMoveY(300f, 1f).SetEase(Ease.InQuad))
            .Append(handTutorialTransform.DOScale(1f, 0.3f).SetEase(Ease.InSine))
            .Append(handTutorialTransform.DOLocalMoveY(-227f, 1f).SetEase(Ease.InQuad))
            .SetLoops(-1);
    }
}
