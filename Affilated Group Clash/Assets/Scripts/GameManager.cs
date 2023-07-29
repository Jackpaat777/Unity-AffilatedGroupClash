using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // �̱��� ���� : �ν��Ͻ��� ������ ������� �ʰ� �ϳ��� �ν��Ͻ��� ����ϱ�

    [Header("---------------[InGame]")]
    public bool isGameLive;
    public float gameTimer;
    public int maxCost;
    public int blueCost;
    public int redCost;
    public float costTimer;
    public TextMeshProUGUI costText;
    public List<GameObject> blueUnitList;
    public List<GameObject> redUnitList;

    [Header("---------------[Base]")]
    public int blueHP;
    public int redHP;
    public Slider blueBaseSlider;
    public Slider redBaseSlider;
    public TextMeshProUGUI blueHpText;
    public TextMeshProUGUI redHpText;
    public TextMeshProUGUI blueHpShadowText;
    public TextMeshProUGUI redHpShadowText;
    public SpriteRenderer blueBaseSpriteRen;
    public SpriteRenderer redBaseSpriteRen;
    public Sprite[] blueBaseSprite;
    public Sprite[] redBaseSprite;
    public GameObject blueDestroyEffect;
    public GameObject redDestroyEffect;

    [Header("---------------[Blue Team Setting]")]
    public string teamBlueName;
    public int startBlueIdx;
    public int groupBlueNum;
    public GameObject[] teamBluePrefabs;
    [Header("---------------[Red Team Setting]")]
    public string teamRedName;
    public int startRedIdx;
    public int groupRedNum;
    public GameObject[] teamRedPrefabs;
    public float spawnTimer;
    public int patternIdx;

    [Header("---------------[UI]")]
    public GameObject gameSet;
    public GameObject[] baseObject;
    public GameObject menuPanel;
    public GameObject selectPanel;
    public GameObject optionPanel;
    public TextMeshProUGUI selectText;
    int selectPageIdx;
    [Header("---------------[Button UI]")]
    public Image[] blueButtonImage;
    public TextMeshProUGUI[] blueTypeText;
    public TextMeshProUGUI[] blueCostText;
    public GameObject lastButton;
    [Header("---------------[Unit Info UI]")]
    public bool isUnitClick;
    public GameObject unitObj;
    public Image unitImage;
    public Slider hpSlider;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atsText;
    public TextMeshProUGUI ranText;
    public TextMeshProUGUI spdText;

    [Header("---------------[Result]")]
    public GameObject overSet;
    public GameObject controlSet;
    public GameObject victoryObj;
    public GameObject defeatObj;

    [Header("---------------[Devil]")]
    public bool isDevilB;
    public bool isDevilR;
    public float devilBTimer;
    public float devilRTimer;
    public bool isDevilBAttack;
    public bool isDevilRAttack;

    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        isGameLive = false;

        // Base
        blueHpText.text = blueHP.ToString();
        blueHpShadowText.text = blueHP.ToString();
        redHpText.text = redHP.ToString();
        redHpShadowText.text = redHP.ToString();
    }

    // ======================================================= �� ���� �Լ�
    public void SelectButton(string tmName)
    {
        if (selectPageIdx == 0)
            BlueTeamSetting(tmName);
        else if (selectPageIdx == 1)
            RedTeamSetting(tmName);

        selectPageIdx = 1;
    }
    public void BackButton()
    {
        selectText.text = "�÷����� �׷��� �������ּ���";
        if (selectPageIdx == 0)
        {
            menuPanel.SetActive(true);
            selectPanel.SetActive(false);
            return;
        }
        selectPageIdx = 0;
    }
    void BlueTeamSetting(string tmName)
    {
        selectText.text = "����� �׷��� �������ּ���";

        teamBlueName = tmName;
        lastButton.SetActive(false);
        switch (teamBlueName)
        {
            case "����A":
                teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 5;
                break;
            case "����B":
                teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                startBlueIdx = 10;
                groupBlueNum = 5;
                break;
            case "����":
                teamBluePrefabs = ObjectManager.instance.juPok_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 6;
                lastButton.SetActive(true);
                break;
            case "����A":
                teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 6;
                lastButton.SetActive(true);
                break;
            case "����B":
                teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                startBlueIdx = 12;
                groupBlueNum = 5;
                break;
            case "V��":
                teamBluePrefabs = ObjectManager.instance.vBand_prefabs;
                startBlueIdx = 0;
                groupBlueNum = 5;
                break;
        }

        // Team Button Setting
        for (int i = startBlueIdx; i < startBlueIdx + groupBlueNum; i++)
        {
            Unit teamUnit = teamBluePrefabs[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = teamUnit.GetComponent<SpriteRenderer>();
            // Image
            blueButtonImage[i - startBlueIdx].sprite = spriteRen.sprite;
            // Type
            TypeTextSetting(blueTypeText[i - startBlueIdx], teamUnit.unitType);
            // Cost
            blueCostText[i - startBlueIdx].text = teamUnit.unitCost.ToString();
        }
    }
    void TypeTextSetting(TextMeshProUGUI text, UnitType typeName)
    {
        switch (typeName)
        {
            case UnitType.Tanker:
                text.text = "��Ŀ";
                text.color = new Color(0, 255, 0);
                break;
            case UnitType.Warrior:
                text.text = "����";
                text.color = new Color(255, 0, 0);
                break;
            case UnitType.Ranger:
                text.text = "����";
                text.color = new Color(0, 200, 255);
                break;
            case UnitType.Buffer:
                text.text = "����";
                text.color = new Color(255, 255, 0);
                break;
            case UnitType.Special:
                text.text = "Ư��";
                text.color = new Color(255, 0, 255);
                break;
        }
    }
    void RedTeamSetting(string enName)
    {
        selectText.text = "�÷����� �׷��� �������ּ���";
        selectPanel.SetActive(false);

        teamRedName = enName;
        switch (teamRedName)
        {
            case "����A":
                teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                startRedIdx = 0;
                groupRedNum = 5;
                break;
            case "����B":
                teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                startRedIdx = 10;
                groupRedNum = 5;
                break;
            case "����":
                teamRedPrefabs = ObjectManager.instance.juPok_prefabs;
                startRedIdx = 0;
                groupRedNum = 6;
                break;
            case "����A":
                teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                startRedIdx = 0;
                groupRedNum = 6;
                break;
            case "����B":
                teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                startRedIdx = 12;
                groupRedNum = 5;
                break;
            case "V��":
                teamRedPrefabs = ObjectManager.instance.vBand_prefabs;
                startRedIdx = 0;
                groupRedNum = 5;
                break;
        }

        // Game Start
        GameStart();
        selectPageIdx = 0;
    }
    void GameStart()
    {
        isGameLive = true;
        // UI ����
        gameSet.SetActive(true);
        baseObject[0].SetActive(true);
        baseObject[1].SetActive(true);
    }

    // ======================================================= �ΰ��� ��ư �Լ�
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // ������ ��ư�� �����ų� ���ʹ�ư�� ���� 2 ���ϱ�
            camSpeed += 3f;
        else if(type == "RightUp" || type == "LeftDown")
            camSpeed -= 3f;
    }
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = teamBluePrefabs[idx];
        Unit unitBLogic = unitB.GetComponent<Unit>();

        // ����ó��
        if (isDevilB && unitBLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost ����
        blueCost -= unitBLogic.unitCost;
        if (blueCost < 0)
        {
            blueCost += unitBLogic.unitCost;
            return;
        }
        // ����
        GetUnitObject(teamBlueName, idx, unitB.transform.position);
        blueUnitList.Add(unitB);
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = teamRedPrefabs[idx];
        Unit unitRLogic = unitR.GetComponent<Unit>();

        // ����ó��
        if (isDevilR && unitRLogic.unitDetail == UnitDetail.Devil)
            return;

        // Cost ����
        redCost -= unitRLogic.unitCost;
        if (redCost < 0)
        {
            redCost += unitRLogic.unitCost;
            return;
        }
        // ����
        GetUnitObject(teamRedName, idx, unitR.transform.position);
        redUnitList.Add(unitR);

        // ���� �Ŀ� �����ε��� ����
        patternIdx = Random.Range(0, 3);
    }
    void GetUnitObject(string teamName, int idx, Vector3 pos)
    {
        switch (teamName)
        {
            case "����A":
            case "����B":
                ObjectManager.instance.GetGiHa(idx, pos);
                break;
            case "����":
                ObjectManager.instance.GetJuPok(idx, pos);
                break;
            case "����A":
            case "����B":
                ObjectManager.instance.GetBakChi(idx, pos);
                break;
            case "V��":
                ObjectManager.instance.GetVBand(idx, pos);
                break;
        }
    }
    public void OptionButton()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OptionOut()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void ResetButton()
    {
        SceneManager.LoadScene("Game");
    }

    void Update()
    {
        if (!isGameLive)
            return;

        // Timer
        gameTimer += Time.deltaTime;

        // Devil
        DevilTimer();
        // Camera Move
        CameraMove();
        // Cost
        CostUp();
        // KeyBoard
        KeyBoard();

        // Loop Pattern
        //StartCoroutine(Pattern(1f, patternIdx));

        // Unit Infomation
        if (isUnitClick)
        {
            Unit unitLogic = unitObj.GetComponent<Unit>();
            UnitInfo(unitLogic);
        }
    }

    void UnitInfo(Unit unitLogic)
    {
        unitImage.gameObject.SetActive(true);

        // Image
        SpriteRenderer spriteRen = unitLogic.GetComponent<SpriteRenderer>();
        unitImage.sprite = spriteRen.sprite;
        // Name
        unitNameText.text = unitLogic.unitName;
        // Hp
        hpSlider.value = (float)unitLogic.unitHp / unitLogic.unitMaxHp;
        hpText.text = $"{unitLogic.unitHp} / {unitLogic.unitMaxHp}";
        // Atk
        atkText.text = $"ATK : {unitLogic.unitAtk}";
        // Atk Speed
        string floatAts = unitLogic.unitAtkSpeed.ToString("F1");
        atsText.text = $"ATS : " + floatAts;
        // Range
        ranText.text = $"RAN : {unitLogic.unitRange}";
        // Speed
        if (unitLogic.unitSpeed > 0)
            spdText.text = $"SPD : {unitLogic.unitSpeed * 10}";
        else
            spdText.text = $"SPD : {unitLogic.unitSpeed * -10}";
    }

    // ======================================================= Update �Լ�
    void DevilTimer()
    {
        if (isDevilB)
        {
            devilBTimer += Time.deltaTime;
            if (devilBTimer > 2.5f)
            {
                isDevilBAttack = true;
                devilBTimer = 0;
            }
            else
                isDevilBAttack = false;
        }
        if (isDevilR)
        {
            devilRTimer += Time.deltaTime;
            if (devilRTimer > 2f)
            {
                isDevilRAttack = true;
                devilRTimer = 0;
            }
            else
                isDevilRAttack = false;
        }
    }
    void CameraMove()
    {
        // ȭ�� ������ �̵��Ϸ��� �ϸ� Move ����
        if ((camSpeed == -3f && camTrans.position.x > -5) || (camSpeed == 3f && camTrans.position.x < 5))
            isMove = true;
        else
            isMove = false;

        if (isMove)
        {
            // ���� ���Ͱ���ŭ �̵�
            Vector3 nextMove = Vector3.right * camSpeed * Time.deltaTime;
            camTrans.Translate(nextMove);
        }
    }
    void CostUp()
    {
        costText.text = blueCost.ToString();

        costTimer += Time.deltaTime;
        if (costTimer > 2f)
        {
            blueCost += 1;
            redCost += 1;
            blueCost = blueCost > maxCost ? maxCost : blueCost;
            redCost = redCost > maxCost ? maxCost : redCost;

            costTimer = 0;
        }
    }
    void KeyBoard()
    {
        // Ű���带 ���� �̵�
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 3f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -3f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // Ű���带 ���� ���� ����
        // 2���� ���������ִ� �׷��� �����鿡�� Blue�� Red�� + Blue�� Red������ ��������
        // teamNum�� ���� Blue/ Red�� �������־�����, startIdx�� �� ���� ������ �������� ����Ͽ���
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(0 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(1 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.D))
            MakeBlueUnit(2 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.F))
            MakeBlueUnit(3 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.G))
            MakeBlueUnit(4 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.H) && groupBlueNum == 6)
            MakeBlueUnit(5 + startBlueIdx);
        // red
        if (Input.GetKeyDown(KeyCode.Z))
            MakeRedUnit(0 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.X))
            MakeRedUnit(1 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.C))
            MakeRedUnit(2 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.V))
            MakeRedUnit(3 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.B))
            MakeRedUnit(4 + startRedIdx + groupRedNum);
        if (Input.GetKeyDown(KeyCode.N) && groupRedNum == 6)
            MakeRedUnit(5 + startRedIdx + groupRedNum);
    }

    // ======================================================= Base �Լ�
    public void BaseHit(int dmg, int layer)
    {
        if (layer == 8)
        {
            // ��������ŭ ����
            blueHP -= dmg;

            // Death
            if (blueHP <= 0)
            {
                blueHP = 0;
                blueBaseSpriteRen.sprite = blueBaseSprite[2];
                blueDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Lose");
            }
            // Alive
            else
            {
                blueBaseSpriteRen.sprite = blueBaseSprite[1];
                StartCoroutine(SpriteChange(0.1f, blueBaseSpriteRen, blueBaseSprite[0]));
            }

            // Text
            blueBaseSlider.value = blueHP;
            blueHpText.text = blueHP.ToString();
            blueHpShadowText.text = blueHP.ToString();
        }
        else if (layer == 9)
        {
            redHP -= dmg;

            if (redHP <= 0)
            {
                redHP = 0;
                redBaseSpriteRen.sprite = redBaseSprite[2];
                redDestroyEffect.SetActive(true);
                GameOver("Win");
            }
            else
            {
                redBaseSpriteRen.sprite = redBaseSprite[1];
                StartCoroutine(SpriteChange(0.1f, redBaseSpriteRen, redBaseSprite[0]));
            }

            redBaseSlider.value = redHP;
            redHpText.text = redHP.ToString();
            redHpShadowText.text = redHP.ToString();
        }
    }
    IEnumerator SpriteChange(float time, SpriteRenderer spriteRen, Sprite sprite)
    {
        yield return new WaitForSeconds(time);

        spriteRen.sprite = sprite;
    }
    void GameOver(string result)
    {
        // �̰��� ���
        if (result == "Win")
        {
            victoryObj.SetActive(true);
        }
        // ���� ���
        else if (result == "Lose")
        {
            defeatObj.SetActive(true);
        }

        // ���� �۵� ���� (�ð�, �ڽ�Ʈ, ī�޶��̵�, ��ȯ��ư, ����Ŭ��)
        isGameLive = false;
        controlSet.SetActive(false);
        overSet.SetActive(true);
    }

    // ======================================================= COM �Լ�
    // ������ �޸����� �̿��� ���� �����ϱ�?
    // �������� ���� �˾Ƽ� ��ȯ�ϱ�?
    IEnumerator Pattern(float time, int typeNum)
    {
        yield return new WaitForSeconds(time);

        // random ������ ����
        int rand = Random.Range(0, 2);
        switch (typeNum)
        {
            // 3�ڽ�Ʈ �̻��� ��, 0�����ְ� 1�������� �����ϰ� ��ȯ
            case 0:
                if (redCost >= 3)
                    MakeRedUnit(rand + startRedIdx + groupRedNum);
                break;
            // 6�ڽ�Ʈ �̻��� ��, rand���� 0�̸� 0������, 1�̸� 2�����ְ� 3�������� �����ϰ� ��ȯ
            case 1:
                if (redCost >= 6)
                {
                    int idx = Random.Range(2, 4);
                    if (rand == 0)
                        StartCoroutine(Pattern(0, 0));
                    else if (rand == 1)
                        MakeRedUnit(idx + startRedIdx + groupRedNum);
                }
                break;
            // 8�ڽ�Ʈ �̻��� ��, rand���� 0�̸� 1������, 1�̸� 4�������� ��ȯ
            case 2:
                if (redCost >= 8)
                {
                    if (rand == 0)
                        StartCoroutine(Pattern(0, 1));
                    else if (rand == 1)
                    {
                        // ���� 6���� ���
                        if (groupRedNum == 6)
                        {
                            int idx = Random.Range(4, 6);
                            MakeRedUnit(idx + startRedIdx + groupRedNum);
                        }
                        // ���� 5���� ���
                        else
                        {
                            MakeRedUnit(4 + startRedIdx + groupRedNum);
                        }
                    }
                }
                break;
        }
    }
}
