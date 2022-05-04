using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ConflictingArgumentAssignmentsAnalyzerTests;

[CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns(Of Integer)", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)")]
public class ReturnsAsOrdinaryMethodTests : ConflictingArgumentAssignmentsDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo) 1, Function(callInfo)
               {previousCallArgAccess}
                Return 1
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(value:= {call}, returnThis:= Function(callInfo)
               {previousCallArgAccess}
                Return 1
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(returnThis:= Function(callInfo)
               {previousCallArgAccess}
                Return 1
            End Function, value:= {call}).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, Descriptor);
    }

    public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim configuredCall = {method}(substitute.Bar(Arg.Any(Of Integer)()), Function(callInfo)
                                                                                               callInfo(0) = 1
                                                                                               Return 1
                                                                                           End Function)

            Dim otherCall = {method}(value:= substitute.Bar(Arg.Any(Of Integer)()), returnThis:= Function(callInfo)
                                                                                               callInfo(0) = 1
                                                                                               Return 1
                                                                                           End Function)

            Dim yetAnotherCall = {method}(returnThis:= Function(callInfo)
                                                                    callInfo(0) = 1
                                                                    Return 1
                                                                 End Function, value:= substitute.Bar(Arg.Any(Of Integer)()))
            configuredCall.AndDoes(Function(callInfo)
                                       callInfo(0) = 1
                                   End Function)

            otherCall.AndDoes(Function(callInfo)
                                       callInfo(0) = 1
                                   End Function)

            yetAnotherCall.AndDoes(Function(callInfo)
                                       callInfo(0) = 1
                                   End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method)
    {
        var source = $@"Imports System
Imports System.Runtime.CompilerServices
Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(Arg.Any(Of Integer)()), Function(callInfo)
                callInfo(0) = 1
                Return 1
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function,
            Function(callInfo)
                callInfo(0) = 1
            End Function)
        End Sub
    End Class

    Module MyExtensions
        <Extension()>
        Function AndDoes(ByVal [call] As ConfiguredCall, ByVal firstCall As Action(Of CallInfo), ByVal secondCall As Action(Of CallInfo))
        End Function

    End Module

End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetsDifferentArgument_AsPreviousSetupMethod(string method, string call, string andDoesArgAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                callInfo(0) = 1
                Return 1
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(value:= {call}, returnThis:= Function(callInfo)
                callInfo(0) = 1
                Return 1
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(returnThis:= Function(callInfo)
                callInfo(0) = 1
                Return 1
            End Function, value:= {call}).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_AccessSameArguments_AsPreviousSetupMethod(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                {argAccess}
                Return 1
            End Function).AndDoes(Function(callInfo)
                {argAccess}
            End Function)

            {method}(value:= {call}, returnThis:= Function(callInfo)
                {argAccess}
                Return 1
            End Function).AndDoes(Function(callInfo)
                {argAccess}
            End Function)
            
            {method}(returnThis:= Function(callInfo)
                {argAccess}
                Return 1
            End Function, value:= {call}).AndDoes(Function(callInfo)
                {argAccess}
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetSameArguments_AsPreviousSetupMethod_SetsIndirectly(string method)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(Arg.Any(Of Integer)()), Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return 1
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function)

            {method}(value:= substitute.Bar(Arg.Any(Of Integer)()), returnThis:= Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return 1
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function)

            {method}(returnThis:= Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return 1
            End Function, value:= substitute.Bar(Arg.Any(Of Integer)())).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function)

        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_When_AndDoesMethod_SetArgument_AndPreviousMethod_IsNotUsedWithCallInfo(string method, string call, string andDoesArgAccess)
    {
        var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, 1).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(value:= {call}, returnThis:= 1).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(returnThis:= 1, value:= {call}).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}