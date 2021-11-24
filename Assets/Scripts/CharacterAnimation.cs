using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] private Animator[] animators;

    private void Start()
    {
        FindObjectOfType<LevelController>().Fsm.Changed += state =>
        {
            if (state == LevelController.GameState.Lose)
            {
                foreach (var animator in animators)
                {
                    animator.SetTrigger("lose");
                }
            } 
            else if (state == LevelController.GameState.Win)
            {
                foreach (var animator in animators)
                {
                    animator.SetTrigger("win");
                }
            }
        };
    }
}
