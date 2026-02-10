using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class GummySpawner : MonoBehaviour
{
    [SerializeField] private GameObject gummyPrefab;
    [SerializeField] private Transform spawnPoint;

    private InputAction spawnAction;

    private void Start()
    {
        spawnAction = new InputAction(binding: "<Keyboard>/1");
        spawnAction.performed += _ => SpawnGummy();
        spawnAction.Enable();
    }

    private void OnDestroy()
    {
        spawnAction?.Disable();
    }

    private void SpawnGummy()
    {
        if (gummyPrefab == null || spawnPoint == null) return;

        GameObject gummy = Instantiate(gummyPrefab, spawnPoint.position, spawnPoint.rotation);

        NavMeshAgent agent = gummy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(spawnPoint.position);
        }
    }
}
