using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.SixMaking.Model
{
    public class Position : IPosition
    {
        private Disk[,] board;
        private int[] heights;
        private Disk winner;
        private IList<Ply> history;
        private Disk next;

        private const int HashParamsLength = 251;
        private readonly IHashing hashing = new ZobristHashing(HashParamsLength);

        public Position()
        {
            board = new Disk[Constants.BoardSize * Constants.BoardSize, 10]; // TODO: magic numbers!
            heights = new int[Constants.BoardSize * Constants.BoardSize];
            winner = Disk.None;
            history = new List<Ply>();
            next = Disk.Red;
        }

        public Position(IList<Ply> history)
            : this()
        {
            foreach (Ply ply in history)
            {
                Do(ply);
            }
        }

        public Disk Next
        {
            get { return next; }
        }

        public ulong Identifier
        {
            get { return hashing.HashCode + /*(LastPly != null ? (ulong)LastPly.Identifier :*/ 0UL; }// TODO: dirty hack!
        }

        public Ply LastPly
        {
            get { return history.LastOrDefault(); }
        }

        public int ColumnHeightAt(int position)
        {
            return heights[position];
        }

        public Disk DiskAt(int position, int height)
        {
            return board[position, height];
        }

        public bool IsQuiet
        {
            get { return true; }
        }

        public bool IsEnded
        {
            get { return winner != Disk.None; } // nem jó
        }

        public IList<Ply> History
        {
            get { return history; }
        }

        public void Do(Ply ply)
        {
            if (winner != Disk.None)
            {
                throw new InvalidOperationException("Closed state");
            }
            if (ply.From == -1)
            {
                board[ply.To, 0] = next;
                heights[ply.To] = 1;
                hashing.Modify(ZobristIndex(ply.To, 0, next));
            }
            else
            {
                Move(ply.From, ply.To, ply.Count);
            }
            history.Add(ply);
            ChangeNext();
        }

        public void Undo()
        {
            if (history.Count == 0)
            {
                throw new InvalidOperationException("Empty state");
            }
            ChangeNext();
            var ply = history[history.Count - 1];
            if (ply.From == -1)
            {
                board[ply.To, 0] = Disk.None;
                heights[ply.To] = 0;
                hashing.Modify(ZobristIndex(ply.To, 0, next));
            }
            else
            {
                Move(ply.To, ply.From, ply.Count);
            }
            history.RemoveAt(history.Count - 1);   
        }

        private void Move(int from, int to, int count)
        {
            IList<int> indices = new List<int>();
            for (int i = heights[from] - count; i < heights[from]; i++)
            {
                var zi1 = ZobristIndex(to, heights[to], board[from, i]);
                if (zi1 != -1)
                    indices.Add(zi1);

                var zi2 = ZobristIndex(from, i, board[from, i]);
                if (zi2 != -1)
                    indices.Add(zi2);

                board[to, heights[to]++] = board[from, i];
                board[from, i] = Disk.None;
            }
            heights[from] -= count;
            winner = heights[to] > 5 ? board[to, heights[to] - 1] : Disk.None;
            hashing.Modify(indices.ToArray());
        }

        private void ChangeNext()
        {
            next = 3 - next;
            hashing.Modify(HashParamsLength - 1);
        }

        private int ZobristIndex(int position, int height, Disk disk)
        {
            if (height > 5)
            {
                return -1;
            }
            if (disk == Disk.Red)
            {
                return 5 * position + height;
            }
            if (disk == Disk.Yellow)
            {
                return 125 + 5 * position + height;
            }
            return -1;
        }

        public bool HasWinner
        {
            get { return winner != Disk.None; }
        }
    }
}
