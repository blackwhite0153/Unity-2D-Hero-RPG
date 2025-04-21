using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// �� ���� ���θ� �����ϴ� ����ü
public struct SelectMap
{
    public bool Forest;
    public bool DarkForest;
}

// �÷��̾��� �⺻ �ɷ�ġ�� �����ϴ� ����ü
public struct PlayerInfo
{
    public int Gold;            // ���
    public int Level;           // ����
    public float MaxHp;         // �ִ� ü��
    public float CurrentHp;     // ���� ü��
    public float MaxExp;        // �ִ� ����ġ
    public float TotalExp;      // ���� ����ġ
    public float Atk;           // ���ݷ�
    public float Critical;        // ũ��Ƽ�� Ȯ��
    public float CriticalDamage;  // ũ��Ƽ�� ������
    public float Speed;         // �̵� �ӵ�
    public float JumpPower;     // ������
    public bool IsAlive;        // ���� ����
}

// �÷��̾� �ɷ�ġ ��ȭ ���¸� �����ϴ� ����ü
public struct PlayerUpgrade
{
    public int HpUpgradeLevel;              // ü�� ��ȭ �ܰ�
    public int AtkUpgradeLevel;             // ���ݷ� ��ȭ �ܰ�
    public int CriticalUpgradeLevel;        // ġ��Ÿ Ȯ�� ��ȭ �ܰ�
    public int CriticalDamageUpgradeLevel;  // ġ��Ÿ ���� ��ȭ �ܰ�
    public float BaseHp;    // ���������� ������ ü��
    public float ExtraHp;   // ��� ��ȭ�� �߰��� ü��
    public float BaseAtk;   // ���������� �������� ���ݷ�
    public float ExtraAtk;  // ��� ��ȭ�� �߰��� ���ݷ�
}

// ���� �⺻ �ɷ�ġ�� �����ϴ� ����ü
public struct EnemyInfo
{
    public float MaxHp;     // �ִ� ü��
    public float CurrentHp; // �ִ� ü��
    public float Atk;       // ���ݷ�
    public float Speed;     // �̵� �ӵ�
    public float ExpDrop;   // ����ġ  
    public int GoldDrop;    // ���
}

// ������ �ֿ� �ý����� �����ϴ� �̱��� Ŭ����
public class GameManager : Singleton<GameManager>
{
    private GameObject GameOver_Object;
    private GameObject Profile_Object;
    private GameObject Hp_Object;
    private GameObject Exp_Object;
    private GameObject Gold_Object;

    // �÷��̾��� �̵� ���� ����
    private Vector2 _moveDir;

    // �̵� ������ ����� �� ����� �̺�Ʈ
    public event Action<Vector2> OnMoveDirChanged;

    // �� ���� ���θ� �����ϴ� ����ü ����
    public SelectMap SelectMap = new SelectMap()
    {
        Forest = false,
        DarkForest = false,
    };

    // �÷��̾��� �ɷ�ġ�� �����ϴ� ����ü ����
    public PlayerInfo PlayerInfo = new PlayerInfo()
    {
        Gold = 0,             // ��� : 0
        Level = 1,            // ���� : 1
        MaxHp = 150.0f,       // �ʱ� ü�� : 150.0
        CurrentHp = 150.0f,   // ���� ü�� : �ʱⰪ MaxHp
        MaxExp = 100.0f,      // �ִ� ����ġ 100.0
        TotalExp = 0.0f,      // ���� ����ġ 0.0
        Atk = 10.0f,          // �ʱ� ���ݷ� : 10.0
        Critical = 0.0f,         // �ʱ� ġ��Ÿ Ȯ�� : 0%
        CriticalDamage = 1.0f,   // �ʱ� ġ��Ÿ ���� : 100%
        Speed = 3.0f,         // �̵� �ӵ� : 3.0
        JumpPower = 7.5f,     // ������ : 7.5
        IsAlive = true,       // ���� ���� : �⺻�� true
    };

    // �÷��̾� �ɷ�ġ ��ȭ ���¸� �����ϴ� ����ü ����
    public PlayerUpgrade PlayerUpgrade = new PlayerUpgrade()
    {
        HpUpgradeLevel = 0,             // ü�� ��ȭ �ܰ� : 0
        AtkUpgradeLevel = 0,            // ���ݷ� ��ȭ �ܰ� : 0
        CriticalUpgradeLevel = 0,       // ġ��Ÿ Ȯ�� ��ȭ �ܰ� : 0
        CriticalDamageUpgradeLevel = 0, // ġ��Ÿ ���� ��ȭ �ܰ� : 0
        BaseHp = 150.0f,
        ExtraHp = 0.0f,
        BaseAtk = 10.0f,
        ExtraAtk = 0.0f,
    };

