using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager //繼承我們一般使用的Unet網路管理者
{
    private bool m_bFirstPlayerJoined = false;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject playerObj = Instantiate(playerPrefab , Vector3.zero , Quaternion.identity);
        //playerObj.SetActive(true);
        List<Transform> listSpawnPos = NetworkManager.singleton.startPositions; //將所有出生點取到list當中

        if(!m_bFirstPlayerJoined)
        {
            m_bFirstPlayerJoined = true;
            playerObj.transform.position = listSpawnPos[0].position;
        }
        else
        {
            playerObj.transform.position = listSpawnPos[1].position;
        }

        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);

    }

    void SetPortAndAddress()
    {
        NetworkManager.singleton.networkPort = 7777;
        NetworkManager.singleton.networkAddress = "localhost";
    }

    public void HostGame()
    {
        this.SetPortAndAddress();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        SetPortAndAddress();
        NetworkManager.singleton.StartClient();
    }

}
