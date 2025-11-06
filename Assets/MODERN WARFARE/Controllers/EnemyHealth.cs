using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GunController
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Enemy Settings")]
        public float maxHealth = 100f;   
        private float currentHealth;

        [Header("UI")]
        public Slider healthBar;         
        public Transform healthBarPivot;  
        private Camera mainCamera;

        [Header("Death Settings")]
        public float deathDelay = 2f;     

        private bool isDead = false;      

        private void Start()
        {
            currentHealth = maxHealth;

            
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = currentHealth;
            }

            
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void Update()
        {
            
            if (healthBarPivot != null && mainCamera != null)
            {
                Vector3 dir = healthBarPivot.position - mainCamera.transform.position;
                healthBarPivot.rotation = Quaternion.LookRotation(dir);
            }
        }

        
        public void TakeDamage(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (healthBar != null)
                healthBar.value = currentHealth;

            if (currentHealth <= 0f && !isDead)
            {
                isDead = true;
                StartCoroutine(DieWithDelay());
            }
        }

        private IEnumerator DieWithDelay()
        {
            yield return new WaitForSeconds(deathDelay);

            if (healthBar != null)
                Destroy(healthBar.gameObject);

            Destroy(gameObject);
        }
    }
}
