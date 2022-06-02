using System.Collections;
using System.IO;
using DefaultNamespace;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] private InputField username;
    [SerializeField] private InputField password;
    [SerializeField] private Text errmessage;
    [SerializeField] private GameObject loginMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Text welcomeText;
    Player player = new Player();
    
    //Check if usernamefield is empty else login
    public void Login()
    { 
        if (username.text != "") {
            player.Name = username.text;
            StartCoroutine(getRequest("http://localhost:5000/record/" + player.Name));
        }else{
            errmessage.text = "Fill out all boxes";
        }
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
            var temp = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
            //if username is found in database
            if (temp != null)
            {
                //if password is correct
                if (temp.Password == password.text)
                {
                    //Serialize player object from json
                    player = JsonConvert.DeserializeObject<Player>(uwr.downloadHandler.text);
                    loginMenu.SetActive(false);
                    welcomeText.text = "Welcome " + player.Name;
                    
                    string path = Application.persistentDataPath + "/CurrentPlayer.txt"; 
                    StreamWriter writer = new StreamWriter(path, false);
                    writer.Write(player.Name);
                    writer.Close(); 
                    mainMenu.SetActive(true);
                    loginButton.gameObject.SetActive(false);
                    logoutButton.gameObject.SetActive(true);
                    PlayButton.gameObject.SetActive(true);
                    OptionsButton.gameObject.SetActive(true);
                    QuitButton.gameObject.transform.position = new Vector2(OptionsButton.gameObject.transform.position.x, OptionsButton.gameObject.transform.position.y-103.53f);
                    errmessage.text = "";

                }
                else
                {
                    errmessage.text = "Incorrect Password!";
                }
            }else {
                errmessage.text = "No such User";
            }
        }
        
    }
}
