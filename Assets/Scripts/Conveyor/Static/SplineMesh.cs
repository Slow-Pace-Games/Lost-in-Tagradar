using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public static class SplineMesh
{
    #region Value
    private static List<int> triangles;
    private static List<Vector3> vertices;
    private static float delta = 0f;
    private static float step = 0f;
    private static int offsetIndicesTriangles = 0;
    private static float3 right;
    private static float3 position;
    private static float3 forward;
    private static float3 upVector;

    private static List<int> triangleBaize;
    private static int offsetBaize = 0;
    private static Vector3 downLeft;
    private static Vector3 downRight;
    private static Vector3 upRight;
    private static Vector3 upLeft;

    private static List<int> triangleBorder;
    private static int offsetBorder = 0;
    private static Vector3 p0;
    private static Vector3 p1;
    private static Vector3 p2;
    private static Vector3 p3;
    private static Vector3 p4;
    private static Vector3 p5;
    private static Vector3 p6;
    private static Vector3 p7;
    private static Vector3 p8;
    private static Vector3 p9;
    private static Vector3 p10;
    private static Vector3 p11;
    private static Vector3 p12;
    private static Vector3 p13;
    private static Vector3 p14;
    private static Vector3 p15;
    private static Vector3 p16;
    private static Vector3 p17;
    private static Vector3 p18;
    private static Vector3 p19;
    private static Vector3 p20;
    private static Vector3 p21;
    private static Vector3 p22;
    private static Vector3 p23;
    private static Vector3 p24;
    private static Vector3 p25;
    private static Vector3 p26;
    private static Vector3 p27;
    private static Vector3 p28;
    private static Vector3 p29;
    private static Vector3 p30;
    private static Vector3 p31;
    #endregion

    #region Build Mesh
    public static void BuildMesh(Spline spline, float extrusionInterval, MeshFilter baize, MeshFilter border)
    {
        step = extrusionInterval / spline.GetLength();

        CreateBaize(spline, baize);
        CreateBorder(spline, border);
    }

    private static void CreateBaize(Spline spline, MeshFilter baize)
    {
        delta = 0f;
        offsetIndicesTriangles = 0;

        if (baize.sharedMesh == null)
        {
            baize.sharedMesh = new Mesh();
        }


        vertices = new List<Vector3>();
        triangles = new List<int>() { 0, 1, 2,
                                      2, 3, 0 }; // base front
        bool finish = false;
        bool takeEnd = false;

        while (!finish)
        {
            SplineUtility.Evaluate(spline, delta, out position, out forward, out upVector);
            right = Vector3.Cross(forward, upVector).normalized;

            downLeft = position + (0.64f * -right);
            downRight = position + (0.64f * right);
            upRight = position + (0.64f * right);
            upLeft = position + (0.64f * -right);

            downLeft.y += -0.04f;
            downRight.y += -0.04f;
            upRight.y += 0.04f;
            upLeft.y += 0.04f;

            vertices.Add(downLeft);
            vertices.Add(downRight);
            vertices.Add(upRight);
            vertices.Add(upLeft);

            if (vertices.Count > 4)
            {
                offsetBaize = offsetIndicesTriangles * 4;
                triangleBaize = new List<int>()
                {
                    1 + offsetBaize,5 + offsetBaize,2 + offsetBaize,//left side
                    6 + offsetBaize,2 + offsetBaize,5 + offsetBaize,//left side

                    7 + offsetBaize,2 + offsetBaize,6 + offsetBaize,//top
                    2 + offsetBaize,7 + offsetBaize,3 + offsetBaize,//top

                    0 + offsetBaize,5 + offsetBaize,1 + offsetBaize,//bottom
                    5 + offsetBaize,0 + offsetBaize,4 + offsetBaize,//bottom

                    3 + offsetBaize,4 + offsetBaize,0 + offsetBaize,//right side
                    4 + offsetBaize,3 + offsetBaize,7 + offsetBaize,//right side
                };
                offsetIndicesTriangles++;
                triangles.AddRange(triangleBaize);
            }

            delta += step;
            if (delta > 1f)
            {
                if (takeEnd)
                {
                    finish = true;
                }

                takeEnd = true;
                delta = 1f;
            }
        }

        int endTriangle = ((vertices.Count / 4) - 1) * 4;
        triangles.AddRange(new List<int> { 2 + endTriangle, 1 + endTriangle, 0 + endTriangle, 0 + endTriangle, 3 + endTriangle, 2 + endTriangle }); // base front

        baize.sharedMesh.Clear();
        baize.sharedMesh.SetVertices(vertices);
        baize.sharedMesh.SetTriangles(triangles, 0);
        baize.sharedMesh.RecalculateNormals();
        baize.sharedMesh.RecalculateTangents();
    }
    private static void CreateBorder(Spline spline, MeshFilter border)
    {
        delta = 0f;
        offsetIndicesTriangles = 0;

        if (border.sharedMesh == null)
        {
            border.sharedMesh = new Mesh();
        }

        vertices = new List<Vector3>();
        triangles = new List<int>() { 
                                                //right
                                                0,1,2,
                                                0,2,15,
                                                2,3,15,
                                                3,4,5,
                                                3,5,6,
                                                3,6,7,
                                                3,7,8,
                                                8,9,10,
                                                8,10,11,
                                                12,13,15,
                                                11,12,15,
                                                3,11,15,
                                                3,8,11,

                                                //left
                                                30,29,28,
                                                17,16,31,
                                                18,17,31,
                                                21,20,19,
                                                22,21,19,
                                                23,22,19,
                                                31,19,18,
                                                30,28,27,
                                                26,25,24,
                                                24,23,19,
                                                27,26,24,
                                                31,30,27,
                                                31,27,24,
                                                31,24,19,
        }; // base front

        bool finish = false;
        bool takeEnd = false;

        while (!finish)
        {
            SplineUtility.Evaluate(spline, delta, out position, out forward, out upVector);
            right = Vector3.Cross(forward, upVector).normalized;

            #region Point
            //right border
            p0 = position + (0.74f * right);
            p1 = position + (0.81f * right);
            p2 = position + (0.78f * right);
            p3 = position + (0.78f * right);
            p4 = position + (0.80f * right);
            p5 = position + (0.80f * right);
            p6 = position + (0.77f * right);
            p7 = position + (0.74f * right);
            p8 = position + (0.72f * right);
            p9 = position + (0.67f * right);
            p10 = position + (0.66f * right);
            p11 = position + (0.64f * right);
            p12 = position + (0.64f * right);
            p13 = position + (0.66f * right);
            p14 = position + (0.70f * right);
            p15 = position + (0.72f * right);

            //left border
            p16 = position + (0.74f * -right);
            p17 = position + (0.81f * -right);
            p18 = position + (0.78f * -right);
            p19 = position + (0.78f * -right);
            p20 = position + (0.80f * -right);
            p21 = position + (0.80f * -right);
            p22 = position + (0.77f * -right);
            p23 = position + (0.74f * -right);
            p24 = position + (0.72f * -right);
            p25 = position + (0.67f * -right);
            p26 = position + (0.66f * -right);
            p27 = position + (0.64f * -right);
            p28 = position + (0.64f * -right);
            p29 = position + (0.66f * -right);
            p30 = position + (0.70f * -right);
            p31 = position + (0.72f * -right);

            //right
            p0.y += -0.13f;
            p1.y += -0.13f;
            p2.y += -0.08f;
            p3.y += -0.06f;
            p4.y += -0.05f;
            p5.y += -0.02f;
            p6.y += 0.02f;
            p7.y += 0.04f;
            p8.y += 0.04f;
            p9.y += 0.13f;
            p10.y += 0.13f;
            p11.y += -0.02f;
            p12.y += -0.10f;
            p13.y += -0.13f;
            p14.y += -0.11f;
            p15.y += -0.11f;

            //left
            p16.y += -0.13f;
            p17.y += -0.13f;
            p18.y += -0.08f;
            p19.y += -0.06f;
            p20.y += -0.05f;
            p21.y += -0.02f;
            p22.y += 0.02f;
            p23.y += 0.04f;
            p24.y += 0.04f;
            p25.y += 0.13f;
            p26.y += 0.13f;
            p27.y += -0.02f;
            p28.y += -0.10f;
            p29.y += -0.13f;
            p30.y += -0.11f;
            p31.y += -0.11f;

            //right
            vertices.Add(p0);
            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
            vertices.Add(p4);
            vertices.Add(p5);
            vertices.Add(p6);
            vertices.Add(p7);
            vertices.Add(p8);
            vertices.Add(p9);
            vertices.Add(p10);
            vertices.Add(p11);
            vertices.Add(p12);
            vertices.Add(p13);
            vertices.Add(p14);
            vertices.Add(p15);

            //left
            vertices.Add(p16);
            vertices.Add(p17);
            vertices.Add(p18);
            vertices.Add(p19);
            vertices.Add(p20);
            vertices.Add(p21);
            vertices.Add(p22);
            vertices.Add(p23);
            vertices.Add(p24);
            vertices.Add(p25);
            vertices.Add(p26);
            vertices.Add(p27);
            vertices.Add(p28);
            vertices.Add(p29);
            vertices.Add(p30);
            vertices.Add(p31);
            #endregion

            if (vertices.Count > 32)
            {
                offsetBorder = offsetIndicesTriangles * 32; triangleBorder = new List<int>
                {
                    //right
                    33+ offsetBorder,1+ offsetBorder,0+ offsetBorder,
                    0+ offsetBorder,32+ offsetBorder,33+ offsetBorder,

                    2+ offsetBorder,1+ offsetBorder,33+ offsetBorder,
                    33+ offsetBorder,34+ offsetBorder,2+ offsetBorder,

                    3+ offsetBorder,2+ offsetBorder,34+ offsetBorder,
                    34+ offsetBorder,35+ offsetBorder,3+ offsetBorder,

                    4+ offsetBorder,3+ offsetBorder,35+ offsetBorder,
                    35+ offsetBorder,36+ offsetBorder,4+ offsetBorder,

                    5+ offsetBorder,4+ offsetBorder,36+ offsetBorder,
                    36+ offsetBorder,37+ offsetBorder,5+ offsetBorder,

                    6+ offsetBorder,5+ offsetBorder,37+ offsetBorder,
                    37+ offsetBorder,38+ offsetBorder,6+ offsetBorder,

                    7+ offsetBorder,6+ offsetBorder,38+ offsetBorder,
                    38+ offsetBorder,39+ offsetBorder,7+ offsetBorder,

                    8+ offsetBorder,7+ offsetBorder,39+ offsetBorder,
                    39+ offsetBorder,40+ offsetBorder,8+ offsetBorder,

                    9+ offsetBorder,8+ offsetBorder,40+ offsetBorder,
                    40+ offsetBorder,41+ offsetBorder,9+ offsetBorder,

                    10+ offsetBorder,9+ offsetBorder,41+ offsetBorder,
                    41+ offsetBorder,42+ offsetBorder,10+ offsetBorder,

                    11+ offsetBorder,10+ offsetBorder,42+ offsetBorder,
                    42+ offsetBorder,43+ offsetBorder,11+ offsetBorder,

                    12+ offsetBorder,11+ offsetBorder,43+ offsetBorder,
                    43+ offsetBorder,44+ offsetBorder,12+ offsetBorder,

                    13+ offsetBorder,12+ offsetBorder,44+ offsetBorder,
                    44+ offsetBorder,45+ offsetBorder,13+ offsetBorder,

                    14+ offsetBorder,13+ offsetBorder,45+ offsetBorder,
                    45+ offsetBorder,46+ offsetBorder,14+ offsetBorder,

                    15+ offsetBorder,14+ offsetBorder,46+ offsetBorder,
                    46+ offsetBorder,47+ offsetBorder,15+ offsetBorder,

                    0+ offsetBorder,15+ offsetBorder,47+ offsetBorder,
                    47+ offsetBorder,32+ offsetBorder,0+ offsetBorder,

                    //left
                    16+ offsetBorder,17+ offsetBorder,49+ offsetBorder,
                    49+ offsetBorder,48+ offsetBorder,16+ offsetBorder,

                    49+ offsetBorder,17+ offsetBorder,18+ offsetBorder,
                    18+ offsetBorder,50+ offsetBorder,49+ offsetBorder,

                    50+ offsetBorder,18+ offsetBorder,19+ offsetBorder,
                    19+ offsetBorder,51+ offsetBorder,50+ offsetBorder,

                    51+ offsetBorder,19+ offsetBorder,20+ offsetBorder,
                    20+ offsetBorder,52+ offsetBorder,51+ offsetBorder,

                    52+ offsetBorder,20+ offsetBorder,21+ offsetBorder,
                    21+ offsetBorder,53+ offsetBorder,52+ offsetBorder,

                    53+ offsetBorder,21+ offsetBorder,22+ offsetBorder,
                    22+ offsetBorder,54+ offsetBorder,53+ offsetBorder,

                    54+ offsetBorder,22+ offsetBorder,23+ offsetBorder,
                    23+ offsetBorder,55+ offsetBorder,54+ offsetBorder,

                    55+ offsetBorder,23+ offsetBorder,24+ offsetBorder,
                    24+ offsetBorder,56+ offsetBorder,55+ offsetBorder,

                    56+ offsetBorder,24+ offsetBorder,25+ offsetBorder,
                    25+ offsetBorder,57+ offsetBorder,56+ offsetBorder,

                    57+ offsetBorder,25+ offsetBorder,26+ offsetBorder,
                    26+ offsetBorder,58+ offsetBorder,57+ offsetBorder,

                    58+ offsetBorder,26+ offsetBorder,27+ offsetBorder,
                    27+ offsetBorder,59+ offsetBorder,58+ offsetBorder,

                    59+ offsetBorder,27+ offsetBorder,28+ offsetBorder,
                    28+ offsetBorder,60+ offsetBorder,59+ offsetBorder,

                    60+ offsetBorder,28+ offsetBorder,29+ offsetBorder,
                    29+ offsetBorder,61+ offsetBorder,60+ offsetBorder,

                    61+ offsetBorder,29+ offsetBorder,30+ offsetBorder,
                    30+ offsetBorder,62+ offsetBorder,61+ offsetBorder,

                    62+ offsetBorder,30+ offsetBorder,31+ offsetBorder,
                    31+ offsetBorder,63+ offsetBorder,62+ offsetBorder,

                    63+ offsetBorder,31+ offsetBorder,16+ offsetBorder,
                    16+ offsetBorder,48+ offsetBorder,63+ offsetBorder,
                };
                offsetIndicesTriangles++;
                triangles.AddRange(triangleBorder);
            }

            delta += step;
            if (delta > 1f)
            {
                if (takeEnd)
                {
                    finish = true;
                }

                takeEnd = true;
                delta = 1f;
            }
        }

        int endTriangle = ((vertices.Count / 32) - 1) * 32;

        triangles.AddRange(new List<int> { 
                                                //right
                                                2 + endTriangle,1 + endTriangle,0 + endTriangle,
                                                15 + endTriangle,2 + endTriangle,0 + endTriangle,
                                                15 + endTriangle,3 + endTriangle,2 + endTriangle,
                                                5 + endTriangle,4 + endTriangle,3 + endTriangle,
                                                6 + endTriangle,5 + endTriangle,3 + endTriangle,
                                                7 + endTriangle,6 + endTriangle,3 + endTriangle,
                                                8 + endTriangle,7 + endTriangle,3 + endTriangle,
                                                10 + endTriangle,9 + endTriangle,8 + endTriangle,
                                                11 + endTriangle,10 + endTriangle,8 + endTriangle,
                                                15 + endTriangle,13 + endTriangle,12 + endTriangle,
                                                15 + endTriangle,12 + endTriangle,11 + endTriangle,
                                                15 + endTriangle,11 + endTriangle,3 + endTriangle,
                                                11 + endTriangle,8 + endTriangle,3 + endTriangle,

                                                //left
                                                28 + endTriangle,29 + endTriangle,30 + endTriangle,
                                                31 + endTriangle,16 + endTriangle,17 + endTriangle,
                                                31 + endTriangle,17 + endTriangle,18 + endTriangle,
                                                19 + endTriangle,20 + endTriangle,21 + endTriangle,
                                                19 + endTriangle,21 + endTriangle,22 + endTriangle,
                                                19 + endTriangle,22 + endTriangle,23 + endTriangle,
                                                18 + endTriangle,19 + endTriangle,31 + endTriangle,
                                                27 + endTriangle,28 + endTriangle,30 + endTriangle,
                                                24 + endTriangle,25 + endTriangle,26 + endTriangle,
                                                19 + endTriangle,23 + endTriangle,24 + endTriangle,
                                                24 + endTriangle,26 + endTriangle,27 + endTriangle,
                                                27 + endTriangle,30 + endTriangle,31 + endTriangle,
                                                24 + endTriangle,27 + endTriangle,31 + endTriangle,
                                                19 + endTriangle,24 + endTriangle,31 + endTriangle,
        }); // base back

        border.sharedMesh.Clear();
        border.sharedMesh.SetVertices(vertices);
        border.sharedMesh.SetTriangles(triangles, 0);
        border.sharedMesh.RecalculateNormals();
        border.sharedMesh.RecalculateTangents();
    }
    #endregion
}