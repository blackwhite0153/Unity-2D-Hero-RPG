using TMPro;
using UnityEngine;

public class EnemyInformation : MonoBehaviour
{
    // 적의 기본 스탯 변수
    private float _enemyHp;         // 체력
    private float _enemyAtk;        // 공격력
    private float _enemyExpDrop;    // 경험치
    private int _enemyGoldDrop;     // 골드

    // 적의 정보 출력 텍스트
    public TMP_Text EnemyHpText;
    public TMP_Text EnemyAtkText;
    public TMP_Text EnemyExpDropText;
    public TMP_Text EnemyGoldDropText;

    private void Update()
    {
        GetState();             // 현재 상태 동기화
        UpdateState(GameManager.Instance.Level);    // 상태 업데이트
        InformationDisplay();   // 적 정보 출력
    }

    // 현재 GameManager의 EnemyInfo에서 적 능력치를 가져와 동기화
    private void GetState()
    {
        _enemyHp = GameManager.Instance.EnemyInfo.MaxHp;
        _enemyAtk = GameManager.Instance.EnemyInfo.Atk;
        _enemyExpDrop = GameManager.Instance.EnemyInfo.ExpDrop;
        _enemyGoldDrop = GameManager.Instance.EnemyInfo.GoldDrop;
    }

    // 플레이어 레벨을 기준으로 적의 능력치 조정
    private void UpdateState(int playerLevel)
    {
        // 플레이어 레벨이 올라갈수록 적의 능력치가 증가
        _enemyHp = 180 * Mathf.Pow(1.15f, playerLevel- 1);                // 체력은 플레이어보다 1.2배 더 강하게 증가
        _enemyAtk = 12 * Mathf.Pow(1.12f, playerLevel- 1);                // 공격력은 플레이어보다 1.15배 더 강하게 증가
        _enemyExpDrop = 20 * Mathf.Pow(1.12f, playerLevel - 1);           // 경험치 드랍 증가
        _enemyGoldDrop = (int)(15 * Mathf.Pow(1.12f, playerLevel - 1));   // 골드 드랍량 증가
    }

    // 정 정보 출력
    private void InformationDisplay()
    {
        EnemyHpText.text = $"Hp : {_enemyHp.ToString("F1")}";
        EnemyAtkText.text = $"Atk : {_enemyAtk.ToString("F1")}";
        EnemyExpDropText.text = $"Exp Drop : {_enemyExpDrop.ToString("F1")}";
        EnemyGoldDropText.text = $"Gold Drop : {_enemyGoldDrop} $";
    }
}