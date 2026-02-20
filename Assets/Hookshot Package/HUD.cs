using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;
    
    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void ChangeScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
        scoreText.text = score.ToString();
    }
}
