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
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        void Update(){
            timeSinceLastAttack += Time.deltaTime;

            if(target == null) return;
            if(target.IsDead()) return;

            if(!(Vector3.Distance(transform.position, target.transform.position) < weaponRange)){
                GetComponent<Mover>().MoveTo(target.transform.position);
            } else {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour(){
            transform.LookAt(target.transform);
            if(timeSinceLastAttack > timeBetweenAttacks){
                //This will trigger Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack() {
            GetComponent<Animator>().ResetTrigger("StopAttack");
            GetComponent<Animator>().SetTrigger("Attack");
        }

        //Animation Event
        public void Hit(){
            if(target == null) return;
            target.TakeDamage(weaponDamage);
        }

        public void Attack(GameObject combatTarget){
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget){
            if(combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Cancel(){
            target = null;
            StopAttack();
        }

        private void StopAttack(){
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }


    }
}