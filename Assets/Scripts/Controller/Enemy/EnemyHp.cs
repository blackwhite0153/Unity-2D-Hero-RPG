using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHp : MonoBehaviour
{
    private EnemyController _enemy;

    private float _maxHp;       // 최대 체력
    private float _currentHp;   // 현재 체력

    // 체력바 UI
    public Slider HpBar;    // 체력바

    private void Start()
    {
        _enemy = GetComponentInParent<EnemyController>();

        // 게임 시작 시 최대 체력 값을 가져와 설정
        _maxHp = GameManager.Instance.EnemyInfo.MaxHp;
        // 현재 체력을 최대 체력으로 초기화
        _currentHp = GameManager.Instance.EnemyInfo.CurrentHp;
    }

    // 프레임마다 체력 상태를 업데이트하고 UI를 갱신
    private void Update()
    {
        UpdateState();
    }

    // 체력 상태를 갱신
    private void UpdateState()
    {
        // GameManager에서 최대 체력 값을 가져온다.
        _maxHp = GameManager.Instance.EnemyInfo.MaxHp;
        // EnemyController에서 현재 체력을 가져온다.
        _currentHp = _enemy.CurrentHp;

        // 체력바의 최대값과 현재값을 설정하여 UI 업데이트
        HpBar.maxValue = _maxHp;
        HpBar.value = _currentHp;
    }
}