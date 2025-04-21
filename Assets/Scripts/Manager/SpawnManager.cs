using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // 적 스폰을 관리하는 코루틴 변수
    private Coroutine _coSpawningPool;
    // 스폰 간격 설정
    private WaitForSeconds _spawnInterval = new WaitForSeconds(5.0f);

    void Start()
    {
        // 게임 오브젝트 리소스 로드
        ObjectManager.Instance.ResourceAllLoad();
        // 지정한 위치에 플레이어 생성
        ObjectManager.Instance.Spawn<PlayerController>(new Vector3(-6.75f, -3.8f, 2.0f));

        // 스폰 코루틴이 실행 중이 아니면 시작
        if (_coSpawningPool == null)
        {
            _coSpawningPool = StartCoroutine(CoSpawningPool());
            _coSpawningPool = null;
        }
    }

    // Enemy를 주기적으로 생성하는 코루틴
    private IEnumerator CoSpawningPool()
    {
        // 플레이어가 생존 상태라면 무한 반복
        while (GameManager.Instance.PlayerInfo.IsAlive)
        {
            // 생성될 적 랜덤 선택
            int randomSpawn = Random.Range(0, 2);

            // 지정한 위치에 적 유닛 생성
            if (randomSpawn == 0) PoolManager.Instance.GetObject<HeavyBanditController>(new Vector3(10.0f, -3.81f, 2.0f));
            else if (randomSpawn == 1) PoolManager.Instance.GetObject<LightBanditController>(new Vector3(10.0f, -3.81f, 2.0f));

            // 생성 간격
            yield return _spawnInterval;
        }
    }
}