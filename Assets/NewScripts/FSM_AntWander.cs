using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    public class FSM_AntWander : FiniteStateMachine
    {
        public enum State { INITIAL, ARRIVE_POINT, EXITING }

        public State currentState = State.INITIAL;

        private AntBlackboard blackboard;
     
        private GameObject target;
        private GameObject objectCarried;

        private FSM_RouteExecution FSMroute;

        void Start()
        {
            // get what we need...
            blackboard = GetComponent<AntBlackboard>();

            FSMroute = GetComponent<FSM_RouteExecution>();
            FSMroute.enabled = false;

            objectCarried = gameObject.transform.GetChild(0).gameObject;
        }

        public override void Exit()
        {
            FSMroute.enabled = false;
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
                    ChangeState(State.ARRIVE_POINT);
                    break;
                case State.ARRIVE_POINT:
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
                case State.ARRIVE_POINT:
                    FSMroute.Exit();                 
                    break;
            }

            // ENTER LOGIC
            switch (newState)
            {
                case State.ARRIVE_POINT:
                    target = blackboard.GetRandomWanderPoint();
                    FSMroute.seekerTarget = target;
                    FSMroute.ReEnter();            
                    break;
                case State.EXITING:
                    target = blackboard.GetRandomExitPoint();
                    FSMroute.seekerTarget = target;
                    FSMroute.ReEnter();            
                    break;
            }
            currentState = newState;
        }
    }
}
