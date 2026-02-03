using UnityEngine;

public class GummyLevel : MonoBehaviour
{
    public enum Weight { Light = 1, Medium = 2, Heavy = 3 }

    public Weight weight = Weight.Light;

    public int WeightValue => (int)weight;
}
