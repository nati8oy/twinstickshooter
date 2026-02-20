using UnityEngine;
using UnityEngine.Events;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private UnityEvent onExitReached;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[LevelExit] OnTriggerEnter: {other.gameObject.name} tag={other.gameObject.tag}");
        if (!other.CompareTag("Player")) return;
        if (triggered) return;
        triggered = true;
        onExitReached.Invoke();
        LevelStats stats = FindObjectOfType<LevelStats>();
        LevelFlowManager.Instance.ShowLevelComplete(stats);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
