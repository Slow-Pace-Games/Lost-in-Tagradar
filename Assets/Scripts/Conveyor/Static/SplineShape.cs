using UnityEngine;
using UnityEngine.Splines;

public static class SplineShape
{
    public static void UpdateSplineShape(Spline spline)
    {
        Update3Point(spline);
        Update4Point(spline);
        UpdateMorePoint(spline);

        spline.SetTangentMode(TangentMode.Broken);
    }

    private static void Update3Point(Spline spline)
    {
        if (SplineUtilities.GetNumberOfSplinePoint(spline) < 3)
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, 0, out Vector3 postion, out Vector3 tangent, out Vector3 forward);
            SplineUtilities.AddSplinePointAtIndex(spline, 1, postion + forward * 2);
        }
        else
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, 0, out Vector3 postion, out Vector3 tangent, out Vector3 forward);

            float dist = SplineUtilities.GetDistanceBetween2Points(spline, 1, 2, true) / 3f;
            float distClamped = Mathf.Clamp(dist, 0f, 50f);

            SplineUtilities.SetTangentsAtSplinePoint(spline, 1, -tangent, forward * distClamped);
        }
    }

    private static void Update4Point(Spline spline)
    {
        if (SplineUtilities.GetNumberOfSplinePoint(spline) < 4)
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, SplineUtilities.GetLastSplinePoint(spline), out Vector3 postion, out Vector3 tangent, out Vector3 forward);
            SplineUtilities.AddSplinePointAtIndex(spline, SplineUtilities.GetLastSplinePoint(spline), postion);
        }
        else
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, SplineUtilities.GetLastSplinePoint(spline), out Vector3 postion, out Vector3 tangent, out Vector3 forward);
            float dist = SplineUtilities.GetDistanceBetween2Points(spline, SplineUtilities.GetLastSplinePoint(spline) - 2, SplineUtilities.GetLastSplinePoint(spline) - 1, true) / 3f;
            float distClamped = Mathf.Clamp(dist, 0f, 50f);

            SplineUtilities.SetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline) - 1, postion - forward * 2);
            SplineUtilities.SetTangentsAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline) - 1, -forward * distClamped, tangent);
        }
    }

    private static void UpdateMorePoint(Spline spline)
    {
        if (SplineUtilities.NearlyEqual(Mathf.Abs(SplineUtilities.GetRelativeRotation(spline)), 165f) || Mathf.Abs(SplineUtilities.GetRelativeRotation(spline)) > 165f)
        {
            Update5Point(spline);
        }
        else
        {
            while (SplineUtilities.GetNumberOfSplinePoint(spline) >= 5)
            {
                SplineUtilities.RemoveSplinePointAtIndex(spline, 2);
            }
        }
    }

    private static void Update5Point(Spline spline)
    {
        if (SplineUtilities.GetNumberOfSplinePoint(spline) < 5)
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, SplineUtilities.GetLastSplinePoint(spline), out Vector3 position, out Vector3 tangent, out Vector3 forward);
            SplineUtilities.AddSplinePointAtIndex(spline, 2, position);
        }

        Vector3 forwardDirection = SplineUtilities.GetTangentAtSplinePoint(spline, 0);
        Vector3 directionToPoint = SplineUtilities.GetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline)) - SplineUtilities.GetPositionAtSplinePoint(spline, 0);
        float dotProduct = Vector3.Dot(forwardDirection, directionToPoint);

        if (dotProduct >= -1f && dotProduct <= 1f)
        {
            if (SplineUtilities.GetNumberOfSplinePoint(spline) > 5)
            {
                SplineUtilities.RemoveSplinePointAtIndex(spline, 2);
            }

            SplineUtilities.GetSplineInfoAtPoint(spline, 1, out Vector3 positionStart, out Vector3 tangentStart, out Vector3 forwardStart);
            SplineUtilities.GetSplineInfoAtPoint(spline, SplineUtilities.GetLastSplinePoint(spline) - 1, out Vector3 positionEnd, out Vector3 tangentEnd, out Vector3 forwardEnd);

            float dist = SplineUtilities.GetLinearDistBetween2Point(spline, 1, SplineUtilities.GetLastSplinePoint(spline) - 1, true) / 4f;

            SplineUtilities.SetPositionAtSplinePoint(spline, 2, ((positionStart + positionEnd) / 2) + dist * forwardStart * 2.2f);

            Vector3 tangent = -Vector3.Cross(forwardStart, Vector3.up).normalized * dist * 2f;
            if (CheckPointPosition(positionEnd, positionStart))
            {
                tangent *= -1f;
            }

            SplineUtilities.SetTangentsAtSplinePoint(spline, 2, -tangent, tangent);
        }
        else
        {
            Update6Point(spline);
        }
    }

    private static void Update6Point(Spline spline)
    {
        if (SplineUtilities.GetNumberOfSplinePoint(spline) < 6)
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, SplineUtilities.GetLastSplinePoint(spline), out Vector3 position, out Vector3 tangent, out Vector3 forward);
            SplineUtilities.AddSplinePointAtIndex(spline, 2, position);
        }

        Vector3 forwardDirection = SplineUtilities.GetTangentAtSplinePoint(spline, 0);
        Vector3 directionToPoint = SplineUtilities.GetPositionAtSplinePoint(spline, SplineUtilities.GetLastSplinePoint(spline)) - SplineUtilities.GetPositionAtSplinePoint(spline, 0);
        float dotProduct = Vector3.Dot(forwardDirection, directionToPoint);

        if (dotProduct >= 1f)
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, 1, out Vector3 positionStart, out Vector3 tangentStart, out Vector3 forwardStart);
            SplineUtilities.GetSplineInfoAtPoint(spline, 4, out Vector3 positionEnd, out Vector3 tangentEnd, out Vector3 forwardEnd);
            Vector3 position = FindIntersection(positionStart, forwardStart, positionEnd, Vector3.Cross(forwardEnd, Vector3.up));

            SplineUtilities.SetPositionAtSplinePoint(spline, 2, position);
            SplineUtilities.SetTangentsAtSplinePoint(spline, 2, tangentEnd, -tangentEnd);

            SplineUtilities.GetSplineInfoAtPoint(spline, 2, out Vector3 newPositionStart, out Vector3 tangent2, out Vector3 forward2);

            float dist = SplineUtilities.GetDistanceBetween2Points(spline, 2, 4, true) / 8f;
            Vector3 tangent = -Vector3.Cross(forwardStart, Vector3.up).normalized * dist;
            if (CheckPointPosition(positionEnd, newPositionStart))
            {
                tangent *= -1f;
            }

            SplineUtilities.SetPositionAtSplinePoint(spline, 3, ((newPositionStart + positionEnd) / 2) + dist * forwardStart * 2.2f);
            SplineUtilities.SetTangentsAtSplinePoint(spline, 3, -tangent, tangent);
        }
        else
        {
            SplineUtilities.GetSplineInfoAtPoint(spline, 1, out Vector3 positionStart, out Vector3 tangentStart, out Vector3 forwardStart);
            SplineUtilities.GetSplineInfoAtPoint(spline, 4, out Vector3 positionEnd, out Vector3 tangentEnd, out Vector3 forwardEnd);
            Vector3 position = new Vector3(positionEnd.x, positionStart.y, positionStart.z);
            position = FindIntersection(positionEnd, forwardEnd, positionStart, Vector3.Cross(forwardStart, Vector3.up));

            SplineUtilities.SetPositionAtSplinePoint(spline, 3, position);
            SplineUtilities.SetTangentsAtSplinePoint(spline, 3, tangentStart, -tangentStart);

            SplineUtilities.GetSplineInfoAtPoint(spline, 3, out Vector3 newPositionEnd, out Vector3 tangent1, out Vector3 forward1);
            SplineUtilities.GetSplineInfoAtPoint(spline, 1, out Vector3 newPositionStart, out Vector3 tangent2, out Vector3 forward2);

            float dist = SplineUtilities.GetDistanceBetween2Points(spline, 1, 3, true) / 8f;

            Vector3 tangent = -Vector3.Cross(forwardStart, Vector3.up).normalized * dist;
            if (CheckPointPosition(newPositionEnd, newPositionStart))
            {
                tangent *= -1f;
            }

            SplineUtilities.SetPositionAtSplinePoint(spline, 2, ((newPositionStart + newPositionEnd) / 2) + dist * forwardStart * 2.2f);
            SplineUtilities.SetTangentsAtSplinePoint(spline, 2, -tangent, tangent);
        }
    }

    private static Vector3 FindIntersection(Vector3 point1, Vector3 direction1, Vector3 point2, Vector3 direction2)
    {
        Vector3 cross = Vector3.Cross(direction1, direction2);
        float t = Vector3.Dot(cross, Vector3.Cross(point2 - point1, direction2)) / cross.magnitude;

        return point1 + t * direction1;
    }
    private static bool CheckPointPosition(Vector3 point1,Vector3 point2)
    {
        return Vector3.Cross(point1, point2).y > 0;
    }
}