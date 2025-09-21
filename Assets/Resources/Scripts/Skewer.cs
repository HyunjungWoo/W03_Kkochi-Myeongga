// Skewer.cs
using NUnit.Framework;
using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;
using UnityEngine.XR;

public class Skewer : MonoBehaviour
{
    public Transform[] ingredientSlots;
    
    // 현재 몇 번째 슬롯까지 재료가 찼는지 기록
    private int nextSlotIndex = 0;
    private List<GameObject> addedIngredients = new List<GameObject>();

    private bool isSeasoned = false;
    public float price = 0f; // 꼬치의 가격

    private Transform holder;
    private PlayerHand hand;
    private Vector3 startPosition;

    // '손'에 의해 잡혔을 때 호출될 함수
    public void Grab(Transform newHolder)
    {
        holder = newHolder;
        hand = newHolder.GetComponent<PlayerHand>(); 
        startPosition = transform.position;
    }

    // '손'에 의해 놓아졌을 때 호출될 함수
    public void Release()
    {// 1. 내 자신의 Collider를 찾아서 변수에 저장
        Collider2D myCollider = GetComponent<Collider2D>();

        // 2. Raycast를 쏘기 직전에 잠시 끈다 (유령으로 변신!)
        myCollider.enabled = false;

        // 3. 이제 나 자신을 통과해서 그 아래에 있는 오브젝트를 감지할 수 있음
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);

        // 4. Raycast가 끝나자마자 즉시 다시 켠다 (원상복구)
        myCollider.enabled = true;

        // --- ▲▲▲ 여기까지 수정됩니다 ▲▲▲ ---

        if (hit.collider != null && hit.collider.CompareTag("Customer"))
        {
            Debug.Log("꼬치를 손님 위에 놓았습니다! 판매합니다.");
            // 판매 로직 (재활용 또는 파괴)
            BoardManager.Instance.ResetCurrentStick();
        }
        else
        {
            // 유효한 위치가 아니면 원래 위치로 복귀
            transform.position = startPosition;
        }
    }


    private void Update()
    {
        // 만약 누군가 나를 잡고 있다면(holder가 null이 아니라면)
        if (holder != null)
        {
            // 그 대상을 계속 따라다님
            transform.position = holder.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 잡혀있는 상태이고, 상대가 Customer라면 즉시 판매
        if (holder != null && other.CompareTag("Customer"))
        {
            Sell();
        }
    }

    // 판매 로직을 별도의 함수로 분리 (중복 방지)
    private void Sell()
    {
        // 이미 판매 처리 중이라면 중복 실행 방지
        if (holder == null) return;

        Debug.Log("손님에게 꼬치를 판매했습니다!");

        // 1. 손에게 내가 사라졌다고 알림
        if (hand != null)
        {
            hand.ForceRelease();
        }

        // 2. BoardManager에게 꼬치 리셋 요청
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.ResetCurrentStick();
        }

        // 3. 이 스크립트의 추가 동작을 막기 위해 holder를 비움
        holder = null;
        hand = null;
    }

    #region 꼬치관련 함수들

    // 외부에서 재료를 받아서 꼬치에 추가하는 함수
    public void AddIngredient(GameObject ingredient)
    {
        if (IsFull()) return;
        isSeasoned = false; // 재료가 추가되면 양념 상태 초기화

        // 1. 현재 채워야 할 슬롯 Transform을 가져옴
        Transform slotTransform = ingredientSlots[nextSlotIndex];
        ingredient.transform.SetParent(slotTransform);
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.transform.localRotation = Quaternion.identity;
        nextSlotIndex++;

        addedIngredients.Add(ingredient);
        price += 5; // 재료 하나당 가격 5 증가
    }

    public void ApplySeasoning(GameObject saucePrefab)
    {
        if (isSeasoned)
        {
            Debug.Log("이미 양념이 발라져 있습니다.");
            return;
        }

        // 꼬치에 꽂힌 모든 재료를 순회
        foreach (GameObject ingredient in addedIngredients)
        {
            bool alreadyHasSauce = false;

            // 2. 재료의 모든 자식 오브젝트를 검사
            foreach (Transform child in ingredient.transform)
            {
                // 3. 만약 자식 중에 "Sauce" 태그를 가진 오브젝트가 있다면
                if (child.CompareTag("Sauce"))
                {
                    alreadyHasSauce = true; // 양념이 있다고 표시
                    break; // 더 이상 검사할 필요 없으므로 반복 중단
                }
            }

            // 4. 검사를 마친 후, 양념이 없는 경우에만 새로 생성
            if (!alreadyHasSauce)
            {
                GameObject sauceEffect = Instantiate(saucePrefab, ingredient.transform.position, Quaternion.identity);
                sauceEffect.transform.SetParent(ingredient.transform);
            }
        }

        isSeasoned = true;
        price += 10;
        Debug.Log("양념 완료! 현재 꼬치 가격: " + price);
    }

    // 꼬치가 꽉 찼는지 확인하는 함수
    public bool IsFull()
    {
        return nextSlotIndex >= ingredientSlots.Length;
    }

    public void ClearSkewer()
    {
        foreach (GameObject ingredient in addedIngredients)
        {
            Destroy(ingredient);
        }
        addedIngredients.Clear();
        nextSlotIndex = 0;
        isSeasoned = false;
        price = 0f;
    }

    // 꼬치가 비어있는지 확인하는 함수
    public bool IsEmpty()
    {
        return addedIngredients.Count == 0;
    }

    public void ResetSkewer()
    {
        // 1. 꼬치에 꽂힌 모든 재료 오브젝트를 파괴
        foreach (GameObject ingredient in addedIngredients)
        {
            Destroy(ingredient);
        }
        // 2. 재료 목록 리스트를 비움
        addedIngredients.Clear();

        // 3. 모든 상태 변수를 초기값으로 리셋
        nextSlotIndex = 0;
        isSeasoned = false; // (isSeasoned 변수가 있다면)
        price = 0f; // (price 변수가 있다면)
        holder = null;
        hand = null;
    }

    #endregion
}