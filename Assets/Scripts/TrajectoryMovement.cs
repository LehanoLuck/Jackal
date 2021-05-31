using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryMovement : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public void ShowTrajectory(Vector3 targetPosition)
    {
        lineRenderer.enabled = true;

        Vector3 middlePosition = (transform.position + (targetPosition - transform.position) / 2) + Vector3.up * 5;

        Vector3[] points = new Vector3[10];
        lineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            float t = (float)i / points.Length;
            points[i] = Mathf.Pow((1 - t), 2) * transform.position + 2 * (1 - t) * t * middlePosition + Mathf.Pow(t, 2) * targetPosition;
        }

        lineRenderer.SetPositions(points);
    }

    public void HideTrajectory()
    {
        lineRenderer.enabled = false;
    }
}
