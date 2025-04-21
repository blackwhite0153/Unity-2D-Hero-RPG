using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// 맵 선택 여부를 저장하는 구조체
public struct SelectMap
{
    public bool Forest;
    public bool DarkForest;
}

// 플레이어의 기본 능력치를 저장하는 구조체
public struct PlayerInfo
{
    public int Gold;            // 골드
    public int Level;           // 레벨
    public float MaxHp;         // 최대 체력
    public float CurrentHp;     // 현재 체력
    public float MaxExp;        // 최대 경험치
    public float TotalExp;      // 누적 경험치
    public float Atk;           // 공격력
    public float Critical;        // 크리티컬 확률
    public float CriticalDamage;  // 크리티컬 데미지
    public float Speed;         // 이동 속도
    public float JumpPower;     // 점프력
    public bool IsAlive;        // 생존 여부
}

// 플레이어 능력치 강화 상태를 저장하는 구조체
public struct PlayerUpgrade
{
    public int HpUpgradeLevel;              // 체력 강화 단계
    public int AtkUpgradeLevel;             // 공격력 강화 단계
    public int CriticalUpgradeLevel;        // 치명타 확률 강화 단계
    public int CriticalDamageUpgradeLevel;  // 치명타 피해 강화 단계
    public float BaseHp;    // 레벨업으로 증가된 체력
    public float ExtraHp;   // 골드 강화로 추가된 체력
    public float BaseAtk;   // 레벨업으로 증가도니 공격력
    public float ExtraAtk;  // 골드 강화로 추가된 공격력
}

// 적의 기본 능력치를 저장하는 구조체
public struct EnemyInfo
{
    public float MaxHp;     // 최대 체력
    public float CurrentHp; // 최대 체력
    public float Atk;       // 공격력
    public float Speed;     // 이동 속도
    public float ExpDrop;   // 경험치  
    public int GoldDrop;    // 골드
}

// 게임의 주요 시스템을 관리하는 싱글톤 클래스
public class GameManager : Singleton<GameManager>
{
    private GameObject GameOver_Object;
    private GameObject Profile_Object;
    private GameObject Hp_Object;
    private GameObject Exp_Object;
    private GameObject Gold_Object;

    // 플레이어의 이동 방향 벡터
    private Vector2 _moveDir;

    // 이동 방향이 변경될 때 실행될 이벤트
    public event Action<Vector2> OnMoveDirChanged;

    // 맵 선택 여부를 저장하는 구조체 변수
    public SelectMap SelectMap = new SelectMap()
    {
        Forest = false,
        DarkForest = false,
    };

    // 플레이어의 능력치를 저장하는 구조체 변수
    public PlayerInfo PlayerInfo = new PlayerInfo()
    {
        Gold = 0,             // 골드 : 0
        Level = 1,            // 레벨 : 1
        MaxHp = 150.0f,       // 초기 체력 : 150.0
        CurrentHp = 150.0f,   // 현재 체력 : 초기값 MaxHp
        MaxExp = 100.0f,      // 최대 경험치 100.0
        TotalExp = 0.0f,      // 누적 경험치 0.0
        Atk = 10.0f,          // 초기 공격력 : 10.0
        Critical = 0.0f,         // 초기 치명타 확률 : 0%
        CriticalDamage = 1.0f,   // 초기 치명타 피해 : 100%
        Speed = 3.0f,         // 이동 속도 : 3.0
        JumpPower = 7.5f,     // 점프력 : 7.5
        IsAlive = true,       // 생존 여부 : 기본값 true
    };

    // 플레이어 능력치 강화 상태를 저장하는 구조체 변수
    public PlayerUpgrade PlayerUpgrade = new PlayerUpgrade()
    {
        HpUpgradeLevel = 0,             // 체력 강화 단계 : 0
        AtkUpgradeLevel = 0,            // 공격력 강화 단계 : 0
        CriticalUpgradeLevel = 0,       // 치명타 확률 강화 단계 : 0
        CriticalDamageUpgradeLevel = 0, // 치명타 피해 강화 단계 : 0
        BaseHp = 150.0f,
        ExtraHp = 0.0f,
        BaseAtk = 10.0f,
        ExtraAtk = 0.0f,
    };

    // 플레이어의 능력치를 저장하는 구조체 변수
    public EnemyInfo EnemyInfo = new EnemyInfo()
    {
        MaxHp = 180.0f,     // 초기 체력 : 50.0
        CurrentHp = 180.0f, // 현재 체력 : 초기값 MaxHp
        Atk = 12.0f,        // 초기 공격력 : 1.0
        Speed = 2.0f,       // 이동 속도 : 2.0
        ExpDrop = 20.0f,    // 경험치 획득량 : 10.0
        GoldDrop = 15,      // 골드 드랍량 : 10
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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
            // 초기엔 저장된 값이 없기에 초기화 한번 해준다.
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

    // 이동 방향을 설정하고 변경될 때 이벤트 호출
    public Vector2 MoveDir
    {
        // 현재 이동 방향 반환
        get { return _moveDir; }
        set
        {
            // 이동 방향 값 업데이트
            _moveDir = value;
            // 이동 방향 변경 이벤트 실행
            OnMoveDirChanged?.Invoke(value);
        }
    }

    // 현재 타겟팅한 적 또는 오브젝트 (예: 공격 대상)
    public GameObject Target { get; set; }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == Define.GameScene) ObjectSetting();
    }

    // GameManager 오브젝트 세팅
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

    // canvas 오브젝트 내부의 path 오브젝트 반환
    private GameObject FindObjectInCanvas(GameObject canvas, string path)
    {
        GameObject child = canvas.transform.Find(path).gameObject;

        return child ? child : null;
    }

    // 게임 오버 세팅
    public void GameOverSetting()
    {
        bool isAlive = PlayerInfo.IsAlive;

        SetActiveState(GameOver_Object, !isAlive);
        SetActiveState(Profile_Object, isAlive);
        SetActiveState(Hp_Object, isAlive);
        SetActiveState(Exp_Object, isAlive);
        SetActiveState(Gold_Object, isAlive);
    }

    // state 상태에 따라 obj 활성화 or 비활성화
    private void SetActiveState(GameObject obj, bool state)
    {
        // obj가 null인지 아닌지 체크
        if (obj) obj.SetActive(state);
    }
}