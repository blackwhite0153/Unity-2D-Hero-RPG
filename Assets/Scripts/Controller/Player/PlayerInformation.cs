using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    // 플레이어의 기본 스탯 변수
    private int _gold;          // 보유 골드
    private int _level;         // 플레이어 레벨
    private float _maxHp;       // 최대 체력
    private float _maxExp;      // 최대 경험치 (레벨업 필요 경험치)
    private float _totalExp;    // 누적 경험치
    private float _atk;         // 공격력
    private float _critical;        // 치명타 확률
    private float _criticalDamage;  // 치명타 피해

    private int _hpCost;                // 체력 강화 비용
    private int _atkCost;               // 공격력 강화 비용
    private int _criticalCost;          // 크리티컬 확률 강화 비용
    private int _criticalDamageCost;    // 크리티컬 데미지 강화 비용

    private int _hpUpgradeLevel;    // 체력 강화 단계
    private int _atkUpgradeLevel;   // 공격력 강화 단계
    private int _criticalUpgradeLevel;          // 치명타 확률 강화 단계
    private int _criticalDamageUpgradeLevel;    // 치명타 피해 강화 단계

    private float _baseHp;     // 레벨업 시 증가하는 기본 체력
    private float _extraHp;    // 골드로 증가한 추가 체력
    private float _baseAtk;    // 레벨업 시 증가하는 기본 공격력
    private float _extraAtk;   // 골드로 증가한 추가 공격력

    // 플레이어 정보 출력 텍스트
    public TMP_Text PlayerLevelText;
    public TMP_Text PlayerGoldText;
    public TMP_Text PlayerHpText;
    public TMP_Text PlayerExpText;
    public TMP_Text PlayerAtkText;
    public TMP_Text PlayerCriticalText;
    public TMP_Text PlayerCriticalDamageText;

    // 강화 비용 출력 텍스트
    public TMP_Text HpUpgradeCostText;
    public TMP_Text AtkUpgradeCostText;
    public TMP_Text CriticalUpgradeCostText;
    public TMP_Text CriticalDamageUpgradeCostText;

    // 플레이어 능력치 증가 버튼
    public Button PlayerLevelUpButton;
    public Button PlayerHpUpgradeButton;
    public Button PlayerAtkUpgradeButton;
    public Button PlayerCriticalUpgradeButton;
    public Button PlayerCriticalDamageUpgradeButton;

    private void Start()
    {
        // 버튼 클릭 이벤트 등록
        PlayerLevelUpButton.onClick.AddListener(OnPlayerLevelUpButtonClick);
        PlayerHpUpgradeButton.onClick.AddListener(OnPlayerHpUpgradeButtonClick);
        PlayerAtkUpgradeButton.onClick.AddListener(OnPlayerAtkUpgradeButtonClick);
        PlayerCriticalUpgradeButton.onClick.AddListener(OnPlayerCriticalUpgradeButtonClick);
        PlayerCriticalDamageUpgradeButton.onClick.AddListener(OnPlayerCriticalDamageUpgradeButtonClick);

        //ResetState();  // 플레이어 능력치 초기화
    }

    // 매 프레임마다 실행되는 업데이트 함수
    private void Update()
    {
        UpdateCost();           // 강화 비용 업데이트
        GetState();             // 현재 상태 동기화
        InformationDisplay();   // 플레이어 정보 출력
    }

    // 플레이어 레벨업
    private void OnPlayerLevelUpButtonClick()
    {
        // 누적 경험치가 요구 경험치 이상일 경우
        if (_totalExp >= _maxExp)
        {
            LevelUp();      // 레벨업 수행
            UpdateState();  // 상태 업데이트
        }
    }

    // 플레이어 체력 강화
    private void OnPlayerHpUpgradeButtonClick()
    {
        // 보유 골드가 강화 비용 이상일 경우
        if (_gold >= _hpCost)
        {
            _gold -= _hpCost;   // 골드 차감
            _extraHp += 15 + (_hpUpgradeLevel * 3);
            _hpUpgradeLevel++;  // 강화 단계 증가

            UpdateState();  // 상태 업데이트
        }
    }

    // 플레이어 공격력 강화
    private void OnPlayerAtkUpgradeButtonClick()
    {
        // 보유 골드가 강화 비용 이상일 경우
        if (_gold >= _atkCost)
        {
            _gold -= _atkCost;  // 골드 차감
            _extraAtk += 2 + (_atkUpgradeLevel * 0.7f);
            _atkUpgradeLevel++; // 강화 단계 증가

            UpdateState();  // 상태 업데이트
        }
    }

    // 플레이어 치명타 확률 강화
    private void OnPlayerCriticalUpgradeButtonClick()
    {
        // 보유 골드가 강화 비용 이상일 경우 & 최대 100%까지 강화 가능
        if (_gold >= _criticalCost && _criticalUpgradeLevel < 100)
        {
            _gold -= _criticalCost;  // 골드 차감
            _critical += 1.0f;       // 1% 증가
            _criticalUpgradeLevel++; // 강화 단계 증가

            UpdateState();  // 상태 업데이트
        }
    }

    // 플레이어 치명타 피해 강화
    private void OnPlayerCriticalDamageUpgradeButtonClick()
    {
        // 보유 골드가 강화 비용 이상일 경우
        if (_gold >= _criticalDamageCost)
        {
            _gold -= _criticalDamageCost;   // 골드 차감
            _criticalDamage += 0.05f;       // 5% 증가
            _criticalDamageUpgradeLevel++;  // 강화 단계 증가

            UpdateState();  // 상태 업데이트
        }
    }

    // 강화 비용 업데이트
    private void UpdateCost()
    {
        _hpCost = 50 + (_hpUpgradeLevel * 25);
        _atkCost = 50 + (_atkUpgradeLevel * 30);
        _criticalCost = 100 + (_criticalUpgradeLevel * 50);
        _criticalDamageCost = 150 + (_criticalDamageUpgradeLevel * 75);
    }

    // 플레이어 상태를 가져와 동기화
    private void GetState()
    {
        var gamaManager = GameManager.Instance;

        _gold = gamaManager.Gold;
        _level = gamaManager.Level;

        _maxHp = gamaManager.MaxHp;

        _maxExp = gamaManager.MaxExp;
        _totalExp = gamaManager.TotalExp;

        _atk = gamaManager.Atk;
        _critical = gamaManager.Critical;
        _criticalDamage = gamaManager.CriticalDamage;

        _hpUpgradeLevel = gamaManager.HpUpgradeLevel;
        _atkUpgradeLevel = gamaManager.AtkUpgradeLevel;
        _criticalUpgradeLevel = gamaManager.CriticalUpgradeLevel;
        _criticalDamageUpgradeLevel = gamaManager.CriticalDamageUpgradeLevel;

        _baseHp = gamaManager.BaseHp;
        _extraHp = gamaManager.ExtraHp;
        _baseAtk = gamaManager.BaseAtk;
        _extraAtk = gamaManager.ExtraAtk;
    }

    // 레벨업 시 능력치 증가 처리
    private void LevelUp()
    {
        // 요구 경험치 이상이면 레벨업 가능
        if (_totalExp >= _maxExp)
        {
            _totalExp -= _maxExp;   // 필요 경험치 차감
            _level++;   // 레벨 증가

            _baseHp *= 1.12f;    // 기본 체력 증가
            _baseAtk *= 1.10f;   // 기본 공격력 증가

            _maxExp *= 1.3f;    // 다음 레벨업 필요 경험치 증가
        }
    }

    // 레벨업 이후 변경된 상태를 GameManager의 PlayerInfo에 반영
    private void UpdateState()
    {
        var gamaManager = GameManager.Instance;

        _maxHp = _baseHp + _extraHp; // 최종 체력 = 기본 체력 + 강화 체력
        _atk = _baseAtk + _extraAtk;    // 최종 공격력 = 기본 공격력 + 강화 공격력

        // PlayerInfo 구조체
        gamaManager.Gold = _gold;
        gamaManager.Level = _level;

        gamaManager.MaxHp = _maxHp;
        gamaManager.CurrentHp = _maxHp;

        gamaManager.MaxExp = _maxExp;
        gamaManager.TotalExp = _totalExp;

        gamaManager.Atk = _atk;
        gamaManager.Critical = _critical;
        gamaManager.CriticalDamage = _criticalDamage;

        // PlayerUpgrade 구조체
        gamaManager.HpUpgradeLevel = _hpUpgradeLevel;
        gamaManager.AtkUpgradeLevel = _atkUpgradeLevel;
        gamaManager.CriticalUpgradeLevel = _criticalUpgradeLevel;
        gamaManager.CriticalDamageUpgradeLevel = _criticalDamageUpgradeLevel;

        gamaManager.BaseHp = _baseHp;
        gamaManager.ExtraHp = _extraHp;
        gamaManager.BaseAtk = _baseAtk;
        gamaManager.ExtraAtk = _extraAtk;
    }

    // 플레이어 정보 출력
    private void InformationDisplay()
    {
        var gamaManager = GameManager.Instance;

        // 플레이어 능력치
        PlayerGoldText.text = $"Gold : {gamaManager.Gold} $";
        PlayerLevelText.text = $"Level : {gamaManager.Level}";
        PlayerHpText.text = $"Hp : {gamaManager.MaxHp.ToString("F1")}";
        PlayerExpText.text = $"Exp : ({gamaManager.TotalExp.ToString("F1")} / {gamaManager.MaxExp.ToString("F1")})";
        PlayerAtkText.text = $"ATK : {gamaManager.Atk.ToString("F1")}";
        PlayerCriticalText.text = $"CRT : {gamaManager.Critical.ToString("F0")} %";
        PlayerCriticalDamageText.text = $"CDMG : {(gamaManager.CriticalDamage * 100).ToString("F0")} %";

        // 강화 비용
        HpUpgradeCostText.text = $"{_hpCost} $";
        AtkUpgradeCostText.text = $"{_atkCost} $";
        CriticalUpgradeCostText.text = $"{_criticalCost} $";
        CriticalDamageUpgradeCostText.text = $"{_criticalDamageCost} $";
    }

    // 플레이어 능력치 초기화
    private void ResetState()
    {
        var gamaManager = GameManager.Instance;

        // PlayerInfo 구조체 초기화
        gamaManager.PlayerInfo.Gold = 0;             // 골드 : 0
        gamaManager.PlayerInfo.Level = 1;            // 레벨 : 1

        gamaManager.PlayerInfo.MaxHp = 150.0f;       // 초기 체력 : 150.0
        gamaManager.PlayerInfo.CurrentHp = 150.0f;   // 현재 체력 : 초기값 MaxHp

        gamaManager.PlayerInfo.MaxExp = 100.0f;      // 최대 경험치 100.0
        gamaManager.PlayerInfo.TotalExp = 0.0f;      // 누적 경험치 0.0

        gamaManager.PlayerInfo.Atk = 10.0f;            // 기본 공격력 : 10.0
        gamaManager.PlayerInfo.Critical = 0.0f;        // 치명타 확률 : 0%
        gamaManager.PlayerInfo.CriticalDamage = 1.0f;  // 치명타 피해 : 100%

        gamaManager.PlayerInfo.Speed = 3.0f;         // 이동 속도 : 3.0
        gamaManager.PlayerInfo.JumpPower = 7.5f;     // 점프력 : 7.5
        gamaManager.PlayerInfo.IsAlive = true;       // 생존 여부 : 기본값 true

        // PlayerUpgrade 구조체 초기화
        gamaManager.PlayerUpgrade.HpUpgradeLevel = 0;   // 체력 강화 단계 : 0
        gamaManager.PlayerUpgrade.AtkUpgradeLevel = 0;  // 공격력 강화 단계 : 0
        gamaManager.PlayerUpgrade.CriticalUpgradeLevel = 0;        // 치명타 확률 강화 단계 : 0
        gamaManager.PlayerUpgrade.CriticalDamageUpgradeLevel = 0;  // 치명타 피해 강화 단계 : 0

        gamaManager.PlayerUpgrade.BaseHp = 150.0f;   // 레벨업으로 인한 체력 : 150.0
        gamaManager.PlayerUpgrade.ExtraHp = 0.0f;    // 골드 강화로 인한 체력 : 0.0
        gamaManager.PlayerUpgrade.BaseAtk = 10.0f;   // 레벨업으로 인한 공격력 : 10.0
        gamaManager.PlayerUpgrade.ExtraAtk = 0.0f;   // 골드 강화로 인한 공격력 : 0.0

        // PlayerPrefs 초기화
        gamaManager.Gold = 0;             // 골드 : 0
        gamaManager.Level = 1;            // 레벨 : 1

        gamaManager.MaxHp = 150.0f;       // 초기 체력 : 150.0
        gamaManager.CurrentHp = 150.0f;   // 현재 체력 : 초기값 MaxHp

        gamaManager.MaxExp = 100.0f;      // 최대 경험치 100.0
        gamaManager.TotalExp = 0.0f;      // 누적 경험치 0.0

        gamaManager.Atk = 10.0f;          // 기본 공격력 : 10.0
        gamaManager.Critical = 0.0f;       // 치명타 확률 : 0%
        gamaManager.CriticalDamage = 1.0f;  // 치명타 피해 : 100%

        gamaManager.HpUpgradeLevel = 0;   // 체력 강화 단계 : 0
        gamaManager.AtkUpgradeLevel = 0;  // 공격력 강화 단계 : 0
        gamaManager.CriticalUpgradeLevel = 0;        // 치명타 확률 강화 단계 : 0
        gamaManager.CriticalDamageUpgradeLevel = 0;  // 치명타 피해 강화 단계 : 0

        gamaManager.BaseHp = 150.0f;   // 레벨업으로 인한 체력 : 150.0
        gamaManager.ExtraHp = 0.0f;    // 골드 강화로 인한 체력 : 0.0
        gamaManager.BaseAtk = 10.0f;   // 레벨업으로 인한 공격력 : 10.0
        gamaManager.ExtraAtk = 0.0f;   // 골드 강화로 인한 공격력 : 0.0
    }
}