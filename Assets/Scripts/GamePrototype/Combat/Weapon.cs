using UnityEngine;

namespace RPG.Combat{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make new weapon", order = 0)]
    public class Weapon : ScriptableObject {
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] float weaponRange = 4f;
        [SerializeField] float weaponDamage = 5f;

        public void Spawn(Transform handTransform, Animator animator){
            if(weaponPrefab != null){
                Instantiate(weaponPrefab, handTransform);
            }
            
            if(weaponOverride != null){
                animator.runtimeAnimatorController = weaponOverride;
            }
        }

        public float GetDamage(){
            return weaponDamage;
        }

        public float GetRange(){
            return weaponRange;
        }
    }
}

