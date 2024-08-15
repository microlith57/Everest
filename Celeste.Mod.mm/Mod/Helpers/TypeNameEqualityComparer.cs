using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod
{
    /// <summary>
    /// Used by the PatchTypeDictionaryComparer rule to allow debuggers to inspect dictionaries with types as keys.
    /// </summary>
    public class TypeNameEqualityComparer : EqualityComparer<Type>
    {
        public override bool Equals(Type x, Type y)
        {
            return (x == y) || x.FullName.Equals(y.FullName, StringComparison.Ordinal);
        }

        public override int GetHashCode(Type type) => type.FullName.GetHashCode();
    }
}
