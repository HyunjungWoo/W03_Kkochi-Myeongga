using UnityEngine;

public class StickBox : MonoBehaviour
{
    private void Start()
    {
        // BoardManager에게 새 꼬치를 만들어달라고 요청
        BoardManager.Instance.CreateNewStick();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 충돌한 오브젝트가 꼬치인지 확인
        Skewer skewer = other.GetComponent<Skewer>();

        // 2. 꼬치라면
        if (skewer != null)
        {
            // 3. 꼬치에 꽂힌 재료가 있는지 확인 (비어있으면 버릴 필요 없음)
            if (!skewer.IsEmpty())
            {
                Debug.Log("꼬치 버림: 새 꼬치를 생성합니다.");

                skewer.ResetSkewer(); // 꼬치 초기화 (재료 제거)

            }
        }
    }
}