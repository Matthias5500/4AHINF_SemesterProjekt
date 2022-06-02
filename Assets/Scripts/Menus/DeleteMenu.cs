using System.Collections;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DeleteMenu : MonoBehaviour
{
  [SerializeField] private InputField password;
  [SerializeField] private Button loginButton;
  [SerializeField] private Button logoutButton;
  [SerializeField] private Button PlayButton;
  [SerializeField] private Button QuitButton;
  [SerializeField] private Button OptionsButton;
  [SerializeField] private Text confirmationText;
  [SerializeField] private GameObject mainMenu;
  [SerializeField] private GameObject deleteMenu;
  [SerializeField] private Text welcometext;
  [SerializeField] private Text errmessage;
  private Player player = new Player();
  
  //Sets the text that says "Do you really want to delete your account?"
  public void DeleteConfirmation()
  {
    string path = Application.persistentDataPath + "/CurrentPlayer.txt";
    StreamReader reader = new StreamReader(path);
    player.Name = reader.ReadToEnd();
    reader.Close();
    confirmationText.text = "Do you really want to delete "+player.Name;
  }

  public void Delete()
  {
    StartCoroutine(getRequest("http://localhost:5000/record/" + player.Name));
  }

  //sets some UI elements to invisible and some to visible
  private void AfterDeletion()
  {
    mainMenu.gameObject.SetActive(true);
    deleteMenu.gameObject.SetActive(false);
    welcometext.text = "Sucesfully deletet "+player.Name;
    string path = Application.persistentDataPath + "/CurrentPlayer.txt";    
    StreamWriter writer = new StreamWriter(path, false);
    writer.Write("");
    writer.Close();
    errmessage.text = "";
    loginButton.gameObject.SetActive(true);
    logoutButton.gameObject.SetActive(false);
    PlayButton.gameObject.SetActive(false);
    QuitButton.gameObject.SetActive(true);
    OptionsButton.gameObject.SetActive(false);
    QuitButton.gameObject.transform.position = new Vector2(logoutButton.gameObject.transform.position.x,logoutButton.gameObject.transform.position.y-104f);

  }
  
  IEnumerator getRequest(string uri)
  {
    var uwr = new UnityWebRequest(uri, "GET");
    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    uwr.SetRequestHeader("Content-Type", "application/json");

    yield return uwr.SendWebRequest();

    if (uwr.isNetworkError)
    {
      Debug.Log("Error While Sending: " + uwr.error);
      errmessage.text = "No internet connection";
    }
    else
    {
      Debug.Log("Received: " + uwr.downloadHandler.text);
      player = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
      if (password.text == player.Password)
      {
        StartCoroutine(deleteRequest("http://localhost:5000/" + player.Name));
        AfterDeletion();
      }
      else
        errmessage.text = "Wrong password";

    }
        
  }
  
  IEnumerator deleteRequest(string url)
  {
    UnityWebRequest uwr = UnityWebRequest.Delete(url);
    yield return uwr.SendWebRequest();

    if (uwr.isNetworkError)
    {
      Debug.Log("Error While Sending: " + uwr.error);
    }
    else
    {
      Debug.Log("Deleted");
    }
  }
}
