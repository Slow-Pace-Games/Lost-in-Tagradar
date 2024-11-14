namespace UnityEngine.Splines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Mathematics;
    using Unity.VisualScripting;

    public static class SplineUtilities
    {
        private static BezierKnot CreateBezierKnot(Vector3 position, Quaternion rotation, Vector3 tangentIn, Vector3 tangentOut)
        {
            BezierKnot knot = new BezierKnot();

            knot.Position = position;

            knot.Rotation = rotation;

            knot.TangentIn = tangentIn;
            knot.TangentOut = tangentOut;

            return knot;
        }

        private static BezierKnot CreateBezierKnot(Vector3 position, Vector3 tangentIn, Vector3 tangentOut)
        {
            BezierKnot knot = new BezierKnot();

            knot.Position = position;

            knot.Rotation = quaternion.identity;

            knot.TangentIn = tangentIn;
            knot.TangentOut = tangentOut;

            return knot;
        }

        private static BezierKnot CreateBezierKnot(Vector3 position, Quaternion rotation)
        {
            BezierKnot knot = new BezierKnot();

            knot.Position = position;

            knot.Rotation = rotation;

            knot.TangentIn = Vector3.one;
            knot.TangentOut = Vector3.one;

            return knot;
        }

        private static BezierKnot CreateBezierKnot(Vector3 position)
        {
            BezierKnot knot = new BezierKnot();

            knot.Position = position;

            knot.Rotation = quaternion.identity;

            knot.TangentIn = Vector3.one;
            knot.TangentOut = Vector3.one;

            return knot;
        }

        public static void AddSplinePointAtIndex(Spline target, int index, Vector3 position, Quaternion rotation)
        {
            target.Insert(index, CreateBezierKnot(position, rotation));
        }

        public static void AddSplinePointAtIndex(Spline target, int index, Vector3 position)
        {
            target.Insert(index, CreateBezierKnot(position));
        }

        public static void AddSplinePoint(Spline target, Vector3 position)
        {
            target.Add(CreateBezierKnot(position));
        }

        public static void AddSplinePoint(Spline target, Vector3 position, Vector3 tangentIn, Vector3 tangentOut)
        {
            target.Add(CreateBezierKnot(position, tangentIn, tangentOut));
        }

        public static void AddSplinePoint(Spline target, Vector3 position, Vector3 tangent)
        {
            target.Add(CreateBezierKnot(position, -tangent, tangent));
        }

        public static void AddSplinePoint(Spline target, Vector3 position, Quaternion rotation, Vector3 tangentIn, Vector3 tangentOut)
        {
            target.Add(CreateBezierKnot(position, rotation, tangentIn, tangentOut));
        }

        public static void SetTangentsAtSplinePoint(Spline target, int index, Vector3 tangentIn, Vector3 tangentOut)
        {
            BezierKnot targetKnot = target.Knots.ToList()[index];

            targetKnot.TangentIn = tangentIn;
            targetKnot.TangentOut = tangentOut;

            target.SetKnot(index, targetKnot);
        }

        public static void SetTangentsAtSplinePoint(Spline target, int index, Vector3 tangent)
        {
            BezierKnot targetKnot = target.Knots.ToList()[index];

            targetKnot.TangentIn = -tangent;
            targetKnot.TangentOut = tangent;

            target.SetKnot(index, targetKnot);
        }

        public static void GetSplineInfoAtPoint(Spline target, int index, out Vector3 position, out Quaternion rotation, out Vector3 tangent, out Vector3 forward)
        {
            BezierKnot targetKnot = target.Knots.ToList()[index];

            position = targetKnot.Position;
            rotation = targetKnot.Rotation;
            tangent = targetKnot.TangentOut;
            forward = Vector3.Normalize(tangent);
        }

        public static void GetSplineInfoAtPoint(Spline target, int index, out Vector3 position, out Vector3 tangentIn, out Vector3 tangentOut, out Quaternion rotation)
        {
            BezierKnot targetKnot = target.Knots.ToList()[index];

            position = targetKnot.Position;
            rotation = targetKnot.Rotation;
            tangentIn = targetKnot.TangentIn;
            tangentOut = targetKnot.TangentOut;
        }

        public static void GetSplineInfoAtPoint(Spline target, int index, out Vector3 position, out Vector3 tangent, out Vector3 forward)
        {
            BezierKnot targetKnot = target.Knots.ToList()[index];

            position = targetKnot.Position;
            tangent = targetKnot.TangentOut;
            forward = Vector3.Normalize(tangent);
        }

        public static float GetDistanceBetween2Points(Spline target, int indexA, int indexB, bool absolute)
        {
            float distance = 0f;

            for (int i = 0; i < indexB - indexA; i++)
            {
                distance += target.GetCurveLength(indexA + i);
            }

            if (absolute)
            {
                return MathF.Abs(distance);
            }
            else
            {
                return distance;
            }
        }

        public static float GetDistanceBetween2Points(Spline target, int indexA, int indexB, float precision, bool absolute)
        {
            float distance = 0f;
            float step = 1f / precision;

            float startT = GetDeltaTimeAtSplinePoint(target, indexA, precision);
            float endT = GetDeltaTimeAtSplinePoint(target, indexB, precision);

            float deltaTimeStart = startT;
            float deltaTimeEnd = startT + step;

            while (deltaTimeEnd < endT)
            {
                distance += Vector3.Distance(target.EvaluatePosition(deltaTimeStart), target.EvaluatePosition(deltaTimeEnd));
                deltaTimeStart = deltaTimeEnd;
                deltaTimeEnd += step;
            }

            if (absolute)
            {
                return MathF.Abs(distance);
            }
            else
            {
                return distance;
            }
        }

        public static Vector3 GetPositionAtSplinePoint(Spline target, int index)
        {
            return target.Knots.ToList()[index].Position;
        }

        public static Vector3 GetWorldPositionAtSplinePoint(Spline target, Vector3 worldPosition, int index)
        {
            return (Vector3)target.Knots.ToList()[index].Position + worldPosition;
        }

        public static float GetDeltaTimeAtSplinePoint(Spline target, int index, float precision)
        {
            float step = 1f / precision;
            float minDist = Vector3.Distance(GetPositionAtSplinePoint(target, 0), GetPositionAtSplinePoint(target, index));
            float deltaTime = step;

            while (deltaTime < 1f)
            {
                float dist = Vector3.Distance(target.EvaluatePosition(deltaTime), GetPositionAtSplinePoint(target, index));

                if (dist < minDist)
                {
                    minDist = dist;
                }
                else
                {
                    return deltaTime;
                }

                deltaTime += step;

                if (deltaTime > 1f && deltaTime != deltaTime + step)
                {
                    deltaTime = 1f;
                }
            }

            return 1f;
        }

        public static int GetNumberOfSplinePoint(Spline target)
        {
            return target.Knots.ToList().Count;
        }

        public static int GetLastSplinePoint(Spline target)
        {
            return (target.Knots.ToList().Count - 1);
        }

        public static void SetPositionAtSplinePoint(Spline target, int index, Vector3 position)
        {
            BezierKnot targetKnot = target.Knots.ToList()[index];
            targetKnot.Position = position;
            target.SetKnot(index, targetKnot);
        }

        public static void SetTransformAtSplinePoint(Spline target, int index, Vector3 position, Quaternion rotation)
        {
            BezierKnot targetKnot = target.Knots.ToList()[index];
            targetKnot.Position = position;
            targetKnot.Rotation = rotation;
            target.SetKnot(index, targetKnot);
        }

        public static void RemoveSplinePointAtIndex(Spline target, int index)
        {
            target.RemoveAt(index);
        }

        public static void RemoveSplinePoint(Spline target)
        {
            target.RemoveAt(GetLastSplinePoint(target));
        }

        public static float GetRelativeRotation(Spline target)
        {
            return Vector3.Angle(GetTangentAtSplinePoint(target, 0), GetTangentAtSplinePoint(target, GetLastSplinePoint(target)));
        }

        public static Quaternion GetRotationAtSplinePoint(Spline target, int index)
        {
            return target.Knots.ToList()[index].Rotation;
        }

        public static Vector2 GetRelativeLocation(Spline target)
        {
            Vector3 targetLocation = GetPositionAtSplinePoint(target, GetLastSplinePoint(target));
            return new Vector2(targetLocation.x, targetLocation.z);
        }

        public static Vector2 GetEndQuad(Spline target)
        {
            Vector2 endLocation = GetRelativeLocation(target);
            return new Vector2(Mathf.Clamp(endLocation.x, -1, 1), Mathf.Clamp(endLocation.y, -1, 1));
        }

        public static Vector3 GetTangentAtSplinePoint(Spline target, int index)
        {
            return Vector3.Normalize(target.Knots.ToList()[index].TangentOut);
        }

        public static bool NearlyEqual(float a, float b, float epsilon = 0.000001f)
        {
            return Mathf.Abs(a - b) <= epsilon;
        }

        public static float GetLinearDistBetween2Point(Spline target, int indexA, int indexB, bool absolute)
        {
            float distance = Vector3.Distance(GetPositionAtSplinePoint(target, indexA), GetPositionAtSplinePoint(target, indexB));

            if (absolute)
            {
                return Mathf.Abs(distance);
            }

            return distance;
        }

        public static void SetPointAtSplinePoint(Spline target, int index, BezierKnot point)
        {
            target.SetKnot(index, point);
        }

        public static bool IsSamePointAtSplinePoint(Spline target, int index, Vector3 position)
        {
            return (Vector3)target.Knots.ToList()[index].Position == position;
        }

        public static bool IsSamePointAtSplinePoint(Spline target, int index, Vector3 position, Quaternion rotation)
        {
            BezierKnot point = target.ToList()[index];
            return (Vector3)point.Position == position && (Quaternion)point.Rotation == rotation;
        }

        public static bool IsSamePointAtSplinePoint(Spline target, int index, Vector3 position, Vector3 tangente)
        {
            BezierKnot point = target.ToList()[index];
            return (Vector3)point.Position == position && (Vector3)point.TangentOut == tangente && (Vector3)point.TangentIn == -tangente;
        }

        public static bool IsSamePointAtSplinePoint(Spline target, int index, Vector3 position, Vector3 tangenteIn, Vector3 tangentOut)
        {
            BezierKnot point = target.ToList()[index];
            return (Vector3)point.Position == position && (Vector3)point.TangentIn == tangenteIn && (Vector3)point.TangentOut == tangentOut;
        }

        public static void ReverseFlowSpline(Spline target)
        {
            List<BezierKnot> knots = target.Knots.ToList();

            int lenght = Mathf.CeilToInt(knots.Count / 2f);

            for (int i = 0; i < lenght; i++)
            {
                BezierKnot first = knots[i];
                BezierKnot second = knots[knots.Count - 1 - i];

                first = InvertTangentsBezierKnot(first);
                second = InvertTangentsBezierKnot(second);

                knots[i] = second;
                knots[knots.Count - 1 - i] = first;
            }

            target.Clear();
            target.AddRange(knots);
        }
        public static void ReverseFlowSplineWithoutTangent(Spline target)
        {
            List<BezierKnot> knots = target.Knots.ToList();

            int lenght = Mathf.CeilToInt(knots.Count / 2f);

            for (int i = 0; i < lenght; i++)
            {
                BezierKnot first = knots[i];
                BezierKnot second = knots[knots.Count - 1 - i];

                first.TangentIn = knots[knots.Count - 1 - i].TangentOut;
                first.TangentOut = knots[knots.Count - 1 - i].TangentIn;

                second.TangentIn = knots[i].TangentOut;
                second.TangentOut = knots[i].TangentIn;

                knots[i] = second;
                knots[knots.Count - 1 - i] = first;
            }

            target.Clear();
            target.AddRange(knots);
        }

        public static void ReverseTangentAtSplinePoint(Spline target, int index)
        {
            SetTangentsAtSplinePoint(target, index, -GetTangentAtSplinePoint(target, index), -GetTangentAtSplinePoint(target, index));
        }

        public static BezierKnot InvertTangentsBezierKnot(BezierKnot knot)
        {
            knot.TangentIn.x *= -1;
            knot.TangentIn.y *= -1;
            knot.TangentIn.z *= -1;

            knot.TangentOut.x *= -1;
            knot.TangentOut.y *= -1;
            knot.TangentOut.z *= -1;

            return knot;
        }

        public static List<BezierKnot> Merge2SplineAtStart(List<BezierKnot> target, List<BezierKnot> data, Vector3 targetWorldPos, Vector3 dataWorldPos)
        {
            List<BezierKnot> finalKnot = new List<BezierKnot>();

            for (int i = 0; i < data.Count - 1; i++)
            {
                BezierKnot knot = data[i];

                knot.Position += (float3)dataWorldPos - (float3)targetWorldPos;
                finalKnot.Add(knot);
            }

            finalKnot.AddRange(target);

            return finalKnot;
        }

        public static List<BezierKnot> Merge2SplineAtEnd(List<BezierKnot> target, List<BezierKnot> data, Vector3 targetWorldPos, Vector3 dataWorldPos)
        {
            List<BezierKnot> finalKnot = new List<BezierKnot>();

            finalKnot.AddRange(target);

            for (int i = 1; i < data.Count; i++)
            {
                BezierKnot knot = data[i];

                knot.Position += (float3)dataWorldPos - (float3)targetWorldPos;
                finalKnot.Add(knot);
            }

            return finalKnot;
        }

        public static Vector3 GetRightVectorAtSplinePoint(Spline target, int index)
        {
            return Vector3.Normalize(Vector3.Cross(Vector3.Normalize(GetTangentAtSplinePoint(target, index)), Vector3.up));
        }

        public static void Evaluate(Spline target, float dt, out Vector3 position, out Quaternion rotation)
        {
            SplineUtility.Evaluate(target, dt, out float3 position3, out float3 tangent, out float3 upVector);
            position = position3;
            rotation = Quaternion.LookRotation(tangent, upVector);
        }

        public static void Evaluate(Spline target, float dt, out Vector3 position, out Vector3 forward, out Vector3 right, out Quaternion rotation)
        {
            SplineUtility.Evaluate(target, dt, out float3 position3, out float3 forward3, out float3 upVector);

            position = position3;
            rotation = Quaternion.LookRotation(forward3, upVector);
            forward = forward3;
            right = Vector3.Cross(forward, upVector).normalized;
        }
    }
}