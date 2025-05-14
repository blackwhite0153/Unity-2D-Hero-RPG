using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    // �÷��̾��� �⺻ ���� ����
    private int _gold;          // ���� ���
    private int _level;         // �÷��̾� ����
    private float _maxHp;       // �ִ� ü��
    private float _maxExp;      // �ִ� ����ġ (������ �ʿ� ����ġ)
    private float _totalExp;    // ���� ����ġ
    private float _atk;         // ���ݷ�
    private float _critical;        // ġ��Ÿ Ȯ��
    private float _criticalDamage;  // ġ��Ÿ ����

    private int _hpCost;                // ü�� ��ȭ ���
    private int _atkCost;               // ���ݷ� ��ȭ ���
    private int _criticalCost;          // ũ��Ƽ�� Ȯ�� ��ȭ ���
    private int _criticalDamageCost;    // ũ��Ƽ�� ������ ��ȭ ���

    private int _hpUpgradeLevel;    // ü�� ��ȭ �ܰ�
    private int _atkUpgradeLevel;   // ���ݷ� ��ȭ �ܰ�
    private int _criticalUpgradeLevel;          // ġ��Ÿ Ȯ�� ��ȭ �ܰ�
    private int _criticalDamageUpgradeLevel;    // ġ��Ÿ ���� ��ȭ �ܰ�

    private float _baseHp;     // ������ �� �����ϴ� �⺻ ü��
    private float _extraHp;    // ���� ������ �߰� ü��
    private float _baseAtk;    // ������ �� �����ϴ� �⺻ ���ݷ�
    private float _extraAtk;   // ���� ������ �߰� ���ݷ�

    // �÷��̾� ���� ��� �ؽ�Ʈ
    public TMP_Text PlayerLevelText;
    public TMP_Text PlayerGoldText;
    public TMP_Text PlayerHpText;
    public TMP_Text PlayerExpText;
    public TMP_Text PlayerAtkText;
    public TMP_Text PlayerCriticalText;
    public TMP_Text PlayerCriticalDamageText;

    // ��ȭ ��� ��� �ؽ�Ʈ
    public TMP_Text HpUpgradeCostText;
    public TMP_Text AtkUpgradeCostText;
    public TMP_Text CriticalUpgradeCostText;
    public TMP_Text CriticalDamageUpgradeCostText;

    // �÷��̾� �ɷ�ġ ���� ��ư
    public Button PlayerLevelUpButton;
    public Button PlayerHpUpgradeButton;
    public Button PlayerAtkUpgradeButton;
    public Button PlayerCriticalUpgradeButton;
    public Button PlayerCriticalDamageUpgradeButton;

    private void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ ���
        PlayerLevelUpButton.onClick.AddListener(OnPlayerLevelUpButtonClick);
        PlayerHpUpgradeButton.onClick.AddListener(OnPlayerHpUpgradeButtonClick);
        PlayerAtkUpgradeButton.onClick.AddListener(OnPlayerAtkUpgradeButtonClick);
        PlayerCriticalUpgradeButton.onClick.AddListener(OnPlayerCriticalUpgradeButtonClick);
        PlayerCriticalDamageUpgradeButton.onClick.AddListener(OnPlayerCriticalDamageUpgradeButtonClick);

        //ResetState();  // �÷��̾� �ɷ�ġ �ʱ�ȭ
    }

    // �� �����Ӹ��� ����Ǵ� ������Ʈ �Լ�
    private void Update()
    {
        UpdateCost();           // ��ȭ ��� ������Ʈ
        GetState();             // ���� ���� ����ȭ
        InformationDisplay();   // �÷��̾� ���� ���
    }

    // �÷��̾� ������
    private void OnPlayerLevelUpButtonClick()
    {
        // ���� ����ġ�� �䱸 ����ġ �̻��� ���
        if (_totalExp >= _maxExp)
        {
            LevelUp();      // ������ ����
            UpdateState();  // ���� ������Ʈ
        }
    }

    // �÷��̾� ü�� ��ȭ
    private void OnPlayerHpUpgradeButtonClick()
    {
        // ���� ��尡 ��ȭ ��� �̻��� ���
        if (_gold >= _hpCost)
        {
            _gold -= _hpCost;   // ��� ����
            _extraHp += 15 + (_hpUpgradeLevel * 3);
            _hpUpgradeLevel++;  // ��ȭ �ܰ� ����

            UpdateState();  // ���� ������Ʈ
        }
    }

    // �÷��̾� ���ݷ� ��ȭ
    private void OnPlayerAtkUpgradeButtonClick()
    {
        // ���� ��尡 ��ȭ ��� �̻��� ���
        if (_gold >= _atkCost)
        {
            _gold -= _atkCost;  // ��� ����
            _extraAtk += 2 + (_atkUpgradeLevel * 0.7f);
            _atkUpgradeLevel++; // ��ȭ �ܰ� ����

            UpdateState();  // ���� ������Ʈ
        }
    }

    // �÷��̾� ġ��Ÿ Ȯ�� ��ȭ
    private void OnPlayerCriticalUpgradeButtonClick()
    {
        // ���� ��尡 ��ȭ ��� �̻��� ��� & �ִ� 100%���� ��ȭ ����
        if (_gold >= _criticalCost && _criticalUpgradeLevel < 100)
        {
            _gold -= _criticalCost;  // ��� ����
            _critical += 1.0f;       // 1% ����
            _criticalUpgradeLevel++; // ��ȭ �ܰ� ����

            UpdateState();  // ���� ������Ʈ
        }
    }

    // �÷��̾� ġ��Ÿ ���� ��ȭ
    private void OnPlayerCriticalDamageUpgradeButtonClick()
    {
        // ���� ��尡 ��ȭ ��� �̻��� ���
        if (_gold >= _criticalDamageCost)
        {
            _gold -= _criticalDamageCost;   // ��� ����
            _criticalDamage += 0.05f;       // 5% ����
            _criticalDamageUpgradeLevel++;  // ��ȭ �ܰ� ����

            UpdateState();  // ���� ������Ʈ
        }
    }

    // ��ȭ ��� ������Ʈ
    private void UpdateCost()
    {
        _hpCost = 50 + (_hpUpgradeLevel * 25);
        _atkCost = 50 + (_atkUpgradeLevel * 30);
        _criticalCost = 100 + (_criticalUpgradeLevel * 50);
        _criticalDamageCost = 150 + (_criticalDamageUpgradeLevel * 75);
    }

    // �÷��̾� ���¸� ������ ����ȭ
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

    // ������ �� �ɷ�ġ ���� ó��
    private void LevelUp()
    {
        // �䱸 ����ġ �̻��̸� ������ ����
        if (_totalExp >= _maxExp)
        {
            _totalExp -= _maxExp;   // �ʿ� ����ġ ����
            _level++;   // ���� ����

            _baseHp *= 1.12f;    // �⺻ ü�� ����
            _baseAtk *= 1.10f;   // �⺻ ���ݷ� ����

            _maxExp *= 1.3f;    // ���� ������ �ʿ� ����ġ ����
        }
    }

    // ������ ���� ����� ���¸� GameManager�� PlayerInfo�� �ݿ�
    private void UpdateState()
    {
        var gamaManager = GameManager.Instance;

        _maxHp = _baseHp + _extraHp; // ���� ü�� = �⺻ ü�� + ��ȭ ü��
        _atk = _baseAtk + _extraAtk;    // ���� ���ݷ� = �⺻ ���ݷ� + ��ȭ ���ݷ�

        // PlayerInfo ����ü
        gamaManager.Gold = _gold;
        gamaManager.Level = _level;

        gamaManager.MaxHp = _maxHp;
        gamaManager.CurrentHp = _maxHp;

        gamaManager.MaxExp = _maxExp;
        gamaManager.TotalExp = _totalExp;

        gamaManager.Atk = _atk;
        gamaManager.Critical = _critical;
        gamaManager.CriticalDamage = _criticalDamage;

        // PlayerUpgrade ����ü
        gamaManager.HpUpgradeLevel = _hpUpgradeLevel;
        gamaManager.AtkUpgradeLevel = _atkUpgradeLevel;
        gamaManager.CriticalUpgradeLevel = _criticalUpgradeLevel;
        gamaManager.CriticalDamageUpgradeLevel = _criticalDamageUpgradeLevel;

        gamaManager.BaseHp = _baseHp;
        gamaManager.ExtraHp = _extraHp;
        gamaManager.BaseAtk = _baseAtk;
        gamaManager.ExtraAtk = _extraAtk;
    }

    // �÷��̾� ���� ���
    private void InformationDisplay()
    {
        var gamaManager = GameManager.Instance;

        // �÷��̾� �ɷ�ġ
        PlayerGoldText.text = $"Gold : {gamaManager.Gold} $";
        PlayerLevelText.text = $"Level : {gamaManager.Level}";
        PlayerHpText.text = $"Hp : {gamaManager.MaxHp.ToString("F1")}";
        PlayerExpText.text = $"Exp : ({gamaManager.TotalExp.ToString("F1")} / {gamaManager.MaxExp.ToString("F1")})";
        PlayerAtkText.text = $"ATK : {gamaManager.Atk.ToString("F1")}";
        PlayerCriticalText.text = $"CRT : {gamaManager.Critical.ToString("F0")} %";
        PlayerCriticalDamageText.text = $"CDMG : {(gamaManager.CriticalDamage * 100).ToString("F0")} %";

        // ��ȭ ���
        HpUpgradeCostText.text = $"{_hpCost} $";
        AtkUpgradeCostText.text = $"{_atkCost} $";
        CriticalUpgradeCostText.text = $"{_criticalCost} $";
        CriticalDamageUpgradeCostText.text = $"{_criticalDamageCost} $";
    }

    // �÷��̾� �ɷ�ġ �ʱ�ȭ
    private void ResetState()
    {
        var gamaManager = GameManager.Instance;

        // PlayerInfo ����ü �ʱ�ȭ
        gamaManager.PlayerInfo.Gold = 0;             // ��� : 0
        gamaManager.PlayerInfo.Level = 1;            // ���� : 1

        gamaManager.PlayerInfo.MaxHp = 150.0f;       // �ʱ� ü�� : 150.0
        gamaManager.PlayerInfo.CurrentHp = 150.0f;   // ���� ü�� : �ʱⰪ MaxHp

        gamaManager.PlayerInfo.MaxExp = 100.0f;      // �ִ� ����ġ 100.0
        gamaManager.PlayerInfo.TotalExp = 0.0f;      // ���� ����ġ 0.0

        gamaManager.PlayerInfo.Atk = 10.0f;            // �⺻ ���ݷ� : 10.0
        gamaManager.PlayerInfo.Critical = 0.0f;        // ġ��Ÿ Ȯ�� : 0%
        gamaManager.PlayerInfo.CriticalDamage = 1.0f;  // ġ��Ÿ ���� : 100%

        gamaManager.PlayerInfo.Speed = 3.0f;         // �̵� �ӵ� : 3.0
        gamaManager.PlayerInfo.JumpPower = 7.5f;     // ������ : 7.5
        gamaManager.PlayerInfo.IsAlive = true;       // ���� ���� : �⺻�� true

        // PlayerUpgrade ����ü �ʱ�ȭ
        gamaManager.PlayerUpgrade.HpUpgradeLevel = 0;   // ü�� ��ȭ �ܰ� : 0
        gamaManager.PlayerUpgrade.AtkUpgradeLevel = 0;  // ���ݷ� ��ȭ �ܰ� : 0
        gamaManager.PlayerUpgrade.CriticalUpgradeLevel = 0;        // ġ��Ÿ Ȯ�� ��ȭ �ܰ� : 0
        gamaManager.PlayerUpgrade.CriticalDamageUpgradeLevel = 0;  // ġ��Ÿ ���� ��ȭ �ܰ� : 0

        gamaManager.PlayerUpgrade.BaseHp = 150.0f;   // ���������� ���� ü�� : 150.0
        gamaManager.PlayerUpgrade.ExtraHp = 0.0f;    // ��� ��ȭ�� ���� ü�� : 0.0
        gamaManager.PlayerUpgrade.BaseAtk = 10.0f;   // ���������� ���� ���ݷ� : 10.0
        gamaManager.PlayerUpgrade.ExtraAtk = 0.0f;   // ��� ��ȭ�� ���� ���ݷ� : 0.0

        // PlayerPrefs �ʱ�ȭ
        gamaManager.Gold = 0;             // ��� : 0
        gamaManager.Level = 1;            // ���� : 1

        gamaManager.MaxHp = 150.0f;       // �ʱ� ü�� : 150.0
        gamaManager.CurrentHp = 150.0f;   // ���� ü�� : �ʱⰪ MaxHp

        gamaManager.MaxExp = 100.0f;      // �ִ� ����ġ 100.0
        gamaManager.TotalExp = 0.0f;      // ���� ����ġ 0.0

        gamaManager.Atk = 10.0f;          // �⺻ ���ݷ� : 10.0
        gamaManager.Critical = 0.0f;       // ġ��Ÿ Ȯ�� : 0%
        gamaManager.CriticalDamage = 1.0f;  // ġ��Ÿ ���� : 100%

        gamaManager.HpUpgradeLevel = 0;   // ü�� ��ȭ �ܰ� : 0
        gamaManager.AtkUpgradeLevel = 0;  // ���ݷ� ��ȭ �ܰ� : 0
        gamaManager.CriticalUpgradeLevel = 0;        // ġ��Ÿ Ȯ�� ��ȭ �ܰ� : 0
        gamaManager.CriticalDamageUpgradeLevel = 0;  // ġ��Ÿ ���� ��ȭ �ܰ� : 0

        gamaManager.BaseHp = 150.0f;   // ���������� ���� ü�� : 150.0
        gamaManager.ExtraHp = 0.0f;    // ��� ��ȭ�� ���� ü�� : 0.0
        gamaManager.BaseAtk = 10.0f;   // ���������� ���� ���ݷ� : 10.0
        gamaManager.ExtraAtk = 0.0f;   // ��� ��ȭ�� ���� ���ݷ� : 0.0
    }
}