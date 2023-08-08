using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Image = UnityEngine.UI.Image;

public static class Variables
{
    public static bool isFirstGame = false;
    public static bool isStage = true;
    public static bool[] isStageClear = { false, false, false, false, false, false };
    public static bool[] isSelectTeam = { false, false, false, false, false, false };
    public static int gameLevel = 0;
    public static int teamBlueNum = 0;
    public static int teamRedNum = 0;
    public static int startBlueIdx = 0;
    public static int groupBlueNum = 0;
    public static int startRedIdx = 0;
    public static int groupRedNum = 0;
    public static float bgmVolume = -15f;
    public static float sfxVolume = -15f;
    public static GameObject[] teamBluePrefabs = { };
    public static GameObject[] teamRedPrefabs = { };
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // �̱��� ���� : �ν��Ͻ��� ������ ������� �ʰ� �ϳ��� �ν��Ͻ��� ����ϱ�

    [Header("---------------[UI]")]
    public Animator fadeAc;
    public Animator menuSetAc;
    public Animator modeSetAc;
    public Animator bookSetAc;
    public Animator normalSetAc;
    public Animator stageSetAc;
    public Animator levelSetAc;
    public Animator settingSetAc;
    public Animator nButtonGroupAc;
    public Animator sButtonGroupAc;
    public Animator creditAc;
    public GameObject modePanel;
    public GameObject normalSelectPanel;
    public GameObject normalLevelPanel;
    public GameObject stageSelectPanel;
    public Button stageButton;
    public Button normalButton;
    public Sprite[] teamLogoSprite;
    public GameObject[] clearObj;

    [Header("---------------[Normal]")]
    public bool isBlueTeam;
    public bool isRedTeam;
    public bool isSelectLevel;
    public int nTeamSelectIdx;
    public Image nBlueTeamLogo;
    public Image nRedTeamLogo;
    public TextMeshProUGUI nBlueTeamNameText;
    public TextMeshProUGUI nRedTeamNameText;
    public TextMeshProUGUI nSelectButtonText;
    public Button[] nSelectButton;
    public Button[] nLevelButton;
    [Header("---------------[Stage]")]
    public int stageTeamSelectIdx;
    public Image stageTeamLogo;
    public TextMeshProUGUI sBlueTeamNameText;
    public TextMeshProUGUI sSelectButtonText;
    public Button[] sTeamSelectButton;

    [Header("---------------[Book]")]
    public int startIdxInBook;
    public int groupNumInBook;
    public int infoPageIdx;
    public string groupNameInInfo;
    public GameObject lastButtonInBook;
    public GameObject unitInfoSet;
    public Image logoImageInBook;
    public Image infoUnitImage;
    public TextMeshProUGUI infoLogoNameText;
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
    public GameObject[] groupPrefabsInBook;
    public Image[] buttonImageInBook;
    public TextMeshProUGUI[] buttonNameInBook;
    public Sprite[] unitPictureSprites;
    public Sprite[] unitLogoSprites;
    float scaleNum;

    void Awake()
    {
        instance = this;

        // Fade In
        fadeAc.SetTrigger("fadeIn");
        StartCoroutine(DisableFade(0.5f));

        // BGM
        SoundManager.instance.BgmPlay("Menu");

        // ����
        for (int i = 0; i < 6; i++)
        {
            // �ҷ�����
            Variables.isStageClear[i] = PlayerPrefs.GetInt("Clear" + i) == 1 ? true : false;

            if (Variables.isStageClear[i])
                clearObj[i].SetActive(true);
        }
        // ù �������� �ҷ�����
        Variables.isFirstGame = PlayerPrefs.GetInt("First") == 1 ? true : false;
    }
    IEnumerator DisableFade(float time)
    {
        yield return new WaitForSeconds(time);

        fadeAc.gameObject.SetActive(false);
    }