    // �÷��̾��� �ɷ�ġ�� �����ϴ� ����ü ����
    public EnemyInfo EnemyInfo = new EnemyInfo()
    {
        MaxHp = 180.0f,     // �ʱ� ü�� : 50.0
        CurrentHp = 180.0f, // ���� ü�� : �ʱⰪ MaxHp
        Atk = 12.0f,        // �ʱ� ���ݷ� : 1.0
        Speed = 2.0f,       // �̵� �ӵ� : 2.0
        ExpDrop = 20.0f,    // ����ġ ȹ�淮 : 10.0
        GoldDrop = 15,      // ��� ����� : 10
    };

#region Player Infomation

    public int Gold
    {
        get { return PlayerPrefs.GetInt("Gold"); }
        set
        {
            PlayerInfo.Gold = value;
            PlayerPrefs.SetInt("Gold", PlayerInfo.Gold);
        }
    }

    public int Level
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetInt("Level") <= 0)
            {
                PlayerPrefs.SetInt("Level", PlayerInfo.Level);
            }
            return PlayerPrefs.GetInt("Level");
        }
        set
        {
            PlayerInfo.Level = value;
            PlayerPrefs.SetInt("Level", PlayerInfo.Level);
        }
    }

    public float MaxHp
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetFloat("MaxHp") <= 0.0f)
            {
                PlayerPrefs.SetFloat("MaxHp", PlayerInfo.MaxHp);
            }
            return PlayerPrefs.GetFloat("MaxHp");
        }
        set
        {
            PlayerInfo.MaxHp = value;
            PlayerPrefs.SetFloat("MaxHp", PlayerInfo.MaxHp);
        }
    }

    public float CurrentHp
    {
        get { return PlayerInfo.CurrentHp; }
        set { PlayerInfo.CurrentHp = value; }
    }

    public float MaxExp
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetFloat("MaxExp") <= 0.0f)
            {
                PlayerPrefs.SetFloat("MaxExp", PlayerInfo.MaxExp);
            }
            return PlayerPrefs.GetFloat("MaxExp");
        }
        set
        {
            PlayerInfo.MaxExp = value;
            PlayerPrefs.SetFloat("MaxExp", PlayerInfo.MaxExp);
        }
    }

    public float TotalExp
    {
        get { return PlayerPrefs.GetFloat("TotalExp"); }
        set
        {
            PlayerInfo.TotalExp = value;
            PlayerPrefs.SetFloat("TotalExp", PlayerInfo.TotalExp);
        }
    }

    public float Atk
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetFloat("Atk") <= 0.0f)
            {
                PlayerPrefs.SetFloat("Atk", PlayerInfo.Atk);
            }
            return PlayerPrefs.GetFloat("Atk");
        }
        set
        {
            PlayerInfo.Atk = value;
            PlayerPrefs.SetFloat("Atk", PlayerInfo.Atk);
        }
    }

    public float Critical
    {
        get { return PlayerPrefs.GetFloat("Critical"); }
        set
        {
            PlayerInfo.Critical = value;
            PlayerPrefs.SetFloat("Critical", PlayerInfo.Critical);
        }
    }

    public float CriticalDamage
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetFloat("CriticalDamage") <= 0.0f)
            {
                PlayerPrefs.SetFloat("CriticalDamage", PlayerInfo.CriticalDamage);
            }
            return PlayerPrefs.GetFloat("CriticalDamage");
        }
        set
        {
            PlayerInfo.CriticalDamage = value;
            PlayerPrefs.SetFloat("CriticalDamage", PlayerInfo.CriticalDamage);
        }
    }

#endregion

