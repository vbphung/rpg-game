using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Movement;
using Combat;
using Attributes;

namespace Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CursorObject[] cursorObjects;

        Mover mover;
        Fighter fighter;
        Health health;

        [System.Serializable]
        struct CursorObject
        {
            public CursorType type;
            public Texture2D texture;
        }

        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            if (InteractWithUI())
                return;

            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithIRaycastable())
                return;

            if (InteractWithMovement())
                return;

            SetCursor(CursorType.None);
        }

        private void SetCursor(CursorType type)
        {
            Cursor.SetCursor(GetCursor(type).texture, new Vector2(0, 0), CursorMode.Auto);
        }

        private CursorObject GetCursor(CursorType type)
        {
            foreach (CursorObject cursor in cursorObjects)
                if (cursor.type == type)
                    return cursor;
            return GetCursor(CursorType.None);
        }

        private bool InteractWithIRaycastable()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
                foreach (IRaycastable raycastable in hit.transform.GetComponents<IRaycastable>())
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
            return false;
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                if (Input.GetMouseButton(0))
                    mover.StartMoveAction(hit.point);
                SetCursor(CursorType.Move);
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}