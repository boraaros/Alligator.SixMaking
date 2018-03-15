using Alligator.Solver;
using System;

namespace Alligator.SixMaking.Demo
{
    public class SolverConfiguration : ISolverConfiguration
    {
        public TimeSpan TimeLimitPerMove => TimeSpan.FromSeconds(3);

        public int SearchDepthLimit => 20;

        public int QuiescenceExtensionLimit => 0;

        public int EvaluationTableSizeExponent => 16;

        public int EvaluationTableRetryLimit => 0;

        public int TranspositionTableSizeExponent => 24;

        public int TranspositionTableRetryLimit => 1;

        public int MinimumSearchDepthToUseMtdf => 99;
    }
}
