using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 4f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;
        Transform target;
        float timeSinceLastAttack = 0;
        void Update(){
            timeSinceLastAttack += Time.deltaTime;

            if(target == null) return;
            
            if(!(Vector3.Distance(transform.position, target.position) < weaponRange)){
                GetComponent<Mover>().MoveTo(target.position);
            } else {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour(){
            if(timeSinceLastAttack > timeBetweenAttacks){
                //This will trigger Hit() event
                GetComponent<Animator>().SetTrigger("Attack");
                timeSinceLastAttack = 0;
            }
        }

        //Animation Event
        public void Hit(){
            Health healthComponent = target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }
        public void Attack(CombatTarget combatTarget){
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform;
            print(combatTarget);
        }

        public void Cancel(){
            target = null;
        }


    }
}