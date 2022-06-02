using System.Collections;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Text welcomeText;
    [SerializeField] private Button LoginButton;
    [SerializeField] private Button LogoutButton;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Text errmessage;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    private Player player = new Player();
    
    //If a user is logged in, show the logout button and hide the login button
    public void Start()
    {
        string path = Application.persistentDataPath + "/CurrentPlayer.txt";
        StreamReader reader = new StreamReader(path);
        if( new FileInfo( path ).Length > 0 )
        {
            LoginButton.gameObject.SetActive(false);
            LogoutButton.gameObject.SetActive(true);
            welcomeText.text = "Welcome "+reader.ReadToEnd();
        }
        else
        {
            PlayButton.gameObject.SetActive(false);
            OptionsButton.gameObject.SetActive(false);
            QuitButton.gameObject.transform.position = new Vector2(LogoutButton.gameObject.transform.position.x,LogoutButton.gameObject.transform.position.y-104f);
        }


        reader.Close();
    }
    
    //If a user is logged in they can play the game else they have to login
    public void PlayGame()
    {
        errmessage.text = "";
        if (IsLoggedIn())
            SceneManager.LoadScene("Game");
        else
            errmessage.text = "You must be logged in to Play!";

    }

    //Check if a user is logged in
    public bool IsLoggedIn()
    {
        string path = Application.persistentDataPath + "/CurrentPlayer.txt";
        StreamReader reader = new StreamReader(path);
        player.Name = reader.ReadToEnd();
        reader.Close();
        if (player.Name != "")
            return true;
        return false;
    }
    
    //Quits the application
    public void QuitGame()
    {
        Application.Quit();
    }

    //Makes sure that the user is logged in before accessing the options menu
    public void Options()
    {
        if (IsLoggedIn())
        {
            errmessage.text = "";
            optionsMenu.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(false);
        }
        else
            errmessage.text = "You must be logged in!";
    }

    //Logout the user
    public void Logout()
    {
        string path = Application.persistentDataPath + "/CurrentPlayer.txt";
        StreamWriter writer = new StreamWriter(path, false);
        welcomeText.text = "";
        writer.Write("");
        writer.Close();
        QuitButton.gameObject.transform.position = new Vector2(LogoutButton.gameObject.transform.position.x,LogoutButton.gameObject.transform.position.y-104f);

    }
}
