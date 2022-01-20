using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Movement;
using Core;
using Attributes;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform leftHand;
        [SerializeField] Weapon defaultWeapon;

        Health target;
        Mover mover;
        Weapon currentWeapon;

        float timeSinceLastAttack = 0f;

        private void Awake()
        {
            mover = GetComponent<Mover>();
        }

        private void Start()
        {
            EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target != null)
            {
                if (target.IsDead())
                    Cancel();
                else if (Vector3.Distance(transform.position, target.transform.position) > currentWeapon.GetRange())
                    mover.MoveTo(target.transform.position);
                else
                {
                    mover.Cancel();
                    AttackBehaviour();
                }
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            currentWeapon.Spawn(rightHand, leftHand, GetComponent<Animator>());
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        public void Hit()
        {
            if (target != null)
                target.TakeDamage(currentWeapon.GetDamage());
        }

        private void Shoot()
        {
            if (target != null && currentWeapon.HasProjectile())
                currentWeapon.LaunchProjectile(rightHand, leftHand, target);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            return combatTarget != null && !combatTarget.GetComponent<Health>().IsDead();
        }

        public void Cancel()
        {
            TriggerStopAttack();
            target = null;
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private void TriggerStopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}