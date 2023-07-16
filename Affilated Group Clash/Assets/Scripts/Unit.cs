using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UnitName
{
    Sword,
    Guard,
    Vampire,
    Bomb,
    Range,
    Sniper,
    Guitar,
    Wizard,
    Noblilty
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
    public UnitState unitState; // ������ ���¸��� �ٸ� ������ �����ϵ���

    public bool isFront;
    float attackTimer;
    float stopTimer;
    float moneyTimer;
    bool isHit;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // ���� ����
        UnitSetting();

        // ù ����
        isFront = false;
        DoMove();
    }
    void UnitSetting()
    {
        // Attack Speed�� �������� ����
        switch (unitName)
        {
            case UnitName.Sword:
                unitHp = 13;
                unitAtk = 2;
                unitAttackSpeed = 1f;
                unitRange = 0.7f;
                unitSpeed = 1f;
                break;
            case UnitName.Guard:
                unitHp = 20;
                unitAtk = 1;
                unitAttackSpeed = 0.8f;
                unitRange = 0.7f;
                unitSpeed = 0.8f;
                break;
            case UnitName.Vampire:
                unitHp = 10;
                unitAtk = 2;
                unitAttackSpeed = 1.3f;
                unitRange = 1f;
                unitSpeed = 0.8f;
                break;
            case UnitName.Bomb:
                unitHp = 2;
                unitAtk = 20;
                unitAttackSpeed = 0;
                unitRange = 0.7f;
                unitSpeed = 0.8f;
                break;
            case UnitName.Range:
                unitHp = 5;
                unitAtk = 1;
                unitAttackSpeed = 1f;
                unitRange = 2.5f;
                unitSpeed = 1f;
                break;
            case UnitName.Sniper:
                unitHp = 4;
                unitAtk = 2;
                unitAttackSpeed = 1.3f;
                unitRange = 5f;
                unitSpeed = 0.5f;
                break;
            case UnitName.Guitar:
                unitHp = 6;
                unitAtk = 1;
                unitAttackSpeed = 0.8f;
                unitRange = 3f;
                unitSpeed = 0.8f;
                break;
            case UnitName.Wizard:
                unitHp = 1;
                unitAtk = 1;
                unitAttackSpeed = 1f;
                unitRange = 3.5f;
                unitSpeed = 0f;
                break;
            case UnitName.Noblilty:
                unitHp = 5;
                unitAtk = 0;
                unitAttackSpeed = 0f;
                unitRange = 1f;
                unitSpeed = 0.8f;
                break;
        }

        // Red �� �����̸� -1 �����ֱ�
        if (gameObject.layer == 9)
            unitSpeed *= -1;
    }


    void Update()
    {
        // ������
        if (unitName == UnitName.Wizard)
            return;
        // ����
        if (unitName == UnitName.Noblilty)
        {
            moneyTimer += Time.deltaTime;
            if (moneyTimer > 1.5f)
            {
                Debug.Log("+Coin");
                anim.SetTrigger("doAttack");
                moneyTimer = 0;
            }
        }

        // ���� �̵� ����
        if (unitState == UnitState.Move)
        {
            Vector3 nextMove = Vector3.right * unitSpeed * Time.deltaTime;
            transform.Translate(nextMove);
        }
        else if (unitState == UnitState.Idle || unitState == UnitState.Hit)
        {
            // �տ� �ƹ��� ���� ��
            if (!isFront)
            {
                // �����ð� �ڿ� �ٽ� ������
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

        if (gameObject.layer == 8)      // Blue �� ����
        {
            dir = Vector2.right;
            enemyLayer = "Red";
        }
        else if (gameObject.layer == 9) // Red �� ����
        {
            dir = Vector2.left;
            enemyLayer = "Blue";
        }

        // RayCast
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(enemyLayer));

        // ������ ������
        if (rayHit.collider != null)
        {
            // �� ������Ʈ ��������
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

            // ���� ������ ���
            if (unitName == UnitName.Sword || unitName == UnitName.Guard || unitName == UnitName.Vampire)
                DirectAttack(enemyLogic);

            // ���Ÿ� ������ ���
            if (unitName == UnitName.Range || unitName == UnitName.Wizard || unitName == UnitName.Sniper || unitName == UnitName.Guitar)
                ShotAttack(enemyLogic.transform);

            // ��ź�� ���
            if (unitName == UnitName.Bomb)
                BombAttack();

            // ������ ���
            if (unitName == UnitName.Noblilty)
            {
                DoStop();
                isFront = true;
            }
        }
        else
        {
            isFront = false;
        }

        // ��ź ����
        if (unitName == UnitName.Bomb)
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
        if (unitState == UnitState.Die)
            return;

        Vector2 dir = Vector2.zero;
        string allyLayer = "";

        if (gameObject.layer == 8)      // Blue �� ����
        {
            dir = Vector2.right;
            allyLayer = "Blue";
        }
        else if (gameObject.layer == 9) // Red �� ����
        {
            dir = Vector2.left;
            allyLayer = "Red";
        }

        // RayCast
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.3f, LayerMask.GetMask(allyLayer));

        // �Ʊ��� ������
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
        // ����
        isFront = true;
        DoStop();

        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer > unitAttackSpeed)
        {
            // �����̾�� ���� �� ü�� ȸ��
            if (unitName == UnitName.Vampire)
            {
                unitHp += unitAtk;
                unitHp = unitHp > 10 ? 10 : unitHp;
            }
            enemyLogic.DoHit(unitAtk);

            anim.SetTrigger("doAttack");
            attackTimer = 0;
        }
    }
    void ShotAttack(Transform enemyTrans)
    {
        // ����
        isFront = true;
        DoStop();

        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAttackSpeed)
            return;

        // Bullet
        GameObject bullet = ObjectManager.instance.bluePrefabs[0];
        int idx = (int)unitName - (int)UnitName.Range;

        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // �ڽ��� �������� �Ѿ� �߻�
            if (unitName == UnitName.Range || unitName == UnitName.Sniper || unitName == UnitName.Guitar)
            {
                bullet = ObjectManager.instance.bluePrefabs[idx];
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            // ��븦 �������� �Ѿ� �߻�
            else if (unitName == UnitName.Wizard)
            {
                bullet = ObjectManager.instance.bluePrefabs[idx];
                Instantiate(bullet, enemyTrans.position, Quaternion.identity);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
        {
            if (unitName == UnitName.Range || unitName == UnitName.Sniper || unitName == UnitName.Guitar)
            {
                bullet = ObjectManager.instance.redPrefabs[idx];
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            else if (unitName == UnitName.Wizard)
            {
                bullet = ObjectManager.instance.redPrefabs[idx];
                Instantiate(bullet, enemyTrans.position, Quaternion.identity);
            }
        }
        // �Ѿ��� ������ �� �ֱ�
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
        // ����
        DoHit(5);

        // ���� ����Ʈ ��������
        GameObject bomb = ObjectManager.instance.bulletPrefabs[0];
        Bomb bombLogic = bomb.GetComponent<Bomb>();
        bombLogic.unitName = unitName;
        bombLogic.layer = gameObject.layer;
        bombLogic.dmg = unitAtk;
        Instantiate(bomb, transform.position, Quaternion.identity);
    }


    void DoMove()
    {
        // �̹� �����̰� ������ ȣ�� X
        if (unitState == UnitState.Move)
            return;

        unitState = UnitState.Move;
        anim.SetTrigger("doMove");
        if (dustObject != null)
            dustObject.Play();
    }
    void DoStop()
    {
        // �ȿ����̰� �ִٸ� ȣ�� X
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
