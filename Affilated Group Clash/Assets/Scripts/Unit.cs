using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UnitType
{
    Warrior,
    Tanker,
    Ranger,
    Buffer,
    Special
}

// 변수위치를 바꾸면 ObjectManager 인스펙터에서 프리펩 위치도 바꿔줘야함
public enum UnitDetail
{
    // Bullet A 사용 유닛
    Archer, Sniper, Farmer, Guitar, Singer, AtkspdUp, Wizard,
    // Bullet B 사용 유닛
    Bomb, Drum, AtkUp, Vampire, Punch, Devil, Heal,
    Sword, Guard, Berserker,
    CostUp
}

public enum UnitState
{
    Idle,
    Move,
    Attack,
    Hit,
    Die
}

public class Unit : MonoBehaviour
{
    [Header("---------------[Stat]")]
    public UnitType unitType;
    public UnitDetail unitDetail;
    public int unitCost;
    public int unitMaxHp;
    public int unitHp;
    public int unitAtk;
    public float unitAtkSpeed;
    public float unitRange;
    public float unitSpeed;

    [Header("---------------[Game]")]
    public ParticleSystem dustObject;
    public UnitState unitState; // 유닛의 상태마다 다른 로직을 실행하도록

    public bool isATSSensor;
    public bool isATSUp;
    public bool isAtkDebuff;
    public int atkBuffCount;
    public float atkDebuffTimer;
    float attackTimer;
    float stopTimer;
    float devilTimer;
    bool isFront;
    bool isHit;
    GameObject atsSensor;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // 상태 설정
        unitState = UnitState.Idle;
        isFront = false;
        DoMove();
    }

    // 재사용 시 초기화
    void OnEnable()
    {
        // 위치 초기화
        if (gameObject.layer == 8)
            transform.localPosition = Vector3.left * 11;
        else if (gameObject.layer == 9)
            transform.localPosition = Vector3.right * 11;

        // 타이머 초기화
        attackTimer = 0; stopTimer = 0; devilTimer = 0;

        // 상태 설정
        unitState = UnitState.Idle;
        isFront = false;
        isHit = false;
        DoMove();

        // 값 초기화
        unitHp = unitMaxHp;
        atsSensor = null;

        // 스킬로 인한 값
        isAtkDebuff = false;
        atkBuffCount = 0;
        if (isATSUp) // 모든 유닛에 적용
        {
            unitAtkSpeed *= 2;
            isATSUp = false;
        }

        // 영구 증가 유닛 초기화
        if (unitDetail == UnitDetail.Guitar)
            unitAtkSpeed = 1.5f;

        if (unitDetail == UnitDetail.Berserker)
            unitAtkSpeed = 1.5f;

        if (unitDetail == UnitDetail.AtkspdUp)
        {
            int idx = (int)unitDetail;
            if (gameObject.layer == 8)
            {
                atsSensor = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                
            }
            else if (gameObject.layer == 9)
            {
                atsSensor = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1), transform.position + Vector3.right * 0.5f);
            }
        }
    }

    // 초기화 함수 -> guard같은 중복함수는? -> 걍 분리할까

    void Update()
    {
        // 특수유닛 - 코스트증가 || 공속증가
        if (unitDetail == UnitDetail.CostUp || unitDetail == UnitDetail.AtkspdUp)
        {
            if (gameObject.layer == 8)
            {
                atsSensor.transform.position = transform.position + Vector3.left * 0.5f;
            }
            else if (gameObject.layer == 9)
            {
                atsSensor.transform.position = transform.position + Vector3.right * 0.5f;
            }

            attackTimer += Time.deltaTime;
            if (attackTimer > unitAtkSpeed)
            {
                // Stop -> doMove 애니메이터로 넘어가기 위해 멈추기 필요
                DoStop();

                // CostUp
                if (unitDetail == UnitDetail.CostUp)
                {
                    if (gameObject.layer == 8 && GameManager.instance.blueCost < 10)
                        GameManager.instance.blueCost += 1;
                    else if (gameObject.layer == 9 && GameManager.instance.redCost < 10)
                        GameManager.instance.redCost += 1;
                }

                // Animation
                anim.SetTrigger("doAttack");
                attackTimer = 0;
            }
        }
        // 마왕
        if (unitDetail == UnitDetail.Devil)
        {
            // Sensor 켜기
            if (gameObject.layer == 8)
                GameManager.instance.allSensor.layer = 10;
            else if (gameObject.layer == 9)
                GameManager.instance.allSensor.layer = 11;
            GameManager.instance.allSensor.SetActive(true);

            // 공격 모션
            attackTimer += Time.deltaTime;
            if (attackTimer > unitAtkSpeed)
            {
                DoStop();
                anim.SetTrigger("doAttack");
                attackTimer = 0;
            }
        }
        // 공격 디버프 (2초뒤에 다시 올림)
        if (isAtkDebuff)
        {
            atkDebuffTimer += Time.deltaTime;
            if (atkDebuffTimer > 2f)
            {
                unitAtk += 1;
                isAtkDebuff = false;
                atkDebuffTimer = 0;
            }
        }
        // 공속 버프
        if (isATSSensor)
        {
            // 버프 발동 (한번만 실행) -> 실행이후 isATSUp가 true가 되어 더이상 공속증가 로직을 실행하지 않음
            // isATSSensor가 발동중이면 공속버프 해제가 될일은 없음
            if (!isATSUp)
            {
                // 공속 증가
                unitAtkSpeed /= 2;
                isATSUp = true;
            }
        }
        else
        {
            // 버프 해제 (한번만 실행)
            if (isATSUp)
            {
                unitAtkSpeed *= 2;
                isATSUp = false;
            }
        }

        // 유닛 이동
        UnitMovement();
    }
    void UnitMovement()
    {
        // 유닛 이동 구현
        if (unitState == UnitState.Move)
        {
            Vector3 nextMove = Vector3.right * unitSpeed * Time.deltaTime;
            transform.Translate(nextMove);
        }
        else if (unitState == UnitState.Idle || unitState == UnitState.Hit)
        {
            // 앞에 아무도 없을 때
            if (!isFront)
            {
                // 일정시간 뒤에 다시 움직임
                stopTimer += Time.deltaTime;
                if (stopTimer > 0.5f)
                {
                    DoMove();
                    stopTimer = 0;
                }
            }
        }
    }

    void FixedUpdate()
    {
        ScanEnemy();
        //ScanAlly();
    }

    // ======================================================= 타겟 감지 함수
    void ScanEnemy()
    {
        if (unitState == UnitState.Die)
            return;

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
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(enemyLayer));

        // 적군이 감지됨
        if (rayHit.collider != null)
        {
            // 적 오브젝트 가져오기
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();


            // 드럼의 경우(전사지만 다른 함수 사용) / 폭탄의 경우
            if (unitDetail == UnitDetail.Drum || unitDetail == UnitDetail.Bomb)
                BombAttack();

            // 근접 유닛의 경우 (Unit 변수 필요)
            else if (unitType == UnitType.Warrior || unitType == UnitType.Tanker)
                DirectAttack(enemyLogic);

            // 원거리 유닛의 경우 (Transform 변수 필요)
            else if (unitType == UnitType.Ranger)
                ShotAttack(enemyLogic.transform);

            // 버퍼의 경우
            else if (unitType == UnitType.Buffer || unitDetail == UnitDetail.Devil)
            {
                // 멈춤 (공격X)
                DoStop();
                isFront = true;
            }
        }
        else
        {
            isFront = false;
        }

        // 폭탄 레이
        if (unitDetail == UnitDetail.Bomb)
        {
            RaycastHit2D bombRayHit = Physics2D.Raycast(transform.position, dir, 4f, LayerMask.GetMask(enemyLayer));
            if (bombRayHit.collider != null)
            {
                anim.SetTrigger("doAttack");
                unitSpeed *= 1.1f;
            }
        }
    }
    void ScanAlly()
    {
        // 버퍼가 아니면 아군체크 X
        if (unitState == UnitState.Die || unitType != UnitType.Buffer)
            return;

        Vector2 dir = Vector2.zero;
        string allyLayer = "";

        if (gameObject.layer == 8)      // Blue 팀 유닛
        {
            dir = Vector2.right;
            allyLayer = "Blue";
        }
        else if (gameObject.layer == 9) // Red 팀 유닛
        {
            dir = Vector2.left;
            allyLayer = "Red";
        }

        // RayCast
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(allyLayer));

        // 아군이 감지됨
        if (rayHit.collider != null)
        {
            // 아군 오브젝트 가져오기
            Unit allyLogic = rayHit.collider.gameObject.GetComponent<Unit>();
            
            if (unitDetail == UnitDetail.Heal)
            {
                // State가 Move가 아닐때 실행 -> Move가 될때까지 실행안하다가 한번에 함
                // 풀피면 실행 X
                if (allyLogic.unitHp < allyLogic.unitMaxHp)
                    DoBuff(allyLogic);
            }
        }
        else
        {
            isFront = false;
        }
    }

    // ======================================================= 공격 함수
    void DirectAttack(Unit enemyLogic)
    {
        // 멈춤
        isFront = true;
        DoStop();

        // 공격속도에 따라서 어택
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;


        // Skill
        int idx = (int)unitDetail - (int)UnitDetail.Bomb;
        // 뱀파이어는 공격 시 체력 회복
        if (unitDetail == UnitDetail.Vampire)
        {
            // 회복 이펙트
            ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1) * 2, transform.position + Vector3.up * 0.5f);
            ExeBuffType("HP", 1);
        }
        // 펀치는 상대 체력이 3이하면 즉사시킴
        else if (unitDetail == UnitDetail.Punch)
        {
            // 즉사
            if (enemyLogic.unitHp <= 5)
            {
                enemyLogic.DoHit(5);
                // 이펙트
                if (gameObject.layer == 8)
                {
                    Vector3 vec = new Vector3(0.5f, 0.5f);
                    ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1) * 2, transform.position + vec);
                }
                else if (gameObject.layer == 9)
                {
                    Vector3 vec = new Vector3(-0.5f, 0.5f);
                    ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1) * 2, transform.position + vec);
                }
            }
        }

        // Attack
        enemyLogic.DoHit(unitAtk);
        anim.SetTrigger("doAttack");
        attackTimer = 0;
    }
    void ShotAttack(Transform targetTrans)
    {
        // 멈춤
        isFront = true;
        DoStop();

        // 공격속도에 따라서 실행
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Bullet
        GameObject bullet = null;
        int idx = (int)unitDetail;
        if (gameObject.layer == 8)          // 블루 팀 유닛일 경우
        {
            // 자신을 기준으로 총알 발사
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar || unitDetail == UnitDetail.Singer)
            {
                bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.up * 0.3f);
            }
            // 타겟를 기준으로 총알 발사
            else if (unitDetail == UnitDetail.Wizard)
            {
                bullet = ObjectManager.instance.GetBullet(idx, targetTrans.position);
            }
        }
        else if (gameObject.layer == 9)     // 레드 팀 유닛일 경우
        {
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar || unitDetail == UnitDetail.Singer)
            {
                bullet = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1), transform.position + Vector3.up * 0.3f); // => Bullet A 사용 유닛 개수 추가
            }
            else if (unitDetail == UnitDetail.Wizard)
            {
                bullet = ObjectManager.instance.GetBullet(idx + ((int)UnitDetail.Wizard + 1), targetTrans.position);
            }
        }
        // Bullet에 값 넘겨주기
        ObjectManager.instance.BulletSetting(bullet, unitDetail, gameObject.layer, unitAtk);

        // Skill
        if (unitDetail == UnitDetail.Guitar)
        {
            ExeBuffType("ATS", 0.05f);
        }

        // Attack
        anim.SetTrigger("doAttack");
        attackTimer = 0;
    }
    void BombAttack()
    {
        // 이펙트 가져오기
        GameObject bullet = null;

        int idx = (int)unitDetail - (int)UnitDetail.Bomb + ((int)UnitDetail.Wizard + 1) * 2;
        if (unitDetail == UnitDetail.Bomb)
        {
            // 자폭
            DoHit(unitMaxHp);
            // 이펙트
            bullet = ObjectManager.instance.GetBullet(idx, transform.position); // 12 => Bullet A 사용 유닛 * 2
        }
        else // 공속에 영향을 받는 유닛들
        {
            // 멈춤
            isFront = true;
            DoStop();

            // 공격속도에 따라서 실행
            attackTimer += Time.deltaTime;
            if (attackTimer < unitAtkSpeed)
                return;

            if (gameObject.layer == 8)
            {
                if (unitDetail == UnitDetail.Drum)
                {
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.right * 0.5f);
                }
            }
            else if (gameObject.layer == 9)
            {
                if (unitDetail == UnitDetail.Drum)
                {
                    bullet = ObjectManager.instance.GetBullet(idx, transform.position + Vector3.left * 0.5f);
                }
            }

            // Attack
            anim.SetTrigger("doAttack");
            attackTimer = 0;
        }

        // Bullet에 값 넘겨주기
        ObjectManager.instance.BulletSetting(bullet, unitDetail, gameObject.layer, unitAtk);
    }

    // ======================================================= 버프 함수
    public void DoBuff(Unit allyLogic)
    {
        // 공격속도에 따라서 실행
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Bullet
        string type = "";
        float value = 0f;
        int idx = (int)unitDetail - (int)UnitDetail.Bomb + ((int)UnitDetail.Wizard + 1) * 2;

        if (gameObject.layer == 8)          // 블루 팀 유닛일 경우
        {
            // 타겟을 기준으로 발사
            if (unitDetail == UnitDetail.Heal)
            {
                type = "HP";
                value = 1;
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
            else if (unitDetail == UnitDetail.AtkUp)
            {
                type = "ATK";
                value = 1;
                allyLogic.atkBuffCount += 1;
                Vector3 vec = new Vector3(-0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }
        else if (gameObject.layer == 9)     // 레드 팀 유닛일 경우
        {
            if (unitDetail == UnitDetail.Heal)
            {
                type = "HP";
                value = 1;
                Vector3 vec = new Vector3(0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
            else if (unitDetail == UnitDetail.AtkUp)
            {
                type = "ATK";
                value = 1;
                atkBuffCount += 1;
                Vector3 vec = new Vector3(0.5f, 0.5f);
                ObjectManager.instance.GetBullet(idx, allyLogic.transform.position + vec);
            }
        }

        // 버프 발동
        allyLogic.ExeBuffType(type, value);
        anim.SetTrigger("doAttack");
        attackTimer = 0;

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }
    }
    void ExeBuffType(string type, float value)
    {
        // 소수점 내림
        int valueInt = Mathf.FloorToInt(value);

        switch (type)
        {
            case "HP":
                unitHp += valueInt;
                unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
                break;
            case "ATK":
                // 버프 중첩은 2번까지
                if (atkBuffCount < 3)
                    unitAtk += valueInt;
                else
                    atkBuffCount = 3;
                break;
            case "ATS":
                unitAtkSpeed -= value;
                unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
                break;
            case "RNG":
                unitRange += value;
                break;
            case "SPD":
                unitSpeed += value;
                break;
        }

    }

    // ======================================================= 상태 함수
    void DoMove()
    {
        // 이미 움직이고 있으면 호출 X
        if (unitState == UnitState.Move)
            return;

        unitState = UnitState.Move;
        anim.SetTrigger("doMove");
        if (dustObject != null)
            dustObject.Play();
    }
    void DoStop()
    {
        // 안움직이고 있다면 호출 X
        if (unitState != UnitState.Move)
            return;

        unitState = UnitState.Idle;
        anim.SetTrigger("doStop");
        if (dustObject != null)
            dustObject.Stop();

        stopTimer = 0;
    }
    public void DoHit(int damage)
    {
        if (unitDetail == UnitDetail.Berserker)
        {
            // 깎인체력 1 당 0.1씩 공속 감소
            unitAtkSpeed -= damage * 0.1f;
            unitAtkSpeed = unitAtkSpeed < 0.3f ? 0.3f : unitAtkSpeed;
        }
        unitState = UnitState.Hit;
        unitHp -= damage;
        isHit = true;
        anim.SetTrigger("doHit");

        stopTimer = 0;
        
        if (dustObject != null)
            dustObject.Stop();

        if (unitHp <= 0)
        {
            DoDie();
            return;
        }
    }
    void DoDie()
    {
        if (unitDetail == UnitDetail.Devil)
        {
            // Sensor 끄기
            GameManager.instance.allSensor.SetActive(false);
        }
        else if (unitDetail == UnitDetail.AtkspdUp)
        {
            // Sensor 끄기
            atsSensor.SetActive(false);
        }
        for (int i = 0; i < atkBuffCount - 1; i++)
        {
            // 공격증가 버프 받은 횟수만큼 차감
            unitAtk -= 1;
        }

        unitState = UnitState.Die;
        gameObject.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // Blue 팀 유닛이 Red Sensor와 닿았을 경우 || Red 팀 유닛이 Blue Sensor와 닿았을 경우
        if ((gameObject.layer == 8 && collision.gameObject.layer == 11) || (gameObject.layer == 9 && collision.gameObject.layer == 10))
        {
            // 이펙트 위치조정 벡터
            Vector3 vec = Vector3.zero;
            if (gameObject.layer == 8)
                vec = new Vector3(-0.5f, 1.5f);
            else if (gameObject.layer == 9)
                vec = new Vector3(0.5f, 1.5f);

            // Hit
            devilTimer += Time.deltaTime;
            if (devilTimer > 2.5f)  // 데미지 받는 주기 (2.5f)
            {
                // 이펙트
                int idx = (int)UnitDetail.Devil - (int)UnitDetail.Bomb + ((int)UnitDetail.Wizard + 1) * 2;
                ObjectManager.instance.GetBullet(idx, transform.position + vec);
                // Hit
                DoHit(2);
                devilTimer = 0;
            }
        }

        // Blue 팀 유닛이 Blue Buff Sensor와 닿았을 경우 || Red 팀 유닛이 Red Buff Sensor와 닿았을 경우
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR"))
        {
            // 버프 적용
            isATSSensor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((gameObject.layer == 8 && collision.gameObject.tag == "AtkSpd_UpB") || (gameObject.layer == 9 && collision.gameObject.tag == "AtkSpd_UpR"))
        {
            // 버프 해제
            isATSSensor = false;
        }
    }
}
