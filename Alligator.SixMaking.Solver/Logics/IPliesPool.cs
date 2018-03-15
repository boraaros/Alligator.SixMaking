using Alligator.SixMaking.Model;
using System;

namespace Alligator.SixMaking.Logics
{
    public interface IPliesPool
    {
        int Size { get; }
        Ply GetInsertPly(int to);
        Ply GetMovePly(int from, int to, int count);
    }
}
