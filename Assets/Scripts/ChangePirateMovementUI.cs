using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePirateMovementUI : MonoBehaviour
{
    public GamePlayer SelfPlayer;
    public Toggle MovementToggle;

    public void ChangePirateMoveState()
    {
        SelfPlayer.isMoveWithItem = MovementToggle.isOn;
    }
}
