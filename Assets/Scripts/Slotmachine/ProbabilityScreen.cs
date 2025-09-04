using UnityEngine;
using TMPro;
using System.Collections;

public class ProbabilityScreen : MonoBehaviour
{
    [SerializeField] private SlotMachine slotMachine;
    [SerializeField] private PatternAnimator patternAnimator;
    private TextMeshProUGUI probabilityText;

    [Header("팝 애니메이션 설정")]
    [Tooltip("팝 애니메이션 총 시간(초)")]
    [SerializeField] private float popDuration = 0.2f;
    [Tooltip("팝 인/아웃 텍스트 크기 배율")]
    [SerializeField] private float popScaleMultiplier = 1.3f;

    private Coroutine displayCoroutine;
    private GameObject container;
    private UnityEngine.UI.Image backgroundImage;

    /// <summary>
    /// 컴포넌트 초기화 및 이벤트 구독 처리.
    /// </summary>
    private void Awake()
    {
        probabilityText = GetComponent<TextMeshProUGUI>();
        if (probabilityText == null)
        {
            Debug.LogError("TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.", this);
            this.enabled = false;
            return;
        }

        // 부모 오브젝트(배경+텍스트 컨테이너)와 배경 이미지를 찾아 저장함.
        if (transform.parent != null)
        {
            container = transform.parent.gameObject;
            backgroundImage = container.GetComponent<UnityEngine.UI.Image>();
        }

        // 필요한 이벤트에 콜백 함수를 등록함.
        if (patternAnimator != null)
        {
            patternAnimator.OnLineAnimate += HandleLineAnimate;
        }
        if (slotMachine != null)
        {
            slotMachine.OnActivationStart += HideText;
        }
    }

    private void Start()
    {
        // 게임 시작 시 UI를 보이지 않게 비활성화함.
        if (container != null) container.SetActive(false);
    }

    /// <summary>
    /// 오브젝트 파괴 시 메모리 누수 방지를 위해 이벤트를 해지함.
    /// </summary>
    private void OnDestroy()
    {
        if (patternAnimator != null)
        {
            patternAnimator.OnLineAnimate -= HandleLineAnimate;
        }
        if (slotMachine != null)
        {
            slotMachine.OnActivationStart -= HideText;
        }
    }

    /// <summary>
    /// 개별 당첨 라인 애니메이션이 시작될 때 호출되는 이벤트 핸들러.
    /// </summary>
    /// <param name="odds">해당 라인의 배율</param>
    private void HandleLineAnimate(float odds)
    {
        if (container != null)
        {
            // 개별 배율 표시는 배경 없이 텍스트만 보이도록 설정함.
            if (backgroundImage != null) backgroundImage.enabled = false;
            container.SetActive(true);
        }

        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
        displayCoroutine = StartCoroutine(ShowOddsAnimation(odds));
    }

    /// <summary>
    /// 개별 배율을 팝 효과와 함께 표시하고 잠시 후 사라지게 하는 코루틴.
    /// </summary>
    private IEnumerator ShowOddsAnimation(float odds)
    {
        probabilityText.text = $"+ {odds:F1}";

        yield return StartCoroutine(PopAnimation());

        yield return new WaitForSeconds(0.5f);
        if (container != null) container.SetActive(false);
        displayCoroutine = null;
    }

    /// <summary>
    /// 텍스트 크기를 키웠다가 원래대로 돌리는 팝 애니메이션을 재생함.
    /// </summary>
    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * popScaleMultiplier;

        float halfDuration = popDuration * 0.5f;

        for (float timer = 0; timer < halfDuration; timer += Time.deltaTime)
        {
            probabilityText.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / halfDuration);
            yield return null;
        }

        for (float timer = 0; timer < halfDuration; timer += Time.deltaTime)
        {
            probabilityText.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / halfDuration);
            yield return null;
        }

        probabilityText.transform.localScale = originalScale;
    }

    /// <summary>
    /// 슬롯머신 스핀 시작 시 UI를 즉시 숨김.
    /// </summary>
    private void HideText()
    {
        if (container != null && container.activeSelf)
        {
            StopAllCoroutines();
            container.SetActive(false);
        }
    }

    /// <summary>
    /// 모든 애니메이션이 끝난 후 총 배율을 표시함.
    /// </summary>
    /// <param name="totalOdds">표시할 총 배율</param>
    public void ShowTotalOdds(float totalOdds)
    {
        if (totalOdds > 0)
        {
            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }
            displayCoroutine = StartCoroutine(ShowTotalOddsAnimation(totalOdds));
        }
    }

    /// <summary>
    /// 총 배율을 배경과 함께 표시하고 0.8초 후 사라지게 하는 코루틴.
    /// </summary>
    private IEnumerator ShowTotalOddsAnimation(float totalOdds)
    {
        if (container != null)
        {
            // 총 배율 표시는 배경 이미지를 다시 활성화함.
            if (backgroundImage != null) backgroundImage.enabled = true;
            container.SetActive(true);
        }

        probabilityText.text = $"총합\n{totalOdds:F1}";

        yield return StartCoroutine(PopAnimation());

        yield return new WaitForSeconds(0.8f);
        if (container != null) container.SetActive(false);
    }
}