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

        private Arrive arrive;
        private Seek seek;
        private Seeker seeker;
        private Path currentPath;

        public int currentWaypointIndex = 0;
        public GameObject seekerTarget;
        public GameObject target;
        private float pointReachedRadius = 2f;

        void Start()
        {
            arrive = GetComponent<Arrive>();
            seek = GetComponent<Seek>();
            seeker = GetComponent<Seeker>();

            seek.enabled = false;
            arrive.enabled = false;
            arrive.closeEnoughRadius = pointReachedRadius;
        }

        public override void Exit()
        {
            seek.enabled = false;
            arrive.enabled = false;
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
                    ChangeState(State.FOLLOWING);
                    break;
                case State.FOLLOWING:
                    if (SensingUtils.DistanceToTarget(gameObject, target) <= pointReachedRadius)
                    {
                        currentWaypointIndex++;
                        if (currentWaypointIndex == currentPath.vectorPath.Count)
                        {
                            ChangeState(State.TERMINATED);
                            break;
                        }       
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
            // EXIT LOGIC
            switch (currentState)
            {
                case State.GENERATING:
                    break;
                case State.FOLLOWING:
                    seek.enabled = false;
                    arrive.enabled = false;
                    break;

            }

            // ENTER LOGIC
            switch (newState)
            {
                case State.GENERATING:
                    seeker.StartPath(this.transform.position, seekerTarget.transform.position, OnPathComplete);
                    break;
                case State.FOLLOWING:
                    target.transform.position = currentPath.vectorPath[currentWaypointIndex];
                    if(currentWaypointIndex == currentPath.vectorPath.Count - 1)
                    {
                        arrive.target = target;
                        arrive.enabled = true;
                        break;
                    }
                    else
                    {
                        seek.target = target;
                        seek.enabled = true;
                        break;
                    }                      
                case State.TERMINATED:

                    break;
            }

            currentState = newState;
        }

        public void OnPathComplete(Path p)
        {
            // this is a "callback" method. if this method is called, a path has been computed and "stored" in p
            currentPath = p;
            currentWaypointIndex = 0;
        }
    }

}
