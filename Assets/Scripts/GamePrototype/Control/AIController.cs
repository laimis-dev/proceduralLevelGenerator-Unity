using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;


namespace RPG.Control{
    public class AIController : MonoBehaviour {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        Fighter fighter;
        Health health;
        Mover mover;
        GameObject player;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;

        private void Start(){
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");

            guardPosition = transform.position;
        }
        private void Update(){
            if(health.IsDead()) return;
            if(InAttackRange() && fighter.CanAttack(player)){
                timeSinceLastSawPlayer = 0;
                fighter.Attack(player);
                // print("chase");
            } else if (timeSinceLastSawPlayer < suspicionTime){
                GetComponent<ActionScheduler>().CancelCurrentAction();
            } else {
                mover.StartMoveAction(guardPosition);
            }

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private bool InAttackRange(){
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance;
        }
    
    }
}
