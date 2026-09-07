using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// 실제 오디오 플레이어
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] 
        private AudioSource _audioSource = new AudioSource(); // 오디오 소스
        [SerializeField] 
        private AudioClip   _audioClip   = null;              // 오디오 클립
        [SerializeField]
        private eSounds      _soundType   = eSounds.MaxCount;   // 오디오 타입

        public AudioSource audioSource { get { return GetAudioSource(); } }                // 오디오 소스
        public eSounds soundType { get { return _soundType; } set { _soundType = value; } } // 오디오 타입

        /// <summary>
        /// Get AudioSource 
        /// </summary>
        /// <returns></returns>
        public AudioSource GetAudioSource()
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            return _audioSource;
        }

        /// <summary>
        /// 현재 플레이 컨트롤 중인 AudioClip
        /// </summary>
        /// <returns></returns>
        public AudioClip GetAudioClip()
        {
            return _audioClip;
        }

        /// <summary>
        /// PlayOneShot (AudioClip 관리 X)
        /// </summary>
        /// <param name="audioClip"></param>
        public void PlayAudioOneShot(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }

        /// <summary>
        /// Play (AudioClip 관리 O)
        /// </summary>
        /// <param name="audioClip"></param>
        public void PlayAudio(AudioClip audioClip)
        {
            _audioClip = audioClip;
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        /// <summary>
        /// Stop AudioSource
        /// </summary>
        public void StopAudio()
        {
            _audioSource.Stop();
            _audioClip = null;
            _audioSource.clip = null;
        }

        /// <summary>
        /// PauseaudioSource
        /// </summary>
        public void PauseAudio()
        {
            _audioSource.Pause();
        }

        public void SetVolume(float volume)
        {
            _audioSource.volume = volume;
        }

    }
}