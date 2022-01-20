using UnityEngine;

using Attributes;
using Control;

namespace Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController player)
        {
            Fighter fighter = player.GetComponent<Fighter>();
            if (fighter.CanAttack(gameObject))
            {
                if (Input.GetMouseButtonDown(0))
                    fighter.Attack(gameObject);
                return true;
            }
            return false;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Attack;
        }
    }
}