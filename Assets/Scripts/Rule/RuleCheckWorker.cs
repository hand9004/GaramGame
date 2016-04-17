using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Constants;
using CustomUtility;
using System;

// 승, 패 조건 체크(적, 아군 객체수로 판단)
//
public class RuleCheckWorker : MonoBehaviour
{
    // Connector의 경우, 외부 클래스의 어떤 모듈과 어떤 모듈이 연결되서 사용되고, 그 후 현재 객체에 결과도 가져오는 delegate에 붙이는 접미사
    // Listener의 경우, 외부 클래스가 특정 모듈로부터 어떤 상황에 따른 이벤트를 얻기만 할 경우 붙이는 접미사
    public delegate int GetAttackSlotIndexConnector(bool targetIsPlayerTeam);

    public delegate void OccuredActionListener(OccuredActionState ActionEvent);
    public delegate void GameWinListener(GameRuleState GameState, bool IsPlayerWin);

    public enum OccuredActionState
    {
        None,
        Attack,
        CardSpecialSkill,
        SkillCard,
        TurnActionEnd,
        PhaseEnded,
    }

    public enum GameRuleState
    {
        WaitGame,
        GamePlaying,
        GameEnd,
    }

    private GameRuleState m_CurrentGameState = GameRuleState.WaitGame;
    private bool m_IsBattleAutomatic = false;
    private bool m_IsPlayerWin = false;
    private int m_CurrentCardIndex = 0;

    private CharacterCard m_CurrentTurnCard = null;

    private GetAttackSlotIndexConnector m_GetAttackSlotIndexConnector = null;
    private GameWinListener m_GameWinListener = null;
    private OccuredActionListener m_ActionListener = null;

    private List<CharacterCard> m_EnemyCardList = null;
    private List<CharacterCard> m_PlayerCardList = null;
    private List<CharacterCard> m_TurnExecuteOrder = new List<CharacterCard>();

    public bool IsBattleAutomatic
    {
        set { m_IsBattleAutomatic = value; }
    }

