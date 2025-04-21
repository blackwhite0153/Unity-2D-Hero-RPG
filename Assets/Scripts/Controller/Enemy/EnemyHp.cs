using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHp : MonoBehaviour
{
    private EnemyController _enemy;

    private float _maxHp;       // �ִ� ü��
    private float _currentHp;   // ���� ü��

    // ü�¹� UI
    public Slider HpBar;    // ü�¹�

    private void Start()
    {
        _enemy = GetComponentInParent<EnemyController>();

        // ���� ���� �� �ִ� ü�� ���� ������ ����
        _maxHp = GameManager.Instance.EnemyInfo.MaxHp;
        // ���� ü���� �ִ� ü������ �ʱ�ȭ
        _currentHp = GameManager.Instance.EnemyInfo.CurrentHp;
    }

    // �����Ӹ��� ü�� ���¸� ������Ʈ�ϰ� UI�� ����
    private void Update()
    {
        UpdateState();
    }

    // ü�� ���¸� ����
    private void UpdateState()
    {
        // GameManager���� �ִ� ü�� ���� �����´�.
        _maxHp = GameManager.Instance.EnemyInfo.MaxHp;
        // EnemyController���� ���� ü���� �����´�.
        _currentHp = _enemy.CurrentHp;

        // ü�¹��� �ִ밪�� ���簪�� �����Ͽ� UI ������Ʈ
        HpBar.maxValue = _maxHp;
        HpBar.value = _currentHp;
    }
}