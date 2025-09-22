// DialogueManager.cs
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // 다른 곳에서 쉽게 접근하기 위한 싱글톤
    public TextMeshProUGUI dialogueTextUI; // 대사를 표시할 UI Text 오브젝트

    void Awake()
    {
        Instance = this;
    }

    public void Speak(string message, float duration = 3f)
    {
        if (dialogueTextUI != null)
        {
            dialogueTextUI.text = message;
        }

    }

}