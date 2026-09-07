using System;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary> 
    /// 오디오 매니저 (실제 음악은 AudioPlayer, AudioPlayer 관리는 AudioSystem)
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        // 오디오 캐싱
        private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

        // Sound 별 오디오 볼륨
        private float[] _volume = new float[(int)eSounds.MaxCount];

        #region 생성자 
        private AudioManager() 
        {
            for (eSounds sounds = 0; sounds < eSounds.MaxCount; sounds++)
            {
                _volume[(int)sounds] = PlayerPrefs.GetFloat($"{sounds}Volume", 1f);
            }   
        }
        #endregion 생성자 

        /// <summary>
        /// Get SoundType 볼륨
        /// </summary>
        /// <param name="sounds"></param>
        /// <returns></returns>
        public float GetValue(eSounds sounds)
        {
            if (_volume == null)
            {
                Debug.LogError("AudioManager Volume is NULL");
                return 0;
            }
            if (_volume.Length < (int)sounds)
            {
                Debug.LogError($"AudioManager Volume Count is less then {sounds} : {(int)sounds}");
                return 0;
            }

            return _volume[(int)sounds];
        }

        /// <summary>
        /// Set SoundType 볼륨
        /// </summary>
        /// <param name="sounds"></param>
        /// <param name="volume"></param>
        public void SetValue(eSounds sounds, float volume)
        {

            if (_volume == null)
            {
                Debug.LogError("AudioManager Volume is NULL");
                _volume = new float[(int)eSounds.MaxCount];
            }
            if (_volume.Length < (int)sounds)
            {
                Debug.LogError($"AudioManager Volume Count is less then {sounds} : {(int)sounds}");
            }

            _volume[(int)sounds] = volume;
            PlayerPrefs.SetFloat($"{sounds}Volume", _volume[(int)sounds] >= 1f ? 1f : _volume[(int)sounds]);
            
        }

        public void PlayOneShot(AudioClip audioClip, eSounds type) => Play(audioClip, type, false, true);
        public void PlayLoop(AudioClip audioClip, eSounds type) => Play(audioClip, type, true);

        public void Play(AudioClip audioClip, eSounds type, bool loop = false, bool OneShot = false)
        {
            if (audioClip == null)
                return;

            AudioPlayer audioPlayer = AudioSystem.Instance.GetAudioPlayer(type);
            if (audioPlayer != null)
            {
                if (OneShot)
                {
                    audioPlayer.PlayAudioOneShot(audioClip);
                    return;
                }
                
                audioPlayer.PlayAudio(audioClip);
                audioPlayer.GetAudioSource().loop = loop;
            }
        }

        AudioClip GetOrAddAudioClip(string path, SystemEnum.eSounds type = SystemEnum.eSounds.SFX)
        {
            if (path.Contains("Sounds/") == false)
                path = $"Sounds/{path}";

            AudioClip audioClip = null;

            if (type == SystemEnum.eSounds.BGM)
            {
                audioClip = ObjectManager.Instance.Load<AudioClip>(path);
            }
            else
            {
                if (_audioClips.TryGetValue(path, out audioClip) == false)
                {
                    audioClip = ObjectManager.Instance.Load<AudioClip>(path);
                    _audioClips.Add(path, audioClip);
                }
            }

            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {path}");

            return audioClip;
        }

        public void SetVolume(SystemEnum.eSounds type, float volume)
        {
            AudioSystem.Instance.GetAudioPlayer(type).SetVolume(volume);
        }

        public void Clear()
        {
            foreach (var audioClips in _audioClips)
            {
                Resources.UnloadAsset(audioClips.Value);
            }
            _audioClips.Clear();
        }
    }
}