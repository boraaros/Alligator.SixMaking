using Alligator.Solver;
using System;

namespace Alligator.SixMaking.Demo
{
    public class SolverConfiguration : ISolverConfiguration
    {
        public TimeSpan TimeLimitPerMove => TimeSpan.FromSeconds(3);

        public int MaxDegreeOfParallelism => 1;     
    }
}