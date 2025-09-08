using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 심볼의 시각적 표현과 애니메이션을 담당하는 컴포넌트
/// </summary>
[RequireComponent(typeof(Image))]
public class SymbolAnimator : MonoBehaviour
{
    [Tooltip("애니메이션 프레임 간의 시간 (초)")]
    [SerializeField] private float frameRate = 0.1f;

    private Image _image;
    private Sprite[] _animationSprites;
    private Sprite _staticSprite;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// 이 심볼이 표시할 스프라이트들을 설정합니다.
    /// </summary>
    /// <param name="sprites">애니메이션에 사용될 스프라이트 배열</param>
    public void SetSprites(Sprite[] sprites)
    {
        _animationSprites = sprites;
        if (_animationSprites != null && _animationSprites.Length > 0)
        {
            _staticSprite = _animationSprites[0];
            _image.sprite = _staticSprite; // 기본 이미지는 첫 프레임으로 설정
        }
    }

    /// <summary>
    /// 심볼 애니메이션을 시작합니다.
    /// </summary>
    public void PlayAnimation()
    {
        if (_animationSprites == null || _animationSprites.Length <= 1) return;

        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(Animate());
    }

    /// <summary>
    /// 심볼 애니메이션을 멈추고 기본 이미지로 돌아갑니다.
    /// </summary>
    public void StopAnimation()
    {
        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
        _animationCoroutine = null;
        if (_image != null && _staticSprite != null)
        {
            _image.sprite = _staticSprite;
        }
    }

    private IEnumerator Animate()
    {
        int index = 0;
        while (true)
        {
            _image.sprite = _animationSprites[index];
            index = (index + 1) % _animationSprites.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }
}