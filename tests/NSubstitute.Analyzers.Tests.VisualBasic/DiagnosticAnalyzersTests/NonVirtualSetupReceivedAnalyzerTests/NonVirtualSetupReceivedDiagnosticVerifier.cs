﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupReceivedAnalyzerTests
{
    public abstract class NonVirtualSetupReceivedDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonVirtualSetupReceivedDiagnosticVerifier
    {
        protected DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualReceivedSetupSpecification;

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer(string method);

        public abstract Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupReceivedAnalyzer();
        }
    }
}