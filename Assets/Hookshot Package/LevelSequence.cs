using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gummy Roundup/Level Sequence")]
public class LevelSequence : ScriptableObject
{
    public List<string> levels = new List<string>();
    public string allLevelsCompleteScene;
}
