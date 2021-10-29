using UnityEngine;

public class LevelUIController : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    [SerializeField] private GameObject startContainer;
    [SerializeField] private GameObject winContainer;
    [SerializeField] private GameObject loseContainer;

    private void Start()
    {
        levelController.Fsm.Changed += OnChangeState;
        OnChangeState(LevelController.GameState.Init);
    }

    private void OnChangeState(LevelController.GameState state)
    {
        if (state == LevelController.GameState.Init)
        {
            startContainer.SetActive(true);
            winContainer.SetActive(false);
            loseContainer.SetActive(false);
        }
        else if (state == LevelController.GameState.Pull)
        {
            startContainer.SetActive(true);
            winContainer.SetActive(false);
            loseContainer.SetActive(false);
        }
        else if (state == LevelController.GameState.Win)
        {
            startContainer.SetActive(false);
            winContainer.SetActive(true);
            loseContainer.SetActive(false);
        }
        else if (state == LevelController.GameState.Lose)
        {
            startContainer.SetActive(false);
            winContainer.SetActive(false);
            loseContainer.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        levelController.Fsm.Changed -= OnChangeState;
    }
}
