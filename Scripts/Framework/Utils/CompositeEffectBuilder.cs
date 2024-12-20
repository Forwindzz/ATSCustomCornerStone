using ATS_API.Effects;
using Eremite.Model;
using Eremite.Model.Effects;
using ForwindzCustomPerks.Scripts.Framework.Utils.Extend;
using System.Collections.Generic;
using System.Linq;

//TODO: replace with API CompositeEffectBuilder

namespace Forwindz.Framework.Utils
{
    public class CompositeEffectBuilder : EffectBuilder<CompositeEffectModel>
    {
        public EffectModel[] Effects
        {
            get => m_effectModel.rewards;
            set => SetEffects(value);
        }

        public CompositeEffectBuilder(string guid, string name, string iconPath) : base(guid, name, iconPath)
        {
            m_effectModel.anyNestedToFit = false;
            m_effectModel.rewards = [];
        }

        /// <summary>
        /// Add an effect to the composite effect
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffect(EffectModel effect)
        {
            m_effectModel.rewards.ForceAdd(effect);
        }

        /// <summary>
        /// Add a series of effects to the composite effect
        /// </summary>
        /// <param name="effects"></param>
        public void AddEffects(IEnumerable<EffectModel> effects)
        {
            // Add Extend Method AddRange for Array?
            List<EffectModel> effectList = m_effectModel.rewards.ToList();
            effectList.AddRange(effects.ToList());
            m_effectModel.rewards = effectList.ToArray();
        }

        public void AddEffects(EffectModel[] effects)
        {
            // Add Extend Method AddRange for Array?
            List<EffectModel> effectList = m_effectModel.rewards.ToList();
            effectList.AddRange(effects.ToList());
            m_effectModel.rewards = effectList.ToArray();
        }

        /// <summary>
        /// Remove current effects, and set with a new effect
        /// </summary>
        /// <param name="effect"></param>
        public void SetEffect(EffectModel effect)
        {
            m_effectModel.rewards = [effect];
        }

        /// <summary>
        /// Remove current effects, and set with new effects
        /// </summary>
        /// <param name="effects"></param>
        public void SetEffects(IEnumerable<EffectModel> effects)
        {
            m_effectModel.rewards = effects.ToArray();
        }

        public void SetEffects(EffectModel[] effects)
        {
            m_effectModel.rewards = effects;
        }

        /// <summary>
        /// Clean all effects in the composite effect
        /// </summary>
        public void ClearEffects()
        {
            m_effectModel.rewards = [];
        }

        public void SetDescriptionArgs(params (TextArgType type, int sourceIndex)[] args)
        {
            m_effectModel.formatDescription = true;
            m_effectModel.dynamicDescriptionArgs = args.ToTextArgArray();
        }

        /// <summary>
        /// Normally the composite effect fits only if 
        /// all nested effects fits as well. Use this 
        /// option to change it so if only one of the 
        /// nested effects fits - this effect can be drawn.
        /// </summary>
        /// <param name="value"></param>
        public void SetAnyNestedToFit(bool value)
        {
            m_effectModel.anyNestedToFit = value;
        }

        /// <summary>
        /// This indicates which effect we should use as a preview.
        /// If you do not want to provide a nested preview,
        /// code like this: 
        /// `SetNestedPreviewIndex(-1);`
        /// </summary>
        /// <param name="index">
        /// if the index is negative, it indicates the 
        /// compsite effect ignores nested preview. 
        /// Otherwise the index indicates we use which sub-effect
        /// as the composite effect preview </param>
        public void SetNestedPreviewIndex(int index)
        {
            if(index<0)
            {
                m_effectModel.hasNestedPreview = false;
            }
            else
            {
                m_effectModel.hasNestedPreview = true;
                m_effectModel.nestedPreviewIndex = index;
            }
        }

        public void SetNestedRetroactivePreviewIndex(int index)
        {
            if (index < 0)
            {
                m_effectModel.hasNestedRetroactivePreview = false;
            }
            else
            {
                m_effectModel.hasNestedRetroactivePreview = true;
                m_effectModel.nestedRetroactivePreviewEffectIndex = index;
            }
        }

        public void SetNestedStatePreviewIndex(int index)
        {
            if (index < 0)
            {
                m_effectModel.hasNestedStatePreview = false;
            }
            else
            {
                m_effectModel.hasNestedStatePreview = true;
                m_effectModel.nestedStatePreviewEffectIndex = index;
            }
        }

        public void SetNestedAmountIndex(int index)
        {
            if (index < 0)
            {
                m_effectModel.hasNestedAmount = false;
            }
            else
            {
                m_effectModel.hasNestedAmount = true;
                m_effectModel.nestedAmountEffectIndex = index;
            }
        }

        public void SetNestedIntAmountIndex(int index)
        {
            if (index < 0)
            {
                m_effectModel.hasNestedIntAmount = false;
            }
            else
            {
                m_effectModel.hasNestedIntAmount = true;
                m_effectModel.nestedIntAmountEffectIndex = index;
            }
        }

        public void SetNestedFloatAmountIndex(int index)
        {
            if (index < 0)
            {
                m_effectModel.hasNestedFloatAmount = false;
            }
            else
            {
                m_effectModel.hasNestedFloatAmount = true;
                m_effectModel.nestedFloatAmountEffectIndex = index;
            }
        }

        public void SetShowEffectAsPerks(bool value)
        {
            m_effectModel.showEffectsAsPerks = value;
        }
    }
}
