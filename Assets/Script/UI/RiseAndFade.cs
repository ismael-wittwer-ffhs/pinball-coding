// RiseAndFade.cs : Description : Moves element up and fades TextMeshPro text to 0 alpha

using System.Collections;
using TMPro;
using UnityEngine;

public class RiseAndFade : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Distance to move upward")]
    [SerializeField] private float riseDistance = 2f;
    
    [Tooltip("Duration of the animation in seconds")]
    [SerializeField] private float duration = 1f;
    
    [Tooltip("Start animation automatically on Start")]
    [SerializeField] private bool playOnStart = true;
    
    [Header("Easing")]
    [Tooltip("Animation curve for custom easing (optional)")]
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private TextMeshPro tmpText;
    private TextMeshProUGUI tmpTextUI;
    private RectTransform rectTransform;
    private Transform transformCache;
    private Vector3 startPosition;
    private Color startColor;
    private bool isAnimating;

    private void Awake()
    {
        transformCache = transform;
        
        // Try to find TextMeshPro component (world space)
        tmpText = GetComponent<TextMeshPro>();
        if (tmpText == null)
            tmpText = GetComponentInChildren<TextMeshPro>();
        
        // Try to find TextMeshProUGUI component (UI space)
        if (tmpText == null)
        {
            tmpTextUI = GetComponent<TextMeshProUGUI>();
            if (tmpTextUI == null)
                tmpTextUI = GetComponentInChildren<TextMeshProUGUI>();
            
            if (tmpTextUI != null)
                rectTransform = GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        if (isAnimating) return;
        
        // Cache start position
        if (rectTransform != null)
            startPosition = rectTransform.anchoredPosition;
        else
            startPosition = transformCache.localPosition;
        
        // Cache start color
        if (tmpText != null)
            startColor = tmpText.color;
        else if (tmpTextUI != null)
            startColor = tmpTextUI.color;
        else
        {
            Debug.LogWarning("RiseAndFade: No TextMeshPro or TextMeshProUGUI component found.");
            return;
        }
        
        StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine()
    {
        isAnimating = true;
        float elapsed = 0f;
        
        Vector3 endPosition;
        if (rectTransform != null)
        {
            endPosition = startPosition + Vector3.up * riseDistance;
        }
        else
        {
            endPosition = startPosition + Vector3.up * riseDistance;
        }
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = easingCurve.Evaluate(t);
            
            // Update position
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, easedT);
            }
            else
            {
                transformCache.localPosition = Vector3.Lerp(startPosition, endPosition, easedT);
            }
            
            // Update alpha
            float alpha = Mathf.Lerp(startColor.a, 0f, easedT);
            Color currentColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            if (tmpText != null)
                tmpText.color = currentColor;
            else if (tmpTextUI != null)
                tmpTextUI.color = currentColor;
            
            yield return null;
        }
        
        // Ensure final values
        if (rectTransform != null)
            rectTransform.anchoredPosition = endPosition;
        else
            transformCache.localPosition = endPosition;
        
        Color finalColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        if (tmpText != null)
            tmpText.color = finalColor;
        else if (tmpTextUI != null)
            tmpTextUI.color = finalColor;
        
        isAnimating = false;
    }
}

