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
        //UnitSetting();

        // ���� ����
        isFront = false;
        DoMove();
    }
    void UnitSetting()
    {
        // Attack Speed�� �������� ����
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

        // Red �� �����̸� -1 �����ֱ�
        if (gameObject.layer == 9)
            unitSpeed *= -1;
    }


    void Update()
    {
        // ����
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

    // ======================================================= Ÿ�� ���� �Լ�
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

            // ���� ������ ��� (Unit ���� �ʿ�)
            if (unitType == UnitType.Warrior || unitType == UnitType.Tanker)
                DirectAttack(enemyLogic);

            // ���Ÿ� ������ ��� (Transform ���� �ʿ�)
            else if (unitType == UnitType.Ranger)
                ShotAttack(enemyLogic.transform);

            // ������ ���
            else if (unitType == UnitType.Buffer)
            {
                // ���� (����X)
                DoStop();
                isFront = true;
            }

            // ��ź�� ���
            else if (unitDetail == UnitDetail.Bomb)
                BombAttack();
        }
        else
        {
            isFront = false;
        }

        // ��ź ����
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
        // ���۰� �ƴϸ� �Ʊ�üũ X
        if (unitState == UnitState.Die || unitType != UnitType.Buffer)
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
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, unitRange, LayerMask.GetMask(allyLayer));

        // �Ʊ��� ������
        if (rayHit.collider != null)
        {
            // �Ʊ� ������Ʈ ��������
            Unit allyLogic = rayHit.collider.gameObject.GetComponent<Unit>();
            
            if (unitDetail == UnitDetail.Book)
            {
                // State�� Move�� �ƴҶ� ���� -> Move�� �ɶ����� ������ϴٰ� �ѹ��� ��
                // Ǯ�Ǹ� ���� X
                if (allyLogic.unitHp < allyLogic.unitMaxHp)
                    DoBuff(allyLogic);
            }
        }
        else
        {
            isFront = false;
        }
    }

    // ======================================================= ���� �Լ�
    void DirectAttack(Unit enemyLogic)
    {
        // ����
        isFront = true;
        DoStop();

        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer > unitAtkSpeed)
        {
            // �����̾�� ���� �� ü�� ȸ��
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
        // ����
        isFront = true;
        DoStop();

        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Bullet
        GameObject bullet = ObjectManager.instance.bulletBPrefabs[0];
        int idx = (int)unitDetail - (int)UnitDetail.Archer;

        if (gameObject.layer == 8)          // ��� �� ������ ���
        {
            // �ڽ��� �������� �Ѿ� �߻�
            if (unitDetail == UnitDetail.Archer || unitDetail == UnitDetail.Sniper || unitDetail == UnitDetail.Farmer || unitDetail == UnitDetail.Guitar)
            {
                bullet = ObjectManager.instance.bulletBPrefabs[idx];
                Instantiate(bullet, transform.position + Vector3.up * 0.3f, Quaternion.identity);
            }
            // Ÿ�ٸ� �������� �Ѿ� �߻�
            else if (unitDetail == UnitDetail.Wizard || unitDetail == UnitDetail.Book)
            {
                bullet = ObjectManager.instance.bulletBPrefabs[idx];
                Instantiate(bullet, targetTrans.position, Quaternion.identity);
            }
        }
        else if (gameObject.layer == 9)     // ���� �� ������ ���
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
        // �Ѿ˿� �� �ֱ�
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
        DoHit(unitMaxHp);

        // ���� ����Ʈ ��������
        GameObject bomb = ObjectManager.instance.bulletTPrefabs[0];
        Bullet bombLogic = bomb.GetComponent<Bullet>();
        bombLogic.unitDetail = unitDetail;
        bombLogic.layer = gameObject.layer;
        bombLogic.dmg = unitAtk;
        Instantiate(bomb, transform.position, Quaternion.identity);
    }

    // ======================================================= ���� �Լ�
    public void DoBuff(Unit allyLogic)
    {
        // ���ݼӵ��� ���� ����
        attackTimer += Time.deltaTime;
        if (attackTimer < unitAtkSpeed)
            return;

        // Bullet
        string type = "";
        float value = 0f;
        //int idx = (int)unitDetail - (int)UnitDetail.Archer;

        //if (gameObject.layer == 8)          // ��� �� ������ ���
        //{
        //    // Ÿ�ٸ� �������� �߻�
        //    if (unitDetail == UnitDetail.Book)
        //    {
        //        type = "ATK";
        //        value = 1;
        //        GameObject bullet = ObjectManager.instance.bulletTPrefabs[1];
        //        Instantiate(bullet, allyLogic.transform.position, Quaternion.identity);
        //    }
        //}
        //else if (gameObject.layer == 9)     // ���� �� ������ ���
        //{
        //    if (unitDetail == UnitDetail.Book)
        //    {
        //        type = "HP";
        //        value = 1;
        //        GameObject bullet = ObjectManager.instance.bulletTPrefabs[1];
        //        Instantiate(bullet, allyLogic.transform.position, Quaternion.identity);
        //    }
        //}

        // Ÿ�ٸ� �������� �߻�
        if (unitDetail == UnitDetail.Book)
        {
            type = "HP";
            value = 1;
            GameObject bullet = ObjectManager.instance.bulletTPrefabs[1];
            Instantiate(bullet, allyLogic.transform.position, Quaternion.identity);
        }
        // ���� �ߵ�
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

    // ======================================================= ���� �Լ�
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
