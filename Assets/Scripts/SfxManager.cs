using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SfxManager : MonoBehaviour
{
    //-Create a script SfxManager where it has two arrays: first array is an array of SfxPlayer Prefabs, and the next array
    // is the names of the SfxPlayer Prefabs. It should have a method PlaySfx(string name, Vector3 pos), which internally
    // it should Instantiate a new SfxPlayer object that corresponds to the name passed it, and move it to pos in the world space,
    // then call Play() on its SfxPlayer script.

    [SerializeField] GameObject[] sfxPlayerPrefabs;
    [SerializeField] string[] sfxPlayerNames;
    public bool global = true;
    public static SfxManager Instance;


    // Start is called before the first frame update
    void Start() {
        if (global && Instance != null) return;
        if (global) DontDestroyOnLoad(gameObject);

        Instance = this;
        sfxPlayerNames = new string[sfxPlayerPrefabs.Length];
        for(int i=0; i < sfxPlayerPrefabs.Length; i++)//stores all prefab names based on given sfxPlayerFrefabs array
        {
            sfxPlayerNames[i] = sfxPlayerPrefabs[i].name;//(I couldn't confirm whether .name() returns a string [this assumes it does])
        }
    }

    public void PlaySfx(string name, Vector3 pos)
    {
        int index = Array.IndexOf(sfxPlayerNames, name);//find out what sfxPrefab corresponds to the given name
        if (index != -1)
        {
            GameObject sfxObject = (GameObject)Instantiate(sfxPlayerPrefabs[index], pos, Quaternion.identity); //initiate said prefab
            SfxPlayer sfxPlayer = sfxObject.GetComponent<SfxPlayer>(); //access its script //run scrip's Play() method
            DontDestroyOnLoad(sfxObject);
            Destroy(sfxObject, sfxPlayer.Play() + .01f);
            Debug.Log("Played " + name);
        }
        else
        {
            Debug.Log("Sfx does not exist :/");//log if it doesn't exist
        }
    }
}

//Ex. void Start (){
//     // Step one
//     GameObject clone = (GameObject)Instantiate(myPrefab);
 
//     // Step two
//     MyComponent myComponent = clone.GetComponent<MyComponent>();
   
//     // Step three
//     myComponent.DoSomething();
// }