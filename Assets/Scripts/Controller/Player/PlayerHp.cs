using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    private float _maxHp;       // �ִ� ü��
    private float _currentHp;   // ���� ü��

    // ü�¹� UI
    public Slider HpBar;    // ü�¹�
    public TMP_Text HpText; // ü�� ��ġ�� ǥ���� �ؽ�Ʈ

    private void Start()
    {
        // ���� ���� �� �ִ� ü�� ���� ������ ����
        _maxHp = GameManager.Instance.MaxHp;

        // ���� ü���� �ִ� ü������ �ʱ�ȭ
        GameManager.Instance.CurrentHp = _maxHp;
        _currentHp = GameManager.Instance.CurrentHp;
    }

    // �����Ӹ��� ü�� ���¸� ������Ʈ�ϰ� UI�� ����
    private void Update()
    {
        UpdateState();
        DisplayHp();
    }

    // ü�� ���¸� ����
    private void UpdateState()
    {
        // GameManager���� ���� ü�°� �ִ� ü�� ���� �����´�.
        _maxHp = GameManager.Instance.MaxHp;
        _currentHp = GameManager.Instance.CurrentHp;

        // ü�¹��� �ִ밪�� ���簪�� �����Ͽ� UI ������Ʈ
        HpBar.maxValue = _maxHp;
        HpBar.value = _currentHp;
    }

    // ü�� ��ġ�� �ؽ�Ʈ�� ǥ��
    private void DisplayHp()
    {
        // (���� ü�� / �ִ� ü��) �Ҽ��� 1�ڸ����� ǥ�� ToString("F1")
        HpText.text = $"{_currentHp.ToString("F1")} / {_maxHp.ToString("F1")}";
    }
}