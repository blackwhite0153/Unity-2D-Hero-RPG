using System;
using System.Collections.Generic;
using UnityEngine;

// 게임 내 오브젝트를 관리하는 싱글톤 클래스
public class ObjectManager : Singleton<ObjectManager>
{
    // 플레이어 컨트롤러 인스턴스 (private 변수)
    private PlayerController _player;

    // 리소스 (프리팹) 참조 변수
    private GameObject _heroKnightResource;
    private GameObject _heavyBanditResource;
    private GameObject _lightBanditResource;

    // 플레이어 컨트롤러에 대한 public 읽기 전용 프로퍼티
    public PlayerController Player { get => _player; }

    // 적 객체들을 관리하는 HashSet
    public HashSet<HeavyBanditController> HeavyBandit { get; set; } = new HashSet<HeavyBanditController>();
    public HashSet<LightBanditController> LightBandit { get; set; } = new HashSet<LightBanditController>();

    // 피해를 준 적을 추적하는 HashSet
    public HashSet<EnemyController> DamagedEnemies = new HashSet<EnemyController>();

    // 싱글톤 초기화 (상속된 Singleton에서 기본 초기화 수행)
    protected override void Initialize()
    {
        // Singleton 초기화 호출
        base.Initialize();
    }

    // 모든 게임 오브젝트 리소스를 로드하는 함수
    public void ResourceAllLoad()
    {
        // Resources.Load<T>(path)를 사용하여 프리팹 로드
        _heroKnightResource = Resources.Load<GameObject>(Define.HeroKnightPath);
        _heavyBanditResource = Resources.Load<GameObject>(Define.HeavyBanditPath);
        _lightBanditResource = Resources.Load<GameObject>(Define.LightBanditPath);
    }

    // 제네릭 타입을 사용한 Spawn 함수
    public T Spawn<T>(Vector3 spawnPos) where T : BaseController
    {
        // 제네릭 타입 T를 받아서 타입을 결정
        Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // 플레이어 캐릭터 생성
            GameObject obj = Instantiate(_heroKnightResource, spawnPos, Quaternion.identity);   // 플레이어 오브젝트 생성
            PlayerController playerController = obj.GetOrAddComponent<PlayerController>();      // PlayerController 컴포넌트 추가
            _player = playerController; // 플레이어 변수에 할당

            // 제네릭 타입 T로 캐스팅하여 반환
            return playerController as T;
        }
        else if (type == typeof(HeavyBanditController))
        {
            // HeavyBandit 유닛 생성
            GameObject obj = Instantiate(_heavyBanditResource, spawnPos, Quaternion.identity);              // 플레이어 오브젝트 생성
            HeavyBanditController heavyBanditController = obj.GetOrAddComponent<HeavyBanditController>();   // HeavyBanditController 컴포넌트 추가
            HeavyBandit.Add(heavyBanditController); // 생성된 HeavyBanditController를 HashSet에 추가

            // 제네릭 타입 T로 캐스팅하여 반환
            return heavyBanditController as T;
        }
        else if (type == typeof(LightBanditController))
        {
            // LightBandit 유닛 생성
            GameObject obj = Instantiate(_lightBanditResource, spawnPos, Quaternion.identity);              // 플레이어 오브젝트 생성
            LightBanditController lightBanditController = obj.GetOrAddComponent<LightBanditController>();   // LightBanditController 컴포넌트 추가
            LightBandit.Add(lightBanditController); // 생성된 LightBanditController를 HashSet에 추가

            // 제네릭 타입 T로 캐스팅하여 반환
            return lightBanditController as T;
        }

        // 해당하는 타입이 없으면 null 반환
        return null;
    }

    // 오브젝트를 비활성화하는 함수
    public void Despawn<T>(T obj) where T : BaseController
    {
        // 오브젝트 풀링 방식을 위한 비활성화
        obj.gameObject.SetActive(false);
    }

    // 객체 관리에서 오브젝트를 정리하는 함수
    protected override void Clear()
    {
        base.Clear();

        DamagedEnemies.Clear(); // DamagedEnemies HashSet 초기화 (모든 DamagedEnemies 제거)
        HeavyBandit.Clear();    // HeavyBandit HashSet 초기화 (모든 HeavyBanditController 제거)
        LightBandit.Clear();    // LightBandit HashSet 초기화 (모든 LightBanditController 제거)
        _player = null;         // 플레이어 변수 초기화 (플레이어 삭제)

        // Resources로 로드한 것 중 메모리에서 사용되지 않는 리소스 해제
        Resources.UnloadUnusedAssets();
    }
}