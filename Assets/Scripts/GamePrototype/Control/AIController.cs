using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;


namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        Fighter fighter;
        Health health;
        Mover mover;
        GameObject player = null;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float guardPosTimer = 0f;

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
        }
        private void Update()
        {
            //this fix for guard position is very bad
            if (guardPosTimer < 5f)
            {
                guardPosition = transform.parent.gameObject.transform.position;
                guardPosTimer += Time.deltaTime;
            }

            if (player == null)
            {
                player = GameObject.FindWithTag("Player");
            }
            if (health.IsDead()) return;
            if (InAttackRange() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                fighter.Attack(player);
                // print("chase");
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else
            {
                mover.StartMoveAction(guardPosition);
            }

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private bool InAttackRange()
        {
            if (player == null) return false;
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance;
        }

    }
}
