using UnityEngine;
using UnityEngine.Events;

public class PlayerStress : MonoBehaviour
{
    public float currentStress { get; private set; } = 50f;
    public float maxStress { get; private set; } = 100f;

    // 다른 스크립트가 스트레스 변화를 감지할 수 있도록 이벤트를 추가합니다.
    public UnityEvent<float> OnStressChanged;
    public UnityEvent OnMaxStressReached;
    public UnityEvent<float> OnMaxStressChanged;

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

}
