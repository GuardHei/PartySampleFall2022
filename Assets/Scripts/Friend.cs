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

    private string _id;
    private bool _rescued = false;
    
    void Awake()
    {
        var currentScene = SceneManager.GetActiveScene();
        _id = currentScene.name + gameObject.transform.position.ToString() + gameObject.name;

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

        if (_rescued)
            Destroy(gameObject);
        
        Debug.Log("Current FriendCount: " + FriendCount + "; Current TotalFriendCount: " + TotalFriendCount);
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
        Destroy(gameObject);
        Debug.Log("Current FriendCount: " + FriendCount + "; Current TotalFriendCount: " + TotalFriendCount);
    }
}
