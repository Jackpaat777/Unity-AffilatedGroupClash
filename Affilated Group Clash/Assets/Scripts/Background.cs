using UnityEngine;

public class Background : MonoBehaviour
{
    // �䷲���� �� ����
    public float offset;

    // ī�޶� �̵��� �� ǥ���Ǿ���ϹǷ� LateUpdate
    void LateUpdate()
    {
        // ���� �Ŵ������� ������ ������ ǥ��
        if (InGameManager.instance.isMove)
        {
            Vector3 nextMove = Vector3.right * InGameManager.instance.camSpeed * offset * Time.deltaTime;
            transform.Translate(nextMove);
        }
    }
}
