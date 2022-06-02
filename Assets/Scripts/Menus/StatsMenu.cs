using System.Collections;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StatsMenu : MonoBehaviour
{
    [SerializeField] private GameObject statsMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Text chestsOpened;
    private Player player = new Player();

    public void OpenStatsMenu()
    {
        statsMenu.SetActive(true);
        optionsMenu.SetActive(false);
        //start coroutine to get player stats
        StartCoroutine(GETRequest("http://localhost:5000/record/"+player.getName()));

    }
    IEnumerator GETRequest(string uri) {
        // Start a download of the given URI
        var uwr = new UnityWebRequest(uri, "GET");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        //set the header
        uwr.SetRequestHeader("Content-Type", "application/json");
        
        // Send the request then wait for the response
        yield return uwr.SendWebRequest();

        //When an error occurs
        if (uwr.isNetworkError) {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            //deserialize the json
            player = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
            //display the stats
            chestsOpened.text = player.ChestsOpened.ToString();

        }
    }
}
