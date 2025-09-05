using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PatternAnimator : MonoBehaviour
{
    [Tooltip("당첨 심볼 주위에 표시될 테두리 UI 프리팹")]
    [SerializeField] private GameObject borderPrefab;

    [Tooltip("슬롯들의 UI 위치를 담는 1차원 Transform 배열 (3x5=15개)")]
    [SerializeField] private Transform[] slotPositions = new Transform[15];

    [Tooltip("각 당첨 라인 애니메이션 사이의 딜레이 (초)")]
    [SerializeField] private float animationDelay = 0.5f;

    [Header("Pop Effect")]
    [Tooltip("당첨 심볼이 커지는 배율")]
    [SerializeField] private float popScale = 1.2f;

    [Tooltip("당첨 심볼이 커졌다가 돌아오는 총 애니메이션 시간")]
    [SerializeField] private float popDuration = 0.2f;

    [Header("Particle Effect")]
    [Tooltip("각 당첨 라인에 표시될 파티클 프리팹")]
    [SerializeField] private GameObject winningParticlePrefab;
    public event Action<float> OnLineAnimate;

    private readonly List<GameObject> borderPool = new();
    private int poolIndex = 0;
    private Coroutine animationCoroutine;
    public bool IsAnimating { get{ return animationCoroutine != null; }}

    private const int numColumns = 5;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (borderPrefab == null) return;

        for (int i = 0; i < 15; i++)
        {
            GameObject border = Instantiate(borderPrefab, transform);
            border.SetActive(false);
            borderPool.Add(border);
        }
    }

    /// <summary>
    /// 당첨 라인 애니메이션 시작
    /// </summary>
    public void AnimateWinning(List<WinningLine> winningLines)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        ClearBorders();
        animationCoroutine = StartCoroutine(AnimateSeq(winningLines));
    }

    private IEnumerator AnimateSeq(List<WinningLine> winningLines)
    {
        // 각 당첨 라인을 순서대로 보여줌
        foreach (WinningLine line in winningLines)
        {
            List<Transform> transformsToAnimate = new();
            List<SymbolAnimator> symbolsToAnimate = new();

            OnLineAnimate?.Invoke(line.Odds);

            foreach (var coord in line.Coordinates)
            {
                if (IsValidCoordinate(coord.x, coord.y))
                {
                    ShowBorderAt(coord.x, coord.y);
                    int index = coord.x * numColumns + coord.y;
                    transformsToAnimate.Add(slotPositions[index]);
                    // slotPositions에 있는 Symbol 컴포넌트를 찾아서 리스트에 추가
                    if (slotPositions[index].TryGetComponent<SymbolAnimator>(out var symbol))
                    {
                        symbolsToAnimate.Add(symbol);
                    }
                }
            }

            if (winningParticlePrefab != null)
            {
                foreach (var coord in line.Coordinates)
                {
                    if (IsValidCoordinate(coord.x, coord.y))
                    {
                        int index = coord.x * numColumns + coord.y;
                        Instantiate(winningParticlePrefab, slotPositions[index]);
                    }
                }
            }

            // 당첨된 심볼 애니메이션 재생
            symbolsToAnimate.ForEach(s => s.PlayAnimation());

            if (transformsToAnimate.Count > 0)
            {
                yield return StartCoroutine(AnimatePop(transformsToAnimate, popScale, popDuration / 2f));
            }

            yield return new WaitForSeconds(animationDelay);

            if (transformsToAnimate.Count > 0)
            {
                yield return StartCoroutine(AnimatePop(transformsToAnimate, 1.0f, popDuration / 2f));
            }

            // 심볼 애니메이션 정지 및 테두리 제거
            symbolsToAnimate.ForEach(s => s.StopAnimation());
            ClearBorders();
        }

        animationCoroutine = null;
    }

    private IEnumerator AnimatePop(List<Transform> targets, float targetScale, float duration)
    {
        if (targets.Count == 0 || duration <= 0)
        {
            yield break;
        }

        List<Vector3> originalScales = new();
        foreach (var t in targets)
        {
            originalScales.Add(t.localScale);
        }
        Vector3 finalScale = Vector3.one * targetScale;

        float timer = 0f;
        while (timer < duration)
        {
            float progress = timer / duration;
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].localScale = Vector3.Lerp(originalScales[i], finalScale, progress);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (var t in targets)
        {
            t.localScale = finalScale;
        }
    }

    private void ShowBorderAt(int row, int col)
    {
        if (poolIndex >= borderPool.Count) return; // 풀이 부족한 경우 방지

        int index = row * numColumns + col;
        GameObject border = borderPool[poolIndex];
        Transform slotTransform = slotPositions[index];

        border.transform.SetParent(slotTransform, false);
        border.SetActive(true);

        poolIndex++;
    }

    /// <summary>
    /// 표시된 모든 테두리를 비활성화하고 풀로 반환
    /// </summary>
    public void ClearBorders()
    {
        for (int i = 0; i < poolIndex; i++)
        {
            borderPool[i].SetActive(false);
            borderPool[i].transform.SetParent(transform, false);
        }
        poolIndex = 0;
    }

    private bool IsValidCoordinate(int row, int col)
    {
        if (slotPositions == null || slotPositions.Length != 15) return false;

        int index = row * numColumns + col;
        return index >= 0 && index < slotPositions.Length && slotPositions[index] != null;
    }
}