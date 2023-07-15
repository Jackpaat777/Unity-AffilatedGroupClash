using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // �䷲���� �� ����
    public float offset;

    // Update is called once per frame
    void LateUpdate()
    {
        if (GameManager.instance.isMove)
        {
            Vector3 nextMove = Vector3.right * GameManager.instance.camSpeed * offset * Time.deltaTime;
            transform.Translate(nextMove);
        }
    }
}
