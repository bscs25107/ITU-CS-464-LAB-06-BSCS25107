using UnityEngine;

// Lecture 06 collectible. Needs: collider with Is Trigger ticked.
// Spin: set the coin prefab's Rotation Z = 90 so the disc stands on its edge,
// then it spins around the WORLD up axis (a flat disc spinning about its own
// up axis looks like it isn't moving at all).
public class coin : MonoBehaviour
{
    [SerializeField] int value = 1;
    [SerializeField] float spin = 120f;

    void Update() => transform.Rotate(0f, spin * Time.deltaTime, 0f, Space.World);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;   // Player object must have tag "Player"
        GameManager.Instance.AddScore(value);
        Destroy(gameObject);
    }
}
