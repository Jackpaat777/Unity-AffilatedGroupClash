using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("---------------[Start]")]
    public GameObject menuSet;
    public GameObject gameSet;
    public GameObject[] baseObject;

    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
    }

    public void GameStart()
    {
        menuSet.SetActive(false);
        gameSet.SetActive(true);
        baseObject[0].SetActive(true);
        baseObject[1].SetActive(true);
    }

    // ��ư�� ���� �̵�
    public void CameraMove(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // ������ ��ư�� �����ų� ���ʹ�ư�� ���� 2 ���ϱ�
            camSpeed += 2f;
        else if(type == "RightUp" || type == "LeftDown")
            camSpeed -= 2f;
    }

    void Update()
    {
        // Ű���带 ���� �̵�
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 2;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -2;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // Camera Move
        // ȭ�� ������ �̵��Ϸ��� �ϸ� Move ����
        if ((camSpeed == -2f && camTrans.position.x > -2.5) || (camSpeed == 2f && camTrans.position.x < 2.5))
            isMove = true;
        else
            isMove = false;


        if (isMove)
        {
            // ���� ���Ͱ���ŭ �̵�
            Vector3 nextMove = Vector3.right * camSpeed * Time.deltaTime;
            camTrans.Translate(nextMove);
        }
    }
}
