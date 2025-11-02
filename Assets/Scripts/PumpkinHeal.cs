using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PumpkinHeal : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f;   // İyileştirme miktarı
    [SerializeField] private AudioClip healSound;      // (opsiyonel) ses
    [SerializeField] private bool destroyOnUse = true; // Alındıktan sonra yok et
    private bool isUsed;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true; // Trigger olmalı
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed) return;

        // Çocuk collider'a çarpsa bile PlayerHealth'i üstte bul
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        // (İsteğe bağlı) kök objenin Player tag'ı kontrolü:
        // if (!other.transform.root.CompareTag("Player")) return;

        isUsed = true; // tekrar tetiklenmesin
        playerHealth.Heal(healAmount);

        if (healSound != null)
            AudioSource.PlayClipAtPoint(healSound, transform.position);

        // Görsel/Collider'ı anında kapat (yok olmuş gibi hissettirmek için)
        var c = GetComponent<Collider>(); if (c) c.enabled = false;
        var r = GetComponentInChildren<Renderer>(); if (r) r.enabled = false;

        if (destroyOnUse)
            Destroy(gameObject, 0.2f);
    }
}
