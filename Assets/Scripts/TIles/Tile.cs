using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    internal Color DefaultColor;
    internal Color EnterColor;

    void Start()
    {
        DefaultColor = GetComponentInChildren<MeshRenderer>().material.color;
        EnterColor = DefaultColor + new Color(0, 0.8f, 0, 0.5f);
    }

    #region MapPosition
    public Tile[][] Map { get; set; }
    public byte XPos { get; set; }
    public byte YPos { get; set; }

    public Vector3 fixedPosition;
    public Vector3 updatePosition;

    protected internal void SetMapPosition(Tile tile)
    {
        this.XPos = tile.XPos;
        this.YPos = tile.YPos;
        this.Map = tile.Map;
        this.Map[XPos][YPos] = this;
    }

    protected internal void SetTransformPosition(Vector3 position)
    {
        this.transform.position = position;
        this.fixedPosition = position;
        this.updatePosition = new Vector3(position.x, position.y + 0.25f, position.z);
    }
    #endregion

    #region PointerEvents
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.position = updatePosition;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        this.transform.position = fixedPosition;
    }
    #endregion

    public void ShowAvailableForMoveCells()
    {
        var tilesList = this.GetPossibleTiles();

        foreach (var tile in tilesList)
        {
            tile.GetComponentInChildren<MeshRenderer>().material.color = tile.EnterColor;
        }
    }

    public void HideAvailableForMoveCells()
    {
        var tilesList = this.GetPossibleTiles();

        foreach (var tile in tilesList)
        {
            tile.GetComponentInChildren<MeshRenderer>().material.color = tile.DefaultColor;
        }
    }

    protected virtual IEnumerable<Tile> GetPossibleTiles()
    {
        var tiles = Map.SelectMany(g => g.ToArray()).Where(t => IsPossibleForMove(t));
        return tiles;
    }

    public virtual bool IsPossibleForMove(Tile targetTile)
    {
        return (targetTile != this &&
                (Math.Abs(this.XPos - targetTile.XPos) < 2) &&
                (Math.Abs(this.YPos - targetTile.YPos) < 2) &&
                IsCanMoveOnWaterTile(targetTile));
    }

    protected bool IsCanMoveOnWaterTile(Tile targetTile)
    {
        if(!(targetTile is WaterTile))
        {
            return true;
        }
        else
        {
            return ((WaterTile)targetTile).PirateShip != null;
        }
    }
    public abstract void EnterPirate(Pirate pirate);
}
