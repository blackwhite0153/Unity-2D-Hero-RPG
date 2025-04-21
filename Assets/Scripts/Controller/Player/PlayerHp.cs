using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    private float _maxHp;       // 최대 체력
    private float _currentHp;   // 현재 체력

    // 체력바 UI
    public Slider HpBar;    // 체력바
    public TMP_Text HpText; // 체력 수치를 표시할 텍스트

    private void Start()
    {
        // 게임 시작 시 최대 체력 값을 가져와 설정
        _maxHp = GameManager.Instance.MaxHp;

        // 현재 체력을 최대 체력으로 초기화
        GameManager.Instance.CurrentHp = _maxHp;
        _currentHp = GameManager.Instance.CurrentHp;
    }

    // 프레임마다 체력 상태를 업데이트하고 UI를 갱신
    private void Update()
    {
        UpdateState();
        DisplayHp();
    }

    // 체력 상태를 갱신
    private void UpdateState()
    {
        // GameManager에서 현재 체력과 최대 체력 값을 가져온다.
        _maxHp = GameManager.Instance.MaxHp;
        _currentHp = GameManager.Instance.CurrentHp;

        // 체력바의 최대값과 현재값을 설정하여 UI 업데이트
        HpBar.maxValue = _maxHp;
        HpBar.value = _currentHp;
    }

    // 체력 수치를 텍스트로 표시
    private void DisplayHp()
    {
        // (현재 체력 / 최대 체력) 소수점 1자리까지 표시 ToString("F1")
        HpText.text = $"{_currentHp.ToString("F1")} / {_maxHp.ToString("F1")}";
    }
}