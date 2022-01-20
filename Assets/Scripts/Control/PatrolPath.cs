using UnityEngine;

namespace Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointGizmosRadius = 0.1f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextWaypointIndex(i)));
            }
        }

        public int GetNextWaypointIndex(int index)
        {
            return index + 1 < transform.childCount ? index + 1 : 0;
        }

        public Vector3 GetWaypoint(int index)
        {
            return transform.GetChild(index).position;
        }
    }
}