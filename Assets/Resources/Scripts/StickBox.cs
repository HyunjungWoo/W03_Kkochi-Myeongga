using UnityEngine;

public class StickBox : MonoBehaviour
{
    private void Start()
    {
        // BoardManager에게 새 꼬치를 만들어달라고 요청
        BoardManager.Instance.CreateNewStick();
    }
}