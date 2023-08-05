using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("---------------[InGame]")]
    public bool isGameLive;
    public int blueMaxCost;
    public int redMaxCost;
    public int blueCost;
    public int redCost;
    public int patternIdx;
    public float gameTimer;
    public float costBTimer;
    public float costRTimer;
    public float costBlueUp;
    public float costRedUp;
    public float redUpgradeTime;
    public float spawnTimer;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI costText;
    public List<GameObject> blueUnitList;
    public List<GameObject> redUnitList;

    [Header("---------------[Base]")]
    public int baseBLevel;
    public int baseRLevel;
    public int blueHP;
    public int redHP;
    public Unit baseB;
    public Unit baseR;
    public Sprite[] baseBSprite;
    public Sprite[] baseRSprite;
    public SpriteRenderer baseBSpriteRen;
    public SpriteRenderer baseRSpriteRen;
    public Slider baseBSlider;
    public Slider baseRSlider;
    public TextMeshProUGUI blueHpText;
    public TextMeshProUGUI redHpText;
    public TextMeshProUGUI blueHpShadowText;
    public TextMeshProUGUI redHpShadowText;
    public TextMeshProUGUI blueUpgradeCostText;
    public TextMeshProUGUI upgradeText;
    public GameObject blueDestroyEffect;
    public GameObject redDestroyEffect;
    public GameObject[] blueUpgradeyEffect;
    public GameObject[] redUpgradeyEffect;


    [Header("---------------[Top UI]")]
    public Animator fadeAc;
    public GameObject optionPanel;
    public TextMeshProUGUI levelText;
    public Image blueLogo;
    public Image redLogo;
    public Sprite[] logoSprites;
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
    public GameObject nextBtn;
    public GameObject resetBtn;

    [Header("---------------[Devil]")]
    public bool isDevilB;
    public bool isDevilR;
    public float devilBTimer;
    public float devilRTimer;
    public bool isDevilBAttack;
    public bool isDevilRAttack;

    [Header("---------------[Description]")]
    public int descriptionPage;
    public Button optionButton;
    public TextMeshProUGUI descriptionText;
    public GameObject descriptionPanel;
    public GameObject desPrevButton;
    public GameObject desNextButton;
    public GameObject[] descriptionRect;


    [Header("---------------[Camera]")]
    public Transform camTrans;
    public float camSpeed;
    public bool isMove;

    void Awake()
    {
        instance = this;
        isGameLive = true;

        // Fade In
        fadeAc.SetTrigger("fadeIn");
        // Fade ���� �� ���Ӽ��� ����
        StartCoroutine(DisableFade(0.5f));

        // Game Setting
        GameSetting();
    }
    void GameSetting()
    {
        // BlueTeam Button Setting
        for (int i = Variables.startBlueIdx; i < Variables.startBlueIdx + Variables.groupBlueNum; i++)
        {
            Unit teamUnit = Variables.teamBluePrefabs[i].GetComponent<Unit>();
            SpriteRenderer spriteRen = teamUnit.GetComponent<SpriteRenderer>();
            // Image
            blueButtonImage[i - Variables.startBlueIdx].sprite = spriteRen.sprite;
            // Type
            TypeTextSetting(blueTypeText[i - Variables.startBlueIdx], teamUnit.unitType);
            // Cost
            blueCostText[i - Variables.startBlueIdx].text = teamUnit.unitCost.ToString();
        }

        // 5�����̸� ��������ư ����
        if (Variables.groupBlueNum == 5)
            lastButton.SetActive(false);

        // Base
        baseBSpriteRen.gameObject.SetActive(true);
        baseRSpriteRen.gameObject.SetActive(true);
        blueHpText.text = blueHP.ToString();
        redHpText.text = redHP.ToString();
        blueHpShadowText.text = blueHP.ToString();
        redHpShadowText.text = redHP.ToString();

        // RedTeam Level
        switch (Variables.gameLevel)
        {
            case 0:
                // �ڽ�Ʈ ���� �ð�
                costRedUp = 3.5f;
                // ���׷��̵� �ð�
                redUpgradeTime = 120;
                // ���ʿ� �Ѹ� ��������
                spawnTimer = 3f;
                // ���� �ؽ�Ʈ ����
                levelText.text = "�ſ콬��";
                break;
            case 1:
                costRedUp = 3f;
                redUpgradeTime = 90;
                spawnTimer = 2f;
                levelText.text = "����";
                break;
            case 2:
                costRedUp = 2.5f;
                redUpgradeTime = 60;
                spawnTimer = 1f;
                levelText.text = "����";
                break;
            case 3:
                costRedUp = 2f;
                redUpgradeTime = 50;
                spawnTimer = 0.75f;
                levelText.text = "�����";
                break;
            case 4:
                costRedUp = 1.75f;
                redUpgradeTime = 30;
                spawnTimer = 0.75f;
                levelText.text = "�ſ�����";
                break;
        }

        // Logo
        blueLogo.sprite = logoSprites[Variables.teamBlueNum];
        redLogo.sprite = logoSprites[Variables.teamRedNum];

        // BGM
        SoundManager.instance.BgmPlay("Game");
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
    IEnumerator DisableFade(float time)
    {
        yield return new WaitForSeconds(time);

        fadeAc.gameObject.SetActive(false);

        // ���� ���� ����
        if (!Variables.isFirstGame)
        {
            DescriptionButton();
            Variables.isFirstGame = true;
        }
    }

    void Update()
    {
        if (!isGameLive)
            return;

        // Timer
        GameTimer();
        // Devil
        DevilTimer();
        // Camera Move
        CameraMove();
        // Red Upgrade;
        RedUpgrade();
        // Cost
        BlueCostUp();
        RedCostUp();
        // KeyBoard
        KeyBoard();
        // Unit Infomation
        UnitInfo();

        // Loop Pattern
        StartCoroutine(Pattern(1f, patternIdx));
    }
    // ======================================================= Update �Լ�
    void GameTimer()
    {
        gameTimer += Time.deltaTime;
        timeText.text = ((int)gameTimer / 60).ToString("D2") + ":" + ((int)gameTimer % 60).ToString("D2");
    }
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
        if ((camSpeed == -4f && camTrans.position.x > -6) || (camSpeed == 4f && camTrans.position.x < 6))
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
    void RedUpgrade()
    {
        // ���׷��̵�
        if (gameTimer > redUpgradeTime &&  baseRLevel == 0)
            UpgradeRed();
        else if (gameTimer > redUpgradeTime * 2.5f && baseRLevel == 1)
            UpgradeRed();
    }
    void BlueCostUp()
    {
        costText.text = $"{blueCost}/{blueMaxCost}";

        costBTimer += Time.deltaTime;
        if (costBTimer > costBlueUp)
        {
            blueCost += 1;
            blueCost = blueCost > blueMaxCost ? blueMaxCost : blueCost;

            costBTimer = 0;
        }
    }
    void RedCostUp()
    {
        costRTimer += Time.deltaTime;
        if (costRTimer > costRedUp)
        {
            redCost += 1;
            redCost = redCost > redMaxCost ? redMaxCost : redCost;

            costRTimer = 0;
        }
    }
    void KeyBoard()
    {
        // Ű���带 ���� �̵�
        if (Input.GetKey(KeyCode.RightArrow))
            camSpeed = 4f;
        if (Input.GetKey(KeyCode.LeftArrow))
            camSpeed = -4f;
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            camSpeed = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            camSpeed = 0;

        // Ű���带 ���� ���� ����
        // 2���� ���������ִ� �׷��� �����鿡�� Blue�� Red�� + Blue�� Red������ ��������
        // teamNum�� ���� Blue/ Red�� �������־�����, startIdx�� �� ���� ������ �������� ����Ͽ���
        if (Input.GetKeyDown(KeyCode.Q))
            MakeBlueUnit(0 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.W))
            MakeBlueUnit(1 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.E))
            MakeBlueUnit(2 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.A))
            MakeBlueUnit(3 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.S))
            MakeBlueUnit(4 + Variables.startBlueIdx);
        if (Input.GetKeyDown(KeyCode.D) && Variables.groupBlueNum == 6)
            MakeBlueUnit(5 + Variables.startBlueIdx);
        // red
        if (Input.GetKeyDown(KeyCode.I))
            MakeRedUnit(0 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.O))
            MakeRedUnit(1 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.P))
            MakeRedUnit(2 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.J))
            MakeRedUnit(3 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.K))
            MakeRedUnit(4 + Variables.startRedIdx + Variables.groupRedNum);
        if (Input.GetKeyDown(KeyCode.L) && Variables.groupRedNum == 6)
            MakeRedUnit(5 + Variables.startRedIdx + Variables.groupRedNum);

        // Ű����� ���׷��̵�
        if (Input.GetKeyDown(KeyCode.B))
            UpgradeBlueButton();
    }
    void UnitInfo()
    {
        if (!isUnitClick)
            return;

        // Unit Setting
        Unit unitLogic = unitObj.GetComponent<Unit>();
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

    // ======================================================= �ΰ��� ��ư �Լ�
    public void CameraMoveButton(string type)
    {
        // Set Speed By Button
        if (type == "RightDown" || type == "LeftUp") // ������ ��ư�� �����ų� ���ʹ�ư�� ���� 2 ���ϱ�
            camSpeed += 4f;
        else if (type == "RightUp" || type == "LeftDown")
            camSpeed -= 4f;
    }
    // ���׷��̵�
    public void UpgradeBlueButton()
    {
        // �ڽ�Ʈ ���
        if (blueCost < blueMaxCost)
            return;
        if (baseBLevel == 2)
        {
            blueUpgradeCostText.text = "-";
            upgradeText.text = "Max";
            return;
        }


        blueCost -= blueMaxCost;

        // ����Ʈ
        blueUpgradeyEffect[baseBLevel].SetActive(true);
        // Level Up
        baseBLevel++;
        // ��������Ʈ ���� (�ʰ���, ����, ����)
        baseBSpriteRen.sprite = baseBSprite[baseBLevel];

        // Max �ڽ�Ʈ ����
        blueMaxCost += 5;
        // cost �ؽ�Ʈ ����
        blueUpgradeCostText.text = blueMaxCost.ToString();
        //�ڽ�Ʈ ���� �ӵ� ���
        costBlueUp -= 0.25f;
        // ���̽� ��ȭ
        baseB.unitAtk += 5;
        baseB.unitAtkSpeed -= 0.2f;
    }
    void UpgradeRed()
    {
        redUpgradeyEffect[baseRLevel].SetActive(true);
        baseRLevel++;
        redMaxCost += 5;
        costRedUp -= 0.25f;
        baseRSpriteRen.sprite = baseRSprite[baseRLevel];

        baseB.unitAtk += 5;
        baseB.unitAtkSpeed -= 0.2f;

    }
    // ���� ��ȯ
    public void MakeBlueUnit(int idx)
    {
        GameObject unitB = Variables.teamBluePrefabs[idx];
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
        GetUnitObject(Variables.teamBlueNum, idx, unitB.transform.position);
        blueUnitList.Add(unitB);
    }
    public void MakeRedUnit(int idx)
    {
        GameObject unitR = Variables.teamRedPrefabs[idx];
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
        GetUnitObject(Variables.teamRedNum, idx, unitR.transform.position);
        redUnitList.Add(unitR);

        // ���� �Ŀ� �����ε��� ����
        PatternRatio();
    }
    void PatternRatio()
    {
        int rand = Random.Range(0, 100);

        // ���� �ε��� ����
        if (rand < 40)
            patternIdx = 0;
        else if (rand < 70)
            patternIdx = 1;
        else
            patternIdx = 2;
    }
    void GetUnitObject(int teamNum, int idx, Vector3 pos)
    {
        switch (teamNum)
        {
            case 0:
            case 1:
                ObjectManager.instance.GetGiHa(idx, pos);
                break;
            case 2:
                ObjectManager.instance.GetJuFok(idx, pos);
                break;
            case 3:
            case 4:
                ObjectManager.instance.GetBakChi(idx, pos);
                break;
            case 5:
                ObjectManager.instance.GetVBand(idx, pos);
                break;
        }
    }
    // �ɼ� ��ư
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
    // ���� ��ư
    public void DescriptionButton()
    {
        if (!descriptionPanel.activeSelf)
        {
            // ����
            Time.timeScale = 0;
            // �г� �ѱ�
            descriptionPanel.SetActive(true);
            Description("None");
            // �ٸ� ��ư �۵�����
            optionButton.interactable = false;
        }
        else if (descriptionPanel.activeSelf)
        {
            // �̹� �����ִ� ��� �ٽ� ����
            Time.timeScale = 1;
            descriptionPage = 0;
            descriptionPanel.SetActive(false);
            optionButton.interactable = true;
            for (int i = 0; i < 6; i++)
            {
                // �簢�� �� ����
                descriptionRect[i].SetActive(false);
            }
        }
    }
    public void PrevDescriptionButton()
    {
        descriptionPage--;

        if (descriptionPage == 0)
            desPrevButton.SetActive(false);
        else
            desNextButton.SetActive(true);

        Description("Prev");
    }
    public void NextDescriptionButton()
    {
        descriptionPage++;

        // �ٺ����� ������
        if (descriptionPage > 8)
        {
            DescriptionButton();
            return;
        }

        desPrevButton.SetActive(true);

        Description("Next");
    }
    void Description(string btnType)
    {
        // �簢�� Ű�� ����
        if (descriptionPage == 0 || descriptionPage == 1)
        {
            // ù �������� �ι�°���� �簢���� �ȳ��;���
            descriptionRect[0].SetActive(false);
        }
        else if (descriptionPage > 1)
        {
            // �簢�� Ű�� (������ ���������� �簢���� �����Ƿ� ��Ŵ)
            if (descriptionPage < 8)
                descriptionRect[descriptionPage - 2].SetActive(true);

            // ���� ��ư�� ������ ���
            if (btnType == "Next")
            {
                // ���� �簢�� ����
                if (descriptionPage > 2)
                    descriptionRect[descriptionPage - 3].SetActive(false);
            }
            // ������ư�� ������ ���
            else if (btnType == "Prev")
            {
                // ���� �簢�� ����
                if (descriptionPage < 7)
                    descriptionRect[descriptionPage - 1].SetActive(false);
            }
        }

        // �����
        switch (descriptionPage)
        {
            case 0:
                descriptionText.text = "ħ������~ ���ϴ�ݵ��� ���Ű��� ȯ���մϴ�!\n\n�׷� �ٷ� ���Ӽ����� �ص帮���� �ϰڽ��ϴ�!\n\n(���� ������ ������ �����Դϴ�.)";
                break;
            case 1:
                descriptionText.text = "���� ���� ȭ�� �̵��Դϴ�.\n\nŰ������ ȭ��ǥŰ�� ���� �¿�� ȭ���� �̵��Ͻ� �� �ֽ��ϴ�.";
                break;
            case 2:
                descriptionText.text = "�ι�°�� ��� ��ȯ�Դϴ�.\n\n�̰����� �ش� ����� �̱� ���� ����, ����� Ÿ���� �����ֽ��ϴ�.\n\n" +
                    "����� ��ȯ�ϱ� ���ؼ��� �ش��ϴ� Ű���带 ������ ��ȯ�Ͻ� �� �ֽ��ϴ�.\n\n���� ������ �ߺ��Ͽ� ��ȯ�Ͻ� ���� �ֽ��ϴ�!";
                break;
            case 3:
                descriptionText.text = "�� ���� ��� ������ ���Դϴ�.\n\n���� ��ȯ�� ����� ���� Ŭ���ϸ� �ش����� �ǽð� ������ Ȯ���Ͻ� �� �ֽ��ϴ�!";
                break;
            case 4:
                descriptionText.text = "�������� ���� �����Դϴ�.\n\n������ �����ϸ� ����� ��ȯ�Ͻ� �� ������, ������ �ڵ����� 1�� �����մϴ�.\n\n" +
                    "���� �ִ뺸������ �ֱ� ������ �ִ뺸������ �Ѿ ������ �����Ͻ� �� �����ϴ�.";
                break;
            case 5:
                descriptionText.text = "Base ���׷��̵��Դϴ�.\n\nBase�� ������ ���� ���� ������ ���� �ν��� �����մϴ�.\n\n" +
                    "�Ʒ� Upgrade�� ���� ������ �Ҹ��Ͽ� Base�� ���ݷ�, ���ݼӵ��� ������ų �� ������, �ִ� 2������ �����մϴ�.";
                break;
            case 6:
                descriptionText.text = "�̰������� �� ���� �ΰ�� ���� Base�� ü���� Ȯ���Ͻ� �� �ֽ��ϴ�.\n\n" +
                    "�� �� ��� 1000���� �����ϸ� ���� ���� ü���� 0�� �Ǵ� ���� �й��մϴ�.\n\n�� �Ʒ����� ���� ���� �÷��� �ð��� ���ɴϴ�.";
                break;
            case 7:
                descriptionText.text = "���������� �Ͻ����� ��ư�� ���� �Ҹ��� �����ϰų� ������ �����Ͻ� �� ������\n\n����ǥ ��ư�� ������ ���Ӽ����� �ٽ� Ȯ���Ͻ� �� �ֽ��ϴ�.";
                break;
            case 8:
                descriptionText.text = "�׷� ��հ� ����ּ���!\n\n�����մϴ�!";
                break;
        }
    }
    // �� �̵� ��ư
    public void NextButton()
    {
        // ���� ����
        Variables.gameLevel++;

        // ������ ��ȣ ����
        int rand = Random.Range(0, 6);
        while (Variables.isSelectTeam[rand])
        {
            rand = Random.Range(0, 6);
        }
        // ���� ��� ����
        StageRedTeamSetting(rand);

        Time.timeScale = 1;
        // Fade Out
        fadeAc.gameObject.SetActive(true);
        fadeAc.SetTrigger("fadeOut");
        StartCoroutine(LoadScene(1f, "InGame"));
    }
    public void ResetButton()
    {
        Time.timeScale = 1;
        // Fade Out
        fadeAc.gameObject.SetActive(true);
        fadeAc.SetTrigger("fadeOut");
        StartCoroutine(LoadScene(1f, "Game"));
    }
    void StageRedTeamSetting(int teamNum)
    {
        switch (teamNum)
        {
            case 0:
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                break;
            case 1:
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 10;
                Variables.groupRedNum = 5;
                break;
            case 2:
                Variables.teamRedPrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                break;
            case 3:
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                break;
            case 4:
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 12;
                Variables.groupRedNum = 5;
                break;
            case 5:
                Variables.teamRedPrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                break;
        }

        // Team Num
        Variables.teamRedNum = teamNum;
        // Random Team List
        Variables.isSelectTeam[teamNum] = true;
    }
    IEnumerator LoadScene(float time, string sceneName)
    {
        yield return new WaitForSeconds(time);

        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    // ======================================================= Base �Լ�
    public void BaseHit(int dmg, int layer)
    {
        // ������ �������� Base Hit ����
        if (!isGameLive)
            return;

        if (layer == 8)
        {
            // ��������ŭ ����
            blueHP -= dmg;

            // Death
            if (blueHP <= 0)
            {
                blueHP = 0;
                baseBSpriteRen.gameObject.SetActive(false);
                blueDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Lose");
            }
            // Alive
            else
            {
                baseBSpriteRen.color = new Color(1, 0.5f, 0.5f);
                StartCoroutine(SpriteWhite(0.1f, baseBSpriteRen, blueHP > 0));
            }

            // Text
            baseBSlider.value = blueHP;
            blueHpText.text = blueHP.ToString();
            blueHpShadowText.text = blueHP.ToString();
        }
        else if (layer == 9)
        {
            redHP -= dmg;

            if (redHP <= 0)
            {
                redHP = 0;
                baseRSpriteRen.gameObject.SetActive(false);
                redDestroyEffect.SetActive(true);
                isGameLive = false;
                GameOver("Win");
            }
            else
            {
                baseRSpriteRen.color = new Color(1, 0.5f, 0.5f);
                StartCoroutine(SpriteWhite(0.1f, baseRSpriteRen, redHP > 0));
            }

            baseRSlider.value = redHP;
            redHpText.text = redHP.ToString();
            redHpShadowText.text = redHP.ToString();
        }
    }
    IEnumerator SpriteWhite(float time, SpriteRenderer spriteRen, bool isLive)
    {
        yield return new WaitForSeconds(time);

        // 0.1�� ���̿� ������ ���������� �Լ��� �����ϸ� �ȵ�
        if (isLive)
            spriteRen.color = Color.white;
    }
    void GameOver(string result)
    {
        // �̰��� ���
        if (result == "Win")
        {
            // Victory
            victoryObj.SetActive(true);

            // Button
            if (Variables.isStage)  // Stage ���
            {
                if (Variables.gameLevel < 4)
                    nextBtn.SetActive(true);
                else
                {
                    // Ŭ����
                    Variables.isStageClear[Variables.teamBlueNum] = true;
                    // ����
                    //PlayerPrefs.SetInt("Clear" + Variables.teamBlueNum, 1);

                    // ����ȹ�� â


                    resetBtn.SetActive(true);
                }
            }
            else                   // Normal ���
                resetBtn.SetActive(true);
        }
        // ���� ���
        else if (result == "Lose")
        {
            // Defeat
            defeatObj.SetActive(true);

            // Button
            resetBtn.SetActive(true);
        }

        // ���� �۵� ���� (�ð�, �ڽ�Ʈ, ī�޶��̵�, ��ȯ��ư, ����Ŭ��)
        isGameLive = false;
        camSpeed = 0;
        controlSet.SetActive(false);
        overSet.SetActive(true);
    }

    // ======================================================= COM �Լ�
    // ������ �޸����� �̿��� ���� �����ϱ�?
    // �������� ���� �˾Ƽ� ��ȯ�ϱ�?
    IEnumerator Pattern(float time, int typeNum)
    {
        yield return new WaitForSeconds(time);

        // ���� ������ ����
        int rand = Random.Range(0, 2);
        switch (typeNum)
        {
            // 3�ڽ�Ʈ �̻��� ��, 0�����ְ� 1�������� �����ϰ� ��ȯ
            case 0:
                if (redCost >= 3)
                    MakeRedUnit(rand + Variables.startRedIdx + Variables.groupRedNum);
                break;
            // 6�ڽ�Ʈ �̻��� ��, rand���� 0�̸� 0������, 1�̸� 2�����ְ� 3�������� �����ϰ� ��ȯ
            case 1:
                if (redCost >= 6)
                    MakeRedUnit(rand + 2 + Variables.startRedIdx + Variables.groupRedNum);
                break;
            // 9�ڽ�Ʈ �̻��� ��, rand���� 0�̸� 1������, 1�̸� 4������ (+5������)�� ��ȯ
            case 2:
                if (redCost >= 9)
                {
                    // ���� 6���� ���
                    if (Variables.groupRedNum == 6)
                        MakeRedUnit(rand + 4 + Variables.startRedIdx + Variables.groupRedNum);
                    // ���� 5���� ���
                    else
                        MakeRedUnit(4 + Variables.startRedIdx + Variables.groupRedNum);
                }
                break;
            //case 3:
            //    if (redCost >= 10)
            //    {
            //        if (rand == 0)
            //            StartCoroutine(Pattern(0, 2));
            //        else if (rand == 1)
            //        {
            //            // Upgrade
            //            UpgradeRed();
            //        }
            //    }
            //    break;
        }
    }
}
