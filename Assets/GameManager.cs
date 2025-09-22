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

    private UIManager uiManager;

    // 고객의 행동을 담은 프리팹 (모든 고객이 동일)
    public GameObject customerPrefab;
    public Transform customerSpawnPoint;

    // 무작위로 소환될 고객들의 데이터를 담은 리스트
    public List<CustomerData> customerProfiles;

    public float spawnDelay = 5f; // 꼬치 완성 후 다음 손님이 오는 시간 (초 단위)
    private float exitSpeed = 7f; // 고객이 나가는 속도

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
        money += amount;
        if (uiManager != null)
        {
            uiManager.UpdateMoneyUI(money);
            uiManager.ShowFloatingMoney(amount);
        }
    }
    public void AddAffection(int amount)
    {
        affection += amount;
        if (uiManager != null)
        {
            uiManager.UpdateAffectionUI(affection);
            uiManager.ShowFloatingAffection(amount);
        }
    }

    public void ResetCurrentCustomer()
    {
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

        // 이동이 끝난 후 고객 오브젝트 비활성화 및 리셋
        currentCustomer.gameObject.SetActive(false);
        ResetCurrentCustomer();
    }

}
