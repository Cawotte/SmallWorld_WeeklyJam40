using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    /* 
     * AudioManager, contains every Sounds ans music played, in a Sound[] array, same for musics.
     * AudioManager is Singleton, so any other object can access it to reach the sounds they want to play.
     * 
     * */
     
    //Sounds
    public Sound[] sounds;

    //Musics
    public Sound[] musics;
    int num_music = 0;
    public Sound music;

    
    //Singleton pattern
    public static AudioManager instance;

	void Awake () {

        //Singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        //WHAT ?
        Screen.SetResolution(1280, 600, false);

        //Sound.source is an AudioSource component, it plays the music. For each Sound in sounds[], we
        //initialize there an AudioSource with the value found in the Sound object.
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        //Choose a random first music.
        //num_music = UnityEngine.Random.Range(0, musics.Length);

        //We initialize the music the same way than for the sound, but just once this time.
        music.source = gameObject.AddComponent<AudioSource>();
        music.source.clip = musics[num_music].clip;
        music.source.volume = musics[num_music].volume;
        music.source.pitch = musics[num_music].pitch;
        music.source.loop = true;
        music.source.Play();

        //Play ocean sound
        Find("ocean").source.Play();

	}
    
    //Return the Sound object with the name given. Used by other objets to access the sounds they want to play.
    public Sound Find(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound:" + name + " not found!");
        }
        return s;
    }
    
    //Mute all SFX sounds
    public void MuteAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.mute = true;
    }

    //Unmute all sounds
    /*
    public void UnMuteAllSounds()
    {
        //On vérifie que le jeu n'est pas en pause et que le bouton qui gère l'activation des bruitages est bien allumé.
        //On ne veut pas que les bruitages continuent d'être joué quand le jeu est en pause.
        //On ne veut pas que les bruitages reprennent quand on reprend la partie si on les avait coupé.
        if (!GameManager.getInstance().gamePaused && GameObject.Find("Audio").GetComponent<Buttons_Behaviour>().On)
        {
            foreach (Sound s in sounds)
            {
                s.source.mute = false;
            }
        }
            
    } */

    //Pause all sounds.
    public void PauseAll()
    {
        foreach (Sound s in sounds)
            s.source.Pause();
        music.source.Pause();
    }
    //Resume all sounds.
    public void ResumeAll()
    {
        foreach (Sound s in sounds)
            s.source.UnPause();
        music.source.UnPause();
    }

    //Change the music for the next one in the array musics[]
    public void nextMusic()
    {
        if ( musics.Length-1 > num_music )
            num_music++;
        else
            num_music = 0;

        music.source.Stop(); //We stop the previous one.
        //We replace it with the new one.
        music.source.clip = musics[num_music].clip;
        music.source.volume = musics[num_music].volume;
        music.source.pitch = musics[num_music].pitch;
        music.source.Play();
    }

    //Singleton pattern.
    //We get the instant of the AudioManager so it can be used by other objects.
    public static AudioManager getInstance()
    {
        return instance;
    }
}
