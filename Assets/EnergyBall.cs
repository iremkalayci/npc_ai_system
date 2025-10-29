using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    // EnemyAI'dan atanacak hasar miktarı
    [HideInInspector] public int damage; 

    private void OnTriggerEnter(Collider other)
    {
        // Player'ı kontrol et (Player'ın GameObject'inde "Player" Tag'i olmalı)
        if (other.CompareTag("Player"))
        {
            // BURADA PLAYER'INIZIN CANINI DÜŞÜREN METODU ÇAĞIRACAKSINIZ!
            // Örnek: other.GetComponent<PlayerHealth>().TakeDamage(damage); 
            
            // Eğer Player'da Can Scripti yoksa, şimdilik sadece Debug Log'u kullan.
            Debug.Log("Enerji Topu Player'a çarptı! Hasar: " + damage);
        }
        
        // Enerji topu çarptığı an yok olsun
        Destroy(gameObject);
    }
}