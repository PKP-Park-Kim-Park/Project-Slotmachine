using UnityEngine;
using TMPro;
using System.Collections;

public class BetScreen : MonoBehaviour
{
    public SlotMachine slotMachine;
    private TextMeshProUGUI bettingGoldText;

    [Header("팝 애니메이션 설정")]
    [Tooltip("팝 애니메이션 총 시간(초)")]
    [SerializeField] private float popDuration = 0.2f;
    [Tooltip("팝 인/아웃 텍스트 크기 배율")]
    [SerializeField] private float popScaleMultiplier = 1.2f;

    private Coroutine popCoroutine;

    private void Awake()
    {
        bettingGoldText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (slotMachine != null)
        {
            slotMachine.OnBetGoldChanged += BetGoldChanged;
            BetGoldChanged(slotMachine.BettingGold);
        }
    }

    private void OnDisable()
    {
        if (slotMachine != null)
        {
            slotMachine.OnBetGoldChanged -= BetGoldChanged;
        }
    }

    private void BetGoldChanged(int newBettingGold)
    {
        // 1,000 단위 쉼표 추가
        bettingGoldText.text = newBettingGold.ToString("N0") + " $";

        // 이전에 실행 중이던 팝 애니메이션이 있다면 중지
        if (popCoroutine != null)
        {
            StopCoroutine(popCoroutine);
        }
        // 팝 애니메이션 시작
        popCoroutine = StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * popScaleMultiplier;

        float growDuration = popDuration * 0.5f;
        float shrinkDuration = popDuration * 0.5f;

        // 커지는 애니메이션
        for (float timer = 0; timer < growDuration; timer += Time.deltaTime)
        {
            bettingGoldText.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / growDuration);
            yield return null;
        }

        // 작아지는 애니메이션
        for (float timer = 0; timer < shrinkDuration; timer += Time.deltaTime)
        {
            bettingGoldText.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / shrinkDuration);
            yield return null;
        }

        // 최종 크기 보정
        bettingGoldText.transform.localScale = originalScale;
        popCoroutine = null;
    }
}
