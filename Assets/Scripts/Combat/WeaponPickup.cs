using UnityEngine;

using Control;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapon weapon;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Player")
                Pickup(collider.GetComponent<Fighter>());
        }

        private void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
            Destroy(gameObject);
        }

        public bool HandleRaycast(PlayerController player)
        {
            if (Input.GetMouseButtonDown(0))
                Pickup(player.GetComponent<Fighter>());
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}