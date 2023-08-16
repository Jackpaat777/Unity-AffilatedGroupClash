using System.Collections;
using UnityEngine;

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
    Collider2D col;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        transform.localRotation = Quaternion.identity;
        isHit = false;

        if (col != null)
            col.enabled = true;

        // 마왕이펙트는 speed값이 있지만 움직이지 않음(Rotate만 하기위해)
        if (unitDetail != UnitDetail.Devil)
            rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * speed);

        // Circle은 제외 (화면밖으로 나갈 일이 있나?)
        //if (unitDetail != UnitDetail.AtkUp || unitDetail != UnitDetail.AtkspdUp || unitDetail != UnitDetail.SpdUp || unitDetail != UnitDetail.Piano)
        //{
        //    // 화면 밖으로 나갈 경우 제거
        //    if (transform.position.x < -15 || transform.position.x > 15)
        //        gameObject.SetActive(false);
        //}

        // 아무리 길어도 1초(버그있으면 1.2~1.5초로 늘려보기)
        StartCoroutine(DisableRoutine(1f));
    }

    void FixedUpdate()
    {
        // 광역공격 유닛, 아군 버프 유닛은 ScanEnemy없이 넘어감
        if (unitDetail == UnitDetail.Stick || unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.Wizard ||
            unitDetail == UnitDetail.AtkUp || unitDetail == UnitDetail.AtkspdUp || unitDetail == UnitDetail.RanUp || unitDetail == UnitDetail.Piano)
            return;

        // 공격하지 않는 Bullet인 경우 ScanEnemy없이 바로 제거
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

        ScanSensor();
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

            // 디버프 총알 (Base는 제외)
            if (isDebuff && enemyLogic.unitDetail != UnitDetail.Base)
            {
                // Singer에 맞으면 공격력 감소 (1번만)
                if (unitDetail == UnitDetail.Singer)
                {
                    // 이미 디버프 중이면 발동하지 않음
                    if (!enemyLogic.isAtkDebuff)
                    {
                        enemyLogic.unitAtk -= 3;
                        enemyLogic.unitAtk = enemyLogic.unitAtk < 0 ? 0 : enemyLogic.unitAtk;
                        enemyLogic.isAtkDebuff = true;
                    }
                    // 타이머는 초기화
                    enemyLogic.atkDebuffTimer = 0;
                }
                // Bass에 맞으면 이속 감소 (1번만)
                if (unitDetail == UnitDetail.Bass)
                {
                    if (!enemyLogic.isSpdDebuff)
                    {
                        if (enemyLogic.gameObject.layer == 8)
                            enemyLogic.unitSpeed -= 0.3f;
                        else if (enemyLogic.gameObject.layer == 9)
                            enemyLogic.unitSpeed += 0.3f;
                        enemyLogic.isSpdDebuff = true;
                    }
                    enemyLogic.spdDebuffTimer = 0;
                }
            }

            // Destroy
            gameObject.SetActive(false);
        }
    }

    // Destroy Bullet 센서 스캔
    void ScanSensor()
    {
        // 베이스의 총알은 못막음
        if (unitDetail == UnitDetail.Base)
            return;

        Vector2 dir = Vector2.zero;
        string enemyLayer = "";

        if (gameObject.layer == 8)
        {
            dir = Vector2.right;
            enemyLayer = "DestroyR";
        }
        else if (gameObject.layer == 9)
        {
            dir = Vector2.left;
            enemyLayer = "DestroyB";
        }

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.4f, LayerMask.GetMask(enemyLayer));

        if (rayHit.collider != null)
        {
            // Destroy
            gameObject.SetActive(false);
        }
    }

    // 광역공격 트리거
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Wizard는 원거리이기 때문에 따로 분류(Destroy Bullet)
        if (unitDetail == UnitDetail.Stick || unitDetail == UnitDetail.Wizard || unitDetail == UnitDetail.Bomb)
        {
            // 적군일 경우
            if ((layer == 8 && collision.gameObject.layer == 9) || (layer == 9 && collision.gameObject.layer == 8))
            {
                Unit unitLogic = collision.GetComponent<Unit>();

                // Bomb 데미지는 적 체력의 절반 (기지의 경우 100의 데미지)
                if (unitDetail == UnitDetail.Bomb)
                {
                    if (unitLogic.unitDetail == UnitDetail.Base)
                        unitLogic.DoHit(100);
                    else
                    {
                        // 적 최대체력이 짝수일 경우
                        if (unitLogic.unitMaxHp % 2 == 0)
                            unitLogic.DoHit(unitLogic.unitMaxHp / 2);
                        // 적 최대체력이 홀수일 경우
                        else
                            unitLogic.DoHit(unitLogic.unitMaxHp / 2 + 1);

                    }
                }
                else
                    unitLogic.DoHit(dmg);

                // collider는 바로 끄기 (끄트머리에서 맞으면 연속데미지 받는 오류)
                if (col != null)
                    col.enabled = false;
                StartCoroutine(DisableRoutine(1f));
            }
        }
    }

    IEnumerator DisableRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
