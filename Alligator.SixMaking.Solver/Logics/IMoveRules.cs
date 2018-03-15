using Alligator.SixMaking.Model;
using System;
using System.Collections.Generic;

namespace Alligator.SixMaking.Logics
{
    public interface IMoveRules
    {
        IEnumerable<int> MovesFrom(IPosition position, int from);
        bool AreInverses(Ply one, Ply other);
    }
}
