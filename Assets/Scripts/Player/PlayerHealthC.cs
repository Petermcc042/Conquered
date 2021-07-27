using UnityEngine;

namespace GlassyGames.Conquered
{
    public class PlayerHealthC : MonoBehaviour
    {
        [SerializeField]
        readonly float maxHealth = 100f;
        float currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float _damage)
        {
            currentHealth -= _damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            print(name + "was destroyed");
            Destroy(gameObject);
        }
    }
}
