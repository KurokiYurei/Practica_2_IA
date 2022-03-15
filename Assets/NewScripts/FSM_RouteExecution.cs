using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Steerings;

namespace FSM
{
    public class FSM_RouteExecution : FiniteStateMachine
    {
        public enum State { INITIAL, GENERATING, FOLLOWING, TERMINATED }

        public State currentState = State.INITIAL;

        private Seeker seeker;
        private Path currentPath;

        private GameObject target;
        private float pointReachedRadius = 2f;

        void Start()
        {
        }

        public override void Exit()
        {

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
                    ChangeState(State.GENERATING);
                    break;
                case State.GENERATING:
                    seeker.StartPath(this.transform.position, target.transform.position, OnPathComplete);
                    break;
                case State.FOLLOWING:
                    if (SensingUtils.DistanceToTarget(gameObject, target) <= pointReachedRadius)
                    {
                        ChangeState(State.FOLLOWING);
                        break;
                    }
                    break;
                case State.TERMINATED:
                    break;
            }
        }

        void ChangeState(State newState)
        {
            // EXIT LOGIC. This particular FSM has no exit logic
            /*
            switch (currentState)
            {
                // could stop the pathFeeder but no need
                case State.WANDERING: break;
            }
            */

            // ENTER LOGIC
            switch (newState)
            {
                case State.GENERATING:
                    target = GetRandomPoint();
                    break;
            }

            currentState = newState;
        }

        GameObject GetRandomPoint()
        {
            GameObject[] wanderPoints = GameObject.FindGameObjectsWithTag("WANDERPOINT");
            return wanderPoints[Random.Range(0, wanderPoints.Length)];
        }

        public void OnPathComplete(Path p)
        {
            // this is a "callback" method. if this method is called, a path has been computed and "stored" in p
            currentPath = p;

            
        }
    }

}
