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

        // ��������Ʈ�� speed���� ������ �������� ����(Rotate�� �ϱ�����)
        if (unitDetail != UnitDetail.Devil)
            rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * speed);

        // ATS�� ����
        if (unitDetail != UnitDetail.AtkspdUp)
        {
            // ȭ�� ������ ���� ���
            if (transform.position.x < -15 || transform.position.x > 13)
                gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (unitDetail == UnitDetail.Drum || unitDetail == UnitDetail.Bomb || unitDetail == UnitDetail.AtkspdUp)
            return;

        // ������ Bullet�� ��� �ٷ� ����
        if (isNotAtk)
        {
            if (unitDetail == UnitDetail.Devil)
            {
                StartCoroutine(DisableRoutine(0.5f));
                return;
            }

            // �̵����� �ʴ� ��� 2�� �ڿ� ����
            if (speed == 0)
                StartCoroutine(DisableRoutine(2f));
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
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, 0.4f, LayerMask.GetMask(enemyLayer));

        // ������ ������
        if (rayHit.collider != null)
        {
            // �� ������Ʈ ��������
            Unit enemyLogic = rayHit.collider.gameObject.GetComponent<Unit>();

            isHit = true;
            enemyLogic.DoHit(dmg);

            // ����� �Ѿ�
            if (isDebuff)
            {
                // Singer�� ������ ���ݷ� ���� (1����)
                if (unitDetail == UnitDetail.Singer && !enemyLogic.isAtkDebuff)
                {
                    enemyLogic.unitAtk -= 5;
                    enemyLogic.isAtkDebuff = true;
                    enemyLogic.atkDebuffTimer = 0;
                }
            }

            // ȭ������ ���������� ���� ���� �ð� ����
            if (speed == 0)
                StartCoroutine(DisableRoutine(3f));
            else
                gameObject.SetActive(false);
        }
    }

    // �������� Ʈ����
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (unitDetail == UnitDetail.Drum)
        {
            // ������ ���
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
            // ������ ���
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
