using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 다른 스크립트에서 쉽게 접근하기 위한 싱글톤

    public int money = 1000;
    public int affection = 0;
    public Customer currentCustomer;

    public int affectionLossOnLeave = -5; // 손님이 나갈 때 잃는 호감도
    public bool isLeaving = false;
    private UIManager uiManager;

    // 고객의 행동을 담은 프리팹 (모든 고객이 동일)
    public GameObject customerPrefab;
    public Transform customerSpawnPoint;

    // 무작위로 소환될 고객들의 데이터를 담은 리스트
    public List<CustomerData> customerProfiles;

    public int bonusPer10Affection = 5;
    public float spawnDelay = 5f; // 꼬치 완성 후 다음 손님이 오는 시간 (초 단위)
    private float exitSpeed = 7f; // 고객이 나가는 속도
    public float fastSpawnDelay = 3f; // 빨라진 손님 등장 속도 (원하는 값으로 설정)

    void Awake()
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

        // UIManager 인스턴스 찾기
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            UnityEngine.Debug.LogError("UIManager를 찾을 수 없습니다. UIManager 스크립트가 포함된 오브젝트를 씬에 추가했는지 확인하세요.");
        }

    }

    void Start()
    {
        if (uiManager != null)
        {
            uiManager.UpdateMoneyUI(money);
            uiManager.UpdateAffectionUI(affection);
        }

        ResetCurrentCustomer();
    }
    public void AddMoney(int amount)
    {
        int bonus = GetBonusAmount(amount);
        money +=(amount + bonus);

        if (uiManager != null && !isLeaving)
        {
            uiManager.UpdateMoneyUI(money);
            if (amount == 0) return;
            uiManager.ShowFloatingMoney(amount+bonus);
        }
    }
    public void AddAffection(int amount)
    {
        if (affection > 20)
        {
            // 호감도가 20 이상이면 1씩 증가
            amount = Mathf.Min(amount, 1);
        }
        else if (affection >= 20 && affection < 30) spawnDelay = fastSpawnDelay; // 호감도가 20 이상 30 미만이면 손님 등장 속도 빨라짐

        affection += amount;
        if (uiManager != null)
        {
            uiManager.UpdateAffectionUI(affection);
        }
        CheckGameOver();
    }

    public void ResetCurrentCustomer()
    {
        isLeaving = false;
        if (currentCustomer == null)
        {
            UnityEngine.Debug.LogError("Current Customer가 할당되지 않았습니다. 인스펙터에서 연결해주세요.");
            return;
        }

        if (customerProfiles == null || customerProfiles.Count == 0)
        {
            UnityEngine.Debug.LogError("고객 프로필 리스트가 비어있습니다. CustomerData를 추가해주세요.");
            return;
        }

        // 나가기 버튼 온
        HandleOutOfStock();
        currentCustomer.transform.position = customerSpawnPoint.position;

        // 1. 고객 오브젝트 활성화
        currentCustomer.gameObject.SetActive(true);
        currentCustomer.GetComponent<Collider2D>().enabled = true;

        // 2. 고객 프로필 리스트에서 무작위로 하나 선택
        int randomIndex = Random.Range(0, customerProfiles.Count);
        CustomerData selectedProfile = customerProfiles[randomIndex];

        // 3. 선택된 프로필 데이터를 현재 고객에게 할당
        currentCustomer.customerData = selectedProfile;

        // 4. 고객 스크립트의 초기 설정 함수 호출
        currentCustomer.SetupCustomer();
    }

    
    public void CompleteOrder()
    {
        currentCustomer.GetComponent<Collider2D>().enabled = false;
        // 주문 완료 시 나가기 버튼 숨기기
        currentCustomer.HideLeaveButton();
        // 고객이 사라지는 효과를 위한 코루틴 시작
        StartCoroutine(MoveCustomerOffscreen());
    }


    private IEnumerator MoveCustomerOffscreen()
    {
        float timer = 0f;

        // spawnDelay 시간 동안 고객을 왼쪽으로 이동
        while (timer < spawnDelay)
        {
            if (currentCustomer != null)
            {
                currentCustomer.transform.Translate(Vector3.left * exitSpeed * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        if (currentCustomer != null)
        {
            currentCustomer.HideLeaveButton();
        }

        // 이동이 끝난 후 고객 오브젝트 비활성화 및 리셋
        currentCustomer.gameObject.SetActive(false);
        ResetCurrentCustomer();
    }

    private int GetBonusAmount(int basePrice)
    {
        // 호감도가 10 이상일 때만 계산
        if (affection >= 10)
        {
            // 호감도 10마다 보너스 지급 (예: 호감도 25 -> 20점 구간에 해당)
            int bonusLevels = affection / 10;
            int bonus = bonusLevels * bonusPer10Affection;
            return bonus;
        }
        return 0; // 호감도 10 미만일 경우 보너스 없음
    }

    private void CheckGameOver()
    {
        // 호감도가 -10 이하거나 돈이 0 이하일 때 게임오버
        if (affection <= -20 || money <= 0)
        {
            // 게임오버 처리 (예: 메시지 출력, 씬 전환 등)
            UnityEngine.Debug.Log("게임 오버! 호감도 또는 자금이 부족합니다.");
            // 씬을 GameOver 씬으로 전환
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }

    public void HandleOutOfStock()
    {
        // 현재 손님이 있다면 나가기 버튼을 보여줌
        if (currentCustomer != null)
        {
            currentCustomer.ShowLeaveButton();
        }
    }

    public void ForceCustomerLeave()
    {
        if (currentCustomer != null)
        {
            currentCustomer.HideLeaveButton(); // 버튼 숨기기
            AddAffection(affectionLossOnLeave); // 호감도 감소
            CompleteOrder(); // 고객 퇴장 처리
        }
    }
}
