using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float sensetive = 5f;
    public float zoomSensitive = 1f;
    private float minZoom = 10f;
    private float maxZoom = 60f;
    public Camera camera;
    private Vector3 target;
    public float damping = 100f;

    void Start()
    {
        target = this.transform.position;
    }

    void Update()
    {
        ZoomCamera(Input.GetAxis("Mouse ScrollWheel"));

        Vector3 point = camera.ScreenToViewportPoint(Input.mousePosition);

        //TODO оптимизировать код
        //if (Input.GetMouseButton(0))
        //{
        //    Debug.Log("Нажата ЛКМ");
        //}
        //else
        //{
            if (point.x < 0.05f)
            {
                if (point.y < 0.05f)
                {
                    target = new Vector3(transform.position.x - sensetive, transform.position.y, transform.position.z - sensetive);
                }
                else if (point.y > 0.95f)
                {
                    target = new Vector3(transform.position.x - sensetive, transform.position.y, transform.position.z + sensetive);
                }
                else
                {
                    target = new Vector3(transform.position.x - sensetive, transform.position.y, transform.position.z);
                }
            }
            else if (point.x > 0.95f)
            {
                if (point.y < 0.05f)
                {
                    target = new Vector3(transform.position.x + sensetive, transform.position.y, transform.position.z - sensetive);
                }
                else if (point.y > 0.95f)
                {
                    target = new Vector3(transform.position.x + sensetive, transform.position.y, transform.position.z + sensetive);
                }
                else
                {
                    target = new Vector3(transform.position.x + sensetive, transform.position.y, transform.position.z);
                }
            }
            else
            {
                if (point.y < 0.05f)
                {
                    target = new Vector3(transform.position.x, transform.position.y, transform.position.z - sensetive);
                }
                else if (point.y > 0.95f)
                {
                    target = new Vector3(transform.position.x, transform.position.y, transform.position.z + sensetive);
                }
            }

            Vector3 currentPosition = Vector3.Lerp(transform.position, target, damping * Time.deltaTime);
            transform.position = currentPosition;
        //}
    }

    void ZoomCamera(float increment)
    {
        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView - increment * zoomSensitive, minZoom, maxZoom);
    }
}
