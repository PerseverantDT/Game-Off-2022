using System;
using System.Collections.Generic;
using System.Linq;
using Enums;

namespace Util
{
    public struct Attribute : IEquatable<Attribute>
    {
        private readonly float baseValue;
        private readonly List<AttributeModifier> modifiers;
        private bool isCached;
        private float cachedValue;
        private float baseValueModifier;

        private float BaseValue => baseValue + baseValueModifier;
        public float Value
        {
            get
            {
                if (isCached) return cachedValue;
                if (modifiers.Count == 0)
                {
                    isCached = true;
                    cachedValue = BaseValue;
                    return BaseValue;
                }

                float addBaseEarly = 0f;
                float multBaseAdd = 1f;
                float multBaseMult = 1f;
                float addBaseLate = 0f;
                float minBase = float.NegativeInfinity;
                float maxBase = float.PositiveInfinity;
                float setBase = float.NaN;
                float addTotEarly = 0f;
                float multTotAdd = 1f;
                float multTotMult = 1f;
                float addTotLate = 0f;
                float minTot = float.NegativeInfinity;
                float maxTot = float.PositiveInfinity;
                float setTot = float.NaN;
                
                foreach (AttributeModifier modifier in modifiers)
                {
                    switch (modifier.Operation)
                    {
                        case AttributeModifierOperation.AddBaseEarly:
                            addBaseEarly += modifier.Value;
                            break;
                        case AttributeModifierOperation.MultiplyBaseAdditive:
                            multBaseAdd += modifier.Value;
                            break;
                        case AttributeModifierOperation.MultiplyBaseMultiplicative:
                            multBaseMult *= modifier.Value;
                            break;
                        case AttributeModifierOperation.AddBaseLate:
                            addBaseLate += modifier.Value;
                            break;
                        case AttributeModifierOperation.MinBase:
                            if (minBase < modifier.Value) minBase = modifier.Value;
                            break;
                        case AttributeModifierOperation.MaxBase:
                            if (maxBase > modifier.Value) maxBase = modifier.Value;
                            break;
                        case AttributeModifierOperation.SetBase:
                            setBase = modifier.Value;
                            break;
                        case AttributeModifierOperation.AddTotalEarly:
                            addTotEarly += modifier.Value;
                            break;
                        case AttributeModifierOperation.MultiplyTotalAdditive:
                            multTotAdd += modifier.Value;
                            break;
                        case AttributeModifierOperation.MultiplyTotalMultiplicative:
                            multTotMult *= modifier.Value;
                            break;
                        case AttributeModifierOperation.AddTotalLate:
                            addTotLate += modifier.Value;
                            break;
                        case AttributeModifierOperation.MinTotal:
                            if (minTot < modifier.Value) minTot = modifier.Value;
                            break;
                        case AttributeModifierOperation.MaxTotal:
                            if (maxTot > modifier.Value) maxTot = modifier.Value;
                            break;
                        case AttributeModifierOperation.SetTotal:
                            setTot = modifier.Value;
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }

                float value = BaseValue;
                value += addBaseEarly;
                value *= multBaseAdd;
                value *= multBaseMult;
                value += addBaseLate;
                if (value < minBase) value = minBase;
                if (value > maxBase) value = maxBase;
                if (!float.IsNaN(setBase)) value = setBase;
                value += addTotEarly;
                value *= multTotAdd;
                value *= multTotMult;
                value += addTotLate;
                if (value < minTot) value = minTot;
                if (value > maxTot) value = maxTot;
                if (!float.IsNaN(setTot)) value = setTot;

                isCached = true;
                cachedValue = value;
                return value;
            }
        }

        public void AddModifier(params AttributeModifier[] modifier)
        {
            modifiers.AddRange(modifier);
            isCached = false;
        }
        public void AddModifier(float value, AttributeModifierOperation operation)
        {
            modifiers.Add(new AttributeModifier(value, operation));
            isCached = false;
        }
        public void AddModifier(float value, string operation)
        {
            modifiers.Add(new AttributeModifier(value, operation));
            isCached = false;
        }
        public void RemoveModifier(params AttributeModifier[] modifier)
        {
            foreach (AttributeModifier mod in modifier)
            {
                if (modifiers.Remove(mod)) isCached = false;
            }
        }
        public void RemoveModifier(float value, AttributeModifierOperation operation)
        {
            int idx = modifiers.FindIndex(mod => Math.Abs(mod.Value - value) < float.Epsilon && mod.Operation == operation);
            if (idx == -1) return;
            
            modifiers.RemoveAt(idx);
            isCached = false;
        }
        public void RemoveModifier(float value, string operation)
        {
            int idx = modifiers.FindIndex(mod => Math.Abs(mod.Value - value) < float.Epsilon && mod.Operation == Enum.Parse<AttributeModifierOperation>(operation));
            if (idx == -1) return;
            
            modifiers.RemoveAt(idx);
            isCached = false;
        }
        public void SetBaseValue(float value)
        {
            baseValueModifier = value - baseValue;
            isCached = false;
        }
        public void ChangeBaseValue(float change)
        {
            baseValueModifier += change;
            isCached = false;
        }

        public override bool Equals(object obj)
        {
            List<AttributeModifier> tempModifiers = modifiers;
            
            if (obj is not Attribute other) return false;
            if (Math.Abs(other.baseValue - baseValue) > float.Epsilon) return false;
            if (Math.Abs(other.baseValueModifier - baseValueModifier) > float.Epsilon) return false;

            if (tempModifiers.Any(modifier => !other.modifiers.Contains(modifier))) return false;
            if (other.modifiers.Any(modifier => !tempModifiers.Contains(modifier))) return false;

            return true;
        }
        public override int GetHashCode()
        {
            int hash = baseValue.GetHashCode() ^ baseValueModifier.GetHashCode();
            return modifiers.Aggregate(hash, (current, modifier) => current ^ modifier.GetHashCode());
        }
        public bool Equals(Attribute other)
        {
            List<AttributeModifier> tempModifiers = modifiers;
            
            if (Math.Abs(other.baseValue - baseValue) > float.Epsilon) return false;
            if (Math.Abs(other.baseValueModifier - baseValueModifier) > float.Epsilon) return false;
            
            if (tempModifiers.Any(modifier => !other.modifiers.Contains(modifier))) return false;
            if (other.modifiers.Any(modifier => !tempModifiers.Contains(modifier))) return false;

            return true;
        }

        public Attribute(float baseValue)
        {
            this.baseValue = baseValue;
            modifiers = new List<AttributeModifier>();
            isCached = true;
            cachedValue = baseValue;
            baseValueModifier = 0f;
        }
    }
}