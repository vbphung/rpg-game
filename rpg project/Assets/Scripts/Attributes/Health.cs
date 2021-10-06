using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

using Core;

namespace Attributes
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float defaultHealthPoint = 100f;
        [SerializeField] Slider healthBar;

        bool isDead = false;
        float healthPoint;

        private void Awake()
        {
            healthPoint = defaultHealthPoint;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(float damage)
        {
            healthPoint = Mathf.Max(healthPoint - damage, 0);
            if (healthPoint == 0)
                Die();
            else
                healthBar.value = (healthPoint / defaultHealthPoint) * 100;
        }

        private void Die()
        {
            if (isDead)
                return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<NavMeshAgent>().enabled = false;
        }

        public float GetPercentage()
        {
            return (healthPoint / defaultHealthPoint) * 100;
        }
    }
}