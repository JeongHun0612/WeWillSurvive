using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive.Core
{
    public enum EBGM
    {
        BGM_Test_1, BGM_Test_2
    }

    public enum ESFX
    {
        SFX_Test_1, SFX_Test_2, SFX_Test_3
    }

    public class SoundManager : MonoSingleton<SoundManager>
    {
        private const string _bgmLabel = "BGM";
        private const string _sfxLabel = "SFX";

        [Range(0f, 1f)]
        [SerializeField] private float _bgmVolume = 1f;
        private AudioSource _bgmPlayer;

        [Range(0f, 1f)]
        [SerializeField] private float _sfxVolume = 1f;
        [SerializeField] private int _channelCount = 5;
        private AudioSource[] _sfxPlayers;
        private int _channelIndex;

        private Dictionary<string, AudioClip> _bgmClips = new();
        private Dictionary<string, AudioClip> _sfxClips = new();

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        public async UniTask InitializeAsync()
        {
            GameObject bgmObject = new GameObject("BGM");
            bgmObject.transform.parent = transform;
            _bgmPlayer = bgmObject.AddComponent<AudioSource>();
            _bgmPlayer.playOnAwake = false;
            _bgmPlayer.loop = true;
            _bgmPlayer.volume = _bgmVolume;

            GameObject sfxObject = new GameObject("sfxPlayer");
            sfxObject.transform.parent = transform;
            _sfxPlayers = new AudioSource[_channelCount];

            for (int i = 0; i < _sfxPlayers.Length; i++)
            {
                _sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
                _sfxPlayers[i].playOnAwake = false;
                _sfxPlayers[i].loop = false;
                _sfxPlayers[i].volume = _sfxVolume;
            }

            await LoadAllClipAsync(_bgmLabel, _bgmClips);
            await LoadAllClipAsync(_sfxLabel, _sfxClips);
        }

        public async UniTask PlayBGM(string clipName, float fadeInTime = 0f)
        {
            if (_bgmClips.TryGetValue(clipName, out AudioClip clip))
            {
                if (_bgmPlayer.isPlaying)
                    _bgmPlayer.Stop();

                _bgmPlayer.clip = clip;

                if (fadeInTime > 0f)
                {
                    _bgmPlayer.volume = 0f;
                    _bgmPlayer.Play();
                    await FadeVolumeAsync(_bgmPlayer, _bgmVolume, fadeInTime);
                }
                else
                {
                    _bgmPlayer.volume = _bgmVolume;
                    _bgmPlayer.Play();
                }
            }
        }

        public async UniTask PlayBGM(EBGM bgm, float fadeInTime = 0f)
        {
            await PlayBGM(bgm.ToString(), fadeInTime);
        }

        public async UniTask PlayBGM(float fadeInTime = 0f)
        {
            if (_bgmPlayer.isPlaying || _bgmPlayer.clip == null) return;
            if (fadeInTime > 0f)
            {
                _bgmPlayer.volume = 0f;
                _bgmPlayer.Play();
                await FadeVolumeAsync(_bgmPlayer, _bgmVolume, fadeInTime);
            }
            else
            {
                _bgmPlayer.volume = _bgmVolume;
                _bgmPlayer.Play();
            }
        }

        public void StopBGM()
        {
            if (_bgmPlayer != null && _bgmPlayer.isPlaying)
                _bgmPlayer.Stop();
        }

        public async UniTask PlaySFX(string clipName, float fadeInTime = 0f, float indiVolume = 1f)
        {
            if (!_sfxClips.TryGetValue(clipName, out AudioClip clip))
                return;

            for (int index = 0; index < _sfxPlayers.Length; index++)
            {
                int loopindex = (index + _channelIndex) % _sfxPlayers.Length;
                if (_sfxPlayers[loopindex].isPlaying)
                    continue;

                _channelIndex = loopindex;
                float targetVolume = Mathf.Clamp01(_sfxVolume * indiVolume);

                _sfxPlayers[loopindex].clip = clip;
                if (fadeInTime > 0f)
                {
                    _sfxPlayers[loopindex].volume = 0f;
                    _sfxPlayers[loopindex].Play();
                    await FadeVolumeAsync(_sfxPlayers[loopindex], targetVolume, fadeInTime);
                }
                else
                {
                    _sfxPlayers[loopindex].PlayOneShot(clip, targetVolume);
                }
                break;
            }
        }

        public async UniTask PlaySFX(ESFX sfx, float fadeInTime = 0f, float indiVolume = 1f)
        {
            await PlaySFX(sfx.ToString(), fadeInTime, indiVolume);
        }

        public void StopSFX(string target)
        {
            for (int i = 0; i < _sfxPlayers.Length; i++)
            {
                if (!_sfxPlayers[i].isPlaying || _sfxPlayers[i].clip == null) continue;
                if (_sfxPlayers[i].clip.name != target) continue;
                _sfxPlayers[i].Stop();
                _sfxPlayers[i].clip = null;
            }
        }

        public void StopSFX(ESFX sfx)
        {
            string target = sfx.ToString();
            StopSFX(target);
        }

        public void StopAllSFX()
        {
            for (int i = 0; i < _sfxPlayers.Length; i++)
            {
                if (!_sfxPlayers[i].isPlaying) continue;
                _sfxPlayers[i].Stop();
                _sfxPlayers[i].clip = null;
            }
        }

        private async UniTask LoadAllClipAsync(string label, Dictionary<string, AudioClip> targetDict)
        {
            var audioClips = await ResourceManager.LoadAssetsByLabelAsync<AudioClip>(label);
            foreach (var audioClip in audioClips)
            {
                if (!targetDict.ContainsKey(audioClip.name))
                    targetDict.Add(audioClip.name, audioClip);
            }
        }

        private async UniTask FadeVolumeAsync(AudioSource source, float targetVolume, float duration)
        {
            float startVolume = source.volume;
            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                await UniTask.Yield();
            }
            source.volume = targetVolume;
        }

        public async UniTask FadeOutBGM(float duration)
        {
            if (_bgmPlayer == null || !_bgmPlayer.isPlaying) return;
            await FadeVolumeAsync(_bgmPlayer, 0f, duration);
            _bgmPlayer.Stop();
        }
        public async UniTask FadeOutSFX(string target, float duration)
        {
            for (int i = 0; i < _sfxPlayers.Length; i++)
            {
                var player = _sfxPlayers[i];
                if (!player.isPlaying || player.clip == null) continue;
                if (player.clip.name != target) continue;
                await FadeVolumeAsync(player, 0f, duration);
                player.Stop();
                player.clip = null;
            }
        }

        public async UniTask FadeOutSFX(ESFX sfx, float duration)
        {
            string target = sfx.ToString();
            FadeOutSFX(target, duration);
        }

        public async UniTask FadeOutAllSFX(float duration)
        {
            for (int i = 0; i < _sfxPlayers.Length; i++)
            {
                var player = _sfxPlayers[i];
                if (!player.isPlaying || player.clip == null) continue;
                await FadeVolumeAsync(player, 0f, duration);
                player.Stop();
                player.clip = null;
            }
        }

        public void SetBGMVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);
            if (_bgmPlayer != null)
                _bgmPlayer.volume = _bgmVolume;
        }

        public void SetSFXVolume(float volume)
        {
            float origin = _sfxVolume;
            _sfxVolume = Mathf.Clamp01(volume);
            if (_sfxPlayers != null)
            {
                foreach (var sfx in _sfxPlayers)
                    sfx.volume *= _sfxVolume / origin;
            }
        }
    }
}
