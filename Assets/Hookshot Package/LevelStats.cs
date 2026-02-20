using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelStats : Singleton<LevelStats>
{
    private int[] totalByColour;
    private int[] collectedByColour;

    private void Start()
    {
        int colourCount = Enum.GetValues(typeof(GummyLevel.GummyColour)).Length;
        totalByColour = new int[colourCount];
        collectedByColour = new int[colourCount];

        GummyBehaviour[] gummies = FindObjectsOfType<GummyBehaviour>();
        foreach (GummyBehaviour g in gummies)
        {
            totalByColour[(int)g.colour]++;
        }
    }

    public void RecordCollection(GummyLevel.GummyColour colour)
    {
        if (collectedByColour == null) return;
        collectedByColour[(int)colour]++;
    }

    public int GetTotal(GummyLevel.GummyColour colour) => totalByColour != null ? totalByColour[(int)colour] : 0;
    public int GetCollected(GummyLevel.GummyColour colour) => collectedByColour != null ? collectedByColour[(int)colour] : 0;

    public GummyLevel.GummyColour[] AllColours
    {
        get
        {
            var all = (GummyLevel.GummyColour[])Enum.GetValues(typeof(GummyLevel.GummyColour));
            var result = new List<GummyLevel.GummyColour>();
            foreach (var c in all)
            {
                if (totalByColour != null && totalByColour[(int)c] > 0)
                    result.Add(c);
            }
            return result.ToArray();
        }
    }
}
