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

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        transform.localRotation = Quaternion.identity;
        isHit = false;


        rigid.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * speed);

        // ��Ÿ� ������ ���� ���?

        // ȭ�� ������ ���� ���
        if (transform.position.x < -15 || transform.position.x > 13)
            ObjectManager.instance.DisableObject(gameObject, 0);
    }

    void FixedUpdate()
    {
        if (unitDetail == UnitDetail.Drum || unitDetail == UnitDetail.Bomb)
            return;

        ScanEnemy();
    }

    void ScanEnemy()
    {
        // ������ Bullet�� ��� �ٷ� ����
        if (isNotAtk)
        {
            if (speed == 0)
                ObjectManager.instance.DisableObject(gameObject, 3f);
            else
                ObjectManager.instance.DisableObject(gameObject, 0);
            return;
        }

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

            // ȭ������ ���������� ���� ���� �ð� ����
            if (speed == 0)
                ObjectManager.instance.DisableObject(gameObject, 3f);
            else
                ObjectManager.instance.DisableObject(gameObject, 0);
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

            ObjectManager.instance.DisableObject(gameObject, 1f);
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

            ObjectManager.instance.DisableObject(gameObject, 1f);
        }
    }
}
