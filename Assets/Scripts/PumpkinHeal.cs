using UnityEngine;

public class PumpkinHeal : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f; // Ne kadar iyileştirecek
    [SerializeField] private AudioClip healSound;    // (isteğe bağlı ses efekti)
    private bool isUsed = false;                     // Tek seferlik olsun diye

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed) return; // zaten kullanıldıysa hiçbir şey yapma

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.health = Mathf.Min(playerHealth.health + healAmount, 100f);
                Debug.Log($"🎃 Pumpkin used! Player healed by {healAmount}. New health: {playerHealth.health}");

                if (healSound != null)
                    AudioSource.PlayClipAtPoint(healSound, transform.position);

                isUsed = true;
                Destroy(gameObject, 0.2f); // pumpkin yok olsun
            }
        }
    }
}
