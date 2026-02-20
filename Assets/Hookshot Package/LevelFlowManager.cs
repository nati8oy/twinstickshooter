using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlowManager : PersistentSingleton<LevelFlowManager>
{
    [SerializeField] private LevelSequence levelSequence;
    [SerializeField] private bool resetHealthOnLoad = true;

    private int currentLevelIndex;

    protected override void Awake()
    {
        base.Awake();
        // base.Awake() may destroy this if a duplicate exists
        if (this == null || !gameObject) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sync index when entering a level directly from the editor
        if (levelSequence != null)
        {
            int index = levelSequence.levels.IndexOf(scene.name);
            if (index >= 0)
                currentLevelIndex = index;
        }

        PositionPlayerAtEntry();
    }

    private void PositionPlayerAtEntry()
    {
        LevelEntry entry = FindObjectOfType<LevelEntry>();
        if (entry == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetPositionAndRotation(entry.transform.position, entry.transform.rotation);

        if (cc != null) cc.enabled = true;

        if (resetHealthOnLoad)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.ResetHealth();
        }
    }

    public void ShowLevelComplete(LevelStats stats)
    {
        Debug.Log("[LevelFlowManager] ShowLevelComplete called");
        LevelCompleteUI ui = FindObjectOfType<LevelCompleteUI>(true);
        if (ui == null)
        {
            Debug.LogError("[LevelFlowManager] No LevelCompleteUI found in scene.");
            return;
        }
        Debug.Log($"[LevelFlowManager] Found LevelCompleteUI on: {ui.gameObject.name}");
        ui.Populate(stats);
        Time.timeScale = 0f;
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        currentLevelIndex++;

        if (levelSequence == null)
        {
            Debug.LogWarning("LevelFlowManager: No LevelSequence assigned.");
            return;
        }

        if (currentLevelIndex < levelSequence.levels.Count)
        {
            SceneManager.LoadScene(levelSequence.levels[currentLevelIndex]);
        }
        else if (!string.IsNullOrEmpty(levelSequence.allLevelsCompleteScene))
        {
            SceneManager.LoadScene(levelSequence.allLevelsCompleteScene);
        }
        else
        {
            Debug.Log("LevelFlowManager: All levels complete. No end scene assigned.");
        }
    }
}
