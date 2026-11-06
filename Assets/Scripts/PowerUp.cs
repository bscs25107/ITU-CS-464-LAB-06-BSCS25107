using UnityEngine;

public enum PowerUpType { Speed, Shield }


public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType type = PowerUpType.Speed;
    [SerializeField] float multiplier = 1.6f;
    [SerializeField] float duration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Health health = other.GetComponentInParent<Health>();

        if (type == PowerUpType.Speed)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null) player.BoostSpeed(multiplier, duration);
        }
        else if (type == PowerUpType.Shield)
        {
            if (health != null)
            {
                health.shielded = true;
                Debug.Log("[PowerUp] Shield active!");
                if (GameManager.Instance != null) GameManager.Instance.UpdateUI();
            }
        }

        Destroy(gameObject);
    }
}