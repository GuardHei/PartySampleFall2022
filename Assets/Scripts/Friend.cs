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
    public static Dictionary<string, Dictionary<int, bool>> Friends = new Dictionary<string, Dictionary<int, bool>>();

    [Tooltip("An ID unique to this friend in this scene.")]
    public int id;

    private bool _rescued = false;
    
    void Awake()
    {
        var currentScene = SceneManager.GetActiveScene();
        
        // ensure current friend is logged & restore logged rescue state
        if (Friends.ContainsKey(currentScene.name))
        {
            if (!Friends[currentScene.name].ContainsKey(id))
            {
                Friends[currentScene.name].Add(id, _rescued);
                TotalFriendCount++;
            }
            else
            {
                _rescued = Friends[currentScene.name][id];   
            }
        } 
        else 
        {
            Friends.Add(currentScene.name, new Dictionary<int, bool>() {{id, _rescued}});
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
        Friends[SceneManager.GetActiveScene().name][id] = true;
        Destroy(gameObject);
        Debug.Log("Current FriendCount: " + FriendCount + "; Current TotalFriendCount: " + TotalFriendCount);
    }
}
