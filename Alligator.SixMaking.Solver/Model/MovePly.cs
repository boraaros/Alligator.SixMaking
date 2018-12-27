using System;

namespace Alligator.SixMaking.Model
{
    public sealed class MovePly : Ply
    {
        private const Disk EmptyDisk = Disk.None;

        private MovePly(int from, int to, int count, int index)
            : base(from, to, count, index)
        {
        }

        public override string ToString()
        {
            return string.Format("{1}--{0}->{2}", count, CellToString(from), CellToString(to));
        }
    }
}