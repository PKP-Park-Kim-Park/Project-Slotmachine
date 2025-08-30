using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReelAnimator : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("심볼 이미지들을 담고 있는 부모 RectTransform")]
    [SerializeField] private RectTransform reelContainer;
    [Tooltip("릴에 표시될 UI 이미지들 (위에서 아래 순서)")]
    [SerializeField] private Image[] symbolImages;

    [Header("Animation Settings")]
    [Tooltip("릴이 돌아가는 속도 (pixels/sec)")]
    [SerializeField] private float spinSpeed = 3000f;
    [Tooltip("릴이 멈출 때 부드럽게 정착하는 시간")]
    [SerializeField] private float settleDuration = 0.5f;
    [Tooltip("정착 애니메이션의 부드러움을 조절하는 커브")]
    [SerializeField] private AnimationCurve settleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // --- 내부 변수 ---
    private float symbolHeight;
    private bool isSpinning = false;
    private Coroutine spinCoroutine;

    // 스핀 중 무작위로 보여줄 스프라이트 목록 (블러 효과용)
    private List<Sprite> availableSpritesForBlur = new List<Sprite>();

    void Start()
    {
        // 초기화 로직을 코루틴으로 분리하여 실행합니다.
        StartCoroutine(InitializeReel());
    }

    /// <summary>
    /// UI 레이아웃이 계산된 후 릴을 초기화하는 코루틴입니다.
    /// </summary>
    private IEnumerator InitializeReel()
    {
        if (reelContainer == null || symbolImages == null || symbolImages.Length < 2)
        {
            Debug.LogError("ReelAnimator에 필요한 컴포넌트가 할당되지 않았습니다.", this);
            this.enabled = false;
            yield break; // 코루틴 중단
        }

        // UI 레이아웃 시스템이 심볼 크기를 계산할 때까지 한 프레임의 끝까지 대기합니다.
        yield return new WaitForEndOfFrame();

        // 심볼 하나의 높이를 계산합니다. (모든 심볼의 높이는 같다고 가정)
        symbolHeight = symbolImages[0].rectTransform.rect.height;

        // 만약 symbolHeight가 0이라면, 인스펙터 설정에 문제가 있는 것입니다.
        if (symbolHeight <= 0)
        {
            Debug.LogError("심볼 높이가 0 또는 음수입니다. ReelStrip의 VerticalLayoutGroup과 자식 Image의 LayoutElement 설정을 확인해주세요.", this);
            this.enabled = false;
            yield break;
        }

        // SymbolManager에서 블러 효과에 사용할 모든 스프라이트를 가져옵니다.
        if (SymbolManager.Instance != null)
        {
            availableSpritesForBlur = SymbolManager.Instance.GetAllSprites();
        }
        else
        {
            Debug.LogError("SymbolManager 인스턴스를 찾을 수 없습니다. 블러 효과용 스프라이트를 로드할 수 없습니다.", this);
            this.enabled = false;
        }
    }


    /// <summary>
    /// 릴 회전 애니메이션을 시작합니다.
    /// </summary>
    public void StartSpin()
    {
        if (isSpinning) return;

        isSpinning = true;
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
        }
        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    /// <summary>
    /// 릴 회전 애니메이션을 멈추고 최종 결과에 정착시킵니다.
    /// </summary>
    /// <param name="finalSprites">중앙 3개 심볼에 표시될 최종 스프라이트</param>
    public IEnumerator StopSpin(Sprite[] finalSprites)
    {
        if (!isSpinning) yield break;

        isSpinning = false;
        // 진행중인 스핀 코루틴이 끝날 때까지 대기
        if (spinCoroutine != null)
        {
            yield return spinCoroutine;
            spinCoroutine = null;
        }

        // 최종 심볼 배치 및 정착 애니메이션
        yield return StartCoroutine(SettleCoroutine(finalSprites));
    }

    /// <summary>
    /// 릴이 무한히 돌아가는 것처럼 보이게 하는 코루틴 (블러 효과)
    /// </summary>
    private IEnumerator SpinCoroutine()
    {
        reelContainer.anchoredPosition = new Vector2(reelContainer.anchoredPosition.x, 0);
        int topImageIndex = 0;

        while (isSpinning)
        {
            // 릴을 아래로 이동
            reelContainer.anchoredPosition -= new Vector2(0, spinSpeed * Time.deltaTime);

            // 맨 위 심볼이 화면 밖으로 완전히 나갔다면
            if (reelContainer.anchoredPosition.y <= -symbolHeight)
            {
                // Y 위치를 초기화하여 끊김없이 보이게 함
                reelContainer.anchoredPosition += new Vector2(0, symbolHeight);

                // 맨 위 심볼을 맨 아래로 이동
                symbolImages[topImageIndex].rectTransform.SetAsLastSibling();

                // 맨 아래로 간 심볼에 무작위 스프라이트 할당 (블러 효과)
                if (availableSpritesForBlur.Count > 0)
                {
                    symbolImages[topImageIndex].sprite = availableSpritesForBlur[Random.Range(0, availableSpritesForBlur.Count)];
                }

                // 다음 심볼을 맨 위 심볼로 지정
                topImageIndex = (topImageIndex + 1) % symbolImages.Length;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 릴이 최종 위치에 부드럽게 정착하는 애니메이션 코루틴
    /// </summary>
    private IEnumerator SettleCoroutine(Sprite[] finalSprites)
    {
        // 스핀으로 인해 순서가 섞인 심볼들을 원래 순서대로 복구
        for (int i = 0; i < symbolImages.Length; i++)
        {
            symbolImages[i].rectTransform.SetSiblingIndex(i);
        }

        // 최종 결과 심볼들을 중앙에 배치
        if (finalSprites != null && finalSprites.Length >= 3)
        {
            int centerIndex = symbolImages.Length / 2;
            symbolImages[centerIndex - 1].sprite = finalSprites[0]; // 결과 (위)
            symbolImages[centerIndex].sprite = finalSprites[1];     // 결과 (중앙)
            symbolImages[centerIndex + 1].sprite = finalSprites[2]; // 결과 (아래)
        }

        // 최종 위치로 부드럽게 이동
        Vector2 startPos = reelContainer.anchoredPosition;
        Vector2 endPos = Vector2.zero;

        float elapsedTime = 0f;
        while (elapsedTime < settleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = settleCurve.Evaluate(elapsedTime / settleDuration);
            reelContainer.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        reelContainer.anchoredPosition = endPos;
    }
}
