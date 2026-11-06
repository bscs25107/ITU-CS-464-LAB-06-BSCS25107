using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            GameManager.Instance.ReachGoal();
        }
    }
}