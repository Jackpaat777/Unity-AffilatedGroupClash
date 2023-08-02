using UnityEngine;

public class Background : MonoBehaviour
{
    // 페럴랙스 용 변수
    public float offset;

    // 카메라가 이동한 뒤 표현되어야하므로 LateUpdate
    void LateUpdate()
    {
        // 게임 매니저에서 가져온 변수로 표현
        if (InGameManager.instance.isMove)
        {
            Vector3 nextMove = Vector3.right * InGameManager.instance.camSpeed * offset * Time.deltaTime;
            transform.Translate(nextMove);
        }
    }
}
