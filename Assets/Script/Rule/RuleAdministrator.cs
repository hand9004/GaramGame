using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CustomUtility;
using Constants;

public class RuleAdministrator : MonoBehaviour
{
    [System.Serializable]
    public struct CardDeckSettingData
    {
        public bool IsDefensiveCard;
        public GameObject CardPrefab;
    };

    public Transform m_PlayerCardSpawnStartPosition = null;
    public Transform m_EnemyCardSpawnStartPosition = null;
    public Transform m_EnemyCharacterSpawnStartPosition = null;

    public List<CardDeckSettingData> m_PlayerSideCardSettingList = new List<CardDeckSettingData>();
    public List<CardDeckSettingData> m_EnemySideCardSettingList = new List<CardDeckSettingData>();

    private List<CharacterCard> m_PlayerCardCharacterList = new List<CharacterCard>();
    private List<CharacterCard> m_EnemyCardCharacterList = new List<CharacterCard>();

    private CardStatusCheckWorker m_CardStatusCheckWorker = null;
    private CardSlotWorker m_CardSlotWorker = null;
    private RuleCheckWorker m_RuleCheckWorker = null;
    private SkillCardWorker m_SkillCardWorker = null;
    private UIControlWorker m_UIControlWorker = null;

    private bool isGameFinished = false;

    void Awake()
    {
        Debug.Assert(m_PlayerSideCardSettingList.Count <= ConstantDefine.MAX_CARD_DECK_COUNT, "List Size Should be Under 6.");
        Debug.Assert(m_EnemySideCardSettingList.Count <= ConstantDefine.MAX_CARD_DECK_COUNT, "List Size Should be Under 6.");

        m_CardStatusCheckWorker = gameObject.AddComponent<CardStatusCheckWorker>();
        m_CardSlotWorker = gameObject.AddComponent<CardSlotWorker>();
        m_RuleCheckWorker = gameObject.AddComponent<RuleCheckWorker>();
        m_SkillCardWorker = gameObject.AddComponent<SkillCardWorker>();
        m_UIControlWorker = gameObject.AddComponent<UIControlWorker>();

        InitCards();

        InitCardSlotWorker();
        InitRuleCheckWorker();
        InitCardStatusCheckWorker();
    }

    void Update()
    {
        StartCoroutine(m_RuleCheckWorker.Run());
        StartCoroutine(m_CardSlotWorker.Run());
        StartCoroutine(m_SkillCardWorker.Run());
        StartCoroutine(m_CardStatusCheckWorker.Run());
    }

    void OnDestroy()
    {
        m_CardStatusCheckWorker = null;
        m_CardSlotWorker = null;
        m_RuleCheckWorker = null;
        m_SkillCardWorker = null;
    }

    private void InitCards()
    {
        InitCard(ref m_PlayerSideCardSettingList, ref m_PlayerCardCharacterList, true);
        InitCard(ref m_EnemySideCardSettingList, ref m_EnemyCardCharacterList, false);
    }

    private void InitCard(ref List<CardDeckSettingData> settingDataList, ref List<CharacterCard> inGameDataList, bool isPlayerTeam)
    {
        ObjectPoolManager objectPoolMgr = GameObject.Find("CharacterCardPool").GetComponent<ObjectPoolManager>();

        for (int i = 0; i < settingDataList.Count; ++i)
        {
            CharacterCard settingCardPrefab = settingDataList[i].CardPrefab.GetComponent<CharacterCard>();
            CharacterCard inGameCardObject = objectPoolMgr.GetObject().GetComponent<CharacterCard>();

            // 카드에 나오는 캐릭터 스프라이트를 바꿔준다.
            SpriteRenderer settingPortraitSpr = settingCardPrefab.m_PortraitRenderer;
            SpriteRenderer inGamePortraitSpr = inGameCardObject.m_PortraitRenderer;
            inGamePortraitSpr.sprite = settingPortraitSpr.sprite;

            inGameCardObject.IsPlayerTeam = isPlayerTeam;
            inGameCardObject.m_HealthPoint = settingCardPrefab.m_HealthPoint;
            inGameCardObject.m_AttackPoint = (settingDataList[i].IsDefensiveCard) ? (int)(settingCardPrefab.m_AttackPoint * 0.5f)
                                                                    : settingCardPrefab.m_AttackPoint;
            inGameCardObject.m_Speed = settingCardPrefab.m_Speed;
            inGameCardObject.IsDefensiveCard = settingDataList[i].IsDefensiveCard;
            inGameCardObject.ResetStatus();

            StartCoroutine(inGameCardObject.UpdateCardStatus());

            inGameDataList.Add(inGameCardObject);
        }
    }

