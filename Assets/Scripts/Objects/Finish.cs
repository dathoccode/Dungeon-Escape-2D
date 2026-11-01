using UnityEngine;

public class Finish : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject confetti;
    
    [Header("Manager Refs")]
    [SerializeField] private CheckpointManager checkpointManager;

    [Header("Level Completion")]
    [SerializeField] private HUDMenu hudMenu;

    private bool levelCompleted = false;
    private static readonly int onCompleteAnim = Animator.StringToHash("OnComplete");

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (checkpointManager == null)
            checkpointManager = FindFirstObjectByType<CheckpointManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            if (collision.gameObject.GetComponent<ItemCollector>().GetKeys() < 3)
            {
                hudMenu.ShowMessage("You need to collect all 3 keys to finish the level!");
                return;
            }
            animator.SetTrigger(onCompleteAnim);
            AudioManager.Instance.PlaySound(AudioType.levelFinish);
            confetti.SetActive(true);
            
            checkpointManager.ClearCheckpoints();
            
            levelCompleted = true;
            Invoke(nameof(CompleteLevel), 2.15f);
        }
    }

    private void CompleteLevel()
    {
        LevelManager.Instance.MarkLevelComplete();
        LevelManager.Instance.GoToNextLevel();
    }
}
