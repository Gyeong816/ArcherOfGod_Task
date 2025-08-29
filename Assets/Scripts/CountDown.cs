using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
   
    [Header("카운트다운 설정")]
    [SerializeField] private Image numberImage;      
    [SerializeField] private Sprite[] numberSprites;  
    [SerializeField] private float fadeDuration = 0.2f;

    [Header("라운드 타이머 UI")] 
    [SerializeField] private TextMeshProUGUI timerText;  
    [SerializeField] private Button startButton;  
    [SerializeField] private GameObject startPanel;  
    

    private void Awake()
    {
        startButton.onClick.AddListener(GameStart);
    }

    private void GameStart()
    {
        startPanel.SetActive(false);
        StartCoroutine(CountdownRoutine());
    }
    private IEnumerator CountdownRoutine()
    {
        for (int i = 0; i < numberSprites.Length; i++)
        {
            numberImage.sprite = numberSprites[i];
            
            yield return StartCoroutine(FadeImage(0f, 1f, fadeDuration));
            yield return new WaitForSeconds(1f - fadeDuration * 2);
            yield return StartCoroutine(FadeImage(1f, 0f, fadeDuration));
        }

        numberImage.enabled = false;
        
        GameManager.Instance.StartGame();
    }

    private IEnumerator FadeImage(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = numberImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(from, to, t);
            numberImage.color = color;
            yield return null;
        }

        color.a = to;
        numberImage.color = color;
    }
    
    public void UpdateTimerUI(float time)
    {
        int seconds = Mathf.CeilToInt(time);
        timerText.text = seconds.ToString();
    }
}
