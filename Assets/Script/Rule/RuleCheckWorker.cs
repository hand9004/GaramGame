using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Constants;
using CustomUtility;

// 승, 패 조건 체크(적, 아군 객체수로 판단)
//
public class RuleCheckWorker : MonoBehaviour
{
    public delegate void GameWinListener(GameRuleState GameState, bool IsPlayerWin);
    public enum GameRuleState
    {
        WaitGame,
        GamePlaying,
        GameEnd,
    }

    private GameRuleState m_CurrentGameState = GameRuleState.WaitGame;
    private bool m_IsPlayerWin = false;
    private int m_CurrentCardIndex = 0;
    private int m_GamePhase = 1;

    private CharacterCard m_CurrentTurnCard = null;
    private GameWinListener m_GameWinListener = null;

    private List<CharacterCard> m_EnemyCardList = null;
    private List<CharacterCard> m_PlayerCardList = null;
    private List<CharacterCard> m_TurnExecuteOrder = new List<CharacterCard>();

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

    public void RegisterGameWinListener(GameWinListener gameListener)
    {
        m_GameWinListener = gameListener;
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
        // 기존의 자신과 팀을 제외하고 공격할 수 있는 적군의 행을 계산한다.
        int realAttackRange = -(m_CurrentTurnCard.RowNumber) + (m_CurrentTurnCard.m_AttackRange + 1);
        realAttackRange = (realAttackRange > ConstantDefine.THIRD_ROW) ? ConstantDefine.THIRD_ROW : realAttackRange;
        if (m_CurrentTurnCard.IsPlayerTeam)
        {
            int selectedRow = GetAttackConfirmedRow(realAttackRange);
            if(selectedRow > 0)
            {
                AttackCardOnRow(m_EnemyCardList, selectedRow);
            }
        }
        else
        {
            int selectedRow = GetAttackConfirmedRow(realAttackRange);
            if (selectedRow > 0)
            {
                AttackCardOnRow(m_PlayerCardList, selectedRow);
            }
        }

        GoingToNextTurn();
    }

    // 사정거리 내에 있을 경우, 공격할 행을 알려준다.
    private int GetAttackConfirmedRow(int realAttackRange)
    {
        int retRow = 0;
        if (realAttackRange > ConstantDefine.FIRST_ROW)
        {
            switch (realAttackRange)
            {
                case ConstantDefine.SECOND_ROW:
                    {
                        float[] percentageDistribute = { 0.6f, 0.4f };
                        int selectedIndex = UtilityFunctions.GetWhereIsCorrect(0, 100, percentageDistribute);
                        if (selectedIndex == 0)
                        {
                            retRow = ConstantDefine.FIRST_ROW;
                        }
                        else
                        {
                            retRow = ConstantDefine.SECOND_ROW;
                        }
                    }
                    break;
                case ConstantDefine.THIRD_ROW:
                    {
                        float[] percentageDistribute = { 0.5f, 0.35f, 0.15f };
                        int selectedIndex = UtilityFunctions.GetWhereIsCorrect(0, 100, percentageDistribute);

                        if (selectedIndex == 0)
                        {
                            retRow = ConstantDefine.FIRST_ROW;
                        }
                        else if(selectedIndex == 1)
                        {
                            retRow = ConstantDefine.SECOND_ROW;
                        }
                        else
                        {
                            retRow = ConstantDefine.THIRD_ROW;
                        }
                    }
                    break;
                default:
                    {
                        Debug.Assert(false);
                    }
                    break;
            }
        }
        else if (realAttackRange == ConstantDefine.FIRST_ROW)
        {
            retRow = ConstantDefine.FIRST_ROW;
        }

        return retRow;
    }

    private void AttackCardOnRow(List<CharacterCard> targetList, int attackRow)
    {
        for (int i = 0; i < targetList.Count; ++i)
        {
            CharacterCard cardObject = targetList[i];
            if (cardObject.RowNumber == attackRow)
            {
                if(cardObject.isActiveAndEnabled && m_CurrentTurnCard.isActiveAndEnabled)
                {
                    if (cardObject.CurrentHealthPoint > 0)
                    {
                        m_CurrentTurnCard.AttackCard(cardObject);
                        break;
                    }
                }
            }
        }
    }

    private void GoingToNextTurn()
    {
        AdjustExecuteOrder();

        if (m_CurrentCardIndex < m_TurnExecuteOrder.Count - 1)
        {
            m_CurrentTurnCard = m_TurnExecuteOrder[++m_CurrentCardIndex];
        }
        else
        {
            m_CurrentTurnCard = m_TurnExecuteOrder[0];
            m_CurrentCardIndex = 0;
            ++m_GamePhase;
        }
    }

    private void AdjustExecuteOrder()
    {
        List<int> removeList = new List<int>();
        for(int i = 0; i < m_TurnExecuteOrder.Count; ++i)
        {
            if(m_TurnExecuteOrder[i].CurrentHealthPoint <= 0)
            {
                removeList.Add(i);
            }
        }

        for(int i = 0; i < removeList.Count; ++i)
        {
            m_TurnExecuteOrder.RemoveAt(removeList[i] - i);
        }

        removeList = null;
    }
}