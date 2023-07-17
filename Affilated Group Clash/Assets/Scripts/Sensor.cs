using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Sensor : MonoBehaviour
{
    public UnitDetail unitDetail;
    public int layer;

    Unit parent;
    CircleCollider2D col;

    void Awake()
    {
        parent = GetComponentInParent<Unit>();
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        // ���� �� ����
        unitDetail = parent.unitDetail;
        layer = parent.gameObject.layer;
        col.radius = parent.unitRange;
    }

    // �Ʊ� ���� Ʈ���� (�Ѹ��� ����)
    // ������ �ѹ��� �����Ϸ��� Unit���� ��������
    void OnTriggerStay2D(Collider2D collision)
    {
        if (unitDetail == UnitDetail.Book)
        {
            // �Ʊ��� ���
            if ((layer == 8 && collision.gameObject.layer == 8) || (layer == 9 && collision.gameObject.layer == 9))
            {
                Unit unitLogic = collision.GetComponent<Unit>();
                if (unitLogic.unitHp < unitLogic.unitMaxHp)
                {
                    parent.DoBuff(unitLogic);
                }
            }
        }
    }
}