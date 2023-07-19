using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [Header("---------------[JiHaDol]")]
    public GameObject[] jiHa_prefabs;

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
        bakChi_pools = new List<GameObject>[bakChi_prefabs.Length];
        bullet_pools = new List<GameObject>[bullet_prefabs.Length];

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

    //public GameObject Get(string typeName, int idx, Vector3 pos)
    //{
    //    List<GameObject>[] pools = null;
    //    GameObject[] prefabs = null;
        
    //    switch (typeName)
    //    {
    //        case "����A":
    //        case "����B":
    //            pools = bakChi_pools;
    //            prefabs = bakChi_prefabs;
    //            break;
    //        case "�Ѿ�":
    //            pools = bullet_pools;
    //            prefabs = bullet_prefabs;
    //            break;
    //    }

    //    return GetBakChi(idx, pos);

    //}

    public GameObject GetBakChi(int idx, Vector3 pos)
    {
        GameObject select = null;

        // ������ Ǯ�� ��Ȱ��ȭ�� ������Ʈ ����
        foreach (GameObject item in bakChi_pools[idx])
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
            select = Instantiate(bakChi_prefabs[idx], transform);

            // ������Ʈ�� ���� ���������� Ǯ�� ���
            bakChi_pools[idx].Add(select);
        }

        // ���� ��ġ�� ��ȯ
        select.transform.position = pos;

        return select;
    }

    public GameObject GetBullet(int idx, Vector3 pos)
    {
        GameObject select = null;

        // ������ Ǯ�� ��Ȱ��ȭ�� ������Ʈ ����
        foreach (GameObject item in bullet_pools[idx])
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
            select = Instantiate(bullet_prefabs[idx], transform);

            // ������Ʈ�� ���� ���������� Ǯ�� ���
            bullet_pools[idx].Add(select);
        }

        // ���� ��ġ�� ��ȯ
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


    public void DisableObject(GameObject obj, float time)
    {
        // time�� 0�̸� �ٷ� ��Ȱ��ȭ(��� �ǳ�?)
        if (time == 0)
        {
            obj.SetActive(false);
            return;
        }
        StartCoroutine(DisableRoutine(obj, time));
    }
    IEnumerator DisableRoutine(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        obj.SetActive(false);
    }
}
