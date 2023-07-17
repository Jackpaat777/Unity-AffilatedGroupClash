using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.CanvasScaler;

public class Bullet : MonoBehaviour
{
    public UnitDetail unitDetail;
    public int layer;
    public int dmg;
    public float speed;
    public bool isHit;
    public bool isRotate;
    public bool isBuff;


    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * speed);

        // 사거리 밖으로 나간 경우?

        // 화면 밖으로 나갈 경우
        if (transform.position.x < -13 || transform.position.x > 13)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (!isHit)
        {
            ScanEnemy();
            //if (isBuff)
            //    ScanAlly();
            //else
            //    ScanEnemy();
        }
    }

    void ScanEnemy()
    {
        // 버프용 Bullet인 경우
        if (isBuff)
        {
            if (speed == 0)
                Destroy(gameObject, 3f);
            else
                Destroy(gameObject);
            return;
        }

        Vector2 dir = Vector2.zero;
        string enemyLayer = "";

        if (gameObject.layer == 8)      // Blue 팀 유닛
        {
            dir = Vector2.right;
            enemyLayer = "Red";
        }
        else if (gameObject.layer == 9) // Red 팀 유닛
        {
            dir = Vector2.left;
            enemyLayer = "Blue";
        }

        // RayCast
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.4f, LayerMask.GetMask(enemyLayer));

        // 적군이 감지됨
        if (rayHit.collider != null)
        {
            // 적 오브젝트 가져오기
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

            isHit = true;
            enemyLogic.DoHit(dmg);

            // 화살인지 마법인지에 따라 삭제 시간 조절
            if (speed == 0)
                Destroy(gameObject, 3f);
            else
                Destroy(gameObject);
        }
    }

    //void ScanAlly()
    //{
    //    Vector2 dir = Vector2.zero;
    //    string allyLayer = "";

    //    if (gameObject.layer == 8)      // Blue 팀 유닛
    //    {
    //        dir = Vector2.right;
    //        allyLayer = "Blue";
    //    }
    //    else if (gameObject.layer == 9) // Red 팀 유닛
    //    {
    //        dir = Vector2.left;
    //        allyLayer = "Red";
    //    }

    //    // RayCast
    //    RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.3f, LayerMask.GetMask(allyLayer));

    //    // 아군이 감지됨
    //    if (rayHit.collider != null)
    //    {
    //        // 아군 오브젝트 가져오기
    //        Unit allyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

    //        isHit = true;
    //        allyLogic.DoBuff("HP", 1);


    //        if (speed == 0)
    //            Destroy(gameObject, 3f);
    //        else
    //            Destroy(gameObject);
    //    }
    //}

    // 광역공격 트리거
    void OnTriggerStay2D(Collider2D collision)
    {
        if (unitDetail == UnitDetail.Bomb)
        {
            if (layer == 8 && collision.gameObject.layer == 9)
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                unitLogic.DoHit(dmg);
            }
            else if (layer == 9 && collision.gameObject.layer == 8)
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                unitLogic.DoHit(dmg);
            }

            Destroy(gameObject, 1.5f);
        }
    }
}
