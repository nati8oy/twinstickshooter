using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string sceneName;
    [TextArea] public string objective;
}

[CreateAssetMenu(menuName = "Gummy Roundup/Level Sequence")]
public class LevelSequence : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
    public string allLevelsCompleteScene;
}
