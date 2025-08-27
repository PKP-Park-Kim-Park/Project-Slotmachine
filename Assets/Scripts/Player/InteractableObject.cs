using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string objectName;

    public void Interact()
    {
        // 이 함수가 호출되면 상호작용 로직을 실행합니다.
        Debug.Log(objectName + "와 상호작용했습니다!");

        // StopBell 컴포넌트가 있는지 확인하고 호출
        StopBell stopBell = GetComponent<StopBell>();
        if (stopBell != null)
        {
            stopBell.StopGame();
            return;
        }

        //예시
        //// Door 컴포넌트가 있는지 확인하고 호출
        //Door door = GetComponent<Door>();
        //if (door != null)
        //{
        //    door.Open(); // 또는 door.Toggle() 등
        //    return;
        //}
    }
}