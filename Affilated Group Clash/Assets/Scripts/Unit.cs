using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UnitType
{
    Sword,
    Range,
    Guard,
    Wizard,
    Bullet
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

    public UnitType unitName;
    public float unitSpeed;
    public float unitRange;
    public float unitAttackSpeed;

    public ParticleSystem dustObject;
    public UnitState unitState;

    public bool isStop;
    public bool isNothing;
    float attackTimer;
    float stopTimer;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // 사거리 설정
        UnitSetting();

        // 첫 상태는 Move
        isStop = true;
        DoMove();
    }
    void UnitSetting()
    {
        // Attack Speed는 낮을수록 좋음
        switch (unitName)
        {
            case UnitType.Sword:
                unitRange = 0.7f;
                unitAttackSpeed = 1f;
                break;
            case UnitType.Range:
                unitRange = 2.5f;
                unitAttackSpeed = 1.5f;
                break;
            case UnitType.Guard:
                unitRange = 0.7f;
                unitAttackSpeed = 1f;
                break;
            case UnitType.Wizard:
                unitRange = 3.5f;
                unitAttackSpeed = 1f;
                break;
            case UnitType.Bullet:
                unitRange = 0.4f;
                unitAttackSpeed = 1f;
                break;

        }
    }


    void Update()
    {
        if (unitState == UnitState.Move)
        {
            Vector3 nextMove = Vector3.right * unitSpeed * Time.deltaTime;
            transform.Translate(nextMove);
        }
        else if (unitState == UnitState.Attack)
        {
            // 공격속도에 따라서 어택
            attackTimer += Time.deltaTime;
            if (attackTimer > unitAttackSpeed)
            {
                DoAttack();
                attackTimer = 0;
            }
        }
        else if (unitState == UnitState.Idle)
        {
            // 앞에 아무도 없을 때
            if (isNothing)
            {
                // 0.5초 뒤에 다시 움직임
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
        ScanAlly();
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
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(enemyLayer));

        // 적군이 감지됨
        if (rayHit.collider != null)
        {
            isNothing = false;
            DoStop();
            unitState = UnitState.Attack;
        }
        else
        {
            isNothing = true;
        }
    }

    void ScanAlly()
    {
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
            isNothing = false;
            DoStop();
        }
        else
        {
            isNothing = true;
        }
    }

    void DoAttack()
    {
        anim.SetTrigger("doAttack");
    }

    void DoMove()
    {
        if (!isStop)
            return;

        unitState = UnitState.Move;
        isStop = false;
        anim.SetTrigger("doMove");
        if (dustObject != null)
            dustObject.Play();
    }

    void DoStop()
    {
        if (isStop)
            return;

        unitState = UnitState.Idle;
        isStop = true;
        anim.SetTrigger("doStop");
        if (dustObject != null)
            dustObject.Stop();
    }
}
