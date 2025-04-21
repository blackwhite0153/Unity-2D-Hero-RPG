using UnityEngine;

public class Background : MonoBehaviour
{
    // 배경의 Renderer 컴포넌트를 저장할 변수
    private Renderer _renderer;

    // 텍스처의 오프셋 값을 저장할 변수
    private float _offsetX = 0.0f;
    private float _offsetY = 0.0f;

    // 배경 스크롤 방향을 설정하는 변수
    public bool Horizontal;         // 수평 방향 스크롤 여부
    public bool Vertical;           // 수직 방향 스크롤 여부
    public bool InverseHorizontal;  // 수평 방향 반전 여부
    public bool InverseVertical;    // 수직 방향 반전 여부

    // 배경 스크롤 속도 설정 변수
    public float ScrollSpeed;

    void Start()
    {
        // Renderer 컴포넌트를 가져와서 _renderer 변수에 저장
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // 수평 이동 활성화 시, 수평 이동 함수 호출
        if (Horizontal) HorizontalMove();
        // 수직 이동 활성화 시, 수직 이동 함수 호출
        if (Vertical) VerticalMove();

        // 텍스처의 오프셋을 적용하는 함수 호출
        BackgroundMove();
    }

    // 수평 이동을 처리하는 함수
    private void HorizontalMove()
    {
        // 시간(Time.deltaTime)을 이용해 지속적으로 X축 방향으로 이동
        // Mathf.Repeat을 사용해 0~1 범위에서 반복되도록 설정
        _offsetX += Time.deltaTime * ScrollSpeed;
        _offsetX = Mathf.Repeat(_offsetX, 1.0f);
    }

    // 수직 이동을 처리하는 함수
    private void VerticalMove()
    {
        // 시간(Time.deltaTime)을 이용해 지속적으로 Y축 방향으로 이동
        // Mathf.Repeat을 사용해 0~1 범위에서 반복되도록 설정
        _offsetY += Time.deltaTime * ScrollSpeed;
        _offsetY = Mathf.Repeat(_offsetY, 1.0f);
    }

    // 텍스처의 이동을 실제로 적용하는 함수
    private void BackgroundMove()
    {
        // 수평 또는 수직 방향 반전 여부에 따라 오프셋을 반대로 적용
        Vector2 offset = new Vector2(InverseHorizontal ? -_offsetX : _offsetX,
                                     InverseVertical ? -_offsetY : _offsetY);

        // Renderer의 Material의 텍스처 오프셋 값을 변경하여 스크롤 효과 적용
        _renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}