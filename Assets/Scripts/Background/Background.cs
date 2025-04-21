using UnityEngine;

public class Background : MonoBehaviour
{
    // ����� Renderer ������Ʈ�� ������ ����
    private Renderer _renderer;

    // �ؽ�ó�� ������ ���� ������ ����
    private float _offsetX = 0.0f;
    private float _offsetY = 0.0f;

    // ��� ��ũ�� ������ �����ϴ� ����
    public bool Horizontal;         // ���� ���� ��ũ�� ����
    public bool Vertical;           // ���� ���� ��ũ�� ����
    public bool InverseHorizontal;  // ���� ���� ���� ����
    public bool InverseVertical;    // ���� ���� ���� ����

    // ��� ��ũ�� �ӵ� ���� ����
    public float ScrollSpeed;

    void Start()
    {
        // Renderer ������Ʈ�� �����ͼ� _renderer ������ ����
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // ���� �̵� Ȱ��ȭ ��, ���� �̵� �Լ� ȣ��
        if (Horizontal) HorizontalMove();
        // ���� �̵� Ȱ��ȭ ��, ���� �̵� �Լ� ȣ��
        if (Vertical) VerticalMove();

        // �ؽ�ó�� �������� �����ϴ� �Լ� ȣ��
        BackgroundMove();
    }

    // ���� �̵��� ó���ϴ� �Լ�
    private void HorizontalMove()
    {
        // �ð�(Time.deltaTime)�� �̿��� ���������� X�� �������� �̵�
        // Mathf.Repeat�� ����� 0~1 �������� �ݺ��ǵ��� ����
        _offsetX += Time.deltaTime * ScrollSpeed;
        _offsetX = Mathf.Repeat(_offsetX, 1.0f);
    }

    // ���� �̵��� ó���ϴ� �Լ�
    private void VerticalMove()
    {
        // �ð�(Time.deltaTime)�� �̿��� ���������� Y�� �������� �̵�
        // Mathf.Repeat�� ����� 0~1 �������� �ݺ��ǵ��� ����
        _offsetY += Time.deltaTime * ScrollSpeed;
        _offsetY = Mathf.Repeat(_offsetY, 1.0f);
    }

    // �ؽ�ó�� �̵��� ������ �����ϴ� �Լ�
    private void BackgroundMove()
    {
        // ���� �Ǵ� ���� ���� ���� ���ο� ���� �������� �ݴ�� ����
        Vector2 offset = new Vector2(InverseHorizontal ? -_offsetX : _offsetX,
                                     InverseVertical ? -_offsetY : _offsetY);

        // Renderer�� Material�� �ؽ�ó ������ ���� �����Ͽ� ��ũ�� ȿ�� ����
        _renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}