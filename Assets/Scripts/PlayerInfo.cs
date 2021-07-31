using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[ExecuteInEditMode]
public class PlayerInfo : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
    public PhotonView photonView;

    public Toggle toggleReady;

    public Text PlayerName;

    public bool isReady;

    private Hashtable customProperties;

    private float sideRatio;

    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            toggleReady.enabled = false;
        }

        rectTransform = GetComponent<RectTransform>();
    }

    
    void Update()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.height * 100 / 130);
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
            this.customProperties["IsReady"] = isReady;
            PhotonNetwork.SetPlayerCustomProperties(customProperties);
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
