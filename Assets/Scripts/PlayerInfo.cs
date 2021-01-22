using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerInfo : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
    public PhotonView photonView;

    public Toggle toggleReady;

    public Text PlayerName;

    public bool isReady;

    private Hashtable customProperties;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            toggleReady.enabled = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(isReady);
        }
        else
        {
            isReady = (bool)stream.ReceiveNext();
            this.toggleReady.SetIsOnWithoutNotify(isReady);
        }

        this.customProperties["IsReady"] = isReady;
        PhotonNetwork.SetPlayerCustomProperties(customProperties);
    }

    public void SwitchIsReady()
    {
        if(photonView.IsMine)
        {
            isReady = !isReady;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var parent = FindObjectOfType<PlayersInLobby>().transform;
        this.transform.SetParent(parent);
        this.transform.localScale = new Vector3(1, 1, 1);
        this.PlayerName.text = photonView.Owner.NickName;

        customProperties = new Hashtable();
        customProperties.Add("IsReady", isReady);
    }
}
