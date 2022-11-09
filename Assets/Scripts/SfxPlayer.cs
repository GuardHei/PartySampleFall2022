using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    //-Create a script SfxPlayer and its corresponding prefab where it has access to an AudioSource, an array of AudioClips,
    // and a method Play(), which will randomly pick one of the clips and play it for one shot.

    //serialized field makes array private, but it also shows up in the editor
    [SerializeField] AudioClip[] clips; //stores GameObject's clips into array
    AudioSource myAudioSource; //create AudioSource object

    // Start is called before the first frame update
    void Awake()
    {
        myAudioSource = GetComponent<AudioSource>(); //Assigns myAudioSource object to the GameObject's AudioSource
    }

    public float Play()
    {
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];//pick out random clip from array of clips
        myAudioSource.PlayOneShot(clip);//from the AudioSource, play said random clip (oneshot)
        return clip.length;
    }
}
