using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// 게임 전반에서 사용되는 BGM과 SFX를 중앙에서 관리하는 AudioManager 클래스
public class AudioManager : Singleton<AudioManager>
{
    // 배경음(Background Music)을 저장하는 딕셔너리
    private Dictionary<string, AudioClip> _bgmDictionary;
    // 효과음(Sound Effect)을 저장하는 딕셔너리
    private Dictionary<string, AudioClip> _sfxDictionary;

    [Header("Audio Sources")]
    // BGM 재생을 위한 AudioSource
    [SerializeField] private AudioSource _bgmSource;
    // SFX 재생을 위한 AudioSource
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clips")]
    // 인스펙터에서 보기만 가능한 BGM 리스트
    [ShowOnly][SerializeField] private List<AudioClip> _bgmClips;
    // 인스펙터에서 보기만 가능한 SFX 리스트
    [ShowOnly][SerializeField] private List<AudioClip> _sfxClips;

    // 현재 설정된 BGM 및 SFX 볼륨을 보여주기 위한 변수
    [ShowOnly][SerializeField] private float _currentBGMVolume;
    [ShowOnly][SerializeField] private float _currentSFXVolume;

    // 싱글톤 초기화 (상속된 Singleton에서 기본 초기화 수행)
    protected override void Initialize()
    {
        base.Initialize();

        Setting();
    }

    // 초기화 설정
    private void Setting()
    {
        _bgmDictionary = new Dictionary<string, AudioClip>();
        _sfxDictionary = new Dictionary<string, AudioClip>();

        _bgmClips = new List<AudioClip>();
        _sfxClips = new List<AudioClip>();

        // BGM 재생용 Audio Source가 없다면 자동으로 생성
        if (_bgmSource == null)
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true; // 기본적으로 BGM은 반복 재생
        }

        // SFX 재생용 Audio Source가 없다면 자동으로 생성
        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false; // 효과음은 즉시 재생만 수행
        }

        ResourceAllLoad();
    }

    // 모든 오디오 리소스를 로드
    private void ResourceAllLoad()
    {
        // BGM 클립 등록
        AudioClip elfPrincessClip = Resources.Load<AudioClip>(Define.ElfPrincessPath);
        AddBGM("Elf Princess", elfPrincessClip);
        AudioClip tightLightClip = Resources.Load<AudioClip>(Define.NightLightPath);
        AddBGM("Night Light", tightLightClip);
        AudioClip theEntranceClip = Resources.Load<AudioClip>(Define.TheEntrancePath);
        AddBGM("The Entrance", theEntranceClip);

        // SFX 클립 등록
        AudioClip swordClip = Resources.Load<AudioClip>(Define.SwordPath);
        AddSFX("Sword", swordClip);
    }

    // 지정된 이름으로 BGM을 등록
    public void AddBGM(string name, AudioClip clip)
    {
        // 해당 타입의 효과음이 처음 추가될 경우 (중복 방지)
        if (!_bgmDictionary.ContainsKey(name))
        {
            _bgmDictionary.Add(name, clip);
            _bgmClips.Add(clip);
        }
    }

    // 이름에 해당하는 BGM 재생 (옵션으로 반복 여부 지정 가능)
    public void PlayBGM(string name, bool isLoop = true)
    {
        if (_bgmDictionary.TryGetValue(name, out var clip))
        {
            // 이미 재생중인 BGM인지 확인
            if (_bgmSource.clip == clip) return;

            _bgmSource.clip = clip;
            _bgmSource.loop = isLoop;
            _bgmSource.Play();
        }
    }

    // 현재 재생 중인 BGM 일시정지
    public void PauseOnBGM()
    {
        _bgmSource.Pause();
    }

    // 현재 재생 중인 BGM 일시정지 해제
    public void PauseOffBGM()
    {
        _bgmSource.Play();
    }

    // 현재 재생 중인 BGM 정지
    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    // BGM 볼륨 설정 및 현재 볼륨 저장
    public void SetBGMVolume(float volume)
    {
        _bgmSource.volume = volume;
        _currentBGMVolume = volume;
    }

    // 이름에 해당하는 BGM 제거
    public void RemoveBGM(string name)
    {
        if (_bgmDictionary.TryGetValue(name, out var clip))
        {
            _bgmDictionary.Remove(name);
            _bgmClips.Remove(clip);
        }
    }

    // 지정된 이름으로 SFX를 등록
    public void AddSFX(string name, AudioClip clip)
    {
        // 해당 타입의 효과음이 처음 추가될 경우 (중복 방지)
        if (!_sfxDictionary.ContainsKey(name))
        {
            _sfxDictionary.Add(name, clip);
            _sfxClips.Add(clip);
        }
    }

    // 이름에 해당하는 SFX 재생 (한 번만 재생됨)
    public void PlaySFX(string name)
    {
        if (_sfxDictionary.TryGetValue(name, out var clip))
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    // SFX 볼륨 설정 및 현재 볼륨 저장
    public void SetSFXVolume(float volume)
    {
        _sfxSource.volume = volume;
        _currentSFXVolume = volume;
    }

    // 이름에 해당하는 SFX 제거
    public void RemoveSFX(string name)
    {
        if (_sfxDictionary.TryGetValue(name, out var clip))
        {
            _sfxDictionary.Remove(name);
            _sfxClips.Remove(clip);
        }
    }

    // 객체 관리에서 오브젝트를 정리하는 함수
    protected override void Clear()
    {
        base.Clear();

        _bgmDictionary.Clear(); // 모든 BGM 클립 제거
        _sfxDictionary.Clear(); // 모든 SFX 클립 제거

        _bgmClips.Clear();
        _sfxClips.Clear();
    }
}