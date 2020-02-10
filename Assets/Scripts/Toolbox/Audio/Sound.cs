namespace Cawotte.Toolbox.Audio
{
    
    using UnityEngine;
    
    /*
     * Class used to define a sound, any playable sound clips.
     * The audio manager contains an Array of 'Sound' which will all contain a sound.
     * */

    /// <summary>
    /// Class encapsulating a playable sound, 
    /// that must be registered in the AudioManager or a SoundList from the AudioManager.
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName ="New Sound", menuName = "Audio/Sound")]
    public class Sound : ScriptableObject
    {

        [Header("Sound Info")]

        public string name; //sound name
        
        public AudioClip clip; //sound asset

        public bool isMusic = false;

        [Header("Sound parameters")]

        [SerializeField]
        [Range(0f, 1f)]
        private float volume = 0.5f;
        [SerializeField]
        [Range(.1f, 3f)]
        private float pitch = 1f;
        [SerializeField]
        private bool loop = false;


        //No volume spatialisation
        private float minDistance = 0f;
        private float maxDistance = 500f;

        //component which will play the sound
        [HideInInspector] public AudioSource source;
        
        public float Volume { get => volume; }
        public float Pitch { get => pitch; }
        public float MinDistance { get => minDistance;  }
        public float MaxDistance { get => maxDistance; }

        public bool Loop { get => loop; set => loop = value; }

        /// <summary>
        /// Load the Sound in the given source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sound"></param>
        public void LoadIn(AudioSource source)
        {
            source.clip = this.clip;
            source.volume = this.Volume;
            source.pitch = this.Pitch;
            source.loop = this.Loop;

            //No spatialization
            source.minDistance = this.MinDistance;
            source.maxDistance = this.MaxDistance;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
        }

    }
}
