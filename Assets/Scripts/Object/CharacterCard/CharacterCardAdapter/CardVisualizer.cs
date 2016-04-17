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

        CardVisualizeEffectData AttackEffectData = new CardVisualizeEffectData.CardVisualizeEffectAttack(1.0f, 1.0f);
        m_VisualizeEffect.InitVisualizeEffect(gameObject, AttackEffectData);
    }

    public void OnCardAttacked()
    {
        m_VisualizeEffect = new CardHitEffect();

        CardVisualizeEffectData HitEffectData = new CardVisualizeEffectData.CardVisualizeEffectHit(4, 0.3f);
        m_VisualizeEffect.InitVisualizeEffect(gameObject, HitEffectData);
    }

    public void OnCardInAction(ref SpriteRenderer backgroundSpriteRenderer)
    {
        m_VisualizeEffect = new CardGlowingEffect();

        CardVisualizeEffectData inActionEffectData = new CardVisualizeEffectData.CardVisualizeEffectGlowing(2.0f, 0.3f, Color.cyan);
        m_VisualizeEffect.InitVisualizeEffect(backgroundSpriteRenderer.gameObject, inActionEffectData);
    }

    public void OnCardSelectedInTarget(ref SpriteRenderer backgroundSpriteRenderer)
    {
        m_VisualizeEffect = new CardGlowingEffect();

        CardVisualizeEffectData selectedEffectData = new CardVisualizeEffectData.CardVisualizeEffectGlowing(2.0f, 0.3f, Color.magenta);
        m_VisualizeEffect.InitVisualizeEffect(backgroundSpriteRenderer.gameObject, selectedEffectData);
    }

    public void ForceQuitVisualizeEffect()
    {
        if(m_VisualizeEffect != null)
        {
            m_VisualizeEffect.ForceQuitVisualizeEffect();
            m_VisualizeEffect = null;
        }
    }

    public CardVisualizeEffect.VisualizeEffectStatus GetVisualizeEffectStatus()
    {
        if(m_VisualizeEffect != null)
        {
            return m_VisualizeEffect.GetVisualizeEffectStatus();
        }

        return CardVisualizeEffect.VisualizeEffectStatus.Idle;
    }
}
