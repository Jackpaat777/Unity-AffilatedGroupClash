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
    public bool isNotAtk;
    public bool isDebuff;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        transform.localRotation = Quaternion.identity;
        isHit = false;

        // 마왕이펙트는 speed값이 있지만 움직이지 않음(Rotate만 하기위해)
        if (unitDetail != UnitDetail.Devil)
            rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * speed);

        // ATK, ATS ,SPD는 제외
        if (unitDetail != UnitDetail.AtkUp || unitDetail != UnitDetail.AtkspdUp || unitDetail != UnitDetail.SpdUp)
        {
            // 화면 밖으로 나갈 경우
            if (transform.position.x < -15 || transform.position.x > 13)
                gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        // 광역공격 유닛, 아군 버프 유닛
        if (unitDetail == UnitDetail.Drum || unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.Wizard ||
            unitDetail == UnitDetail.AtkUp || unitDetail == UnitDetail.AtkspdUp || unitDetail == UnitDetail.SpdUp)
            return;

        // 버프용 Bullet인 경우 바로 제거
        if (isNotAtk)
        {
            if (unitDetail == UnitDetail.Devil)
            {
                StartCoroutine(DisableRoutine(0.5f));
                return;
            }

            // 이동하지 않는 경우 2초 뒤에 제거
            if (speed == 0)
                StartCoroutine(DisableRoutine(1f));
            else
                gameObject.SetActive(false);
            return;
        }

        ScanEnemy();
    }

    void ScanEnemy()
    {
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

            // 디버프 총알
            if (isDebuff)
            {
                // Singer에 맞으면 공격력 감소 (1번만)
                if (unitDetail == UnitDetail.Singer && !enemyLogic.isAtkDebuff)
                {
                    enemyLogic.unitAtk -= 5;
                    enemyLogic.unitAtk = enemyLogic.unitAtk < 0 ? 0 : enemyLogic.unitAtk;
                    enemyLogic.isAtkDebuff = true;
                    enemyLogic.atkDebuffTimer = 0;
                }
            }

            // Destroy
            gameObject.SetActive(false);
        }
    }

    // 광역공격 트리거
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (unitDetail == UnitDetail.Drum)
        {
            // 적군일 경우
            if ((layer == 8 && collision.gameObject.layer == 9) || (layer == 9 && collision.gameObject.layer == 8))
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                unitLogic.DoHit(dmg);
            }

            StartCoroutine(DisableRoutine(1f));
        }
        else if (unitDetail == UnitDetail.Wizard)
        {
            // 적군일 경우
            if ((layer == 8 && collision.gameObject.layer == 9) || (layer == 9 && collision.gameObject.layer == 8))
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                unitLogic.DoHit(dmg);
            }

            StartCoroutine(DisableRoutine(1f));
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (unitDetail == UnitDetail.Bomb)
        {
            // 적군일 경우
            if ((layer == 8 && collision.gameObject.layer == 9) || (layer == 9 && collision.gameObject.layer == 8))
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                unitLogic.DoHit(dmg);
            }

            StartCoroutine(DisableRoutine(1f));
        }
    }

    IEnumerator DisableRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
