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
            camSpeed += 2.5f;
        else if(type == "RightUp" || type == "LeftDown")
            camSpeed -= 2.5f;
    }

    // ��ư�� ���� ���� ����
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = ObjectManager.instance.bakChiB_blue_Prefabs[idx];
        Instantiate(unitB);
    }
    public void MakRedUnit(int idx)
    {
        GameObject unitR = ObjectManager.instance.bakChiA_red_Prefabs[idx];
        Instantiate(unitR);
    }

    void Update()
    {
        // Ű���带 ���� �̵�
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 2.5f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -2.5f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // Ű���带 ���� ���� ����
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(0);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(1);
        if (Input.GetKeyDown(KeyCode.D))
            MakeBlueUnit(2);
        if (Input.GetKeyDown(KeyCode.F))
            MakeBlueUnit(3);
        if (Input.GetKeyDown(KeyCode.G))
            MakeBlueUnit(4);
        if (Input.GetKeyDown(KeyCode.H))
            MakeBlueUnit(5);
        // red
        if (Input.GetKeyDown(KeyCode.Z))
            MakRedUnit(0);
        if (Input.GetKeyDown(KeyCode.X))
            MakRedUnit(1);
        if (Input.GetKeyDown(KeyCode.C))
            MakRedUnit(2);
        if (Input.GetKeyDown(KeyCode.V))
            MakRedUnit(3);
        if (Input.GetKeyDown(KeyCode.B))
            MakRedUnit(4);
        if (Input.GetKeyDown(KeyCode.N))
            MakRedUnit(5);

        // Camera Move
        // ȭ�� ������ �̵��Ϸ��� �ϸ� Move ����
        if ((camSpeed == -2.5f && camTrans.position.x > -5) || (camSpeed == 2.5f && camTrans.position.x < 5))
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
