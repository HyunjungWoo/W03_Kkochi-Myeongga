using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton 패턴: 어디서든 이 스크립트에 쉽게 접근할 수 있게 함
    public static BoardManager Instance { get; private set; }

    [Header("생성할 프리팹")]
    public GameObject stickPrefab; // 꼬치 막대기(Skewer) 프리팹

    [Header("생성 위치")]
    public Transform stickSpawnPoint; // 꼬치가 생성될 위치

    [Header("연동 대상")]
    public IngredientBox[] ingredientBoxes; // 씬에 있는 모든 재료 박스들
    public SeasoningBottle[] seasoningBottles; // 씬에 있는 모든 양념통들

    private Skewer currentStick; // 현재 활성화된 꼬치

    private void Awake()
    {
        // Singleton 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // StickBox가 이 함수를 호출하여 꼬치를 생성하게 함
    public void CreateNewStick()
    {
        // 이미 꼬치가 있다면 새로 만들지 않음 (필요에 따라 로직 변경 가능)
        if (currentStick != null)
        {
            Debug.Log("이미 작업 중인 꼬치가 있습니다.");
            return;
        }

        // 1. 꼬치 프리팹을 지정된 위치에 생성
        GameObject stickObject = Instantiate(stickPrefab, stickSpawnPoint.position, Quaternion.identity);
        currentStick = stickObject.GetComponent<Skewer>();

        // 2. 생성된 꼬치가 있는지 확인
        if (currentStick != null)
        {
            Debug.Log("새 꼬치 생성 완료!");
            // 3. 모든 재료 박스와 양념통에 새로 만든 꼬치를 알려줌
            UpdateAllTargets(currentStick);
        }
        else
        {
            Debug.LogError("Stick 프리팹에 Skewer 스크립트가 없습니다!");
        }
    }

    // 모든 대상에게 현재 꼬치를 연동시켜주는 함수
    private void UpdateAllTargets(Skewer targetSkewer)
    {
        // 모든 재료 박스에 연동
        foreach (var box in ingredientBoxes)
        {
            box.currentSkewer = targetSkewer;
        }

        // 모든 양념통에 연동
        foreach (var bottle in seasoningBottles)
        {
            bottle.currentSkewer = targetSkewer;
        }

        Debug.Log("모든 재료/양념통에 새 꼬치를 연동했습니다.");
    }

    // (추가) 꼬치 판매 등 꼬치를 치워야 할 때 호출할 함수
    public void ClearBoard()
    {
        if (currentStick != null)
        {
            Destroy(currentStick.gameObject);
            currentStick = null;
            UpdateAllTargets(null); // 모든 연결을 끊음
        }
    }
}