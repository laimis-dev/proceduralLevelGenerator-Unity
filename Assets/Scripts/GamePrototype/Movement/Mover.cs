using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using Utils;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent navMeshAgent = null;
        Health health;
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if(health !=  null){
                navMeshAgent.enabled = !health.IsDead();
            }
            
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination){
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination){
            if(!Helpers.navBaked) return;
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        public void Cancel(){
            if(!Helpers.navBaked) return;
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator(){
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
        }
    }
}
