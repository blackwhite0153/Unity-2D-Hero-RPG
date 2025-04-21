using TMPro;
using UnityEngine;

public class EnemyInformation : MonoBehaviour
{
    // ���� �⺻ ���� ����
    private float _enemyHp;         // ü��
    private float _enemyAtk;        // ���ݷ�
    private float _enemyExpDrop;    // ����ġ
    private int _enemyGoldDrop;     // ���

    // ���� ���� ��� �ؽ�Ʈ
    public TMP_Text EnemyHpText;
    public TMP_Text EnemyAtkText;
    public TMP_Text EnemyExpDropText;
    public TMP_Text EnemyGoldDropText;

    private void Update()
    {
        GetState();             // ���� ���� ����ȭ
        UpdateState(GameManager.Instance.Level);    // ���� ������Ʈ
        InformationDisplay();   // �� ���� ���
    }

    // ���� GameManager�� EnemyInfo���� �� �ɷ�ġ�� ������ ����ȭ
    private void GetState()
    {
        _enemyHp = GameManager.Instance.EnemyInfo.MaxHp;
        _enemyAtk = GameManager.Instance.EnemyInfo.Atk;
        _enemyExpDrop = GameManager.Instance.EnemyInfo.ExpDrop;
        _enemyGoldDrop = GameManager.Instance.EnemyInfo.GoldDrop;
    }

    // �÷��̾� ������ �������� ���� �ɷ�ġ ����
    private void UpdateState(int playerLevel)
    {
        // �÷��̾� ������ �ö󰥼��� ���� �ɷ�ġ�� ����
        _enemyHp = 180 * Mathf.Pow(1.15f, playerLevel- 1);                // ü���� �÷��̾�� 1.2�� �� ���ϰ� ����
        _enemyAtk = 12 * Mathf.Pow(1.12f, playerLevel- 1);                // ���ݷ��� �÷��̾�� 1.15�� �� ���ϰ� ����
        _enemyExpDrop = 20 * Mathf.Pow(1.12f, playerLevel - 1);           // ����ġ ��� ����
        _enemyGoldDrop = (int)(15 * Mathf.Pow(1.12f, playerLevel - 1));   // ��� ����� ����
    }

    // �� ���� ���
    private void InformationDisplay()
    {
        EnemyHpText.text = $"Hp : {_enemyHp.ToString("F1")}";
        EnemyAtkText.text = $"Atk : {_enemyAtk.ToString("F1")}";
        EnemyExpDropText.text = $"Exp Drop : {_enemyExpDrop.ToString("F1")}";
        EnemyGoldDropText.text = $"Gold Drop : {_enemyGoldDrop} $";
    }
}