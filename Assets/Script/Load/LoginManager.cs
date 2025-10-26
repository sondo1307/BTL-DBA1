using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public class LoginData
{
    public string server;
    public string port;
    public string password;
}


public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _server;
    [SerializeField] private TMP_InputField _port;
    [SerializeField] private TMP_InputField _password;
    [SerializeField] private Button _login;
    [SerializeField] private MySQLManager _mySqlManager;
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _mainUIGob;
    
    
    private void Start()
    {
        _login.onClick.AddListener(SaveData);
    }

    private void SaveData()
    {
        var dataToSave = new LoginData()
        {
            server = _server.text,
            port = _port.text,
            password = _password.text,
        };
        string json = JsonUtility.ToJson(dataToSave);
        print(json);
        string path = Path.Combine(Application.persistentDataPath, SonConst.LoginFileName + ".json");

        try
        {
            File.WriteAllText(path, json);
            Debug.Log("Data saved to: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save data: " + e.Message);
        }

        _mySqlManager.enabled = true;
        _loginPanel.gameObject.SetActive(false);
        _mainUIGob.SetActive(true);
    }
}
