using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    public class FSM_Ladybug : FiniteStateMachine
    {
        public enum State
        {
            INITIAL,
            WANDERING,
            ARRIVE_EGG,
            TRANSPORTING_EGG,
            ARRRIVE_SEED,
            TRANSPORTING_SEED
        }

        public State currentState = State.INITIAL;

        private LadyBlackboard blackboard;
        private PathFeeder pathFeeder;
        private PathFollowing pathFollowing;
        private GameObject target;

        private GameObject egg;
        private GameObject seed;

        void Start()
        {
            blackboard = GetComponent<LadyBlackboard>();
            pathFeeder = GetComponent<PathFeeder>();
            pathFollowing = GetComponent<PathFollowing>();
            pathFeeder.enabled = false;
            pathFollowing.enabled = false;

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

                    egg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG_DROPPED", blackboard.closestEggRadius);

                    if (egg != null)
                    {
                        ChangeState(State.ARRIVE_EGG);
                        break;
                    }
                    else
                    {
                        egg = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "EGG_DROPPED", blackboard.randomEggRadius);
                        if (egg != null)
                        {
                            ChangeState(State.ARRIVE_EGG);
                            break;
                        }
                    }

                    seed = SensingUtils.FindInstanceWithinRadius(gameObject, "SEED_DROPPED", blackboard.closestSeedRadius);

                    if (seed != null)
                    {
                        ChangeState(State.ARRRIVE_SEED);
                        break;
                    }
                    else
                    {
                        seed = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "SEED_DROPPED", blackboard.randomSeedRadius);
                        if (seed != null)
                        {
                            ChangeState(State.ARRRIVE_SEED);
                            break;
                        }
                    }
                    if (SensingUtils.DistanceToTarget(gameObject, target) <= blackboard.pointReachedRadius)
                    {
                        ChangeState(State.WANDERING);
                        break;
                    }

                    break;
                case State.ARRIVE_EGG:

                    GameObject otherEgg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG_DROPPED", blackboard.closestEggRadius);

                    if (otherEgg != null && otherEgg != egg)
                    {
                        if (SensingUtils.DistanceToTarget(gameObject, egg) > SensingUtils.DistanceToTarget(gameObject, otherEgg))
                        {
                            egg = otherEgg;
                            ChangeState(State.ARRIVE_EGG);
                            break;
                        }
                    }

                    if (SensingUtils.DistanceToTarget(gameObject, egg) <= blackboard.pointReachedRadius)
                    {
                        ChangeState(State.TRANSPORTING_EGG);
                        break;
                    }

                    if (egg.tag != "EGG_DROPPED")
                    {
                        ChangeState(State.WANDERING);
                        break;
                    }
                    break;

                case State.TRANSPORTING_EGG:
                    if (SensingUtils.DistanceToTarget(gameObject, target) <= blackboard.pointReachedRadius)
                    {
                        egg.tag = "EGG_STORED";
                        egg.transform.parent = null;
                        GraphNode node = AstarPath.active.GetNearest(egg.transform.position, NNConstraint.Default).node;
                        egg.transform.position = (Vector3)node.position;
                        ChangeState(State.WANDERING);
                        break;
                    }
                    break;
                case State.ARRRIVE_SEED:

                    egg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG_DROPPED", blackboard.eggWhileSeedRadius);

                    if (egg != null)
                    {
                        ChangeState(State.ARRIVE_EGG);
                        break;
                    }

                    if (SensingUtils.DistanceToTarget(gameObject, seed) <= blackboard.pointReachedRadius)
                    {
                        ChangeState(State.TRANSPORTING_SEED);
                        break;
                    }

                    if (seed.tag != "SEED_DROPPED")
                    {
                        ChangeState(State.WANDERING);
                        break;
                    }
                    break;
                case State.TRANSPORTING_SEED:

                    egg = SensingUtils.FindInstanceWithinRadius(gameObject, "EGG_DROPPED", blackboard.eggWhileSeedRadius);

                    if (egg != null)
                    {
                        seed.tag = "SEED_DROPPED";
                        seed.transform.parent = null;
                        GraphNode node = AstarPath.active.GetNearest(seed.transform.position, NNConstraint.Default).node;
                        seed.transform.position = (Vector3)node.position;
                        ChangeState(State.ARRIVE_EGG);
                        break;
                    }

                    if (SensingUtils.DistanceToTarget(gameObject, target) <= blackboard.pointReachedRadius)
                    {
                        seed.tag = "SEED_STORED";
                        seed.transform.parent = null;
                        GraphNode node = AstarPath.active.GetNearest(seed.transform.position, NNConstraint.Default).node;
                        seed.transform.position = (Vector3)node.position;
                        ChangeState(State.WANDERING);
                        break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ChangeState(State newState)
        {
            //EXIT LOGIC
            switch (currentState)
            {
                case State.WANDERING:
                    pathFeeder.enabled = false;
                    pathFollowing.enabled = false;
                    break;
                case State.ARRIVE_EGG:
                    pathFeeder.enabled = false;
                    pathFollowing.enabled = false;
                    break;
                case State.TRANSPORTING_EGG:
                    pathFeeder.enabled = false;
                    pathFollowing.enabled = false;
                    break;
                case State.ARRRIVE_SEED:
                    pathFeeder.enabled = false;
                    pathFollowing.enabled = false;
                    break;
                case State.TRANSPORTING_SEED:
                    pathFeeder.enabled = false;
                    pathFollowing.enabled = false;
                    break;
                default:
                    break;
            }

            //ENTER LOGIC
            switch (newState)
            {
                case State.WANDERING:
                    target = blackboard.GetRandomWanderPoint();
                    pathFeeder.target = target;
                    pathFeeder.enabled = true;
                    pathFollowing.enabled = true;
                    break;
                case State.ARRIVE_EGG:
                    target = egg;
                    pathFeeder.target = target;
                    pathFeeder.enabled = true;
                    pathFollowing.enabled = true;
                    break;
                case State.TRANSPORTING_EGG:
                    egg.tag = "TRANSPORTING_EGG";
                    egg.transform.parent = gameObject.transform;
                    target = blackboard.GetRandomHatchingPoint();
                    pathFeeder.target = target;
                    pathFeeder.enabled = true;
                    pathFollowing.enabled = true;
                    break;
                case State.ARRRIVE_SEED:
                    target = seed;
                    pathFeeder.target = target;
                    pathFeeder.enabled = true;
                    pathFollowing.enabled = true;
                    break;
                case State.TRANSPORTING_SEED:
                    seed.tag = "TRANSPORTING_SEED";
                    seed.transform.parent = gameObject.transform;
                    target = blackboard.GetRandomStoringPoint();
                    pathFeeder.target = target;
                    pathFeeder.enabled = true;
                    pathFollowing.enabled = true;
                    break;
                default:
                    break;
            }

            currentState = newState;
        }


    }
}

