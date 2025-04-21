using UnityEngine;

// 적(Enemy)의 능력치를 관리하는 클래스 (BaseController를 상속)
public class Enemy : BaseController
{
    // 적의 기본 스탯 변수
    private float _maxHp;       // 최대 체력
    private float _atk;         // 공격력
    private float _expDrop;     // 경험치
    private int _goldDrop;      // 골드

    // 초기화 함수 (BaseController의 Initialize 오버라이드)
    protected override void Initialize()
    {

    }

    // 오브젝트 활성화 시 호출
    private void OnEnable()
    {
        GetState();
        UpdateState(GameManager.Instance.Level);
    }

    // 현재 GameManager의 EnemyInfo에서 적 능력치를 가져와 동기화
    private void GetState()
    {
        _maxHp = GameManager.Instance.EnemyInfo.MaxHp;
        _atk = GameManager.Instance.EnemyInfo.Atk;
        _expDrop = GameManager.Instance.EnemyInfo.ExpDrop;
        _goldDrop = GameManager.Instance.EnemyInfo.GoldDrop;
    }

    // 플레이어 레벨을 기준으로 적의 능력치 조정
    private void UpdateState(int playerLevel)
    {
        // 플레이어 레벨이 올라갈수록 적의 능력치가 증가
        _maxHp = 180 * Mathf.Pow(1.15f, playerLevel - 1);            // 체력은 플레이어보다 1.2배 더 강하게 증가
        _atk = 12 * Mathf.Pow(1.12f, playerLevel - 1);               // 공격력은 플레이어보다 1.15배 더 강하게 증가
        _expDrop = 20 * Mathf.Pow(1.12f, playerLevel - 1);           // 경험치 드랍 증가
        _goldDrop = (int)(15 * Mathf.Pow(1.12f, playerLevel - 1));   // 골드 드랍량 증가

        // 변경된 능력치를 GameManager의 EnemyInfo에 반영
        GameManager.Instance.EnemyInfo.MaxHp = _maxHp;
        GameManager.Instance.EnemyInfo.CurrentHp = _maxHp;
        GameManager.Instance.EnemyInfo.Atk = _atk;
        GameManager.Instance.EnemyInfo.ExpDrop = _expDrop;
        GameManager.Instance.EnemyInfo.GoldDrop = _goldDrop;
    }
}