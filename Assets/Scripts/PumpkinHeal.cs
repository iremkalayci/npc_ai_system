using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PumpkinHeal : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Temasta verilecek can miktarı.")]
    [SerializeField] private float healAmount = 25f;

    [Tooltip("Sadece Player tag'li objeler alsın istiyorsan işaretle.")]
    [SerializeField] private bool requirePlayerTag = false;

    [Tooltip("Player tag adı (requirePlayerTag açıkken kullanılır).")]
    [SerializeField] private string playerTag = "Player";

    [Header("Görsel/İşitsel")]
    [Tooltip("Toplandığında çalınacak ses (opsiyonel).")]
    [SerializeField] private AudioClip healSound;

    [Tooltip("Toplandıktan sonra objeyi yok et.")]
    [SerializeField] private bool destroyOnUse = true;

    private bool isUsed;

    private void Reset()
    {
       
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnValidate()
    {
        
        if (healAmount < 0f) healAmount = 0f;
        if (string.IsNullOrWhiteSpace(playerTag)) playerTag = "Player";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed || healAmount <= 0f) return;

        
        if (requirePlayerTag && !other.transform.root.CompareTag(playerTag))
            return;

       
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        isUsed = true; 

        
        playerHealth.Heal(healAmount);

       
        if (healSound != null)
            AudioSource.PlayClipAtPoint(healSound, transform.position);

        
        var col = GetComponent<Collider>();     if (col) col.enabled = false;
        var rend = GetComponentInChildren<Renderer>(); if (rend) rend.enabled = false;

        if (destroyOnUse)
            Destroy(gameObject, 0.2f);
        else
            gameObject.SetActive(false);
    }
}
