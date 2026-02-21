using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI[] colourLabels;
    [SerializeField] private TextMeshProUGUI[] colourCountTexts;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;

    public void Populate(LevelStats stats)
    {
        Debug.Log($"[LevelCompleteUI] Populate called. panel={panel}, replayButton={replayButton}, continueButton={continueButton}");
        if (stats != null)
        {
            GummyLevel.GummyColour[] colours = stats.AllColours;
            for (int i = 0; i < colourLabels.Length && i < colours.Length; i++)
            {
                colourLabels[i].text = colours[i].ToString();
                colourCountTexts[i].text = $"{stats.GetCollected(colours[i])} / {stats.GetTotal(colours[i])}";
            }
        }

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(LevelFlowManager.Instance.ReplayLevel);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(LevelFlowManager.Instance.LoadNextLevel);

        Debug.Log($"[LevelCompleteUI] Setting panel active");
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
