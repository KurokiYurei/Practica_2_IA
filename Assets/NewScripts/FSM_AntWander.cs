using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    public class FSM_AntWander : FiniteStateMachine
    {
        public enum State { INITIAL, WANDERING, EXITING }

        public State currentState = State.INITIAL;

        private AntBlackboard blackboard;
        private PathFeeder pathFeeder;
        private PathFollowing pathFollowing;
        private GameObject target;
        private GameObject objectCarried;

        void Start()
        {
            // get what we need...
            blackboard = GetComponent<AntBlackboard>();
            pathFeeder = GetComponent<PathFeeder>();
            pathFollowing = GetComponent<PathFollowing>();
            pathFeeder.enabled = false;
            pathFollowing.enabled = false;

            objectCarried = gameObject.transform.GetChild(0).gameObject;
        }

        public override void Exit()
        {
            pathFeeder.enabled = false;
            pathFollowing.enabled = false;
            base.Exit();
        }

        public override void ReEnter()
        {
            ChangeState(State.INITIAL);
            base.ReEnter();
        }

        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.WANDERING);
                    break;
                case State.WANDERING:
                    if (SensingUtils.DistanceToTarget(gameObject, target) <= blackboard.pointReachedRadius)
                    {
                        if (objectCarried.tag == "TRANSPORTING_SEED")
                        {
                            objectCarried.tag = "SEED_DROPPED";
                        }
                        else
                        {
                            objectCarried.tag = "EGG_DROPPED";
                        }

                        objectCarried.transform.parent = null;
                        GraphNode node = AstarPath.active.GetNearest(objectCarried.transform.position, NNConstraint.Default).node;
                        objectCarried.transform.position = (Vector3)node.position;
                        ChangeState(State.EXITING);
                        break;
                    }
                    break;
                case State.EXITING:
                    if (SensingUtils.DistanceToTarget(gameObject, target) <= blackboard.pointReachedRadius)
                    {
                        //Debug.Log("EXITED");
                        Destroy(gameObject);
                        break;
                    }
                    break;
            }
        }

        void ChangeState(State newState)
        {
            // EXIT LOGIC. 
            switch (currentState)
            {
                case State.WANDERING:
                    pathFeeder.enabled = false;
                    pathFollowing.enabled = false;
                    break;
            }

            // ENTER LOGIC
            switch (newState)
            {
                case State.WANDERING:
                    target = blackboard.GetRandomWanderPoint();
                    pathFeeder.target = target;
                    pathFeeder.enabled = true;
                    pathFollowing.enabled = true;
                    break;
                case State.EXITING:
                    target = blackboard.GetRandomExitPoint();
                    pathFeeder.target = target;
                    pathFeeder.enabled = true;
                    pathFollowing.enabled = true;
                    break;
            }
            currentState = newState;
        }
    }
}
