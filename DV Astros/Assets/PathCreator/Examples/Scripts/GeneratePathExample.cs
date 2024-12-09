using UnityEngine;
using System.Collections.Generic;
using System;


namespace PathCreation.Examples {
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]

    

    public class GeneratePathExample : MonoBehaviour {

        public bool closedLoop = true;
        public List<Transform> waypoints = new List<Transform>();

        public GameObject program;

        void Start () {

            //waypoints = program.GetData

            string[,] data = program.GetComponent<Program>().GetData();
            for (int i = 0; i < data.Length; i++) {
                float valueX = 0;
                float valueY = 0;
                float valueZ = 0;
                try {
                    valueX = float.Parse(data[i, 0]);
                }
                catch (Exception e) { Debug.Log("Catch"); }
                try {
                    valueY = float.Parse(data[i, 1]);
                }
                catch (Exception e) { Debug.Log("Catch"); }

                try {
                    valueZ = float.Parse(data[i, 2]);
                }
                catch (Exception e) { Debug.Log("Catch"); }

                GameObject emptyGO = new GameObject();
                Transform newTransform = emptyGO.transform;
                newTransform.position = new Vector3(valueX, valueY, valueZ);
                waypoints.Add(newTransform);
            }

            if (waypoints.Count > 0) {
                // Create a new bezier path from the waypoints.
                BezierPath bezierPath = new BezierPath (waypoints, closedLoop, PathSpace.xyz);
                GetComponent<PathCreator> ().bezierPath = bezierPath;
            }
        }
    }
}