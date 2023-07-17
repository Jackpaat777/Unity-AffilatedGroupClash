using System.Collections;
using System.Collections.Generic;
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

public enum UnitDetail
{
    Sword, Guard, Vampire, Bomb, Archer, Sniper,
    Farmer, Guitar, Wizard, Book, Noblilty
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

    public bool isFront;
    float attackTimer;
    float stopTimer;
    float moneyTimer;
    bool isHit;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // 유닛 설정
        //UnitSetting();

        // 상태 설정
        isFront = false;
        DoMove();
    }
    void UnitSetting()
    {
        // Attack Speed는 낮을수록 좋음
        switch (unitDetail)
        {
            case UnitDetail.Sword:
                unitHp = 13;
                unitAtk = 2;
                unitAtkSpeed = 1f;
                unitRange = 0.7f;
                unitSpeed = 0.9f;
                break;
            case UnitDetail.Guard:
                unitHp = 20;
                unitAtk = 1;
                unitAtkSpeed = 0.8f;
                unitRange = 0.7f;
                unitSpeed = 1f;
                break;
            case UnitDetail.Vampire:
                unitHp = 10;
                unitAtk = 2;
                unitAtkSpeed = 1.3f;
                unitRange = 1f;
                unitSpeed = 0.8f;
                break;
            case UnitDetail.Bomb:
                unitHp = 2;
                unitAtk = 20;
                unitAtkSpeed = 0;
                unitRange = 0.7f;
                unitSpeed = 0.8f;
                break;
            case UnitDetail.Archer:
                unitHp = 5;
                unitAtk = 1;
                unitAtkSpeed = 1f;
                unitRange = 2.5f;
                unitSpeed = 1f;
                break;
            case UnitDetail.Sniper:
                unitHp = 4;
                unitAtk = 2;
                unitAtkSpeed = 1.3f;
                unitRange = 5f;
                unitSpeed = 0.5f;
                break;
            case UnitDetail.Guitar:
                unitHp = 6;
                unitAtk = 1;
                unitAtkSpeed = 0.8f;
                unitRange = 3f;
                unitSpeed = 0.7f;
                break;
            case UnitDetail.Wizard:
                unitHp = 1;
                unitAtk = 1;
                unitAtkSpeed = 1f;
                unitRange = 3.5f;
                unitSpeed = 0f;
                break;
            case UnitDetail.Noblilty:
                unitHp = 5;
                unitAtk = 0;
                unitAtkSpeed = 0f;
                unitRange = 1f;
                unitSpeed = 0.8f;
                break;
        }

        // Red 팀 유닛이면 -1 곱해주기
        if (gameObject.layer == 9)
            unitSpeed *= -1;
    }


    void Update()
    {
        // 귀족
        if (unitDetail == UnitDetail.Noblilty)
        {
            moneyTimer += Time.deltaTime;
            if (moneyTimer > unitAtkSpeed)
            {
                DoStop();
                Debug.Log("+Coin");
                anim.SetTrigger("doAttack");
                moneyTimer = 0;
            }
        }

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

            // 근접 유닛의 경우 (Unit 변수 필요)
            if (unitType == UnitType.Warrior || unitType == UnitType.Tanker)
                DirectAttack(enemyLogic);

            // 원거리 유닛의 경우 (Transform 변수 필요)
            else if (unitType == UnitType.Ranger)
                ShotAttack(enemyLogic.transform);

            // 버퍼의 경우
            else if (unitType == UnitType.Buffer)
            {
                // 멈춤 (공격X)
                DoStop();
                isFront = true;
            }

            // 폭탄의 경우
            else if (unitDetail == UnitDetail.Bomb)
                BombAttack();
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
            
            if (unitDetail == UnitDetail.Book)
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
        if (attackTimer > unitAtkSpeed)
        {
            // 뱀파이어는 공격 시 체력 회복
            if (unitDetail == UnitDetail.Vampire)
            {
                unitHp += unitAtk;
                unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
            }
            enemyLogic.DoHit(unitAtk);

            anim.SetTrigger("doAttack");
            attackTimer = 0;
        }
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
        GameObject bullet = ObjectManager.instance.bulletBPrefabs[0];
        int idx = (int)unitDetail - (int)UnitDetail.Archer;

        if (gameObject.layer == 8)          // 블루 팀 유닛일 경우
        {
            // 자신을 기준으로 총알 발사
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar)
            {
                bullet = ObjectManager.instance.bulletBPrefabs[idx];
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            // 타겟를 기준으로 총알 발사
            else if (unitDetail == UnitDetail.Wizard || unitDetail == UnitDetail.Book)
            {
                bullet = ObjectManager.instance.bulletBPrefabs[idx];
                Instantiate(bullet, targetTrans.position, Quaternion.identity);
            }
        }
        else if (gameObject.layer == 9)     // 레드 팀 유닛일 경우
        {
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar)
            {
                bullet = ObjectManager.instance.bulletRPrefabs[idx];
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            else if (unitDetail == UnitDetail.Wizard || unitDetail == UnitDetail.Book)
            {
                bullet = ObjectManager.instance.bulletRPrefabs[idx];
                Instantiate(bullet, targetTrans.position, Quaternion.identity);
            }
        }
        // 총알에 값 넣기
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bulletLogic.dmg = unitAtk;


        anim.SetTrigger("doAttack");
        attackTimer = 0;

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }
    }
    void BombAttack()
    {
        // 자폭
        DoHit(unitMaxHp);

        // 폭발 이펙트 가져오기
        GameObject bomb = ObjectManager.instance.bulletTPrefabs[0];
        Bullet bombLogic = bomb.GetComponent<Bullet>();
        bombLogic.unitDetail = unitDetail;
        bombLogic.layer = gameObject.layer;
        bombLogic.dmg = unitAtk;
        Instantiate(bomb, transform.position, Quaternion.identity);
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
        //int idx = (int)unitDetail - (int)UnitDetail.Archer;

        //if (gameObject.layer == 8)          // 블루 팀 유닛일 경우
        //{
        //    // 타겟를 기준으로 발사
        //    if (unitDetail == UnitDetail.Book)
        //    {
        //        type = "ATK";
        //        value = 1;
        //        GameObject bullet = ObjectManager.instance.bulletTPrefabs[1];
        //        Instantiate(bullet, allyLogic.transform.position, Quaternion.identity);
        //    }
        //}
        //else if (gameObject.layer == 9)     // 레드 팀 유닛일 경우
        //{
        //    if (unitDetail == UnitDetail.Book)
        //    {
        //        type = "HP";
        //        value = 1;
        //        GameObject bullet = ObjectManager.instance.bulletTPrefabs[1];
        //        Instantiate(bullet, allyLogic.transform.position, Quaternion.identity);
        //    }
        //}

        // 타겟를 기준으로 발사
        if (unitDetail == UnitDetail.Book)
        {
            type = "HP";
            value = 1;
            GameObject bullet = ObjectManager.instance.bulletTPrefabs[1];
            Instantiate(bullet, allyLogic.transform.position, Quaternion.identity);
        }
        // 버프 발동
        allyLogic.SetBuffType(type, value);
        anim.SetTrigger("doAttack");
        attackTimer = 0;

        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }
    }
    void SetBuffType(string type, float value)
    {
        int valueInt = Mathf.FloorToInt(value);

        switch (type)
        {
            case "HP":
                unitHp += valueInt;
                unitHp = unitHp > unitMaxHp ? unitMaxHp : unitHp;
                break;
            case "ATK":
                unitAtk += valueInt;
                break;
            case "ATS":
                unitAtkSpeed += value;
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
        unitState = UnitState.Die;
        Destroy(gameObject);
    }
}
