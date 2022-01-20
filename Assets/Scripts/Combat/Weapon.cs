using UnityEngine;

using Attributes;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] Texture2D icon;
        [SerializeField] GameObject prefab;
        [SerializeField] AnimatorOverrideController overrideAnimator;
        [SerializeField] float damage = 10f;
        [SerializeField] float range = 2f;
        [SerializeField] bool rightHanded = true;
        [SerializeField] Projectile projectile;

        const string objectName = "Weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (prefab != null)
            {
                GameObject weapon = Instantiate(prefab, GetHand(rightHand, leftHand));
                weapon.name = objectName;
            }

            if (overrideAnimator != null)
                animator.runtimeAnimatorController = overrideAnimator;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(objectName);
            if (oldWeapon == null)
                oldWeapon = leftHand.Find(objectName);

            if (oldWeapon != null)
                Destroy(oldWeapon.gameObject);
        }

        public Texture2D GetIcon()
        {
            return icon;
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetRange()
        {
            return range;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health targetHealth)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHand(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(targetHealth);
            projectileInstance.SetDamage(damage);
        }

        private Transform GetHand(Transform rightHand, Transform leftHand)
        {
            return rightHanded ? rightHand : leftHand;
        }
    }
}