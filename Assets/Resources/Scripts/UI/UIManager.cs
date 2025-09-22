using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI floatingMoneyText;
    public TextMeshProUGUI affectionText;
    public TextMeshProUGUI floatingAffectionText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (floatingMoneyText != null)
        {
            floatingMoneyText.gameObject.SetActive(false);
        }
        if (floatingAffectionText != null)
        {
            floatingAffectionText.gameObject.SetActive(false);
        }
    }

    // 총 금액 UI 업데이트
    public void UpdateMoneyUI(int currentMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"자금: {currentMoney}원";
        }
    }

    // 상승 금액 애니메이션 표시
    public void ShowFloatingMoney(int amount)
    {// 금액이 양수일 경우 노란색, 음수일 경우 빨간색
        if(amount < 0)
        {
            StartCoroutine(AnimateFloatingText(floatingMoneyText, amount.ToString(), floatingMoneyText.transform.position, Color.red));
            return;
        }
        StartCoroutine(AnimateFloatingText(floatingAffectionText, "+" + amount.ToString(), floatingMoneyText.transform.position, Color.green));
    }

    // 총 호감도 UI 업데이트
    public void UpdateAffectionUI(int currentAffection)
    {
        if (affectionText != null)
        {
            affectionText.text = $"호감도: {currentAffection}";
        }
    }

    // 상승 호감도 애니메이션 표시
    public void ShowFloatingAffection(int amount)
    {
        StartCoroutine(AnimateFloatingText(floatingAffectionText, "+" + amount.ToString(), floatingAffectionText.transform.position, Color.green));
    }

    // 재사용 가능한 범용 애니메이션 코루틴
    private IEnumerator AnimateFloatingText(TextMeshProUGUI textToAnimate, string text, Vector3 startPos, Color color)
    {
        if (textToAnimate == null) yield break;

        textToAnimate.text = text;
        textToAnimate.color = color;
        textToAnimate.transform.position = startPos;
        textToAnimate.gameObject.SetActive(true);

        float duration = 1.0f;
        float timer = 0f;
        Vector3 endPos = startPos + new Vector3(0, 1, 0); // 위로 1만큼 이동
        Color endColor = new Color(color.r, color.g, color.b, 0); // 투명하게

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            textToAnimate.transform.position = Vector3.Lerp(startPos, endPos, t);
            textToAnimate.color = Color.Lerp(color, endColor, t);

            yield return null;
        }

        textToAnimate.gameObject.SetActive(false);
        textToAnimate.transform.position = startPos;
    }
}