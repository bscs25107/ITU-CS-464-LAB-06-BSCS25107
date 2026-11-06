using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 6f;
    [SerializeField] float restTime = 2f, warnTime = 0.7f, strikeTime = 0.4f;
    [SerializeField] GameObject warning;
    [SerializeField] Renderer warningRenderer;
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float flashInterval = 0.1f;

    Color originalColor = Color.white;
    Collider zone;

    void Awake()
    {
        zone = GetComponent<Collider>();
        if (warning != null && warningRenderer == null)
            warningRenderer = warning.GetComponent<Renderer>();

        if (warningRenderer != null)
            originalColor = warningRenderer.material.color;
    }

    IEnumerator Start()
    {
        while (true)
        {
            // 1. Rest Phase (Hazard Off)
            if (zone != null) zone.enabled = false;
            if (warning != null) warning.SetActive(false);

            if (restTime > 0f)
                yield return new WaitForSeconds(restTime);

            // 2. Warning Phase (Flashing Red)
            if (warning != null) warning.SetActive(true);
            yield return StartCoroutine(FlashWarningRoutine());
            if (warning != null) warning.SetActive(false);

            // 3. Strike Phase (Hazard Active)
            if (zone != null) zone.enabled = true;

            // Instantly process anything standing inside the trigger at activation time
            CheckOverlap();

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
                warning.SetActive(!warning.activeSelf);
            }

            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        if (warningRenderer != null)
            warningRenderer.material.color = originalColor;
    }

    private void OnTriggerEnter(Collider other) => ProcessDamage(other);
    private void OnTriggerStay(Collider other) => ProcessDamage(other);

    void CheckOverlap()
    {
        if (zone == null) return;

        // Manual overlap check to hit stationary players the instant the hazard turns on
        Collider[] hits = Physics.OverlapBox(zone.bounds.center, zone.bounds.extents, transform.rotation);
        foreach (var hit in hits)
        {
            if (hit != zone) ProcessDamage(hit);
        }
    }

    void ProcessDamage(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Search for Health component on object or its parent
        Health health = other.GetComponentInParent<Health>();
        if (health == null || !health.TakeDamage(damage)) return;

        // Apply knockback
        Rigidbody body = other.attachedRigidbody ?? other.GetComponentInParent<Rigidbody>();
        if (body != null)
        {
            body.WakeUp(); // Ensure rigid body wakes up from sleep
            Vector3 away = body.position - transform.position;
            away.y = 0f;
            body.AddForce(away.normalized * knockback, ForceMode.Impulse);
        }
    }
}