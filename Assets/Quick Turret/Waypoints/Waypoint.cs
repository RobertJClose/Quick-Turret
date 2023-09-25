using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.Waypoints
{
    [System.Serializable]
    public class Waypoint
    {
        public string Name;
        public Vector3 Position;
        [SerializeReference]
        public List<WaypointConnection> Connections = new List<WaypointConnection>();

        public List<Waypoint> ConnectedWaypoints { get { return Connections.ConvertAll(c => c.destination); } }

        public Waypoint()
        {
            Name = string.Empty;
            Position = Vector3.zero;
        }

        public Waypoint(string name, Vector3 position)
        {
            this.Name = name;
            this.Position = position;
        }

        public WaypointConnection AddConnection(Waypoint destination)
        {
            WaypointConnection newConnection = new WaypointConnection(this, destination);
            Connections.Add(newConnection);
            return newConnection;
        }

        public bool HasConnection(Waypoint destination)
        {
            return Connections.Exists(item => item.destination == destination);
        }

        public void RemoveConnection(Waypoint destination)
        {
            Connections.RemoveAll((connection) => connection.destination == destination);
        }

        public void RemoveConnection(WaypointConnection connection)
        {
            RemoveConnection(connection.destination);
        }
    }
}
