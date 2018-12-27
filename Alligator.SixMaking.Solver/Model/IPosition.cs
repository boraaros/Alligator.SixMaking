using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.SixMaking.Model
{
    public interface IPosition : IPosition<Ply>
    {
        Disk Next { get; }
        Ply LastPly { get; }
        IList<Ply> History { get; }
        int ColumnHeightAt(int cell);
        Disk DiskAt(int cell, int height);
    }
}