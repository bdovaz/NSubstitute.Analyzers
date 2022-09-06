﻿using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SubstituteForInternalMemberCodeFixProviderTests;

public class ForAsNonGenericMethodTests : SubstituteForInternalMemberCodeFixVerifier
{
    public override async Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass(int diagnosticIndex)
    {
        var oldSource = @"using NSubstitute;
namespace MyNamespace
{
    namespace MyInnerNamespace
    {
        internal class Foo
        {
        }

        public class FooTests
        {
            public void Test()
            {
                var substitute = Substitute.For(new[] {typeof(Foo)}, null);
                var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
                var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
            }
        }
    }
}";
        var newSource = @"using NSubstitute;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    namespace MyInnerNamespace
    {
        internal class Foo
        {
        }

        public class FooTests
        {
            public void Test()
            {
                var substitute = Substitute.For(new[] {typeof(Foo)}, null);
                var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
                var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
            }
        }
    }
}";
        await VerifyFix(oldSource, newSource, diagnosticIndex: diagnosticIndex);
    }

    public override async Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass(int diagnosticIndex)
    {
        var oldSource = @"using NSubstitute;
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo)}, null);
            var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
            var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
        }
    }
}";
        var newSource = @"using NSubstitute;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo)}, null);
            var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
            var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
        }
    }
}";
        await VerifyFix(oldSource, newSource, diagnosticIndex: diagnosticIndex);
    }

    public override async Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty(int diagnosticIndex)
    {
        var oldSource = @"using System.Reflection;
using NSubstitute;
[assembly: AssemblyVersion(""1.0.0"")]
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo)}, null);
            var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
            var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
        }
    }
}";
        var newSource = @"using System.Reflection;
using NSubstitute;
[assembly: AssemblyVersion(""1.0.0"")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo)}, null);
            var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo)}, constructorArguments: null);
            var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo)});
        }
    }
}";
        await VerifyFix(oldSource, newSource, diagnosticIndex: diagnosticIndex);
    }

    public override async Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass(int diagnosticIndex)
    {
        var oldSource = @"using NSubstitute;
namespace MyNamespace
{
    internal class Foo
    {
        internal class Bar
        {

        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo.Bar)}, null);
            var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo.Bar)}, constructorArguments: null);
            var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo.Bar)});
        }
    }
}";
        var newSource = @"using NSubstitute;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    internal class Foo
    {
        internal class Bar
        {

        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo.Bar)}, null);
            var otherSubstitute = Substitute.For(typesToProxy: new[] {typeof(Foo.Bar)}, constructorArguments: null);
            var yetAnotherSubstitute = Substitute.For(constructorArguments: null, typesToProxy: new[] {typeof(Foo.Bar)});
        }
    }
}";
        await VerifyFix(oldSource, newSource, diagnosticIndex: diagnosticIndex);
    }

    public override async Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass()
    {
        var oldSource = @"using NSubstitute;
namespace MyNamespace
{
    public class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo)}, null);
        }
    }
}";
        await VerifyFix(oldSource, oldSource);
    }

    public override async Task DoesNot_AppendsInternalsVisibleTo_WhenInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
    {
        var oldSource = @"using NSubstitute;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherSecondAssembly"")]

namespace MyNamespace
{
    internal class Foo
    {
        internal class Bar
        {

        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For(new[] {typeof(Foo)}, null);
        }
    }
}";

        await VerifyFix(oldSource, oldSource);
    }
}