#region Player Upgrade

    public int HpUpgradeLevel
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetInt("HpUpgradeLevel") <= 0)
            {
                PlayerPrefs.SetInt("HpUpgradeLevel", PlayerUpgrade.HpUpgradeLevel);
            }
            return PlayerPrefs.GetInt("HpUpgradeLevel");
        }
        set
        {
            PlayerUpgrade.HpUpgradeLevel = value;
            PlayerPrefs.SetInt("HpUpgradeLevel", PlayerUpgrade.HpUpgradeLevel);
        }
    }

    public int AtkUpgradeLevel
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetInt("AtkUpgradeLevel") <= 0)
            {
                PlayerPrefs.SetInt("AtkUpgradeLevel", PlayerUpgrade.AtkUpgradeLevel);
            }
            return PlayerPrefs.GetInt("AtkUpgradeLevel");
        }
        set
        {
            PlayerUpgrade.AtkUpgradeLevel = value;
            PlayerPrefs.SetInt("AtkUpgradeLevel", PlayerUpgrade.AtkUpgradeLevel);
        }
    }

    public int CriticalUpgradeLevel
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetInt("CriticalUpgradeLevel") <= 0)
            {
                PlayerPrefs.SetInt("CriticalUpgradeLevel", PlayerUpgrade.CriticalUpgradeLevel);
            }
            return PlayerPrefs.GetInt("CriticalUpgradeLevel");
        }
        set
        {
            PlayerUpgrade.CriticalUpgradeLevel = value;
            PlayerPrefs.SetInt("CriticalUpgradeLevel", PlayerUpgrade.CriticalUpgradeLevel);
        }
    }

    public int CriticalDamageUpgradeLevel
    {
        get
        {
            // �ʱ⿣ ����� ���� ���⿡ �ʱ�ȭ �ѹ� ���ش�.
            if (PlayerPrefs.GetInt("CriticalDamageUpgradeLevel") <= 0)
            {
                PlayerPrefs.SetInt("CriticalDamageUpgradeLevel", PlayerUpgrade.CriticalDamageUpgradeLevel);
            }
            return PlayerPrefs.GetInt("CriticalDamageUpgradeLevel");
        }
        set
        {
            PlayerUpgrade.CriticalDamageUpgradeLevel = value;
            PlayerPrefs.SetInt("CriticalDamageUpgradeLevel", PlayerUpgrade.CriticalDamageUpgradeLevel);
        }
    }

    public float BaseHp
    {
        get
        {
            if (PlayerPrefs.GetFloat("BaseHp") <= 0.0f)
            {
                PlayerPrefs.SetFloat("BaseHp", PlayerUpgrade.BaseHp);
            }
            return PlayerPrefs.GetFloat("BaseHp");
        }
        set
        {
            PlayerUpgrade.BaseHp = value;
            PlayerPrefs.SetFloat("BaseHp", PlayerUpgrade.BaseHp);
        }
    }

    public float ExtraHp
    {
        get { return PlayerPrefs.GetFloat("ExtraHp"); }
        set
        {
            PlayerUpgrade.ExtraHp = value;
            PlayerPrefs.SetFloat("ExtraHp", PlayerUpgrade.ExtraHp);
        }
    }

    public float BaseAtk
    {
        get
        {
            if (PlayerPrefs.GetFloat("BaseAtk") <= 0.0f)
            {
                PlayerPrefs.SetFloat("BaseAtk", PlayerUpgrade.BaseAtk);
            }
            return PlayerPrefs.GetFloat("BaseAtk");
        }
        set
        {
            PlayerUpgrade.BaseAtk = value;
            PlayerPrefs.SetFloat("BaseAtk", PlayerUpgrade.BaseAtk);
        }
    }

    public float ExtraAtk
    {
        get { return PlayerPrefs.GetFloat("ExtraAtk"); }
        set
        {
            PlayerUpgrade.ExtraAtk = value;
            PlayerPrefs.SetFloat("ExtraAtk", PlayerUpgrade.ExtraAtk);
        }
    }

#endregion

    // �̵� ������ �����ϰ� ����� �� �̺�Ʈ ȣ��
    public Vector2 MoveDir
    {
        // ���� �̵� ���� ��ȯ
        get { return _moveDir; }
        set
        {
            // �̵� ���� �� ������Ʈ
            _moveDir = value;
            // �̵� ���� ���� �̺�Ʈ ����
            OnMoveDirChanged?.Invoke(value);
        }
    }

    // ���� Ÿ������ �� �Ǵ� ������Ʈ (��: ���� ���)
    public GameObject Target { get; set; }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == Define.GameScene) ObjectSetting();
    }

    // GameManager ������Ʈ ����
    private void ObjectSetting()
    {
        GameObject canvas = GameObject.Find("UI_Game");
        if (canvas == null) return;

        GameOver_Object = FindObjectInCanvas(canvas, "GameOver");
        Profile_Object = FindObjectInCanvas(canvas, "Profile");
        Hp_Object = FindObjectInCanvas(canvas, "Profile/Hp");
        Exp_Object = FindObjectInCanvas(canvas, "Profile/Exp");
        Gold_Object = FindObjectInCanvas(canvas, "Profile/Gold");
    }

    // canvas ������Ʈ ������ path ������Ʈ ��ȯ
    private GameObject FindObjectInCanvas(GameObject canvas, string path)
    {
        GameObject child = canvas.transform.Find(path).gameObject;

        return child ? child : null;
    }

    // ���� ���� ����
    public void GameOverSetting()
    {
        bool isAlive = PlayerInfo.IsAlive;

        SetActiveState(GameOver_Object, !isAlive);
        SetActiveState(Profile_Object, isAlive);
        SetActiveState(Hp_Object, isAlive);
        SetActiveState(Exp_Object, isAlive);
        SetActiveState(Gold_Object, isAlive);
    }

    // state ���¿� ���� obj Ȱ��ȭ or ��Ȱ��ȭ
    private void SetActiveState(GameObject obj, bool state)
    {
        // obj�� null���� �ƴ��� üũ
        if (obj) obj.SetActive(state);
    }
}