using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiNetwork : MonoBehaviour
{
    public GameObject network;
    public GameObject panelNetwork;

    void Start()
    {
        
    }

    public void StartServer()
    {
        network.GetComponent<GameServer>().enabled = true;
        panelNetwork.SetActive(false);
    }

    public void StartClient()
    {
        network.GetComponent<GameClient>().enabled = true;
        panelNetwork.SetActive(false);
    }
}
