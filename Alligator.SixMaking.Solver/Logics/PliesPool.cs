using Alligator.SixMaking.Model;
using System;
using System.Reflection;

namespace Alligator.SixMaking.Logics
{
    public sealed class PliesPool : Singleton<PliesPool>, IPliesPool
    {
        private int poolSize = 0;

        private readonly Ply[] insertPlies;
        private readonly Ply[][][] movePlies;

        public int Size
        {
            get { return poolSize; }
        }

        private PliesPool()
        {
            int totalSize = Constants.BoardSize * Constants.BoardSize;

            insertPlies = new Ply[totalSize];
            CreateInsertPlyPool();

            movePlies = new Ply[totalSize][][];
            CreateMovePlyPool();
        }

        private void CreateInsertPlyPool()
        {
            // the types of the constructor parameters, in order
            Type[] paramTypes = new Type[] { typeof(int), typeof(int) };

            for (int i = 0; i < insertPlies.Length; i++)
            {
                // the values of the constructor parameters, in order
                object[] paramValues = new object[] { i, poolSize++ };
                insertPlies[i] = Construct<InsertPly>(paramTypes, paramValues);
            }
        }

        private void CreateMovePlyPool()
        {
            int totalSize = Constants.BoardSize * Constants.BoardSize;

            // the types of the constructor parameters, in order
            Type[] paramTypes = new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) };

            for (int from = 0; from < totalSize; from++)
            {
                movePlies[from] = new Ply[totalSize][];

                for (int to = 0; to < totalSize; to++)
                {
                    if (!IsValidMove(from, to))
                    {
                        continue;
                    }
                    movePlies[from][to] = new Ply[Constants.WinnerHeight];

                    for (int count = 1; count < Constants.WinnerHeight; count++)
                    {
                        // the values of the constructor parameters, in order
                        object[] paramValues = new object[] { from, to, count, poolSize++ };
                        movePlies[from][to][count] = Construct<MovePly>(paramTypes, paramValues);
                    }
                }
            }
        }

        private bool IsValidMove(int from, int to)
        {
            return from != to;
        }

        private T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            Type t = typeof(T);
            ConstructorInfo ci = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null);
            return (T)ci.Invoke(paramValues);
        }

        public Ply GetInsertPly(int to)
        {
            return insertPlies[to];
        }

        public Ply GetMovePly(int from, int to, int count)
        {
            return movePlies[from][to][count];
        }
    }
}