    // ======================================================= �ΰ��� ��ư �Լ�
    public void ClickButton(string panelName)
    {
        switch (panelName)
        {
            case "���":
                menuSetAc.SetTrigger("oldOut");
                modeSetAc.SetTrigger("newIn");
                break;
            case "����":
                menuSetAc.SetTrigger("oldOut");
                bookSetAc.SetTrigger("newIn");
                break;
            case "����":
                menuSetAc.SetTrigger("oldOut");
                settingSetAc.SetTrigger("newIn");
                break;
            case "����":
                modeSetAc.SetTrigger("oldOut");
                stageSetAc.SetTrigger("newIn");
                normalButton.interactable = false;
                break;
            case "�Ϲ�":
                modeSetAc.SetTrigger("oldOut");
                normalSetAc.SetTrigger("newIn");
                stageButton.interactable = false;
                break;
            case "����":
                TeamSelectButton();
                break;
        }
    }
    public void BackButton(string panelName)
    {
        switch (panelName)
        {
            case "���":
                menuSetAc.SetTrigger("oldIn");
                modeSetAc.SetTrigger("newOut");
                break;
            case "����":
                menuSetAc.SetTrigger("oldIn");
                bookSetAc.SetTrigger("newOut");
                break;
            case "����":
                menuSetAc.SetTrigger("oldIn");
                settingSetAc.SetTrigger("newOut");
                break;
            case "����":
                StageBackButton();
                break;
            case "�Ϲ�":
                BackSelectButton();
                break;
            case "����":
                BackLevelButton();
                break;
        }
    }
    public void CreditButton()
    {
        creditAc.SetTrigger("doCredit");
    }
    public void SkipButton()
    {
        creditAc.SetTrigger("doOut");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    // ======================================================= Normal �� ���� �Լ�
    public void TeamSetting(string tmName)
    {
        if (nTeamSelectIdx == 0)
        {
            BlueTeamSetting(tmName);
        }
        else if (nTeamSelectIdx == 1)
        {
            RedTeamSetting(tmName);
        }
    }
    void BlueTeamSetting(string tmName) // Variables�� ���� �������ִ� �Լ� -> InGame���� �޾ƿ� ������ ���
    {
        // Disable�� ��ư�� �ִٸ� true
        if (isBlueTeam)
            nSelectButton[Variables.teamBlueNum].interactable = true;

        isBlueTeam = true;
        switch (tmName)
        {
            case "����A":
                // �� ��ȣ
                Variables.teamBlueNum = 0;
                // �� ������ ��������
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                // ������ ���� ��
                Variables.startBlueIdx = 0;
                // ����� ������ ����
                Variables.groupBlueNum = 5;
                // �ؽ�Ʈ UI ����
                nBlueTeamNameText.text = "�����￣��";
                break;
            case "����B":
                Variables.teamBlueNum = 1;
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startBlueIdx = 10;
                Variables.groupBlueNum = 5;
                nBlueTeamNameText.text = "woo.a.woo";
                break;
            case "����":
                Variables.teamBlueNum = 2;
                Variables.teamBluePrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                nBlueTeamNameText.text = "�����ҳ��";
                break;
            case "����A":
                Variables.teamBlueNum = 3;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                nBlueTeamNameText.text = "�Ӹ� ��翣�� 6��";
                break;
            case "����B":
                Variables.teamBlueNum = 4;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 12;
                Variables.groupBlueNum = 5;
                nBlueTeamNameText.text = "�Ἦ���";
                break;
            case "V��":
                Variables.teamBlueNum = 5;
                Variables.teamBluePrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 5;
                nBlueTeamNameText.text = "V�޹��";
                break;
        }

        // Logo Setting
        nBlueTeamLogo.sprite = teamLogoSprite[Variables.teamBlueNum];
        nBlueTeamLogo.SetNativeSize();
        // Button Disable
        nSelectButton[Variables.teamBlueNum].interactable = false;
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
        // �̹� ���� ������ ���� �־ Disable�� ��ư�� �ִٸ� true
        if (isRedTeam)
            nSelectButton[Variables.teamRedNum].interactable = true;

        isRedTeam = true;
        switch (enName)
        {
            case "����A":
                Variables.teamRedNum = 0;
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "�����￣��";
                break;
            case "����B":
                Variables.teamRedNum = 1;
                Variables.teamRedPrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startRedIdx = 10;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "woo.a.woo";
                break;
            case "����":
                Variables.teamRedNum = 2;
                Variables.teamRedPrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                nRedTeamNameText.text = "�����ҳ��";
                break;
            case "����A":
                Variables.teamRedNum = 3;
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 6;
                nRedTeamNameText.text = "�Ӹ� ��翣�� 6��";
                break;
            case "����B":
                Variables.teamRedNum = 4;
                Variables.teamRedPrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startRedIdx = 12;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "�Ἦ���";
                break;
            case "V��":
                Variables.teamRedNum = 5;
                Variables.teamRedPrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startRedIdx = 0;
                Variables.groupRedNum = 5;
                nRedTeamNameText.text = "V�޹��";
                break;
        }

        // Logo Setting
        nRedTeamLogo.sprite = teamLogoSprite[Variables.teamRedNum];
        nRedTeamLogo.SetNativeSize();
        // Button Disable
        nSelectButton[Variables.teamRedNum].interactable = false;
    }
    void TeamSelectButton()
    {
        // Blue�� ���� ������
        if (nTeamSelectIdx == 0 && isBlueTeam)
        {
            nSelectButtonText.text = "�������� ����";
            nTeamSelectIdx++;
        }
        // Red�� ���� ������
        else if (nTeamSelectIdx == 1 && isRedTeam)
        {
            // ��ư ������
            nButtonGroupAc.SetTrigger("btnDown");

            nSelectButtonText.text = "���� �ܰ�";
            nTeamSelectIdx++;
        }
        // ���� �ܰ��
        else if (nTeamSelectIdx == 2)
        {
            // ���� ��� ��ư�� ������ ���� �ܰ��
            if (isBlueTeam && isRedTeam)
            {
                // Animation
                normalSetAc.SetTrigger("oldOut");
                levelSetAc.SetTrigger("newIn");
            }
        }
    }
    void BackSelectButton()
    {
        // ������
        if (nTeamSelectIdx == 0)
        {
            stageButton.interactable = true;
            // Animation
            modeSetAc.SetTrigger("oldIn");
            normalSetAc.SetTrigger("newOut");
            // Blue���� �������� ���� ����
            isBlueTeam = false;
            // �̹��� UI �ʱ�ȭ
            nBlueTeamLogo.sprite = teamLogoSprite[6];
            nBlueTeamLogo.SetNativeSize();
            // �ؽ�Ʈ UI �ʱ�ȭ
            nBlueTeamNameText.text = "-";
            // �̹� ������ ����ư Ȱ��ȭ
            nSelectButton[Variables.teamBlueNum].interactable = true;
        }
        // Blue �� ���� ������
        else if (nTeamSelectIdx == 1)
        {
            isRedTeam = false;
            nRedTeamLogo.sprite = teamLogoSprite[6];
            nRedTeamLogo.SetNativeSize();
            nRedTeamNameText.text = "-";
            nSelectButtonText.text = "������ ����";
            nSelectButton[Variables.teamRedNum].interactable = true;
            nTeamSelectIdx--;
        }
        // Red �� ���� ������
        else if (nTeamSelectIdx == 2)
        {
            // ��ư �ø���
            nButtonGroupAc.SetTrigger("btnUp");

            nSelectButtonText.text = "�������� ����";
            nTeamSelectIdx--;
        }
    }

    // ======================================================= Stage �� ���� �Լ�
    public void StageBlueTeamSetting(string tmName)
    {
        // Disable�� ��ư�� �ִٸ� true
        if (isBlueTeam)
            sTeamSelectButton[Variables.teamBlueNum].interactable = true;

        isBlueTeam = true;
        switch (tmName)
        {
            case "����A":
                // �� ���� ��ȣ (��ư ��Ȱ��ȭ���� ��)
                Variables.teamBlueNum = 0;
                // �� ������ ��������
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                // ������ ���� ��
                Variables.startBlueIdx = 0;
                // ����� ������ ����
                Variables.groupBlueNum = 5;
                // �ؽ�Ʈ UI ����
                sBlueTeamNameText.text = "�����￣��";
                break;
            case "����B":
                Variables.teamBlueNum = 1;
                Variables.teamBluePrefabs = ObjectManager.instance.giHa_prefabs;
                Variables.startBlueIdx = 10;
                Variables.groupBlueNum = 5;
                sBlueTeamNameText.text = "woo.a.woo";
                break;
            case "����":
                Variables.teamBlueNum = 2;
                Variables.teamBluePrefabs = ObjectManager.instance.juFok_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                sBlueTeamNameText.text = "�����ҳ��";
                break;
            case "����A":
                Variables.teamBlueNum = 3;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 6;
                sBlueTeamNameText.text = "�Ӹ� ��翣�� 6��";
                break;
            case "����B":
                Variables.teamBlueNum = 4;
                Variables.teamBluePrefabs = ObjectManager.instance.bakChi_prefabs;
                Variables.startBlueIdx = 12;
                Variables.groupBlueNum = 5;
                sBlueTeamNameText.text = "�Ἦ���";
                break;
            case "V��":
                Variables.teamBlueNum = 5;
                Variables.teamBluePrefabs = ObjectManager.instance.vBand_prefabs;
                Variables.startBlueIdx = 0;
                Variables.groupBlueNum = 5;
                sBlueTeamNameText.text = "V�޹��";
                break;
        }

        // Logo Setting
        stageTeamLogo.sprite = teamLogoSprite[Variables.teamBlueNum];
        stageTeamLogo.SetNativeSize();
        // Button Disable
        sTeamSelectButton[Variables.teamBlueNum].interactable = false;
        // Random Team List
        for (int i = 0; i < 6; i++)
        {
            Variables.isSelectTeam[i] = false;
        }
        Variables.isSelectTeam[Variables.teamBlueNum] = true;
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
    public void StageTeamSelectButton()
    {
        // Blue�� ���� ������
        if (stageTeamSelectIdx == 0 && isBlueTeam)
        {
            // ��ư ������
            sButtonGroupAc.SetTrigger("btnDown");

            sSelectButtonText.text = "���� ����";
            stageTeamSelectIdx++;
        }
        else if (stageTeamSelectIdx == 1)
        {
            // �������� ���
            Variables.isStage = true;

            // Red�� ���� (���� �������� ���� -> Blue���� �Ȱ�ġ��)
            int rand = Random.Range(0, 6);
            while (Variables.isSelectTeam[rand])
            {
                rand = Random.Range(0, 6);
            }
            // ���� ��� ����
            StageRedTeamSetting(rand);

            // ���� ����
            Variables.gameLevel = 0;

            // Fade Out
            fadeAc.gameObject.SetActive(true);
            fadeAc.SetTrigger("fadeOut");
            // ���� ����
            StartCoroutine(StartGame(1f));
            
        }
    }
    void StageBackButton()
    {
        // ������
        if (stageTeamSelectIdx == 0)
        {
            normalButton.interactable = true;
            // Animation
            modeSetAc.SetTrigger("oldIn");
            stageSetAc.SetTrigger("newOut");
            // Blue���� �������� ���� ����
            isBlueTeam = false;
            // �̹��� UI �ʱ�ȭ
            stageTeamLogo.sprite = teamLogoSprite[6];
            stageTeamLogo.SetNativeSize();
            // �ؽ�Ʈ UI �ʱ�ȭ
            sBlueTeamNameText.text = "-";
            // �̹� ������ ����ư Ȱ��ȭ
            sTeamSelectButton[Variables.teamBlueNum].interactable = true;
        }
        // Blue �� ���� ������
        else if (stageTeamSelectIdx == 1)
        {
            // ��ư �ø���
            sButtonGroupAc.SetTrigger("btnUp");

            sSelectButtonText.text = "�� ����";
            stageTeamSelectIdx--;
        }
    }

    // ======================================================= ���� ���� �Լ� 
    public void SelectLevelButton(int idx)
    {
        // ��� ��ư Ȱ��ȭ
        for (int i = 0; i < 5; i++)
            nLevelButton[i].interactable = true;

        // ���� ����
        isSelectLevel = true;
        Variables.gameLevel = idx;

        // �ش� �ε��� ��ư ��Ȱ��ȭ
        nLevelButton[idx].interactable = false;
    }
    void BackLevelButton()
    {
        // Animation
        normalSetAc.SetTrigger("oldIn");
        levelSetAc.SetTrigger("newOut");

        isSelectLevel = false;

        // ��� ��ư Ȱ��ȭ
        for (int i = 0; i < 5; i++)
            nLevelButton[i].interactable = true;
    }
    public void GameStartButton()
    {
        // Level�� �����ؾ� ���� ����
        if (isSelectLevel)
        {
            // �Ϲ� ���
            Variables.isStage = false;

            // Fade Out
            fadeAc.gameObject.SetActive(true);
            fadeAc.SetTrigger("fadeOut");
            // ���� ����
            StartCoroutine(StartGame(1f));
        }
    }
    IEnumerator StartGame(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene("InGame");
    }

    // ======================================================= Book �Լ�
    public void TeamSelectInBook(string teamName)
    {
        groupNameInInfo = teamName;
        lastButtonInBook.SetActive(false);
        int teamIdx = 0;
        string logoName = "";
        
        switch (teamName)
        {
            case "����A":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                teamIdx = 0;
                logoName = "�����￣��";
                break;
            case "����B":
                groupPrefabsInBook = ObjectManager.instance.giHa_prefabs;
                startIdxInBook = 10;
                groupNumInBook = 5;
                teamIdx = 1;
                logoName = "��ƿ�";
                break;
            case "����":
                groupPrefabsInBook = ObjectManager.instance.juFok_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                teamIdx = 2;
                logoName = "�����ҳ��";
                lastButtonInBook.SetActive(true);
                break;
            case "����A":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 6;
                teamIdx = 3;
                logoName = "�Ӹ���翣��6��";
                lastButtonInBook.SetActive(true);
                break;
            case "����B":
                groupPrefabsInBook = ObjectManager.instance.bakChi_prefabs;
                startIdxInBook = 12;
                groupNumInBook = 5;
                teamIdx = 4;
                logoName = "�Ἦ���";
                break;
            case "V��":
                groupPrefabsInBook = ObjectManager.instance.vBand_prefabs;
                startIdxInBook = 0;
                groupNumInBook = 5;
                teamIdx = 5;
                logoName = "V�޹��";
                break;
        }

        // Top UI Setting
        // �ΰ�
        logoImageInBook.sprite = unitLogoSprites[teamIdx];
        infoLogoNameText.text = logoName;

        // Button Setting
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

        // Unit ���� ������Ʈ
        //infoImage.sprite = spriteRen.sprite;
        infoUnitImage.sprite = UnitImage(idx);
        infoUnitImage.SetNativeSize();
        infoUnitImage.transform.localScale = Vector3.one * scaleNum;
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
                        image = unitPictureSprites[0];
                        break;
                    case 1:
                        image = unitPictureSprites[1];
                        break;
                    case 2:
                        image = unitPictureSprites[2];
                        break;
                    case 3:
                        image = unitPictureSprites[3];
                        break;
                    case 4:
                        image = unitPictureSprites[4];
                        break;
                }
                scaleNum = 0.2f;
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[5];
                        break;
                    case 1:
                        image = unitPictureSprites[6];
                        break;
                    case 2:
                        image = unitPictureSprites[7];
                        break;
                    case 3:
                        image = unitPictureSprites[8];
                        break;
                    case 4:
                        image = unitPictureSprites[9];
                        break;
                }
                scaleNum = 0.2f;
                break;
            case "����":
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[10];
                        break;
                    case 1:
                        image = unitPictureSprites[11];
                        break;
                    case 2:
                        image = unitPictureSprites[12];
                        break;
                    case 3:
                        image = unitPictureSprites[13];
                        break;
                    case 4:
                        image = unitPictureSprites[14];
                        break;
                    case 5:
                        image = unitPictureSprites[15];
                        break;
                }
                scaleNum = 0.4f;
                break;
            case "����A":
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[16];
                        break;
                    case 1:
                        image = unitPictureSprites[17];
                        break;
                    case 2:
                        image = unitPictureSprites[18];
                        break;
                    case 3:
                        image = unitPictureSprites[19];
                        break;
                    case 4:
                        image = unitPictureSprites[20];
                        break;
                    case 5:
                        image = unitPictureSprites[21];
                        break;
                }
                scaleNum = 0.2f;
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[22];
                        break;
                    case 1:
                        image = unitPictureSprites[23];
                        break;
                    case 2:
                        image = unitPictureSprites[24];
                        break;
                    case 3:
                        image = unitPictureSprites[25];
                        break;
                    case 4:
                        image = unitPictureSprites[26];
                        break;
                }
                scaleNum = 0.2f;
                break;
            case "V��":
                switch (idx)
                {
                    case 0:
                        image = unitPictureSprites[27];
                        break;
                    case 1:
                        image = unitPictureSprites[28];
                        break;
                    case 2:
                        image = unitPictureSprites[29];
                        break;
                    case 3:
                        image = unitPictureSprites[30];
                        break;
                    case 4:
                        image = unitPictureSprites[31];
                        break;
                }
                scaleNum = 0.4f;
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
                        skillText = "���� �� ������ ���ݷ¸�ŭ ü�� ����.";
                        break;
                    case 3:
                        skillText = "3.5�ʸ��� 1���ξ� ����.";
                        break;
                    case 4:
                        skillText = "�Ϲ����� ����.\n(���� �� ���ݹ���)";
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
                        skillText = "���� �� ���Ÿ� ���� ��ȿ.\n(������ ������ �ش����� ����.)";
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
                        detailText = "��ȹ���� ���� ���ؾ��� �پ���� ���������� ����� �������� �Ҹ���. ���� �����ϴ� ��������� �ֺ� ������� ���� �Ѵ�.";
                        break;
                    case 3:
                        detailText = "������ ���� ���ϵ����� �Ϲ������� Ư���ϰ� �հ��ߴ�. ��ο��� ����ϸ� ����ϴ� ������ ������� �ŷ��� �˳��� �ִ�.";
                        break;
                    case 4:
                        detailText = "��Ī ���迡�� �� ���մ����� Ư���� ������ ��糢�� �ִ�. ���� ������� �ΰ��鿡�� ���ָ� ���� �� �ִٰ� �Ѵ�.";
                        break;
                }
                break;
            case "����B":
                switch (idx)
                {
                    case 0:
                        detailText = "�ʵ�̰� �ִ� ���뿩��. ���� ������� ������ �� ������ϸ� ü���� ���� �ʴ�. �׻� ������Ʈ��� ��ѱ�� �Բ� �ٴѴ�.";
                        break;
                    case 1:
                        detailText = "��߳����� ��߰���. �׳ุ�� �＾ ������ �˾Ƶ�� ���� ���� Ư¡. ���� �뿪�� �ʿ��� ������ �˾Ƶ�� ����� ���� �ִ�.";
                        break;
                    case 2:
                        detailText = "�������� ���� 100% â���� ������ ������ �� ���� �ÿ��ÿ��ϰ� �ϴ� �����̴�. �ǿܷ� �����鿪�� ���� ���׿������� �Ҹ��� ��.";
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
                        detailText = "���ϳ� �迭�� ���̵������� �������� ���� ������ ���� Ư¡. ����� ���� �������� �������� �븮�� �ִٴ� �ҹ��� �����Ѵ�.";
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
                        detailText = "��ģ���� �����ټ����� ������ �� ó���� �ʿ��ϴٰ� �Ѵ�. ������ ����ϰ� ǥ���ϴ� �Ͱ� ����� �������̶�� Ư¡�� �ִ�.";
                        break;
                    case 3:
                        detailText = "�� ���� ����� �ư���. �縳�б��� ��̰� ��� ģ������ ����� ���� ���пԴ�. ������ �������� ���ϴ� ����� ���� �����ش�.";
                        break;
                    case 4:
                        detailText = "������ ������Ƹ��ÿ´��긮������. ������ �����̸� ������ \"���\"�� ���̴� Ư���� ������ ����. ���� ������ �� ���.";
                        break;
                    case 5:
                        detailText = "�����ϰ� �������ִ� ��Ҹ����� �����븦 ������ �Ѽ����� �����ϴ� �����ŷ��� ������ �ִ�. ��Ÿ���� ���� ��Ƽ��Ʈ.";
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
                        detailText = "���濡�� ����� �ð�ҳ�� ������ �������� ����. �뷡�Ƿ��� �ſ� ������ ��Ī ����ȣ���������� �ƹ��� ���������� �ʴ´�.";
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
}
