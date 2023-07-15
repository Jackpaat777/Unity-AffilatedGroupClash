using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UnitName
{
    Sword,
    Range,
    Guard,
    Wizard,
    Sniper
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

    public UnitName unitName;
    public int unitHp;
    public int unitAtk;
    public float unitSpeed;
    public float unitRange;
    public float unitAttackSpeed;

    public ParticleSystem dustObject;
    public UnitState unitState; // 유닛의 상태마다 다른 로직을 실행하도록

    public bool isFront;
    float attackTimer;
    float stopTimer;
    bool isHit;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // 유닛 설정
        UnitSetting();

        // 첫 상태
        isFront = false;
        DoMove();
    }
    void UnitSetting()
    {
        // Attack Speed는 낮을수록 좋음
        switch (unitName)
        {
            case UnitName.Sword:
                unitHp = 10;
                unitAtk = 2;
                unitAttackSpeed = 1f;
                unitRange = 0.7f;
                unitSpeed = 1f;
                break;
            case UnitName.Range:
                unitHp = 5;
                unitAtk = 1;
                unitAttackSpeed = 1f;
                unitRange = 2.5f;
                unitSpeed = 1f;
                break;
            case UnitName.Guard:
                unitHp = 20;
                unitAtk = 1;
                unitAttackSpeed = 0.8f;
                unitRange = 0.7f;
                unitSpeed = 0.8f;
                break;
            case UnitName.Wizard:
                unitHp = 1;
                unitAtk = 1;
                unitAttackSpeed = 1f;
                unitRange = 3.5f;
                unitSpeed = 0f;
                break;
            case UnitName.Sniper:
                unitHp = 4;
                unitAtk = 2;
                unitAttackSpeed = 1.3f;
                unitRange = 5f;
                unitSpeed = 0.5f;
                break;
        }

        // Red 팀 유닛이면 -1 곱해주기
        if (gameObject.layer == 9)
            unitSpeed *= -1;
    }


    void Update()
    {
        if (unitName == UnitName.Wizard)
            return;

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

            // 일반 유닛의 경우
            if (unitName == UnitName.Sword || unitName == UnitName.Guard)
                DirectAttack(enemyLogic);

            // 원거리 유닛의 경우
            if (unitName == UnitName.Range || unitName == UnitName.Wizard || unitName == UnitName.Sniper)
                ShotAttack(enemyLogic.transform);
        }
        else
        {
            isFront = false;
        }
    }
    void ScanAlly()
    {
        if (unitState == UnitState.Die)
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
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.3f, LayerMask.GetMask(allyLayer));

        // 아군이 감지됨
        if (rayHit.collider != null)
        {
            isFront = true;
            DoStop();
        }
        else
        {
            isFront = false;
        }
    }

    void DirectAttack(Unit enemyLogic)
    {
        isFront = true;
        DoStop();

        // 공격속도에 따라서 어택
        attackTimer += Time.deltaTime;
        if (attackTimer > unitAttackSpeed)
        {
            enemyLogic.DoHit(unitAtk);

            anim.SetTrigger("doAttack");
            attackTimer = 0;
        }
    }
    void ShotAttack(Transform enemyTrans)
    {
        DoStop();

        // 공격속도에 따라서 어택
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAttackSpeed)
            return;

        if (gameObject.layer == 8)
        {
            if (unitName == UnitName.Range)
            {
                GameObject bullet = ObjectManager.instance.bluePrefabs[0];
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.dmg = unitAtk;
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            else if (unitName == UnitName.Wizard)
            {
                GameObject bullet = ObjectManager.instance.bluePrefabs[1];
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.dmg = unitAtk;
                Instantiate(bullet, enemyTrans.position, Quaternion.identity);
            }
            else if (unitName == UnitName.Sniper)
            {
                GameObject bullet = ObjectManager.instance.bluePrefabs[2];
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.dmg = unitAtk;
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }

        }
        else if (gameObject.layer == 9)
        {
            if (unitName == UnitName.Range)
            {
                GameObject bullet = ObjectManager.instance.redPrefabs[0];
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.dmg = unitAtk;
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            else if (unitName == UnitName.Wizard)
            {
                GameObject bullet = ObjectManager.instance.redPrefabs[1];
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.dmg = unitAtk;
                Instantiate(bullet, enemyTrans.position, Quaternion.identity);
            }
            else if (unitName == UnitName.Sniper)
            {
                GameObject bullet = ObjectManager.instance.redPrefabs[2];
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.dmg = unitAtk;
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
        }
        anim.SetTrigger("doAttack");
        attackTimer = 0;


        if (isHit)
        {
            anim.SetTrigger("doHit");
            isHit = false;
        }
    }

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
