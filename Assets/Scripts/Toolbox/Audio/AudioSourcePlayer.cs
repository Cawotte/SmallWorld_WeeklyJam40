namespace Cawotte.Toolbox.Audio
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Component that enables a gameobject to play sounds. 
    /// Will automatically create AudioSources and manage them.
    /// <para />
    /// Call a sound to play through this class. Compatible with spatialization.
    /// </summary>
    public class AudioSourcePlayer : MonoBehaviour
    {
        /*
         * Component that enables a gameobject to play sounds. 
         * 
         * Call a Sound through its methods, and it will fetch it in the AudioManager, then
         * create a AudioSource on the gameobject to play them. 
         * 
         * When a sound stops playing, disable it and keep track of it, 
         * so we can re-use AudioSource components instead of instanting new ones each time (Pooling)
         * 
         */

        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private Sound[] playableSounds = null;

        /// <summary>
        /// Keep tracks of all sounds being currently played.
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private List<PlayingSound> currentlyPlaying = new List<PlayingSound>();
        
        /// <summary>
        /// Keep tracks of all existing unused AudioSources (Pooling)
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private Stack<AudioSource> availableAudioSources = new Stack<AudioSource>();

        public AudioManager AudioManager { set => audioManager = value; }


        /// <summary>
        /// Contains some data about a Sound being currently played
        /// </summary>
        [Serializable]
        private class PlayingSound
        {
            public Sound Sound;
            public AudioSource Source; //Source playing the sound
            public string Name; 
            public Action OnPlayEnd; //Action to perform when the sounds ends (if uninterrupted)
        }

        private void OnEnable()
        {
            audioManager.RegisterAudioSourcePlayer(this);
        }

        private void OnDisable()
        {
            audioManager.UnregisterAudioSourcePlayer(this);
        }
        #region Public Methods

        public static AudioSourcePlayer AddAsComponent(GameObject gameObject, AudioManager audioManager)
        {
            //Disable OnEnable call at AddComponent() call
            gameObject.SetActive(false);

            AudioSourcePlayer audioPlayer = gameObject.AddComponent<AudioSourcePlayer>();
            audioPlayer.audioManager = audioManager;

            gameObject.SetActive(true);

            return audioPlayer;
        }
        /// <summary>
        /// Play the Soundwith the given name. 
        /// Does nothing if a sound is already being played.
        /// Does nothing if the sound doesn't exists in the AudioManager.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="onEndPlay"></param>
        /*
        public void PlaySound(string name, bool canRepeat = true, Action onEndPlay = null)
        {
            //Verify if the sound can be repeated multiple times
            if (!canRepeat && IsCurrentlyPlayed(name))
            {
                return;
            }

            //Does the sound exist ?
            Sound sound = Find(name);
            if (sound == null)
                return;

            //Play the Sound
            PlaySound(sound, name, onEndPlay);
        } */

        public void PlaySound(Sound sound, bool canRepeat = true, Action onEndPlay = null)
        {
            //Verify if the sound can be repeated multiple times
            if (!canRepeat && IsCurrentlyPlayed(sound.name))
            {
                return;
            }

            //Play the Sound
            PlaySound(sound, sound.name, onEndPlay);
        }

        /// <summary>
        /// Interrupt a sound with the given name or list name. (The name that was used to play the sound)
        /// </summary>
        /// <param name="name"></param>
        public void InterruptSound(string name)
        {

            //Does the sound exist ?
            PlayingSound play;

            if (!IsCurrentlyPlayed(name, out play))
            {
                return;
            }
            
            FreeAudioSource(play.Source);
        }

        /// <summary>
        /// Return true if the sound with the given name or list name is being played. 
        /// (The name that was used to play the sound)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsCurrentlyPlayed(string name)
        {
            PlayingSound play = null;
            return IsCurrentlyPlayed(name, out play);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Return true if the sound with the given name is played, and if true,
        /// load its PlayingSound class in the out var.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="play"></param>
        /// <returns></returns>
        private bool IsCurrentlyPlayed(string name, out PlayingSound play)
        {
            play = null;
            foreach (PlayingSound playing in currentlyPlaying)
            {
                if (playing.Name.Equals(name))
                {
                    play = playing;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if the sound is currently played.
        /// </summary>
        /// <param name="play"></param>
        /// <returns></returns>
        private bool IsCurrentlyPlayed(PlayingSound play)
        {
            return currentlyPlaying.Contains(play);
        }

        /// <summary>
        /// Play the given Sound and attribute it the given name. (Either sound or list name)
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="name"></param>
        /// <param name="onEndPlay"></param>
        private void PlaySound(Sound sound, string name, Action onEndPlay = null)
        {
            AudioSource source;

            //Get a source, enables it.
            source = GetAvailableAudioSource();
            source.enabled = true;

            //Encapsulate in the PlayingSound class
            PlayingSound playing = new PlayingSound();
            playing.Name = name;
            playing.Sound = sound;
            playing.Source = source;
            playing.OnPlayEnd = onEndPlay;

            //Remember has currently playing
            currentlyPlaying.Add(playing);

            if (!sound.Loop)
            {
                //Play it once if not looped.
                StartCoroutine(_PlaySoundOnce(playing));
            }
            else
            {
                //Play it indefinitively.
                sound.LoadIn(source);
                source.Play();
            }
        }

        /// <summary>
        /// Play the given sound once, then free the audio source and invoke optional OnPlayEnd action.
        /// </summary>
        /// <param name="playing"></param>
        /// <returns></returns>
        private IEnumerator _PlaySoundOnce(PlayingSound playing)
        {
            AudioSource source = playing.Source;
            Sound sound = playing.Sound;

            sound.LoadIn(source);
            source.loop = false;
            source.Play();
            

            yield return new WaitForSeconds(source.clip.length);

            //Interrupt if it was removed from the current play list
            if (!IsCurrentlyPlayed(playing))
            {
                yield break;
            }

            FreeAudioSource(source);
            playing.OnPlayEnd?.Invoke();
            
        }

        /// <summary>
        /// Return an available audio source to play a sound, create a new one if there's none.
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAvailableAudioSource()
        {
            if (availableAudioSources.Count > 0)
            {
                return availableAudioSources.Pop();
            }
            return gameObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// Stop the sound played by the audio source and set it as available.
        /// </summary>
        /// <param name="source"></param>
        private void FreeAudioSource(AudioSource source)
        {
            source.Stop();
            source.enabled = false;
            RemoveFromCurrentlyPlayed(source);
            availableAudioSources.Push(source);
        }

        /// <summary>
        /// Remove the source from the currentPlaying list, interrupting it.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>True if the source was playing a sound, else false.</returns>
        private bool RemoveFromCurrentlyPlayed(AudioSource source) {
            foreach (PlayingSound entry in currentlyPlaying)
            {
                if (entry.Source == source)
                {
                    currentlyPlaying.Remove(entry);
                    return true;
                }
            }
            return false;
        }

        private Sound Find(string soundName)
        {
            Sound s = Array.Find(playableSounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound:" + soundName + " not found!", gameObject);
            }
            return s;
        }

    #endregion
    }
}
