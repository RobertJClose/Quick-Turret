using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace QuickTurret.Waypoints
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class WaypointsNavigator : MonoBehaviour
    {
        NavMeshAgent agent;
        bool isNavigating;
        Waypoints waypoints;

        public Waypoint LastVisitedWaypoint { get; private set; }
        public Waypoint TargetWaypoint { get; private set; }
        public Waypoints Waypoints => waypoints;

        private void Awake()
        {
            isNavigating = false;
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (isNavigating && agent.remainingDistance < 20.0f)
            {
                Waypoint nextWaypoint = GetNextWaypoint(TargetWaypoint);

                if (nextWaypoint != null)
                    SetDestination(nextWaypoint);
                else
                    Destroy(gameObject);
            }
        }

        public Waypoint GetNextWaypoint(Waypoint waypoint)
        {
            // The next waypoint is chosen at random, with all
            // possibilities getting an equal chance.
            int numConnections = waypoint.Connections.Count;
            if (numConnections == 0)
                return null;

            int roll = Random.Range(0, numConnections);
            return waypoint.ConnectedWaypoints[roll];
        }

        public void Teleport(Waypoint waypoint)
        {
            if (waypoint != null && waypoints.Contains(waypoint))
            {
                transform.position = waypoint.Position;
                LastVisitedWaypoint = waypoint;
                TargetWaypoint = null;
            }
        }

        public void SetDestination(Waypoint waypoint)
        {
            isNavigating = true;
            TargetWaypoint = waypoint;
            agent.SetDestination(waypoint.Position);
        }

        public void SetWaypoints(Waypoints waypoints)
        {
            this.waypoints = waypoints;
        }
    }
}
