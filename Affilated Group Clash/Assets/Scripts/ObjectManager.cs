using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[JiHaDol]")]
    public GameObject[] jiHaA_blue_prefabs;
    public GameObject[] jiHaB_blue_prefabs;
    public GameObject[] jiHaA_red_prefabs;
    public GameObject[] jiHaB_red_prefabs;

    [Header("---------------[JuPok]")]
    public GameObject[] juPok_blue_prefabs;
    public GameObject[] juPok_red_prefabs;

    [Header("---------------[BackChiTheRock]")]
    public GameObject[] bakChi_prefabs;
    // Ǯ ��� ����Ʈ
    List<GameObject>[] bakChi_pools;

    [Header("---------------[V_band]")]
    public GameObject[] vBand_blue_prefabs;
    public GameObject[] vBand_red_prefabs;

    [Header("---------------[Bullet]")]
    public GameObject[] bullet_prefabs;
    //public GameObject[] bulletR_prefabs;
    //public GameObject[] bulletT_prefabs;
    List<GameObject>[] bullet_pools;

    void Awake()
    {
        instance = this;

        // �������� ���̸�ŭ ����Ʈ ����
        bakChi_pools = new List<GameObject>[bakChi_prefabs.Length];
        bullet_pools = new List<GameObject>[bullet_prefabs.Length];

        for (int i = 0; i < bakChi_pools.Length; i++)
        {
            // �� ����Ʈ�� �����ڸ� ���� �ʱ�ȭ
            bakChi_pools[i] = new List<GameObject>();
        }
    }

    public GameObject GetBakChi(int index)
    {
        GameObject select = null;

        // ������ Ǯ�� ��Ȱ��ȭ�� ������Ʈ ����
        foreach (GameObject item in bakChi_pools[index])
        {
            if (!item.activeSelf)
            {
                // �߰��ϸ� select�� �Ҵ�
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // select == null�̶� �ص� ������ �����Ͱ� ���� ��츦 �ǹ��ϴ� !select�� �ᵵ ��
        if (!select)
        {
            // �� ã���� ��� ���Ӱ� �����Ͽ� select�� �Ҵ�
            // Hierarchyâ�� �ƴ� PoolManager �Ʒ��� �Ҵ��ϱ� ���� transform�̶� ����
            select = Instantiate(bakChi_prefabs[index], transform);

            // ������Ʈ�� ���� ���������� Ǯ�� ���
            bakChi_pools[index].Add(select);
        }

        return select;
    }
}
