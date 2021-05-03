using Assets.Scripts;
using Assets.Scripts.TIles;
using Assets.Scripts.TIles.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClosedTile : Tile
{
    public OpenedTile LinkedTile;

    public OpenedTile OpenTile()
    {
        var tile = Instantiate(LinkedTile, this.transform.parent);
        tile.SetMapPosition(this);
        tile.SetTransformPosition(this.transform.position);

        Destroy(gameObject);
        return tile;
    }

    public override void EnterPirate(Pirate pirate)
    {
        var tile = OpenTile();
        tile.Open(pirate);
    }
}
