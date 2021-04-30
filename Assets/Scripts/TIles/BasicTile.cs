using Assets.Scripts.TIles;
using Assets.Scripts.TIles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BasicTile: OpenedTile
{
    public List<Pirate> Pirates { get; set; } = new List<Pirate>();
    public int MaxPirateSize = 5;
    public bool isHavePirates => Pirates.Count > 0;
}
