using System;

namespace Alligator.SixMaking.Model
{
    public sealed class InsertPly : Ply
    {
        private const int FromIndex = -1;
        private const int DiskCount = 1;

        private InsertPly(int to, int index)
            : base(FromIndex, to, DiskCount, index)
        {
        }

        public override string ToString()
        {
            return string.Format("-->{0}", CellToString(to));
        }
    }
}
