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

        [Header("BGM Setting")]
        [Range(0f, 1f)]
        [SerializeField] private float _bgmVolume = 1f;
        private AudioSource _bgmPlayer;

        [Header("SFX Setting")]
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
            // TODO 사운드 데이터

            // BGM Initialize
            GameObject bgmObject = new GameObject("BGM");
            bgmObject.transform.parent = transform;
            _bgmPlayer = bgmObject.AddComponent<AudioSource>();
            _bgmPlayer.playOnAwake = false;
            _bgmPlayer.loop = true;
            _bgmPlayer.volume = _bgmVolume;

            // SFX Initialize
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

        public void PlayBGM(string clipName)
        {
            if (_bgmClips.TryGetValue(clipName, out AudioClip clip))
            {
                if (_bgmPlayer.isPlaying)
                    _bgmPlayer.Stop();

                _bgmPlayer.clip = clip;
                _bgmPlayer.Play();
            }
            else
            {
                Debug.LogWarning($"[BGM Clip] - '{clipName}' is not found.");
            }
        }

        public void PlayBGM(EBGM bgm)
        {
            var clipName = bgm.ToString();
            PlayBGM(clipName);
        }

        public void PlayBGM()
        {
            if (_bgmPlayer.isPlaying)
            {
                Debug.LogWarning("현재 BGM이 플레이중입니다.");
                return;
            }

            if (_bgmPlayer.clip == null)
            {
                Debug.LogWarning("BGM 플레이어에 클립이 존재하지 않습니다.");
                return;
            }

            _bgmPlayer.Play();
        }

        public void StopBGM()
        {
            if (_bgmPlayer.isPlaying)
                _bgmPlayer.Stop();
        }


        public void PlaySFX(ESFX sfx)
        {
            var clipName = sfx.ToString();
            PlaySFX(clipName);
        }

        public void PlaySFX(string clipName)
        {
            if (!_sfxClips.TryGetValue(clipName, out AudioClip clip))
            {
                Debug.LogWarning($"[SFX Clip] - '{clipName}' is not found.");
                return;
            }

            for (int index = 0; index < _sfxPlayers.Length; index++)
            {
                int loopindex = (index + _channelIndex) % _sfxPlayers.Length;
                if (_sfxPlayers[loopindex].isPlaying)
                    continue;

                _channelIndex = loopindex;
                _sfxPlayers[loopindex].PlayOneShot(clip);
                break;
            }
        }

        public void StopSFX(ESFX sfx)
        {
            for (int i = 0; i < _sfxPlayers.Length; i++)
            {
                if (!_sfxPlayers[i].isPlaying)
                    continue;

                if (_sfxPlayers[i].clip.name == sfx.ToString())
                {
                    _sfxPlayers[i].Stop();
                    _sfxPlayers[i].clip = null;
                }
            }
        }

        public void StopAllSFX()
        {
            for (int i = 0; i < _sfxPlayers.Length; i++)
            {
                if (!_sfxPlayers[i].isPlaying)
                    continue;

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
                {
                    targetDict.Add(audioClip.name, audioClip);
                }
            }
        }
    }
}
