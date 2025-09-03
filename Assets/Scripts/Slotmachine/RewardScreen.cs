using UnityEngine;
using TMPro;
using System.Collections;

public class RewardScreen : MonoBehaviour
{
    public SlotMachine slotMachine;
    private TextMeshProUGUI rewardGoldText;

    [Header("팝 애니메이션 설정")]
    [Tooltip("팝 애니메이션 총 시간(초)")]
    [SerializeField] private float popDuration = 0.3f;
    [Tooltip("팝 인/아웃 텍스트 크기 배율")]
    [SerializeField] private float popScaleMultiplier = 1.2f;

    private Coroutine countUpCoroutine;

    private void Awake()
    {
        rewardGoldText = GetComponent<TextMeshProUGUI>();
        rewardGoldText.text = "^ o ^";
    }

    private void OnEnable()
    {
        if (slotMachine != null)
        {
            slotMachine.OnRewardGold += HandleRewardGold;
            slotMachine.OnActivationEnd += ResetToDefaultText;
        }
    }

    private void OnDisable()
    {
        if (slotMachine != null)
        {
            slotMachine.OnRewardGold -= HandleRewardGold;
            slotMachine.OnActivationEnd -= ResetToDefaultText;
        }
    }

    private void HandleRewardGold(int newRewardGold)
    {
        // 이전에 실행 중인 코루틴 중지합니다.
        if (countUpCoroutine != null)
        {
            StopCoroutine(countUpCoroutine);
        }

        if (newRewardGold > 0)
        {
            countUpCoroutine = StartCoroutine(CountUpAnimation(newRewardGold));
        }
    }

    private void ResetToDefaultText()
    {
        if (countUpCoroutine != null)
        {
            StopCoroutine(countUpCoroutine);
            countUpCoroutine = null;
        }
        rewardGoldText.text = "^ o ^";
    }

    private IEnumerator CountUpAnimation(int targetGold)
    {
        Debug.Log($"숫자 카운트 업 애니메이션 시작: {targetGold}");
        float duration = 1.0f; // 숫자가 올라가는 데 걸리는 총 시간 (초)
        float currentGold = 0f;

        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            float progress = timer / duration;
            currentGold = Mathf.Lerp(0, targetGold, progress);
            rewardGoldText.text = ((int)currentGold).ToString("N0") + " $";
            yield return null;
        }

        // 최종 금액 표시
        rewardGoldText.text = targetGold.ToString("N0") + " $";

        // 팝인/아웃 애니메이션 실행
        yield return StartCoroutine(PopAnimation());

        countUpCoroutine = null;
    }

    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * popScaleMultiplier;

        float bigDuration = popDuration * 0.5f; // 전체 시간의 50%
        float smallDuration = popDuration * 0.5f;

        // 커지는 애니메이션
        for (float timer = 0; timer < bigDuration; timer += Time.deltaTime)
        {
            float progress = timer / bigDuration;
            rewardGoldText.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // 작아지는 애니메이션
        for (float timer = 0; timer < smallDuration; timer += Time.deltaTime)
        {
            float progress = timer / smallDuration;
            rewardGoldText.transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        // 최종 크기 보정
        rewardGoldText.transform.localScale = originalScale;
    }
}
