using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using Unity.VisualScripting;

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

    [Header("---------------[Book]")]
    public GameObject[] groupPrefabsInBook;
    public int startIdxInBook;
    public int groupNumInBook;
    public GameObject lastButtonInBook;
    public Image[] buttonImageInBook;
    public TextMeshProUGUI[] buttonNameInBook;
    public int infoPageIdx;
    public string groupNameInInfo;
    public GameObject unitInfoSet;
    public Image infoImage;
    public TextMeshProUGUI infoUnitNameText;
    public TextMeshProUGUI infoUnitTypeText;
    public TextMeshProUGUI infoCostText;
    public TextMeshProUGUI infoHpText;
    public TextMeshProUGUI infoAtkText;
    public TextMeshProUGUI infoAtsText;
    public TextMeshProUGUI infoRanText;
    public TextMeshProUGUI infoSpdText;
    public TextMeshProUGUI infoSkillText;
    public TextMeshProUGUI infoDetailText;
    public Sprite[] unitSprites;
    float scaleNum;

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
                teamBluePrefabs = ObjectManager.instance.juFok_prefabs;
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
                teamRedPrefabs = ObjectManager.instance.juFok_prefabs;
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
                ObjectManager.instance.GetJuFok(idx, pos);
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
        Time.timeScale = 1;
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
        if (Input.GetKeyDown(KeyCode.Q))
            MakeBlueUnit(0 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.W))
            MakeBlueUnit(1 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.E))
            MakeBlueUnit(2 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(3 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(4 + startBlueIdx);
        if (Input.GetKeyDown(KeyCode.D) && groupBlueNum == 6)
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

    // ======================================================= Book �Լ�
    public void TeamSelectInBook(string teamName)
    {
        groupNameInInfo = teamName;
        switch (teamName)
        {
            case "����A":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
            case "����B":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 10;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
            case "����":
                groupPrefabsInBook = ObjectManager.instance.juFok_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                lastButtonInBook.SetActive(true);
                break;
            case "����A":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                lastButtonInBook.SetActive(true);
                break;
            case "����B":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 12;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
            case "V��":
                groupPrefabsInBook = ObjectManager.instance.vBand_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                lastButtonInBook.SetActive(false);
                break;
        }

        for (int i= startIdxInBook; i< startIdxInBook + groupNumInBook; i++)
        {
            Unit bookUnit = groupPrefabsInBook[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = bookUnit.GetComponent<SpriteRenderer>();
            buttonImageInBook[i - startIdxInBook].sprite = spriteRen.sprite;
            buttonNameInBook[i - startIdxInBook].text = bookUnit.unitName;
        }

    }
    public void UnitInfoButton(int idx)
    {
        infoPageIdx = idx;
        // groupPrefabsInBook�� �̸� ������
        Unit infoUnit = groupPrefabsInBook[startIdxInBook + infoPageIdx].GetComponent<Unit>();
        SpriteRenderer spriteRen = infoUnit.GetComponent<SpriteRenderer>();

        // Unit ���� ������Ʈ
        //infoImage.sprite = spriteRen.sprite;
        infoImage.sprite = UnitImage(idx);
        infoImage.SetNativeSize();
        infoImage.transform.localScale = Vector3.one * scaleNum;
        infoUnitNameText.text = infoUnit.unitName;
        TypeTextSetting(infoUnitTypeText, infoUnit.unitType);
        infoCostText.text = $"���� : {infoUnit.unitCost}";
        infoHpText.text = $"ü�� : {infoUnit.unitMaxHp}";
        infoAtkText.text = $"���� : {infoUnit.unitAtk}";
        infoAtsText.text = $"���ݼӵ� : {infoUnit.unitAtkSpeed}";
        infoRanText.text = $"���ݹ��� : {infoUnit.unitRange}";
        infoSpdText.text = $"�̵��ӵ� : {infoUnit.unitSpeed * 10}";
        infoSkillText.text = UnitSkillText(infoPageIdx);
        infoDetailText.text = UnitDetailText(infoPageIdx);

        unitInfoSet.SetActive(true);
    }
    Sprite UnitImage(int idx)
    {
        Sprite image = null;

        switch (groupNameInInfo)
        {
            case "����A":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[0];
                        break;
                    case 1:
                        image = unitSprites[1];
                        break;
                    case 2:
                        image = unitSprites[2];
                        break;
                    case 3:
                        image = unitSprites[3];
                        break;
                    case 4:
                        image = unitSprites[4];
                        break;
                }
                scaleNum = 0.18f;
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[5];
                        break;
                    case 1:
                        image = unitSprites[6];
                        break;
                    case 2:
                        image = unitSprites[7];
                        break;
                    case 3:
                        image = unitSprites[8];
                        break;
                    case 4:
                        image = unitSprites[9];
                        break;
                }
                scaleNum = 0.18f;
                break;
            case "����":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[10];
                        break;
                    case 1:
                        image = unitSprites[11];
                        break;
                    case 2:
                        image = unitSprites[12];
                        break;
                    case 3:
                        image = unitSprites[13];
                        break;
                    case 4:
                        image = unitSprites[14];
                        break;
                    case 5:
                        image = unitSprites[15];
                        break;
                }
                scaleNum = 0.5f;
                break;
            case "����A":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[16];
                        break;
                    case 1:
                        image = unitSprites[17];
                        break;
                    case 2:
                        image = unitSprites[18];
                        break;
                    case 3:
                        image = unitSprites[19];
                        break;
                    case 4:
                        image = unitSprites[20];
                        break;
                    case 5:
                        image = unitSprites[21];
                        break;
                }
                scaleNum = 0.25f;
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[22];
                        break;
                    case 1:
                        image = unitSprites[23];
                        break;
                    case 2:
                        image = unitSprites[24];
                        break;
                    case 3:
                        image = unitSprites[25];
                        break;
                    case 4:
                        image = unitSprites[26];
                        break;
                }
                scaleNum = 0.25f;
                break;
            case "V��":
                switch (idx)
                {
                    case 0:
                        image = unitSprites[27];
                        break;
                    case 1:
                        image = unitSprites[28];
                        break;
                    case 2:
                        image = unitSprites[29];
                        break;
                    case 3:
                        image = unitSprites[30];
                        break;
                    case 4:
                        image = unitSprites[31];
                        break;
                }
                scaleNum = 0.5f;
                break;
        }
        return image;
    }
    string UnitSkillText(int idx)
    {
        string skillText = "";

        switch (groupNameInInfo)
        {
            case "����A":
                switch (idx)
                {
                    case 0:
                        skillText = "�Ϲ����� ��Ŀ.";
                        break;
                    case 1:
                        skillText = "���ݿ� ���ߴ��� �� 2�ʰ� ���ݷ� 5 ����.\n(��ø�Ұ�)";
                        break;
                    case 2:
                        skillText = "HP�� ���������� ���ݼӵ� ����.\n(�ִ�ġ : 0.3)";
                        break;
                    case 3:
                        skillText = "���� ���� �Ʊ� ��ü���� ���ݼӵ� 2�� ����.\n(��ø�Ұ�)";
                        break;
                    case 4:
                        skillText = "�� ��ü���� ���ظ� ����.\n(���ݼӵ� ����)\n(�ߺ� ��ȯ �Ұ�)";
                        break;
                }
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        skillText = "�Ϲ����� ����.";
                        break;
                    case 1:
                        skillText = "������ ���ع��� ������ ���� ���ݷ��� ���ݸ�ŭ ���ظ� �ǵ�����.\n(���� ���� �ش�)";
                        break;
                    case 2:
                        skillText = "���ݿ� ���ߴ��� �� 2�ʰ� ���ݼӵ� 2�� ����.\n(��ø�Ұ�)";
                        break;
                    case 3:
                        skillText = "���� �� �Ʊ� �Ѹ��� 10��ŭ ��.";
                        break;
                    case 4:
                        skillText = "���� ų�ϸ� �ִ�ü�� 10, ���ݷ� 5, ���ݼӵ� 0.1 ����\n(���ݼӵ� �ִ�ġ : 0.3)";
                        break;
                }
                break;
            case "����":
                switch (idx)
                {
                    case 0:
                        skillText = "�Ϲ����� ����.";
                        break;
                    case 1:
                        skillText = "���� �� �Ʊ� ��ü���� �̵��ӵ� 10 ����.\n(��ø�Ұ�)";
                        break;
                    case 2:
                        skillText = "�ڽ��� �޴� ��� �ǰݵ����� 3 ����.";
                        break;
                    case 3:
                        skillText = "���� ���Ÿ� ����.";
                        break;
                    case 4:
                        skillText = "���� ���ΰ� ������������ ���� �齺��.";
                        break;
                    case 5:
                        skillText = "������ ó�� �����ϱ� ������ ����.";
                        break;
                }
                break;
            case "����A":
                switch (idx)
                {
                    case 0:
                        skillText = "�Ϲ����� ��Ŀ.";
                        break;
                    case 1:
                        skillText = "���� ���� ����.";
                        break;
                    case 2:
                        skillText = "3.5�ʸ��� 1���ξ� ����.";
                        break;
                    case 3:
                        skillText = "���� �� ������ ���ݷ¸�ŭ ü�� ����.";
                        break;
                    case 4:
                        skillText = "�Ϲ����� ����.\n(���� �� ��Ÿ� ����)";
                        break;
                    case 5:
                        skillText = "���� ���� ���� ���� �ִٸ� ������ ������ �̵� �� �������� ����.";
                        break;
                }
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        skillText = "�Ϲ����� ��Ŀ.";
                        break;
                    case 1:
                        skillText = "3��° ���ݸ��� �߰� ������ +5.";
                        break;
                    case 2:
                        skillText = "������ ������ ���ݼӵ� ����.\n(�ִ� 0.3)\n(�̵� �� �ʱ�ȭ)";
                        break;
                    case 3:
                        skillText = "���� �� �Ʊ� ��ü���� ���ݷ� 5 ����.\n(��ø�Ұ�)";
                        break;
                    case 4:
                        skillText = "���� �� ü�� 50���� ���� Ȯ�� ų.";
                        break;
                }
                break;
            case "V��":
                switch (idx)
                {
                    case 0:
                        skillText = "�Ϲ����� ����.";
                        break;
                    case 1:
                        skillText = "���� �� ���� ���ĳ�.";
                        break;
                    case 2:
                        skillText = "���ݿ� ���ߴ��� �� 2�ʰ� �̵��ӵ� 4 ����.\n(��ø�Ұ�)";
                        break;
                    case 3:
                        skillText = "���� �� ���Ÿ� ���� ��ȿ.";
                        break;
                    case 4:
                        skillText = "���Ÿ����ָ� ���� ����. ���������� ������ �� ����. �ǰݴ��ص� ������ ����.";
                        break;
                }
                break;
        }

        return skillText;
    }
    string UnitDetailText(int idx)
    {
        string detailText = "";

        switch (groupNameInInfo)
        {
            case "����A":
                switch (idx)
                {
                    case 0:
                        detailText = "�Ӹ��� ��Ʈ�ø��� �޸� ����. ������ �ΰ��� �ƴϰ� �ڵ����̶�� �Ѵ�. Ư�̻����� ���Ҷ����� �ڵ��� ȭ���� ������ ���.";
                        break;
                    case 1:
                        detailText = "���ϵ��� ������ ���� ������ ���κ���. Ư���� �߼����п� ����� �̸��� ���߽�Ų��. ���� �ϿͿ� ������� �´ٰ� �Ѵ�.";
                        break;
                    case 2:
                        detailText = "���ؾ��� �پ���� ����̸�� ����� �������� ����� �������� �Ҹ���. ���� �����ϴ� ��������� �ֺ� ������� ���� �Ѵ�.";
                        break;
                    case 3:
                        detailText = "������ ���� ���ϵ����� �Ϲ������� Ư���ϰ� �հ��ߴ�. ��ο��� ����ϸ� ����ϴ� ������ ������� �ŷ��� �˳��� �ִ�.";
                        break;
                    case 4:
                        detailText = "��Ī ���迡�� �� ���մ����� Ư���� ������ ��糢�� �ִ�. ������ ������ ���� �ʴ� ������� ���ָ� ������.";
                        break;
                }
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        detailText = "����ƽγ�. ���� ������� ������ �� ������ϸ� ü���� ���� �ʴ�. �׻� ������Ʈ��� ��ѱ�� �Բ� �ٴѴ�.";
                        break;
                    case 1:
                        detailText = "��߳����� ��߰���. �׳ุ�� �＾ ������ �˾Ƶ�� ���� ���� Ư¡. �������� �پ�� ��Ŀ���� �޾�ġ�� �ɷ��� ����.";
                        break;
                    case 2:
                        detailText = "�ſ� �������� ���� 100% â���� ������ �ִ�. ������ ���� ���� ���� ���ϰ� ���ϴ� �� ���� ������ �ִ�.";
                        break;
                    case 3:
                        detailText = "���ѿܱ������� ������� �ѱ�� �� �����Ѵ�. ��ο��� ���� ����� �Ҿ�ִ� ġ���迭 ����̸� �������� Ű�� ũ��.";
                        break;
                    case 4:
                        detailText = "�����ΰ� �ᵥ�� ��ġ. ��ҿ��� �����ϰ� �ҽ��� ��������� �����ϸ� �ſ� ������ �ᵥ���� ����� ���̴� Ư¡�� �ִ�.";
                        break;
                }
                break;
            case "����":
                switch (idx)
                {
                    case 0:
                        detailText = "�����ҳ���� ����. �������̸� Ȱ���� ���ݴ��п� �������� �����ϰų� ���� Ȱ���� �����ϴ� ����� ���δ�.";
                        break;
                    case 1:
                        detailText = "���ϳ� �迭�� ���̵������� ���� ������ ���� Ư¡. ����� ���� ���ڵ��� �������� �븮�� �ִٴ� �ҹ��� �����Ѵ�.";
                        break;
                    case 2:
                        detailText = "��Ȱ�� �л�ȸ�� �Ӽ��� ���� �� \"��������\"�ϸ� ȣ���ϰ� ���� ����� Ư¡�̴�. �ǿܷ� ī���� ������ �ִ� �� ����.";
                        break;
                    case 3:
                        detailText = "���� ������ ����̼ҳ�. ������ ���ؼ� ���ڱ� �������⵵ ������ �̷� ������� �ҵ鿡�Լ� �𼺾ָ� �ڱؽ�Ų��.";
                        break;
                    case 4:
                        detailText = "�翬�� ���ƺ��̴� ����. ���ڰ� �ٰ����� �ҽ���ġ�� ���� ����ģ��. �䷷�� ���׸� ġ�ų� ���� �̻��� ���� �� ���� �ִ�.";
                        break;
                    case 5:
                        detailText = "���������� �ڻ��� ��Ҹ��� ���� ������ Ȧ���� ����� �����ش�. ����� �����̽��� ���� ��� Ȱ���� �ߴ��� �����̴�.";
                        break;
                }
                break;
            case "����A":
                switch (idx)
                {
                    case 0:
                        detailText = "����� ���� ������ ������ ������ ������ ���� �ҳ�. ������ ���� ����� ���� ������ ������ ��ī�ο� ��ħ�� �����⵵ �Ѵ�.";
                        break;
                    case 1:
                        detailText = "��ҿ��� �����ϸ� �ҽ������� �����δ� �ٷ°� ü���� �ſ� ���ϴ�. ���� ������ ���� ������ ��ĥ������ ����� ���δ�.";
                        break;
                    case 2:
                        detailText = "����� �ư���. �縳�б��� ��̰� ��� ģ������ ����� ���� ���пԴ�. ���� �������н����� ��� ���̱⵵ �Ѵ�.";
                        break;
                    case 3:
                        detailText = "��������� ���� ���������� ������ �� ó���� �ʿ��ϴ�. ������ ����ϰ� ǥ���ϴ� �Ͱ� ����� �������̶�� Ư¡�� �ִ�.";
                        break;
                    case 4:
                        detailText = "������ ������Ƹ��ÿ´��긮������. ������ �����̸� ������ \"���\"�� ���̴� Ư���� ������ ����. ���� ������ �� ���.";
                        break;
                    case 5:
                        detailText = "�����ϰ� �������ִ� ��Ҹ��� ������ ������ �Ѽ����� ������ ���� �ִ�. ��Ÿ���� ���� ��Ƽ��Ʈ.";
                        break;
                }
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        detailText = "���� 8���� ������ ���л� ���/����. ������ �������� �����ִ� ������ �ൿ�� Ư¡�̴�. �׻� ü���� ������ ������Ѵ�.";
                        break;
                    case 1:
                        detailText = "���濡�� ����� �ð�ҳ�� ������ �������� ����. ��Ī ����ȣ���������� �ƹ��� ���������� �ʴ´�.";
                        break;
                    case 2:
                        detailText = "Say-No Crystal(�ٿ��� ũ¯)�̶�� �̸��� ��Ÿ�� ���� �ҳ�. �Ϳ��� �ڿ��� �Ŀ�Ǯ�� ���ֽǷ��� ������ �ִ�.";
                        break;
                    case 3:
                        detailText = "��ġó�� �ҽ������� �ǿܷ� �Ҹ��� �ϴ� �����̴�. �ν�ó�� ���̰� �ʹٴ� ������ ����� �����ϰ� �ٴѴ�.";
                        break;
                    case 4:
                        detailText = "��°� �����ϴ� �ν� �����. SNS�� ����ϸ� ���� �����Ѵ�. �뷡�� �����ϴ� �� ������ ���ϴ� ���� �ƴ� �� ����.";
                        break;
                }
                break;
            case "V��":
                switch (idx)
                {
                    case 0:
                        detailText = "���߳����� ��Ÿ����Ʈ. �ǿ����ǰ��� �������� �پ �Ƿ¶����� V�޹�忡 ��������� ���� �ƴϳĴ� ���� ���� ��´�.";
                        break;
                    case 1:
                        detailText = "ĳ���ٿ��� �� �巯��. �ؿ��Ĵ�� ����Ƿ� ����. ȣ����� ������ ���� ƥ�� �𸣴� �ŷ��� ������ �ִ�.";
                        break;
                    case 2:
                        detailText = "���溣(��î�� ����� ���̽ý�Ʈ). �̵� ���� �� ������ V�޹�忡�� ��Ÿ�ΰ��� Ȱ�⸦ �־��ִ� ������ �Ѵ�.";
                        break;
                    case 3:
                        detailText = "������ �ʺ� �ǾƴϽ�Ʈ. �Ƿ��� �ο����� ���� ������ ����ϸ� �Ƿ��� Ű��� �ִ�. �̸����� ������̶�� �Ҹ��� �ִ�.";
                        break;
                    case 4:
                        detailText = "V�޹���� ������Ʈ. ��������� �ſ� �ȶ��ϸ� ������� �Ѵٴ� �ҹ��� �ִ�. ������ ���ݶ����� ���ڱ� �ٰ����� ��������.";
                        break;
                }
                break;
        }

        return detailText;
    }
    public void LeftInfoButton()
    {
        // �ε��� ����
        infoPageIdx--;
        // 0���� �۾��� ��� �׷��� �ִ밪-1 ���� ����
        infoPageIdx = infoPageIdx < 0 ? groupNumInBook - 1 : infoPageIdx;
        // ���� ������Ʈ
        UnitInfoButton(infoPageIdx);
    }
    public void RightInfoButton()
    {
        // �ε��� ����
        infoPageIdx++;
        // �׷��� �ִ밪-1���� Ŀ�� ��� 0 ���� ����
        infoPageIdx = infoPageIdx > groupNumInBook - 1 ? 0 : infoPageIdx;
        // ���� ������Ʈ
        UnitInfoButton(infoPageIdx);
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
            // 9�ڽ�Ʈ �̻��� ��, rand���� 0�̸� 1������, 1�̸� 4�������� ��ȯ
            case 2:
                if (redCost >= 9)
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

    IEnumerator PatternBA(float time, int typeNum)
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
            // 9�ڽ�Ʈ �̻��� ��, rand���� 0�̸� 1������, 1�̸� 4�������� ��ȯ
            case 2:
                if (redCost >= 9)
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
