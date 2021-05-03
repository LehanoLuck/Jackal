using Assets.Scripts.TIles.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public ICoinInteractor CurrentTile;
    
    public void Move(GroundTile tile)
    {
        CurrentTile.PopCoin();
        tile.AddCoin(this);
        this.CurrentTile = tile;
        //Исправить, при добавлении анимации
        this.transform.position = tile.transform.position;
    }
}
