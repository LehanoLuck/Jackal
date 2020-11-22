using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenerateShip : MonoBehaviour
{
    private ShipTile placingShip;
    private Camera GameCamera;

    public void StartPlacingShip(ShipTile tile)
    {
        if (placingShip != null)
        {
            Destroy(placingShip.gameObject);
        }

        placingShip = Instantiate(tile);
    }

    private void Start()
    {
        GameCamera = Camera.main;
    }

    private void Update()
    {
        if (placingShip != null)
        {
            var groundPlane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                worldPosition.y += 1f;

                placingShip.transform.position = worldPosition;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        var obj = hit.collider.gameObject;
                        if (obj.GetComponent<WaterTile>())
                        {
                            placingShip.Move(obj.GetComponent<WaterTile>());
                            placingShip = null;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Destroy(placingShip.gameObject);
                    placingShip = null;
                }
            }
        }
    }
}
