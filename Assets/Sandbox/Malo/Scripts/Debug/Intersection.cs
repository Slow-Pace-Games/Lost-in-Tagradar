using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private GameObject pointC;

    private void OnDrawGizmos()
    {
        pointC.transform.position = FindIntersection(pointA.transform.position, pointA.transform.forward, pointB.transform.position, pointB.transform.right);

        if (CheckPointPosition())
        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawSphere(pointA.transform.position, 0.05f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(pointB.transform.position, 0.05f);
        Gizmos.DrawSphere(pointC.transform.position, 0.05f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA.transform.position, pointC.transform.position);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointB.transform.position, pointC.transform.position);
    }

    Vector3 FindIntersection(Vector3 point1, Vector3 direction1, Vector3 point2, Vector3 direction2)
    {
        Vector3 cross = Vector3.Cross(direction1, direction2);
        float t = Vector3.Dot(cross, Vector3.Cross(point2 - point1, direction2)) / cross.magnitude;

        return point1 + t * direction1;
    }

    bool CheckPointPosition()
    {
        return Vector3.Cross(pointA.transform.position, pointB.transform.position).z > 0;
    }
}