    // Descend Order Sort Comparer
    private class TurnExecuteOrderComparer : IComparer<CharacterCard>
    {
        int IComparer<CharacterCard>.Compare(CharacterCard a, CharacterCard b)
        {
            CharacterCard firstCard = (CharacterCard)a;
            CharacterCard secondCard = (CharacterCard)b;

            if (firstCard.m_Speed < secondCard.m_Speed)
            {
                return 1;
            }
            else if (firstCard.m_Speed > secondCard.m_Speed)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public void InitRuleCheckWorker(ref List<CharacterCard> playerCardList, ref List<CharacterCard> enemyCardList)
    {
        m_PlayerCardList = playerCardList;
        m_EnemyCardList = enemyCardList;

        for(int i = 0; i < m_PlayerCardList.Count; ++i)
        {
            m_TurnExecuteOrder.Add(m_PlayerCardList[i]);
        }
        for(int i = 0; i < m_EnemyCardList.Count; ++i)
        {
            m_TurnExecuteOrder.Add(m_EnemyCardList[i]);
        }

        m_TurnExecuteOrder.Sort(new TurnExecuteOrderComparer());

        m_CurrentTurnCard = m_TurnExecuteOrder[m_CurrentCardIndex];
        m_CurrentGameState = GameRuleState.GamePlaying;
    }

    public void RegisterGetAttackSlotIndexConnector(GetAttackSlotIndexConnector attackableConnector)
    {
        m_GetAttackSlotIndexConnector = attackableConnector;
    }

    public void RegisterGameWinListener(GameWinListener gameListener)
    {
        m_GameWinListener = gameListener;
    }

    public void RegisterOccuredActionListener(OccuredActionListener actionListener)
    {
        m_ActionListener = actionListener;
    }

    public IEnumerator Run()
    {
        if(m_CurrentGameState == GameRuleState.GamePlaying)
        {
            CardTurnAction();

            CheckGameOver();
        }

        yield return null;
    }

    private void CheckGameOver()
    {
        if (m_EnemyCardList.Count <= 0)
        {
            m_IsPlayerWin = true;
            m_CurrentGameState = GameRuleState.GameEnd;
            m_GameWinListener(m_CurrentGameState, m_IsPlayerWin);
        }
        else
        {
            if (m_PlayerCardList.Count <= 0)
            {
                m_IsPlayerWin = false;
                m_CurrentGameState = GameRuleState.GameEnd;
                m_GameWinListener(m_CurrentGameState, m_IsPlayerWin);
            }
        }
    }

    private void CardTurnAction()
    {
        if (m_CurrentTurnCard != null)
        {
            int selectedSlot = -1;
            if (m_CurrentTurnCard.CurrentCardStatus == CharacterCard.CardStatus.Idle)
            {
                if (!m_IsBattleAutomatic)
                {
                    if (m_CurrentTurnCard.IsPlayerTeam)
                    {
                        bool isMouseButtonPressed = Input.GetMouseButtonUp(0);
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        m_CurrentTurnCard.OnCardInAction();
                        for (int i = 0; i < m_EnemyCardList.Count; ++i)
                        {
                            mousePosition.z = m_EnemyCardList[i].transform.position.z;
                            if (m_EnemyCardList[i].IsContainPoint(ref mousePosition))
                            {
                                m_EnemyCardList[i].OnCardSelectedInTarget();
                                if (isMouseButtonPressed)
                                {
                                    selectedSlot = i;
                                    m_CurrentTurnCard.ForceQuitVisualizeEffect();
                                    break;
                                }
                            }
                            else
                            {
                                m_EnemyCardList[i].ForceQuitVisualizeEffect();
                            }
                        }
                    }
                    else
                    {
                        selectedSlot = m_GetAttackSlotIndexConnector(!m_CurrentTurnCard.IsPlayerTeam);
                        m_CurrentTurnCard.ForceQuitVisualizeEffect();
                    }
                }
                else
                {
                    selectedSlot = m_GetAttackSlotIndexConnector(!m_CurrentTurnCard.IsPlayerTeam);
                    m_CurrentTurnCard.ForceQuitVisualizeEffect();
                }

                OnAttack(selectedSlot);
            }

            GoingToNextTurn();
        }
    }

    private void AttackCardOnSlot(List<CharacterCard> targetList, int attackSlot)
    {
        if (m_CurrentTurnCard.CurrentCardStatus == CharacterCard.CardStatus.Idle)
        {
            CharacterCard cardObject = targetList[attackSlot];
            if(cardObject != null)
            {
                if (cardObject.isActiveAndEnabled && m_CurrentTurnCard.isActiveAndEnabled)
                {
                    if (cardObject.CurrentHealthPoint > 0)
                    {
                        cardObject.ForceQuitVisualizeEffect();
                        m_CurrentTurnCard.AttackCard(cardObject);
                        Debug.Log("Attacked Slot = " + attackSlot);
                        m_ActionListener(OccuredActionState.Attack);
                    }
                }
            }
        }
    }

    private void GoingToNextTurn()
    {
        AdjustExecuteOrder();

        if (m_CurrentTurnCard.CurrentCardStatus == CharacterCard.CardStatus.TurnActionEnded)
        {
            if (m_CurrentCardIndex < m_TurnExecuteOrder.Count - 1)
            {
                m_CurrentTurnCard.ForceQuitVisualizeEffect();
                m_CurrentTurnCard = m_TurnExecuteOrder[++m_CurrentCardIndex];
                m_ActionListener(OccuredActionState.TurnActionEnd);
            }
            else
            {
                // 하나의 페이즈가 끝날 때마다, 턴에 관련된 모든 상태를 초기화한다.
                // 추후, 디버프나, 버프 같은 상태이상의 지속시간 소비부분도 이 부분에서 결정한다.
                for(int i = 0; i < m_TurnExecuteOrder.Count; ++i)
                {
                    m_TurnExecuteOrder[i].PhaseEndAction();
                    m_CurrentTurnCard.ForceQuitVisualizeEffect();
                }

                m_CurrentTurnCard = m_TurnExecuteOrder[0];
                m_CurrentCardIndex = 0;

                m_ActionListener(OccuredActionState.PhaseEnded);
            }
        }
    }

    private void AdjustExecuteOrder()
    {
        List<CharacterCard> removeList = new List<CharacterCard>();
        for(int i = 0; i < m_TurnExecuteOrder.Count; ++i)
        {
            if(m_TurnExecuteOrder[i].CurrentHealthPoint <= 0)
            {
                removeList.Add(m_TurnExecuteOrder[i]);
            }
        }

        for(int i = 0; i < removeList.Count; ++i)
        {
            m_TurnExecuteOrder.Remove(removeList[i]);
        }

        m_CurrentCardIndex = m_TurnExecuteOrder.IndexOf(m_CurrentTurnCard);

        removeList = null;
    }

    private void OnAttack(int selectedSlot)
    {
        if (m_CurrentTurnCard.IsPlayerTeam)
        {
            if (selectedSlot >= 0)
            {
                AttackCardOnSlot(m_EnemyCardList, selectedSlot);
            }
            else
            {
                if(m_IsBattleAutomatic)
                {
                    m_CurrentTurnCard.OnCardDoNothing();
                }
            }
        }
        else
        {
            if (selectedSlot >= 0)
            {
                AttackCardOnSlot(m_PlayerCardList, selectedSlot);
            }
            else
            {
                m_CurrentTurnCard.OnCardDoNothing();
            }
        }
    }
}