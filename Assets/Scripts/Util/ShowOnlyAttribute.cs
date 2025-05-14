using UnityEngine;

// ShowOnlyAttribute는 Unity Inspector에서 해당 변수를 읽기 전용으로 표시하기 위한 사용자 정의 속성(Attribute)
// 이 속성은 단순히 표시 용도로 사용되며, 직접적으로 변수의 수정 가능 여부를 제어하지는 않는다.
// 실제로 Inspector에서 읽기 전용으로 보이게 하려면 커스텀 에디터(Editor)를 통해 이 속성을 처리해주는 로직이 필요
public class ShowOnlyAttribute : PropertyAttribute { }