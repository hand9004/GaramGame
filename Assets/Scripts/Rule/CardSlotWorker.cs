using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CustomUtility;
using Constants;

public class CardSlotWorker : MonoBehaviour
{
    private class CardSlot
    {
        public Vector3 SlotPosition;
        public CharacterCard SlotObject;
    }

    private Vector3 m_PlayerCardSpawnStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 m_EnemyCardSpawnStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 m_EnemyCharacterSpawnStartPosition = new Vector3(0.0f, 0.0f, 0.0f);

    private List<CharacterCard> m_PlayerCardList = null;
    private List<CharacterCard> m_EnemyCardList = null;

    private List<CardSlot> m_PlayerCardSlotList = new List<CardSlot>();
    private List<CardSlot> m_EnemyCardSlotList = new List<CardSlot>();

    public Vector3 PlayerCardSpawnStartPosition
    {
        set { m_PlayerCardSpawnStartPosition = value; }
    }

    public Vector3 EnemyCardSpawnStartPosition
    {
        set { m_EnemyCardSpawnStartPosition = value; }
    }

    public Vector3 EnemyCharacterSpawnStartPosition
    {
        set { m_EnemyCharacterSpawnStartPosition = value; }
    }

    public void InitCardPositionSlots()
    {
        InitCardSlotsInList(m_PlayerCardSlotList, m_PlayerCardSpawnStartPosition, false);
        InitCardSlotsInList(m_EnemyCardSlotList, m_EnemyCardSpawnStartPosition, false);
    }

    public void AttachPlayerCardToSlot(ref List<CharacterCard> targetCardList)
    {
        SyncCardWithSlot(ref targetCardList, ref m_PlayerCardSlotList);
        m_PlayerCardList = targetCardList;
    }

    public void AttachEnemyCardToSlot(ref List<CharacterCard> targetCardList)
    {
        SyncCardWithSlot(ref targetCardList, ref m_EnemyCardSlotList);
        m_EnemyCardList = targetCardList;
    }

    public int[] GetAttackableSlotIndex(bool targetIsPlayerTeam)
    {
        int[] retAttackableRowIndexArr = null;

        if(targetIsPlayerTeam)
        {
            retAttackableRowIndexArr = GetAttackableSlotCountOperation(ref m_PlayerCardSlotList);
        }
        else
        {
            retAttackableRowIndexArr = GetAttackableSlotCountOperation(ref m_EnemyCardSlotList);
        }

        return retAttackableRowIndexArr;
    }

    public int[] GetNonDefensiveCardIndex(bool targetIsPlayerTeam)
    {
        int[] retNonDefensiveCardIndexArr = null;

        if (targetIsPlayerTeam)
        {
            retNonDefensiveCardIndexArr = GetNonDefensiveCardIndexOperation(ref m_PlayerCardSlotList, ref m_PlayerCardList);
        }
        else
        {
            retNonDefensiveCardIndexArr = GetNonDefensiveCardIndexOperation(ref m_EnemyCardSlotList, ref m_EnemyCardList);
        }

        return retNonDefensiveCardIndexArr;
    }

    public int[] GetDefensiveCardIndex(bool targetIsPlayerTeam)
    {
        int[] retDefensiveCardIndexArr = null;

        if(targetIsPlayerTeam)
        {
            retDefensiveCardIndexArr = GetDefensiveCardIndexOperation(ref m_PlayerCardSlotList, ref m_PlayerCardList);
        }
        else
        {
            retDefensiveCardIndexArr = GetDefensiveCardIndexOperation(ref m_EnemyCardSlotList, ref m_EnemyCardList);
        }

        return retDefensiveCardIndexArr;
    }

    public IEnumerator Run()
    {
        CheckAndEmptySlot(ref m_PlayerCardSlotList);
        CheckAndEmptySlot(ref m_EnemyCardSlotList);

        yield return null;
    }

