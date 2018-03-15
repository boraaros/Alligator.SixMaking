using Alligator.Solver;
using System;

namespace Alligator.SixMaking.Model
{
    public interface IPosition : IPosition<Ply>
    {
        Disk Next { get; }
        Ply LastPly { get; }
        int ColumnHeightAt(int cell);
        Disk DiskAt(int cell, int height);
    }
}
