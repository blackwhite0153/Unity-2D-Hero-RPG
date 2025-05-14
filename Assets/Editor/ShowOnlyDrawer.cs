using UnityEditor;
using UnityEngine;

// 커스텀 속성인 ShowOnlyAttribute가 붙은 필드에 대해 사용자 정의 Drawer를 설정
[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    // Unity가 인스펙터에서 해당 속성을 그릴 때 호출되는 메서드
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string valueString;

        // SerializedProperty의 타입에 따라 문자열로 변환하여 읽기 전용으로 보여주기 위해 switch문 사용
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueString = property.intValue.ToString();     // int 값을 문자열로 변환
                break;
            case SerializedPropertyType.Float:
                valueString = property.floatValue.ToString();   // float 값을 문자열로 변환
                break;
            case SerializedPropertyType.String:
                valueString = property.stringValue;             // 문자열은 그대로 사용
                break;
            case SerializedPropertyType.Boolean:
                valueString = property.boolValue.ToString();    // bool 값을 문자열로 변환
                break;
            case SerializedPropertyType.ObjectReference:
                // 오브젝트가 null이 아니면 해당 이름을 사용, null이면 ( None ) 표시
                valueString = property.objectReferenceValue != null ? property.objectReferenceValue.name : "( None )";
                break;
            default:
                // 지원하지 않는 타입일 경우 기본 메시지 표시
                valueString = "( Not Supported )";
                break;
        }
        // 실제 인스펙터에 라벨 필드로 출력 (편집 불가능한 형태)
        EditorGUI.LabelField(position, label.text, valueString);
    }
}