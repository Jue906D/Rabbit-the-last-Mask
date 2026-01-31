using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using Script.Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Script.Manager
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        [SerializeField]
        public ClipDict bgmDict;
        public AudioSource bgmSource;
        public float bgmFadeTime = 1f; 
        
        [SerializeField]
        public ClipDict sfxDict;
        public AudioSource[] sfxSources;      // 10个SFX通道
        private int lastUsedChannel = -1; 

        [Header("混音")] public AudioMixer audioMixer;
        
        [Serializable]
        public class ClipDict : SerializableDictionaryBase<string, AudioClip>{};
        [Serializable]
        public class SourceDict : SerializableDictionaryBase<string, AudioSource>{};

        public int bgm_sampleRate;

        public float bgm_delay;
        public int bgm_interval;

        AudioSource GetAvailableChannel()
        {
            for (int i = 0; i < sfxSources.Length; i++)
            {
                if (!sfxSources[i].isPlaying)
                    return sfxSources[i];
            }
            lastUsedChannel = (lastUsedChannel + 1) % sfxSources.Length;
            return sfxSources[lastUsedChannel];
        }
        public void PlaySfx(string  clipName, Vector3 position = default)
        {
            AudioSource channel = GetAvailableChannel();
        
            // 设置参数
            channel.transform.position = position;
            channel.volume = 1f;
            channel.clip = sfxDict[clipName];
            channel.Play();
        }
        public void PlaySfx(AudioClip clip, Vector3 position = default)
        {
            AudioSource channel = GetAvailableChannel();
        
            // 设置参数
            channel.transform.position = position;
            channel.volume = 1f;
            channel.clip =clip;
            channel.Play();
        }
        public void PlayBGM(string clipName)
        {
            bgmSource.clip = bgmDict[clipName];
            bgmSource.volume = 1f;
            bgmSource.Play();
            
            bgm_sampleRate = bgmSource.clip.frequency;
            bgm_delay = Mathf.RoundToInt(230f / 1000f *bgm_sampleRate);
            bgm_interval = Mathf.RoundToInt(500f / 1000f *bgm_sampleRate);
        }


        public void Awake()
        {
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            
            for (int i = 0; i < sfxSources.Length; i++)
            {

                sfxSources[i].playOnAwake = false;
                sfxSources[i].volume = 1f;
            }
        }
    }
}