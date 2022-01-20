using UnityEngine;

using Attributes;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 5f;
        [SerializeField] float lifeTime = 10f;

        Health target;
        Rigidbody body;
        float damage;
        float existTime = 0f;
        Vector3 targetPosition;

        private void Start()
        {
            body = GetComponent<Rigidbody>();

            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            if (targetCollider == null)
                targetPosition = target.transform.position;
            else
                targetPosition = target.transform.position + Vector3.up * targetCollider.height / 2;

            SetMovement();
        }

        private void Update()
        {
            existTime += Time.deltaTime;
            if (existTime >= lifeTime)
                Destroy(gameObject);
            else
                transform.rotation = Quaternion.LookRotation(body.velocity);
        }

        private void SetMovement()
        {
            float distance = Vector3.Distance(targetPosition, transform.position);

            Vector3 verticalVelocity = -Physics.gravity * distance / (2 * speed);
            Vector3 horizontalVelocity = speed * (targetPosition - transform.position) / distance;

            body.velocity = horizontalVelocity + verticalVelocity;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.GetComponent<Health>() == target)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        public void SetTarget(Health targetHealth)
        {
            target = targetHealth;
        }

        public void SetDamage(float weaponDamage)
        {
            damage = weaponDamage;
        }
    }
}