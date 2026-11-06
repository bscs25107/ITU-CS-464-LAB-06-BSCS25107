using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] float restTime = 2f, warnTime = 0.7f, strikeTime = 0.4f;
    [SerializeField] GameObject warning;
    [SerializeField] Renderer warningRenderer; // Target renderer for flashing
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float flashInterval = 0.1f; // Speed of the red flash

    Color originalColor = Color.white;

    void Awake()
    {
        if (warning != null && warningRenderer == null)
            warningRenderer = warning.GetComponent<Renderer>();

        if (warningRenderer != null)
            originalColor = warningRenderer.material.color;
    }

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;
        Collider zone = GetComponent<Collider>();

        while (true)
        {
            // Rest Phase
            zone.enabled = false;
            if (warning != null) warning.SetActive(false);
            yield return new WaitForSeconds(restTime);

            // Warning Phase (Flashing Red)
            if (warning != null) warning.SetActive(true);
            yield return StartCoroutine(FlashWarningRoutine());
            if (warning != null) warning.SetActive(false);

            // Strike Phase
            zone.enabled = true;
            yield return new WaitForSeconds(strikeTime);
        }
    }

    IEnumerator FlashWarningRoutine()
    {
        float timer = 0f;
        bool isRed = false;

        while (timer < warnTime)
        {
            if (warningRenderer != null)
            {
                warningRenderer.material.color = isRed ? originalColor : flashColor;
                isRed = !isRed;
            }
            else if (warning != null)
            {
                // Fallback: Toggle active state if no Renderer is found
                warning.SetActive(!warning.activeSelf);
            }

            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        // Restore original color when warning finishes
        if (warningRenderer != null)
            warningRenderer.material.color = originalColor;
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("hit " + other.name);
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage)) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        Vector3 away = body.position - transform.position; away.y = 0f;
        body.AddForce(away.normalized * knockback, ForceMode.Impulse);
    }
}