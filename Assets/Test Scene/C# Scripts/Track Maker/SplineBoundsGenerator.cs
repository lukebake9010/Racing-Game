using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RacingGame.TrackMaker
{
    public class SplineBoundsGenerator : MonoBehaviour
    {
        [Header("Spline Containers")]
        public SplineContainer centerSplineContainer;
        public SplineContainer insideSplineContainer;
        public SplineContainer outsideSplineContainer;

        [Header("Spline Generation")]
        [Tooltip("Optional: If no SplineExtrude is found on the center spline, this value (half of total road width) will be used.")]
        public float fallbackHalfRoadWidth = 2.5f;

        [Tooltip("Density of initial evaluation points along the spline per meter. Higher means more accurate initial path for simplification.")]
        [Range(1f, 20f)]
        public float evaluationPointsPerMeter = 5f;

        [Tooltip("Maximum distance a point can deviate from the simplified line segment. Higher value = fewer knots, but less accurate to original curve. This affects the SplineContainer's actual knots.")]
        [Range(0.01f, 10f)]
        public float simplificationTolerance = 0.1f;

        [Header("2D Collider Settings")]
        [Tooltip("Enable to generate 2D EdgeColliders on the boundary SplineContainers.")]
        public bool generate2DColliders = false;

        [Tooltip("Density of points for the EdgeCollider2D per meter of spline length. Higher value = smoother collider. Independent of spline knot simplification.")]
        [Range(1f, 50f)]
        public float colliderPointsPerMeter = 10f; //Edge collider resolution


        [ContextMenu("Generate Bezier Bounds")]
        public void GenerateBezierBounds()
        {
            if (centerSplineContainer == null)
            {
                Debug.LogWarning("Please assign the Center Spline Container.");
                return;
            }

            if (insideSplineContainer == null || outsideSplineContainer == null)
            {
                GameObject boundsParent = GameObject.Find("Spline Bounds") ?? new GameObject("Spline Bounds");
                if (boundsParent.transform.parent != this.transform.parent)
                {
                    boundsParent.transform.parent = this.transform.parent;
                }

                if (insideSplineContainer == null)
                {
                    GameObject innerObj = new GameObject("Inner_Road_Bound");
                    innerObj.transform.parent = boundsParent.transform;
                    insideSplineContainer = innerObj.AddComponent<SplineContainer>();
                }
                if (outsideSplineContainer == null)
                {
                    GameObject outerObj = new GameObject("Outer_Road_Bound");
                    outerObj.transform.parent = boundsParent.transform;
                    outsideSplineContainer = outerObj.AddComponent<SplineContainer>();
                }
            }

            float currentHalfRoadWidth = fallbackHalfRoadWidth;
            SplineExtrude extrude = centerSplineContainer.GetComponent<SplineExtrude>();
            if (extrude != null)
            {
                currentHalfRoadWidth = extrude.Radius;
            }

            var centerSpline = centerSplineContainer.Spline;
            if (centerSpline.Count == 0)
            {
                Debug.LogWarning("Center spline has no knots. Cannot generate bounds.");
                return;
            }

            float splineLength = centerSpline.GetLength();

            //High-resolution points for spline simplification
            int numEvaluationPointsForSpline = Mathf.Max(2, Mathf.CeilToInt(splineLength * evaluationPointsPerMeter));
            List<float3> highResInsidePointsForSpline = new List<float3>();
            List<float3> highResOutsidePointsForSpline = new List<float3>();
            List<quaternion> highResRotationsForSpline = new List<quaternion>();

            for (int i = 0; i < numEvaluationPointsForSpline; i++)
            {
                float t;
                if (centerSpline.Closed && numEvaluationPointsForSpline > 0)
                {
                    t = (float)i / numEvaluationPointsForSpline;
                }
                else if (numEvaluationPointsForSpline > 1)
                {
                    t = (float)i / (numEvaluationPointsForSpline - 1);
                }
                else
                {
                    t = 0f;
                }

                float3 evalPos, evalTangent, evalUp;
                SplineUtility.Evaluate(centerSpline, t, out evalPos, out evalTangent, out evalUp);

                evalTangent = math.normalize(evalTangent);
                evalUp = math.normalize(evalUp);

                float3 offsetDirection = math.normalize(math.cross(evalTangent, evalUp));
                if (math.lengthsq(offsetDirection) < 0.0001f)
                {
                    offsetDirection = math.normalize(math.cross(evalTangent, math.right()));
                    if (math.lengthsq(offsetDirection) < 0.0001f)
                    {
                        offsetDirection = math.normalize(math.cross(evalTangent, math.forward()));
                    }
                    if (math.lengthsq(offsetDirection) < 0.0001f && math.lengthsq(evalTangent) > 0.0001f)
                    {
                        float3 tempUp = math.abs(math.dot(evalTangent, math.up())) < 0.9f ? math.up() : math.forward();
                        offsetDirection = math.normalize(math.cross(evalTangent, tempUp));
                    }
                    else if (math.lengthsq(offsetDirection) < 0.0001f)
                    {
                        offsetDirection = math.right();
                    }
                }

                float3 offsetAmount = offsetDirection * currentHalfRoadWidth;

                highResInsidePointsForSpline.Add(evalPos - offsetAmount);
                highResOutsidePointsForSpline.Add(evalPos + offsetAmount);
                highResRotationsForSpline.Add(quaternion.LookRotation(evalTangent, evalUp));
            }

            // Simplify points for the SplineContainer knots
            List<BezierKnot> simplifiedInsideKnots = SimplifyPointsToKnots(highResInsidePointsForSpline, highResRotationsForSpline, simplificationTolerance, centerSpline.Closed);
            List<BezierKnot> simplifiedOutsideKnots = SimplifyPointsToKnots(highResOutsidePointsForSpline, highResRotationsForSpline, simplificationTolerance, centerSpline.Closed);

            Spline newInsideSpline = new Spline(simplifiedInsideKnots, centerSpline.Closed);
            Spline newOutsideSpline = new Spline(simplifiedOutsideKnots, centerSpline.Closed);

            SetSplineKnotsToAutoTangentMode(newInsideSpline);
            SetSplineKnotsToAutoTangentMode(newOutsideSpline);

            insideSplineContainer.Spline = newInsideSpline;
            outsideSplineContainer.Spline = newOutsideSpline;

            // Optional: Add SplineExtrude to the new bounds if they don't have it
            if (insideSplineContainer.GetComponent<SplineExtrude>() == null)
            {
                var se = insideSplineContainer.gameObject.AddComponent<SplineExtrude>();
                se.Radius = 0.1f;
                se.SegmentsPerUnit = 2f;
            }
            if (outsideSplineContainer.GetComponent<SplineExtrude>() == null)
            {
                var se = outsideSplineContainer.gameObject.AddComponent<SplineExtrude>();
                se.Radius = 0.1f;
                se.SegmentsPerUnit = 2f;
            }

            //Generate 2D Colliders using EdgeCollider2D with its own resolution
            if (generate2DColliders)
            {
                // Generate a NEW set of high-resolution points specifically for the colliders
                // using colliderPointsPerMeter. This ensures collider fidelity
                // regardless of the spline's simplificationTolerance.
                int numEvaluationPointsForCollider = Mathf.Max(2, Mathf.CeilToInt(splineLength * colliderPointsPerMeter));
                List<float3> highResInsidePointsForCollider = new List<float3>();
                List<float3> highResOutsidePointsForCollider = new List<float3>();

                for (int i = 0; i < numEvaluationPointsForCollider; i++)
                {
                    float t;
                    if (centerSpline.Closed && numEvaluationPointsForCollider > 0)
                    {
                        t = (float)i / numEvaluationPointsForCollider;
                    }
                    else if (numEvaluationPointsForCollider > 1)
                    {
                        t = (float)i / (numEvaluationPointsForCollider - 1);
                    }
                    else
                    {
                        t = 0f;
                    }

                    float3 evalPos, evalTangent, evalUp;
                    SplineUtility.Evaluate(centerSpline, t, out evalPos, out evalTangent, out evalUp);

                    evalTangent = math.normalize(evalTangent);
                    evalUp = math.normalize(evalUp);

                    float3 offsetDirection = math.normalize(math.cross(evalTangent, evalUp));
                    if (math.lengthsq(offsetDirection) < 0.0001f)
                    {
                        // Fallback for straight segments
                        offsetDirection = math.normalize(math.cross(evalTangent, math.right()));
                        if (math.lengthsq(offsetDirection) < 0.0001f)
                        {
                            offsetDirection = math.normalize(math.cross(evalTangent, math.forward()));
                        }
                        if (math.lengthsq(offsetDirection) < 0.0001f && math.lengthsq(evalTangent) > 0.0001f)
                        {
                            float3 tempUp = math.abs(math.dot(evalTangent, math.up())) < 0.9f ? math.up() : math.forward();
                            offsetDirection = math.normalize(math.cross(evalTangent, tempUp));
                        }
                        else if (math.lengthsq(offsetDirection) < 0.0001f)
                        {
                            offsetDirection = math.right();
                        }
                    }

                    float3 offsetAmount = offsetDirection * currentHalfRoadWidth;

                    highResInsidePointsForCollider.Add(evalPos - offsetAmount);
                    highResOutsidePointsForCollider.Add(evalPos + offsetAmount);
                }

                Generate2DEdgeCollider(insideSplineContainer, highResInsidePointsForCollider, centerSpline.Closed);
                Generate2DEdgeCollider(outsideSplineContainer, highResOutsidePointsForCollider, centerSpline.Closed);
            }

            Debug.Log($"Generated Inside Spline with {simplifiedInsideKnots.Count} knots. Original evaluation points for spline: {numEvaluationPointsForSpline}");
            Debug.Log($"Generated Outside Spline with {simplifiedOutsideKnots.Count} knots. Original evaluation points for spline: {numEvaluationPointsForSpline}");
        }

        /// <summary>
        /// Generates an EdgeCollider2D from a list of high-resolution 3D points.
        /// Assumes 2D game is on the XY plane.
        /// </summary>
        /// <param name="splineContainer">The SplineContainer to add the collider to.</param>
        /// <param name="points3D">The 3D points to define the 2D collider path.</param>
        /// <param name="isClosed">Whether the original spline was closed, to close the EdgeCollider2D if needed.</param>
        private void Generate2DEdgeCollider(SplineContainer splineContainer, List<float3> points3D, bool isClosed)
        {
            if (points3D == null || points3D.Count == 0)
            {
                Debug.LogWarning($"No points provided for 2D collider generation on {splineContainer.name}.");
                return;
            }

            List<Vector2> colliderPoints2D = new List<Vector2>();
            // Convert 3D points to 2D (world space)
            foreach (var point3D in points3D)
            {
                // Convert 3D point from centerSpline's world space to the EdgeCollider's local space.
                // Assuming the EdgeCollider is on the splineContainer.gameObject,
                // we need to convert the point relative to that object's transform.
                Vector3 localPoint = splineContainer.transform.InverseTransformPoint(point3D);
                colliderPoints2D.Add(new Vector2(localPoint.x, localPoint.y)); // Project to XY plane
            }

            // Get or add an EdgeCollider2D component
            EdgeCollider2D edgeCollider = splineContainer.gameObject.GetComponent<EdgeCollider2D>();
            if (edgeCollider == null)
            {
                edgeCollider = splineContainer.gameObject.AddComponent<EdgeCollider2D>();
            }

            if (colliderPoints2D.Count < 2)
            {
                Debug.LogWarning($"Not enough points ({colliderPoints2D.Count}) to create an EdgeCollider2D for {splineContainer.name}.");
                edgeCollider.points = new Vector2[0];
                return;
            }

            // Handle closed splines for EdgeCollider2D
            if (isClosed && colliderPoints2D.Count > 1)
            {
                // EdgeCollider2D does not automatically close. If the spline is closed,
                // we must explicitly add the first point to the end to close the loop.
                if (Vector2.Distance(colliderPoints2D[0], colliderPoints2D[colliderPoints2D.Count - 1]) > 0.01f)
                {
                    colliderPoints2D.Add(colliderPoints2D[0]);
                }
            }

            edgeCollider.points = colliderPoints2D.ToArray();

            Debug.Log($"Generated 2D EdgeCollider for {splineContainer.name} with {colliderPoints2D.Count} points.");
        }


        private List<BezierKnot> SimplifyPointsToKnots(List<float3> points, List<quaternion> rotations, float tolerance, bool isClosed)
        {
            List<BezierKnot> simplifiedKnots = new List<BezierKnot>();

            if (points.Count < 2)
            {
                if (points.Count == 1)
                {
                    simplifiedKnots.Add(new BezierKnot(points[0], float3.zero, float3.zero, rotations[0]));
                }
                return simplifiedKnots;
            }

            simplifiedKnots.Add(new BezierKnot(points[0], float3.zero, float3.zero, rotations[0]));
            int lastKeptIndex = 0;

            for (int i = 1; i < points.Count; i++)
            {
                int endOfSegmentIndex = i;
                if (isClosed && i == points.Count - 1)
                {
                    endOfSegmentIndex = 0;
                }
                else if (i == points.Count - 1 && !isClosed)
                {
                    simplifiedKnots.Add(new BezierKnot(points[i], float3.zero, float3.zero, rotations[i]));
                    break;
                }

                float maxDistance = 0f;
                int furthestPointIndex = i;

                for (int j = lastKeptIndex + 1; j <= i; j++)
                {
                    float dist = PointLineSegmentDistance(points[j], points[lastKeptIndex], points[endOfSegmentIndex]);
                    if (dist > maxDistance)
                    {
                        maxDistance = dist;
                        furthestPointIndex = j;
                    }
                }

                if (maxDistance > tolerance)
                {
                    simplifiedKnots.Add(new BezierKnot(points[furthestPointIndex], float3.zero, float3.zero, rotations[furthestPointIndex]));
                    lastKeptIndex = furthestPointIndex;
                }
            }

            if (!isClosed && lastKeptIndex != points.Count - 1 && points.Count > 0)
            {
                simplifiedKnots.Add(new BezierKnot(points[points.Count - 1], float3.zero, float3.zero, rotations[points.Count - 1]));
            }

            return simplifiedKnots;
        }

        private float PointLineSegmentDistance(float3 point, float3 lineStart, float3 lineEnd)
        {
            float3 line = lineEnd - lineStart;
            float lineLengthSq = math.lengthsq(line);

            if (lineLengthSq == 0.0f)
            {
                return math.distance(point, lineStart);
            }

            float t = math.dot(point - lineStart, line) / lineLengthSq;
            t = math.clamp(t, 0.0f, 1.0f);

            float3 closestPoint = lineStart + t * line;

            return math.distance(point, closestPoint);
        }

        private void SetSplineKnotsToAutoTangentMode(Spline spline)
        {
            for (int i = 0; i < spline.Count; i++)
            {
                spline.SetTangentMode(i, TangentMode.AutoSmooth);
            }
        }
    }
}