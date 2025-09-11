using UnityEngine;
using UnityEngine.Events;

public class PlayerStress : MonoBehaviour, IItemEffectReceiver
{
    public float currentStress { get; private set; } = 50f;
    public float maxStress { get; private set; } = 100f;

    // 다른 스크립트가 스트레스 변화를 감지할 수 있도록 이벤트를 추가합니다.
    public UnityEvent<float> OnStressChanged;
    public UnityEvent OnMaxStressReached;
    public UnityEvent<float> OnMaxStressChanged;

    private void Start()
    {
        // ItemManager가 존재하면 자신을 등록합니다.
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.RegisterReceiver(this);
        }
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 ItemManager에서 등록을 해제합니다. (null 참조 방지)
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.UnregisterReceiver(this);
        }
    }

    public void AddStress(float amount)
    {
        currentStress = Mathf.Min(currentStress + amount, maxStress);
        OnStressChanged?.Invoke(currentStress);
        if (currentStress >= maxStress)
        {
            OnMaxStressReached?.Invoke();
        }
    }

    public void ReduceStress(float amount)
    {
        currentStress = Mathf.Max(currentStress - amount, 0f);
        OnStressChanged?.Invoke(currentStress);
    }

    public void IncreaseMaxStress(float amount)
    {
        maxStress += amount;
        OnMaxStressChanged?.Invoke(maxStress);

        // 최대치가 증가하면 현재 스트레스도 그에 맞춰 조정
        currentStress = Mathf.Min(currentStress, maxStress);
        OnStressChanged?.Invoke(currentStress);
    }

    public void DecreaseMaxStress(float amount)
    {
        maxStress = Mathf.Max(maxStress - amount, 0f);
        OnMaxStressChanged?.Invoke(maxStress);

        // 최대치가 감소하면 현재 스트레스가 최대치를 초과하지 않도록 조정
        currentStress = Mathf.Min(currentStress, maxStress);
        OnStressChanged?.Invoke(currentStress);
    }

    #region IItemEffectReceiver Implementation
    public bool CanReceive(EffectType type)
    {
        // 이 클래스는 스트레스 효과만 처리할 수 있습니다.
        return type == EffectType.Stress;
    }

    public void ReceiveEffect(EffectType type, object data)
    {
        if (type == EffectType.Stress && data is StressEffectData stressData)
        {
            float amount = stressData.Amount;
            switch (stressData.StressType)
            {
                case StressType.CurrentStress:
                    if (amount > 0) AddStress(amount); else ReduceStress(Mathf.Abs(amount));
                    break;
                case StressType.MaxStress:
                    if (amount > 0) IncreaseMaxStress(amount); else DecreaseMaxStress(Mathf.Abs(amount));
                    break;
            }
            Debug.Log($"스트레스 효과 적용 완료: 타입({stressData.StressType}), 양({amount})");
        }
    }
    #endregion
}
