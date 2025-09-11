/// <summary>
/// 아이템 효과의 종류를 나타내는 열거형
/// </summary>
public enum EffectType { Symbol, Pattern, Stress }

/// <summary>
/// 아이템 효과를 받을 수 있는 모든 객체가 구현해야 하는 인터페이스
/// </summary>
public interface IItemEffectReceiver
{
    // 자신이 처리할 수 있는 효과 타입인지 확인
    bool CanReceive(EffectType type);
    // 실제 효과를 적용
    void ReceiveEffect(EffectType type, object data);
}