    private void InitCardSlotsInList(List<CardSlot> targetSlotList, Vector3 standardPosition, bool isCharacterSlot)
    {
        float distToStandardPosition = (isCharacterSlot == false) ? ConstantDefine.DISTANCE_FROM_STANDARD_POSITION_CARD :
                                            ConstantDefine.DISTANCE_FROM_STANDARD_POSITION_CHARACTER;

        for (int i = 0; i < ConstantDefine.MAX_CARD_DECK_COUNT; ++i)
        {
            CardSlot createdCardSlot = new CardSlot();

            Vector3 realPosition = new Vector3(standardPosition.x + (distToStandardPosition * i), standardPosition.y, standardPosition.z);
            createdCardSlot.SlotPosition = realPosition;
            createdCardSlot.SlotObject = null;

            targetSlotList.Add(createdCardSlot);
        }
    }

    private void SyncCardWithSlot(ref List<CharacterCard> targetCardList, ref List<CardSlot> cardSlotList)
    {
        for (int i = 0; i < targetCardList.Count; ++i)
        {
            CharacterCard targetCardObject = targetCardList[i];
            for(int j = 0; j < cardSlotList.Count; ++j)
            {
                int rowOfSlot = (j / 2) + 1;
                CardSlot cardSlotObject = cardSlotList[j];
                if (cardSlotObject.SlotObject == null)
                {
                    targetCardObject.transform.position = cardSlotObject.SlotPosition;
                    cardSlotObject.SlotObject = targetCardObject;
                    break;
                }
                else
                {
                    if (targetCardObject == cardSlotObject.SlotObject)
                    {
                        targetCardObject.transform.position = cardSlotObject.SlotPosition;
                        break;
                    }
                }
            }
        }
    }

    private void CheckAndEmptySlot(ref List<CardSlot> targetCardSlotList)
    {
        for (int i = 0; i < targetCardSlotList.Count; ++i)
        {
            CardSlot playerCardSlot = targetCardSlotList[i];

            if (playerCardSlot.SlotObject != null)
            {
                if (playerCardSlot.SlotObject.CurrentHealthPoint <= 0)
                {
                    targetCardSlotList[i].SlotObject = null;
                }
            }
        }
    }

    private int[] GetAttackableSlotCountOperation(ref List<CardSlot> targetList)
    {
        List<int> retAttackableRowIndexArr = new List<int>();

        for (int i = 0; i < targetList.Count; ++i)
        {
            CardSlot targetSlot = targetList[i];
            if (targetSlot.SlotObject != null)
            {
                if(targetSlot.SlotObject.m_HealthPoint > 0)
                {
                    retAttackableRowIndexArr.Add(i);
                }
            }
        }

        return retAttackableRowIndexArr.ToArray();
    }

    private int[] GetNonDefensiveCardIndexOperation(ref List<CardSlot> targetSlotList, ref List<CharacterCard> targetSideCardList)
    {
        List<int> retCardIndexArr = new List<int>();

        for (int i = 0; i < targetSlotList.Count; ++i)
        {
            CardSlot cardSlot = targetSlotList[i];
            if(cardSlot.SlotObject != null)
            {
                if (!cardSlot.SlotObject.IsDefensiveCard)
                {
                    int realNonDefensiveIndex = targetSideCardList.IndexOf(cardSlot.SlotObject);
                    retCardIndexArr.Add(realNonDefensiveIndex);
                }
            }
        }

        return retCardIndexArr.ToArray();
    }

    private int[] GetDefensiveCardIndexOperation(ref List<CardSlot> targetSlotList, ref List<CharacterCard> targetSideCardList)
    {
        List<int> retCardIndexArr = new List<int>();

        for(int i = 0; i < targetSlotList.Count; ++i)
        {
            CardSlot cardSlot = targetSlotList[i];

            if(cardSlot.SlotObject != null)
            {
                if (cardSlot.SlotObject.IsDefensiveCard)
                {
                    int realNonDefensiveIndex = targetSideCardList.IndexOf(cardSlot.SlotObject);
                    retCardIndexArr.Add(realNonDefensiveIndex);
                }
            }
        }

        return retCardIndexArr.ToArray();
    }
}
