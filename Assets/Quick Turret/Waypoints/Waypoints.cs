using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickTurret.Waypoints
{
    public class Waypoints : MonoBehaviour
    {
        [SerializeField]
        private int totalAddedCounter;

        [SerializeReference]
        public List<Waypoint> points = new List<Waypoint>();

        public string NextWaypointName { get { return $"Waypoint {totalAddedCounter:000}"; } }
        public List<Waypoint> Points { get { return points; } }

        private void OnEnable()
        {
            totalAddedCounter = 0;
        }

        public Waypoint Add(Vector3 position)
        {
            return Add(NextWaypointName, position);
        }

        public Waypoint Add(string name, Vector3 position)
        {
            totalAddedCounter += 1;

            Waypoint w = new Waypoint(name, position);
            points.Add(w);
            return w;
        }

        public bool Contains(Waypoint waypoint)
        {
            return points.Contains(waypoint);
        }

        public void DeleteAll()
        {
            totalAddedCounter = 0;
            points.Clear();
        }

        public Waypoint FindWaypoint(string name)
        {
            return points.Find(x => x.Name == name);
        }

        public void Remove(int index)
        {
            Waypoint waypoint = points[index];

            foreach (Waypoint item in points)
            {
                item.RemoveConnection(waypoint);
            }

            points.RemoveAt(index);
        }

        public void Remove(Waypoint waypoint)
        {
            foreach (Waypoint item in points)
            {
                item.RemoveConnection(waypoint);
            }

            points.Remove(waypoint);
        }
    }
}
