using System.Reflection;
using NSubstitute.Analyzers.Benchmarks.Shared;
using NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.DiagnosticsSources;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Benchmarks.VisualBasic
{
    public class VisualBasicDiagnosticAnalyzersBenchmarks : AbstractDiagnosticAnalyzersBenchmarks
    {
        protected override AnalyzerBenchmark CallInfoAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark ConflictingArgumentAssignmentsAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark NonSubstitutableMemberAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark NonSubstitutableMemberReceivedAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark NonSubstitutableMemberWhenAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark ReEntrantSetupAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark SubstituteAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark UnusedReceivedAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark ArgumentMatcherAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark ReceivedInReceivedInOrderAnalyzerBenchmark { get; }

        protected override AnalyzerBenchmark AsyncReceivedInOrderCallbackAnalyzerBenchmark { get; }

        protected override AbstractSolutionLoader SolutionLoader { get; }

        protected override string SourceProjectFolderName { get; }

        protected override Assembly BenchmarkSourceAssembly { get; }

        public VisualBasicDiagnosticAnalyzersBenchmarks()
        {
            SolutionLoader = new VisualBasicSolutionLoader();
            SourceProjectFolderName = "NSubstitute.Analyzers.Benchmarks.Source.VisualBasic";
            BenchmarkSourceAssembly = typeof(NonSubstitutableMemberDiagnosticSource).Assembly;

            CallInfoAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new CallInfoAnalyzer());
            ConflictingArgumentAssignmentsAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new ConflictingArgumentAssignmentsAnalyzer());
            NonSubstitutableMemberAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new NonSubstitutableMemberAnalyzer());
            NonSubstitutableMemberReceivedAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new NonSubstitutableMemberReceivedAnalyzer());
            NonSubstitutableMemberWhenAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new NonSubstitutableMemberWhenAnalyzer());
            ReEntrantSetupAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new ReEntrantSetupAnalyzer());
            SubstituteAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new SubstituteAnalyzer());
            UnusedReceivedAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new UnusedReceivedAnalyzer());
            ArgumentMatcherAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new NonSubstitutableMemberArgumentMatcherAnalyzer());
            ReceivedInReceivedInOrderAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new ReceivedInReceivedInOrderAnalyzer());
            AsyncReceivedInOrderCallbackAnalyzerBenchmark = AnalyzerBenchmark.CreateBenchmark(Solution, new AsyncReceivedInOrderCallbackAnalyzer());
        }
    }
}