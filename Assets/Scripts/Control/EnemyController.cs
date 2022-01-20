using UnityEngine;

using Combat;
using Attributes;
using Core;
using Movement;

namespace Control
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 10f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;

        Fighter fighter;
        Health health;
        Mover mover;

        GameObject player;
        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            guardPosition = transform.position;
        }

        private void Update()
        {
            if (!health.IsDead())
            {
                if (PlayerInAttackRange() && fighter.CanAttack(player))
                {
                    timeSinceLastSawPlayer = 0;
                    fighter.Attack(player);
                }
                else if (timeSinceLastSawPlayer < suspicionTime)
                    GetComponent<ActionScheduler>().CancelCurrentAction();
                else
                    Patrol();
            }

            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private bool PlayerInAttackRange()
        {
            if (player == null)
                return false;
            return Vector3.Distance(transform.position, player.transform.position) < chaseDistance;
        }

        private void Patrol()
        {
            if (patrolPath == null)
            {
                mover.StartMoveAction(guardPosition);
                return;
            }

            if (Vector3.Distance(transform.position, patrolPath.GetWaypoint(currentWaypointIndex)) < waypointTolerance)
            {
                timeSinceArrivedAtWaypoint = 0;
                currentWaypointIndex = patrolPath.GetNextWaypointIndex(currentWaypointIndex);
            }

            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
                mover.StartMoveAction(patrolPath.GetWaypoint(currentWaypointIndex));
        }
    }
}