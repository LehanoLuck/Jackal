﻿using Assets.Scripts;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipManager : MonoBehaviour
{
    public ShipTile ShipTileTemplate;
    private ShipTile placingShip;
    private Camera GameCamera;
    public MapManager mapManager;
    //Лога тут быть не должно!!!
    public Text Log;
    public Button CreateShipButton;

    public void StartPlacingShip()
    {
        if((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsMyTurn"])
        {
            if (placingShip != null)
            {
                Destroy(placingShip.gameObject);
            }

            byte id = (byte)mapManager.ShipTiles.Count;
            var ship = PhotonNetwork.Instantiate(ShipTileTemplate.name, Input.mousePosition, Quaternion.identity, 0, new object[] { id });

            placingShip = ship.GetComponent<ShipTile>();
            var name = StepByStepSystem.StartNextTurn();
            Log.text += $"turn to {name}\n";
        }
    }

    private void Start()
    {
        GameCamera = Camera.main;
        RaiseEventManager.EventShipManager = this;
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
                            var tile = obj.GetComponent<WaterTile>();
                            RaiseEventManager.RaiseMoveShipEvent(new ShipMovementData 
                            { Id = placingShip.Id, 
                                XPos = tile.HorizontalIndex, 
                                YPos = tile.VerticalIndex 
                            });
                            placingShip = null;

                            CreateShipButton.gameObject.SetActive(false);
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