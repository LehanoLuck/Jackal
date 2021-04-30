using Assets.Scripts;
using Assets.Scripts.TIles;
using Assets.Scripts.TIles.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClosedTile : Tile, IPirateEnter
{
    public OpenedTile LinkedTile;

    public OpenedTile OpenTile()
    {
        Instantiate(LinkedTile, this.transform.parent);
        LinkedTile.SetTransformPosition(this.transform.position);
        LinkedTile.SetMapPosition(this);
        LinkedTile.DoActionAfterOpenning();

        Destroy(gameObject);
        return LinkedTile;
    }

    public void EnterPirate(Pirate pirate)
    {
        var tile = OpenTile();
        tile.EnterPirate(pirate);
    }
}
