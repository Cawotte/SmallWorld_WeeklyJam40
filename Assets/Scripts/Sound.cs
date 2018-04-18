
using UnityEngine.Audio;
using UnityEngine;


/*
 * Class used to define a sound, any playable sound clips.
 * The audio manager contains an Array of 'Sound' which will all contain a sound.
 * */

[System.Serializable]
public class Sound  {

    public string name; //sound name
    public AudioClip clip; //sound asset
    [Range(0f, 1f)] public float volume;
    [Range(.1f, 3f)] public float pitch;
    public bool loop;

    //component which will play the sound
    [HideInInspector] public AudioSource source;

}
