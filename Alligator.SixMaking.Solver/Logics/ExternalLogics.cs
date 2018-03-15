using Alligator.SixMaking.Model;
using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.SixMaking.Logics
{
    public class ExternalLogics : IExternalLogics<IPosition, Ply>
    {
        protected readonly IPliesPool pliesPool;
        protected readonly IMoveRules moveRules;

        private readonly Disk own;

        public ExternalLogics(IPliesPool pliesPool, IMoveRules moveRules, Disk own)
        {
            this.pliesPool = pliesPool ?? throw new ArgumentNullException(nameof(pliesPool));
            this.moveRules = moveRules ?? throw new ArgumentNullException(nameof(moveRules));
            this.own = own;
        }

        private readonly int[] FigureFactor = new int[]
        {
            50, 100, 250, 500, 1000
        };

        private readonly int CoverFactor = 3;

        private static int[] PawnPositionFactor = new int[]
        {
            2, 3, 3, 3, 2,
            3, 4, 4, 4, 3,
            3, 4, 4, 4, 3,
            3, 4, 4, 4, 3,
            2, 3, 3, 3, 2
        };

        private static int[] RookPositionFactor = new int[]
        {
            8, 8, 8, 8, 8,
            8, 8, 8, 8, 8,
            8, 8, 8, 8, 8,
            8, 8, 8, 8, 8,
            8, 8, 8, 8, 8
        };

        private static int[] KnightPositionFactor = new int[]
        {
            2, 3, 4, 3, 2,
            3, 4, 6, 4, 3,
            4, 6, 8, 6, 4,
            3, 4, 6, 4, 3,
            2, 3, 4, 3, 2
        };

        private static int[] BishopPositionFactor = new int[]
        {
            4, 4, 4, 4, 4,
            4, 6, 6, 6, 4,
            4, 6, 8, 6, 4,
            4, 6, 6, 6, 4,
            4, 4, 4, 4, 4
        };

        private static int[] QueenPositionFactor = new int[]
        {
            12, 12, 12, 12, 12,
            12, 14, 14, 14, 12,
            12, 14, 16, 14, 12,
            12, 14, 14, 14, 12,
            12, 12, 12, 12, 12
        };

        private static int[][] PositionsFactor = new int[][]
        {
            PawnPositionFactor,
            RookPositionFactor,
            KnightPositionFactor,
            BishopPositionFactor,
            QueenPositionFactor
        };

        private readonly int AttackFactor = 1;
        private readonly int DefenseFactor = 1;

        public IEnumerable<Ply> GetStrategiesFrom(IPosition position)
        {
            var result = new List<Ply>();
            if (position.IsEnded)
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

        public int StaticEvaluate(IPosition position)
        {
            var covers = new int[25];

            var utility = 0;

            for (int i = 0; i < 25; i++)
            {
                var height = position.ColumnHeightAt(i);

                if (height == 0)
                {
                    continue;
                }

                var ownColumn = own == position.DiskAt(i, height - 1);

                var feasibleMoves = moveRules.MovesFrom(position, i);

                var factor = 1;

                foreach (var to in feasibleMoves)
                {
                    if (position.ColumnHeightAt(to) > 5 - height)
                    {
                        if (ownColumn)
                        {
                            return int.MaxValue - 100; // todo!!
                        }
                        utility -= 1000 * factor++ * (height + position.ColumnHeightAt(to));
                    }
                    else
                    {
                        covers[to] = (own == position.DiskAt(i, height - 1)) ? covers[to] + 1 : covers[to] - 1;
                    }
                }
            }

            for (int i = 0; i < 25; i++)
            {
                var h = position.ColumnHeightAt(i);

                if (h > 0)
                {
                    utility += FigureFactor[h - 1] * PositionsFactor[h - 1][i] * covers[i];
                }

                for (int k = 0; k < h; k++)
                {
                    if (own == position.DiskAt(i, k))
                    {
                        utility += (FigureFactor[k] * PositionsFactor[k][i] * AttackFactor * (k == h - 1 ? CoverFactor : 1));
                    }
                    else
                    {
                        utility -= (FigureFactor[k] * PositionsFactor[k][i] * DefenseFactor * (k == h - 1 ? CoverFactor : 1));
                    }
                }
            }
            return utility;
        }

        public IPosition CreateEmptyPosition()
        {
            return new Position();
        }
    }
}
