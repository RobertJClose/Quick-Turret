using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.Waypoints
{
    [System.Serializable]
    public class WaypointConnection
    {
        [SerializeReference]
        public Waypoint source;
        [SerializeReference]
        public Waypoint destination;

        public WaypointConnection()
        {
            destination = null;
        }

        public WaypointConnection(Waypoint destination)
        {
            this.destination = destination;
        }

        public WaypointConnection(Waypoint source, Waypoint destination)
        {
            this.source = source;
            this.destination = destination;
        }
    }
}
