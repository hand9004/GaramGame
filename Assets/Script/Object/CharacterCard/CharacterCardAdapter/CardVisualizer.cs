using UnityEngine;
using System.Collections;
using System;

public class CardVisualizer : MonoBehaviour
{
    public delegate void OnActionActive(CardVisualizeEffect.EffectType effectType);
    public delegate void OnActionEnded();

    private OnActionActive m_ActionActive = null;
    private OnActionEnded m_ActionEnded = null;

    private CardVisualizeEffect m_VisualizeEffect = null;

    public IEnumerator Run()
    {
        if(m_VisualizeEffect != null)
        {
            StartCoroutine(m_VisualizeEffect.UpdateVisualizeEffect());

            CardVisualizeEffect.VisualizeEffectStatus effectStatus = m_VisualizeEffect.GetVisualizeEffectStatus();
            switch (effectStatus)
            {
                case CardVisualizeEffect.VisualizeEffectStatus.Execute:
                    {
                        m_ActionActive(m_VisualizeEffect.GetVisualizeEffectType());
                    }
                    break;
                case CardVisualizeEffect.VisualizeEffectStatus.End:
                    {
                        m_ActionEnded();
                        m_VisualizeEffect = null;
                    }
                    break;
                default:
                    {}
                    break;
            }
        }

        yield return null;
    }

    public void RegisterOnActionActive(OnActionActive actionActive)
    {
        m_ActionActive = actionActive;
    }

    public void RegisterOnActionEnded(OnActionEnded actionEnded)
    {
        m_ActionEnded = actionEnded;
    }

    public void OnCardAttacking()
    {
        m_VisualizeEffect = new CardAttackEffect();

        CardVisualizeEffect.VisualizeEffectData AttackEffectData = new CardVisualizeEffect.VisualizeEffectData();
        AttackEffectData.m_DistanceToSource = 1.0f;
        AttackEffectData.m_EffectTime = 1.0f;

        m_VisualizeEffect.InitVisualizeEffect(gameObject, ref AttackEffectData);
    }

    public void OnCardAttacked()
    {
        m_VisualizeEffect = new CardHitEffect();

        CardVisualizeEffect.VisualizeEffectData HitEffectData = new CardVisualizeEffect.VisualizeEffectData();
        HitEffectData.m_DistanceToSource = 0.3f;
        HitEffectData.m_VibrationCount = 4;

        m_VisualizeEffect.InitVisualizeEffect(gameObject, ref HitEffectData);
    }
}
