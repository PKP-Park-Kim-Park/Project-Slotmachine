using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReelAnimator : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("심볼 이미지들을 담고 있는 부모 RectTransform")]
    [SerializeField] private RectTransform reelStrip;
    [Tooltip("릴에 표시될 UI 이미지들 (위에서 아래 순서)")]
    [SerializeField] private Image[] symbolImages;

    [Header("Animation Settings")]
    [Tooltip("릴이 돌아가는 속도")]
    [SerializeField] private float spinSpeed = 3000f;
    [Tooltip("릴이 멈출 때 부드럽게 정착하는 시간")]
    [SerializeField] private float settleDuration = 0.5f;
    [Tooltip("정착 애니메이션의 부드러움을 조절하는 커브")]
    [SerializeField] private AnimationCurve settleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // --- 내부 변수 ---
    private float symbolHeight;
    private bool isSpinning = false;
    private Coroutine spinCoroutine;

    // 스핀 중 무작위로 보여줄 스프라이트 목록 (스핀 효과)
    private List<Sprite> spritesBlur = new List<Sprite>();

    void Start()
    {
        // 초기화 로직 분리 실행
        StartCoroutine(InitializeReel());
    }

    /// <summary>
    /// UI 레이아웃이 계산된 후 릴 초기화
    /// </summary>
    private IEnumerator InitializeReel()
    {
        if (reelStrip == null || symbolImages == null || symbolImages.Length < 2)
        {
            Debug.LogError("ReelAnimator에 필요한 컴포넌트가 할당되지 않았습니다.", this);
            this.enabled = false;
            yield break;
        }

        // UI 레이아웃이 심볼 크기 계산할 때까지 1프렘 대기
        yield return new WaitForEndOfFrame();

        // 심볼 하나 높이 계산
        symbolHeight = symbolImages[0].rectTransform.rect.height;

        if (symbolHeight <= 0)
        {
            Debug.LogError("심볼 높이가 0 또는 음수 입니다.", this);
            this.enabled = false;
            yield break;
        }

        // SymbolManager에서 스핀 효과에 사용할 스프라이트 가져옴
        if (SymbolManager.Instance != null)
        {
            spritesBlur = SymbolManager.Instance.GetAllSprites();
        }
        else
        {
            Debug.LogError("SymbolManager를 추가하세요.", this);
            this.enabled = false;
        }
    }

    /// <summary>
    /// 릴 회전 애니메이션
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
    /// 릴 회전 애니메이션을 멈추고 최종 결과 3개 인덱스 표시
    /// </summary>
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

        yield return StartCoroutine(SettleCoroutine(finalSprites));
    }

    /// <summary>
    /// 릴이 돌아가는 것처럼 보이게 하는 코루틴 (스핀 효과)
    /// </summary>
    private IEnumerator SpinCoroutine()
    {
        reelStrip.anchoredPosition = new Vector2(reelStrip.anchoredPosition.x, 0);
        int topImageIndex = 0;

        while (isSpinning)
        {
            reelStrip.anchoredPosition -= new Vector2(0, spinSpeed * Time.deltaTime);

            // 맨 위 심볼이 뷰포트 밖으로 나갔을 때
            if (reelStrip.anchoredPosition.y <= -symbolHeight)
            {
                // Y 위치를 초기화해서 연결되는 듯한 느낌
                reelStrip.anchoredPosition += new Vector2(0, symbolHeight);

                // 맨 위 심볼을 맨 아래로 이동
                symbolImages[topImageIndex].rectTransform.SetAsLastSibling();

                // 맨 아래로 간 심볼에 무작위 스프라이트 할당 (스핀 효과)
                if (spritesBlur.Count > 0)
                {
                    symbolImages[topImageIndex].sprite = spritesBlur[Random.Range(0, spritesBlur.Count)];
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
        Vector2 startPos = reelStrip.anchoredPosition;
        Vector2 endPos = Vector2.zero;

        float elapsedTime = 0f;
        while (elapsedTime < settleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = settleCurve.Evaluate(elapsedTime / settleDuration);
            reelStrip.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        reelStrip.anchoredPosition = endPos;
    }
}
