using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Friend : MonoBehaviour
{
    public static int TotalFriendCount = 0;
    public static int FriendCount = 0;
    public static Dictionary<string, bool> Friends = new Dictionary<string, bool>();

    private static GameObject _ui;

    private string _id;
    private bool _rescued = false;

    GameObject LoadUI()
    {
        var uiPrefab = Resources.Load("FriendUI");
        var go = (GameObject)Instantiate(uiPrefab);
        go.name = "FriendUI";
        return go;
    }
    
    void Awake()
    {
        var currentScene = SceneManager.GetActiveScene();
        _id = currentScene.name + gameObject.transform.position.ToString() + gameObject.name;
        
        // get the ui if it already exists, if not, instantiate it
        var uiInScene = GameObject.Find("FriendUI");
        _ui = (uiInScene) ? uiInScene : LoadUI();

        // ensure current friend is logged & restore logged rescue state
        if (Friends.ContainsKey(_id))
        {
            _rescued = Friends[_id];
        } 
        else 
        {
            Friends.Add(_id, _rescued);
            TotalFriendCount++;
        }

        // remove friend if already rescued
        if (_rescued)
            Destroy(gameObject);

        LogFriendStatus();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Rescue();
    }

    void Rescue()
    {
        FriendCount++;
        _rescued = true;
        Friends[_id] = true;
        
        _ui.GetComponent<FriendUIManager>().UpdateFriendCount(FriendCount);
        Destroy(gameObject);
        LogFriendStatus();
    }

    void LogFriendStatus()
    {
        Debug.Log("Current FriendCount: " + FriendCount + "; Current TotalFriendCount: " + TotalFriendCount);
    }
}
