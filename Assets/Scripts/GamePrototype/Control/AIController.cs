using UnityEngine;
using RPG.Combat;
using RPG.Core;


namespace RPG.Control{
    public class AIController : MonoBehaviour {
        [SerializeField] float chaseDistance = 5f;
        Fighter fighter;
        Health health;
        GameObject player;

        private void Start(){
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
        }
        private void Update(){
            if(health.IsDead()) return;
            print(fighter.CanAttack(player));
            if(InAttackRange() && fighter.CanAttack(player)){
                fighter.Attack(player);
                // print("chase");
            } else {
                fighter.Cancel();
            }
        }

        private bool InAttackRange(){
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance;
        }
    
    }
}
