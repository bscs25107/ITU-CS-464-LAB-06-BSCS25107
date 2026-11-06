using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;
    public bool shielded;

    int current;
    float invulnerableUntil;

    public int Current => current;
    public int Max => max;

    void Awake() => current = max;

    public bool TakeDamage(int damage)
    {
        // 1. Check if invulnerability window (iFrames / spawn protection) is active
        if (Time.time < invulnerableUntil)
        {
            Debug.Log($"[Health] Ignored damage: Invulnerable for {invulnerableUntil - Time.time:F2}s more.");
            return false;
        }

        // 2. Shield absorbs the attack
        if (shielded)
        {
            shielded = false;
            invulnerableUntil = Time.time + iFrames;
            Debug.Log("[Health] Shield broke and absorbed the hit!");

            if (GameManager.Instance != null)
                GameManager.Instance.UpdateUI();

            return true;
        }

        // 3. Take Damage
        current -= damage;
        invulnerableUntil = Time.time + iFrames;
        Debug.Log($"[Health] Took {damage} damage. Health remaining: {current}/{max}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI();
        }

        // 4. Handle Death
        if (current <= 0)
        {
            current = 0;
            Debug.Log("[Health] Health reached 0! PlayerDied() called.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }

        return true;
    }

    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
        Debug.Log($"[Health] Health restored to max ({max}). Protection active for {protection}s.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI();
        }
    }
}