using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArrowTile : GroundTile
{
    protected int rotationAngle;

    public override void EnterPirate(Pirate pirate)
    {
        base.EnterPirate(pirate);

        pirate.isActive = true;
    }

    public override void Open(Pirate pirate)
    {
        SetRandomRotationAngle(pirate);
        EnterPirate(pirate);
    }

    private void SetRandomRotationAngle(Pirate pirate)
    {
        this.rotationAngle = (XPos ^ YPos ^ pirate.Ship.CurrentTile.YPos ^ pirate.Ship.CurrentTile.XPos) % 4;
    }

    public (int, int) RotateTile(int angle, int x, int y)
    {
        int xPos;
        int yPos;

        switch (angle)
        {
            case 1:
                xPos = y;
                yPos = -x;
                break;
            case 2:
                xPos = -x;
                yPos = -y;
                break;
            case 3:
                yPos = x;
                xPos = -y;
                break;
            default:
                xPos = x;
                yPos = y;
                break;
        }

        return (xPos, yPos);
    }
}
