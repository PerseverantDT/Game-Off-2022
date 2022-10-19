using System;
using Enums;

namespace Util
{
    public readonly struct AttributeModifier : IEquatable<AttributeModifier>
    {
        public float Value { get; }
        public AttributeModifierOperation Operation { get; }

        public override bool Equals(object obj)
        {
            return obj is AttributeModifier other
                   && Math.Abs(other.Value - Value) < float.Epsilon
                   && other.Operation == Operation;
        }
        public override int GetHashCode() => (Value, Operation).GetHashCode();
        public bool Equals(AttributeModifier other)
        {
            return Math.Abs(other.Value - Value) < float.Epsilon
                   && other.Operation == Operation;
        }

        public AttributeModifier(float value, AttributeModifierOperation operation)
        {
            Value = value;
            Operation = operation;
        }
        public AttributeModifier(float value, string operation)
        {
            Value = value;
            if (Enum.TryParse(operation, true, out AttributeModifierOperation op))
                Operation = op;
            else throw new ArgumentException("Unknown attribute modifier operation. operation can only be of the following values: " + string.Join(',', Enum.GetValues(typeof(AttributeModifierOperation))));
        }
    }
}