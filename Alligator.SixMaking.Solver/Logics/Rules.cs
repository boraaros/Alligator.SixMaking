using Alligator.SixMaking.Model;
using Alligator.Solver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.SixMaking.Logics
{
    public class Rules : IRules<IPosition, Ply>
    {
        protected readonly IPliesPool pliesPool;
        protected readonly IMoveRules moveRules;

        private readonly Disk own;

        public Rules(IPliesPool pliesPool, IMoveRules moveRules, Disk own)
        {
            this.pliesPool = pliesPool ?? throw new ArgumentNullException(nameof(pliesPool));
            this.moveRules = moveRules ?? throw new ArgumentNullException(nameof(moveRules));
            this.own = own;
        }

        private bool IsEnded(IPosition position)
        {
            return Enumerable.Range(0, 25).Any(t => position.ColumnHeightAt(t) > 5);
        }

        public IEnumerable<Ply> LegalMovesAt(IPosition position)
        {
            var result = new List<Ply>();
            if (IsEnded(position))
            {
                return result;
            }
            for (int cell = 0; cell < Constants.BoardSize * Constants.BoardSize; cell++)
            {
                var columnHeight = position.ColumnHeightAt(cell);

                if (columnHeight == 0)
                {
                    result.Add(pliesPool.GetInsertPly(cell));
                }
                else
                {
                    var isOwn = position.Next == position.DiskAt(cell, columnHeight - 1); // Todo: ez így jó?

                    foreach (var to in moveRules.MovesFrom(position, cell))
                    {
                        for (int diskCount = columnHeight; diskCount > 0; diskCount--)
                        {
                            var ply = pliesPool.GetMovePly(cell, to, diskCount);

                            if (!moveRules.AreInverses(position.LastPly, ply))
                            {
                                if (position.ColumnHeightAt(to) > Constants.WinnerHeight - diskCount - 1)
                                {
                                    if (isOwn)
                                    {
                                        return new List<Ply>() { ply };
                                    }
                                }
                                else
                                { 
                                    result.Add(ply);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public IPosition InitialPosition()
        {
            return new Position();
        }

        public bool IsGoal(IPosition position)
        {
            return Enumerable.Range(0, 25).Any(t => position.ColumnHeightAt(t) > 5);
        }
    }
}