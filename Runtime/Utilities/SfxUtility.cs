using UnityEngine;

namespace GlitchedPolygons.Utilities
{
    /// <summary>
    /// Static class useful for playing sound clips easily.
    /// </summary>
    public static class SfxUtility
    {
        /// <summary>
        /// The <see cref="AudioSource"/> component that plays global sfx.
        /// </summary>
        private static AudioSource globalAudioSoos = null;

        /// <summary>
        /// The <see cref="AudioSource"/> component that plays local sfx.
        /// </summary>
        private static AudioSource localAudioSoos = null;

        /// <summary>
        /// The <see cref="Transform"/> that has the <see cref="localAudioSoos"/> 
        /// component that will move around the scene to play sounds.
        /// </summary>
        private static Transform localAudioSoosTransform = null;

        /// <summary>
        /// Initialize a global SFX entity if there is none in the scene.
        /// </summary>
        private static void Check()
        {
            if (globalAudioSoos == null)
            {
                var audioSoosObj = new GameObject("GlobalSFX") { isStatic = true };

                globalAudioSoos = audioSoosObj.AddComponent<AudioSource>();

                globalAudioSoos.playOnAwake = false;
                globalAudioSoos.rolloffMode = AudioRolloffMode.Linear;
                globalAudioSoos.minDistance = 999999f;
                globalAudioSoos.maxDistance = 1000000f;
            }

            if (localAudioSoos == null)
            {
                var audioSoosObj = new GameObject("LocalSFX");
                localAudioSoos = audioSoosObj.AddComponent<AudioSource>();

                localAudioSoosTransform = audioSoosObj.transform;

                localAudioSoos.playOnAwake = false;
            }
        }

        /// <summary>
        /// Play a specific AudioClip globally (will be heard everywhere in the scene).
        /// </summary>
        /// <param name="clip">The AudioClip to play</param>
        /// <param name="volume">The volume at which the clip will be played [0 - 1]</param>
        /// <param name="pitch">The pitch at which the clip will be played [-3 - 3]</param>
        public static void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
        {
            Check();

            globalAudioSoos.pitch = pitch;
            globalAudioSoos.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Play a specific AudioClip at a specific location in the world (like for instance bullet impact sfx).
        /// </summary>
        /// <param name="playLocation">The location at which the audio clip should be played</param>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume">The volume at which the clip should be played</param>
        /// <param name="pitch">Pitch</param>
        /// <param name="rolloffMode">Audio Rolloff Mode</param>
        /// <param name="minDistance">Within the minimum distance, the sound will be played at 100% volume. Between min and max the rolloff kicks in.</param>
        /// <param name="maxDistance">Listeners that are further away from the playLocation than this maximum distance value won't hear this clip</param>
        public static void PlayOnSpot(Vector3 playLocation, AudioClip clip, float volume = 1.0f, float pitch = 1.0f, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic, float minDistance = 1.0f, float maxDistance = 500.0f)
        {
            Check();

            if (clip == null)
            {
                return;
            }

            localAudioSoosTransform.position = playLocation;

            localAudioSoos.pitch = pitch;

            localAudioSoos.rolloffMode = rolloffMode;
            localAudioSoos.minDistance = minDistance;
            localAudioSoos.maxDistance = maxDistance;

            localAudioSoos.PlayOneShot(clip, volume);
        }
    }
}

// Copyright (C) Raphael Beck, 2016 | https://glitchedpolygons.com
