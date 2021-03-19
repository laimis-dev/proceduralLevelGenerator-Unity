using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform handTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        Weapon currentWeapon = null;

        void Start(){
            EquipWeapon(defaultWeapon);
        }

        void Update(){
            timeSinceLastAttack += Time.deltaTime;

            if(target == null) return;
            if(target.IsDead()) return;

            if(!(Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange())){
                GetComponent<Mover>().MoveTo(target.transform.position);
            } else {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(Weapon weapon){
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            currentWeapon.Spawn(handTransform, animator); 
            
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
            target.TakeDamage(currentWeapon.GetDamage());
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