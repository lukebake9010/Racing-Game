using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace RacingGame.TrackMaker
{

    public class SplineBoundsGenerator : MonoBehaviour
    {
        public SplineContainer centerSplineContainer;
        public SplineContainer insideSplineContainer;
        public SplineContainer outsideSplineContainer;

        public float halfRoadWidth = 0.5f;
        public int samplePoints = 30;

        [ContextMenu("Generate Bounds Splines")]
        public void GenerateBounds()
        {
            Spline centerSpline = centerSplineContainer.Spline;

            List<BezierKnot> insideKnots = new();
            List<BezierKnot> outsideKnots = new();

            for (int i = 0; i <= samplePoints; i++)
            {
                float t = i / (float)samplePoints;

                Vector3 pos = centerSpline.EvaluatePosition(t);
                Vector3 tangent = ((Vector3)(centerSpline.EvaluateTangent(t))).normalized;

                Vector2 normal = new Vector2(-tangent.y, tangent.x);

                Vector3 left = pos + (Vector3)(normal * halfRoadWidth);
                Vector3 right = pos - (Vector3)(normal * halfRoadWidth);

                insideKnots.Add(new BezierKnot(left));
                outsideKnots.Add(new BezierKnot(right));
            }

            if (insideSplineContainer == null || outsideSplineContainer == null)
            {
                GameObject boundsContainer = GameObject.Instantiate(new GameObject());
                boundsContainer.name = "Bounds";

                if(insideSplineContainer == null)
                {
                    GameObject innerBoundsContainer = GameObject.Instantiate(new GameObject());
                    innerBoundsContainer.name = "Inner Bounds";
                    innerBoundsContainer.transform.parent = boundsContainer.transform;
                    insideSplineContainer = innerBoundsContainer.AddComponent<SplineContainer>();
                    innerBoundsContainer.AddComponent<SplineExtrude>();
                }
                if(outsideSplineContainer == null)
                {
                    GameObject outerBoundsContainer = GameObject.Instantiate(new GameObject());
                    outerBoundsContainer.name = "Outer Bounds";
                    outerBoundsContainer.transform.parent = boundsContainer.transform;
                    outsideSplineContainer = outerBoundsContainer.AddComponent<SplineContainer>();
                    outerBoundsContainer.AddComponent<SplineExtrude>();
                }
            }

            insideSplineContainer.Spline = new Spline(insideKnots);
            outsideSplineContainer.Spline = new Spline(outsideKnots);
        }
    }
}