    private void InitCardSlotWorker()
    {
        m_CardSlotWorker.PlayerCardSpawnStartPosition = m_PlayerCardSpawnStartPosition.position;
        m_CardSlotWorker.EnemyCardSpawnStartPosition = m_EnemyCardSpawnStartPosition.position;
        m_CardSlotWorker.EnemyCharacterSpawnStartPosition = m_EnemyCharacterSpawnStartPosition.position;

        m_CardSlotWorker.InitCardPositionSlots();

        m_CardSlotWorker.AttachPlayerCardToSlot(ref m_PlayerCardCharacterList);
        m_CardSlotWorker.AttachEnemyCardToSlot(ref m_EnemyCardCharacterList);
    }

    private void InitRuleCheckWorker()
    {
        m_RuleCheckWorker.InitRuleCheckWorker(ref m_PlayerCardCharacterList, ref m_EnemyCardCharacterList);
        m_RuleCheckWorker.RegisterGetAttackSlotIndexConnector((bool targetIsPlayerTeam) =>
        {
            int attackSlot = -1;
            int[] nonDefensiveSlotIndexArr = m_CardSlotWorker.GetNonDefensiveCardIndex(targetIsPlayerTeam);
            int[] defensiveSlotIndexArr = m_CardSlotWorker.GetDefensiveCardIndex(targetIsPlayerTeam);

            if(defensiveSlotIndexArr.Length > 0)
            {
                if(nonDefensiveSlotIndexArr.Length > 0)
                {
                    float[] defensiveAttackDistribution = { 0.7f, 0.3f };
                    int defensiveAttackIndex = UtilityFunctions.GetWhereIsCorrect(0.0f, 1.0f, defensiveAttackDistribution);
                    switch (defensiveAttackIndex)
                    {
                        case 0:
                            {
                                int selectedIndex = UtilityFunctions.GetWhereIsCorrect(0.0f, 1.0f, defensiveSlotIndexArr.Length);
                                attackSlot = defensiveSlotIndexArr[selectedIndex];
                            }
                            break;
                        case 1:
                            {
                                int selectedIndex = UtilityFunctions.GetWhereIsCorrect(0.0f, 1.0f, nonDefensiveSlotIndexArr.Length);
                                attackSlot = nonDefensiveSlotIndexArr[selectedIndex];
                            }
                            break;
                        default:
                            {
                                Debug.Assert(false, "This Can't be Happened.");
                            }
                            break;
                    }
                }
                else
                {
                    int selectedIndex = UtilityFunctions.GetWhereIsCorrect(0.0f, 1.0f, defensiveSlotIndexArr.Length);
                    attackSlot = defensiveSlotIndexArr[selectedIndex];
                }
            }
            else if(nonDefensiveSlotIndexArr.Length > 0)
            {
                int selectedIndex = UtilityFunctions.GetWhereIsCorrect(0.0f, 1.0f, nonDefensiveSlotIndexArr.Length);
                attackSlot = nonDefensiveSlotIndexArr[selectedIndex];
            }

            return attackSlot;
        });
        m_RuleCheckWorker.RegisterGameWinListener(
            (RuleCheckWorker.GameRuleState GameState, bool IsPlayerWin) =>
            {
                if (RuleCheckWorker.GameRuleState.GameEnd == GameState)
                {
                    if (!IsPlayerWin)
                    {
                        isGameFinished = true;
                    }
                    else
                    {
                        isGameFinished = true;
                    }
                }
            });
        m_RuleCheckWorker.RegisterOccuredActionListener(
            (RuleCheckWorker.OccuredActionState ActionEvent) =>
            {
                switch (ActionEvent)
                {
                    case RuleCheckWorker.OccuredActionState.Attack:
                        {
                        }
                        break;
                    case RuleCheckWorker.OccuredActionState.CardSpecialSkill:
                        {
                        }
                        break;
                    case RuleCheckWorker.OccuredActionState.SkillCard:
                        {
                        }
                        break;
                    default:
                        { }
                        break;
                }
            });
    }

    private void InitCardStatusCheckWorker()
    {
        m_CardStatusCheckWorker.InitCardStatusCheckWorker(ref m_PlayerCardCharacterList, ref m_EnemyCardCharacterList);
        m_CardStatusCheckWorker.RegisterDiedCardObjectListener(() =>
        {
        });
    }
}
