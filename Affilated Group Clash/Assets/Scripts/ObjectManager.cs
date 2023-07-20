using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[JiHaDol]")]
    public GameObject[] jiHa_prefabs;
    List<GameObject>[] jiHa_pools;

    [Header("---------------[JuPok]")]
    public GameObject[] juPok_prefabs;

    [Header("---------------[BackChiTheRock]")]
    public GameObject[] bakChi_prefabs;
    // Ǯ ��� ����Ʈ
    List<GameObject>[] bakChi_pools;

    [Header("---------------[V_band]")]
    public GameObject[] vBand_prefabs;

    [Header("---------------[Bullet]")]
    public GameObject[] bullet_prefabs;
    List<GameObject>[] bullet_pools;

    void Awake()
    {
        instance = this;

        // �������� ���̸�ŭ ����Ʈ ����
        jiHa_pools = new List<GameObject>[jiHa_prefabs.Length];
        bakChi_pools = new List<GameObject>[bakChi_prefabs.Length];
        bullet_pools = new List<GameObject>[bullet_prefabs.Length];
        
        for (int i = 0; i < jiHa_pools.Length; i++)
        {
            // �� ����Ʈ�� �����ڸ� ���� �ʱ�ȭ
            jiHa_pools[i] = new List<GameObject>();
        }
        for (int i = 0; i < bakChi_pools.Length; i++)
        {
            // �� ����Ʈ�� �����ڸ� ���� �ʱ�ȭ
            bakChi_pools[i] = new List<GameObject>();
        }
        for (int i = 0; i < bullet_pools.Length; i++)
        {
            // �� ����Ʈ�� �����ڸ� ���� �ʱ�ȭ
            bullet_pools[i] = new List<GameObject>();
        }
    }

    public GameObject GetJiHa(int idx, Vector3 pos)
    {
        GameObject select = null;

        // ������ Ǯ�� ��Ȱ��ȭ�� ������Ʈ ����
        foreach (GameObject item in jiHa_pools[idx])
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
            select = Instantiate(jiHa_prefabs[idx], transform);

            // ������Ʈ�� ���� ���������� Ǯ�� ���
            jiHa_pools[idx].Add(select);
        }

        // ���� ��ġ�� ��ȯ
        select.transform.position = pos;

        return select;
    }

    public GameObject GetBakChi(int idx, Vector3 pos)
    {
        GameObject select = null;

        foreach (GameObject item in bakChi_pools[idx])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(bakChi_prefabs[idx], transform);
            bakChi_pools[idx].Add(select);
        }

        select.transform.position = pos;

        return select;
    }

    public GameObject GetBullet(int idx, Vector3 pos)
    {
        GameObject select = null;
        foreach (GameObject item in bullet_pools[idx])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(bullet_prefabs[idx], transform);
            bullet_pools[idx].Add(select);
        }

        select.transform.position = pos;

        return select;
    }

    public void BulletSetting(GameObject bulletObj, UnitDetail uDetail, int uLayer, int uDmg)
    {
        Bullet bulletLogic = bulletObj.GetComponent<Bullet>();

        bulletLogic.unitDetail = uDetail;
        bulletLogic.layer = uLayer;
        bulletLogic.dmg = uDmg;
    }
}
