using UnityEngine;

// ��(Enemy)�� �ɷ�ġ�� �����ϴ� Ŭ���� (BaseController�� ���)
public class Enemy : BaseController
{
    // ���� �⺻ ���� ����
    private float _maxHp;       // �ִ� ü��
    private float _atk;         // ���ݷ�
    private float _expDrop;     // ����ġ
    private int _goldDrop;      // ���

    // �ʱ�ȭ �Լ� (BaseController�� Initialize �������̵�)
    protected override void Initialize()
    {

    }

    // ������Ʈ Ȱ��ȭ �� ȣ��
    private void OnEnable()
    {
        GetState();
        UpdateState(GameManager.Instance.Level);
    }

    // ���� GameManager�� EnemyInfo���� �� �ɷ�ġ�� ������ ����ȭ
    private void GetState()
    {
        _maxHp = GameManager.Instance.EnemyInfo.MaxHp;
        _atk = GameManager.Instance.EnemyInfo.Atk;
        _expDrop = GameManager.Instance.EnemyInfo.ExpDrop;
        _goldDrop = GameManager.Instance.EnemyInfo.GoldDrop;
    }

    // �÷��̾� ������ �������� ���� �ɷ�ġ ����
    private void UpdateState(int playerLevel)
    {
        // �÷��̾� ������ �ö󰥼��� ���� �ɷ�ġ�� ����
        _maxHp = 180 * Mathf.Pow(1.15f, playerLevel - 1);            // ü���� �÷��̾�� 1.2�� �� ���ϰ� ����
        _atk = 12 * Mathf.Pow(1.12f, playerLevel - 1);               // ���ݷ��� �÷��̾�� 1.15�� �� ���ϰ� ����
        _expDrop = 20 * Mathf.Pow(1.12f, playerLevel - 1);           // ����ġ ��� ����
        _goldDrop = (int)(15 * Mathf.Pow(1.12f, playerLevel - 1));   // ��� ����� ����

        // ����� �ɷ�ġ�� GameManager�� EnemyInfo�� �ݿ�
        GameManager.Instance.EnemyInfo.MaxHp = _maxHp;
        GameManager.Instance.EnemyInfo.CurrentHp = _maxHp;
        GameManager.Instance.EnemyInfo.Atk = _atk;
        GameManager.Instance.EnemyInfo.ExpDrop = _expDrop;
        GameManager.Instance.EnemyInfo.GoldDrop = _goldDrop;
    }
}