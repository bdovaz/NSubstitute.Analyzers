﻿using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SubstituteForInternalMemberCodeFixProviderTests;

public class ForPartsOfMethodTests : SubstituteForInternalMemberCodeFixVerifier
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
                var substitute = Substitute.ForPartsOf<Foo>();
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
                var substitute = Substitute.ForPartsOf<Foo>();
            }
        }
    }
}";
        await VerifyFix(oldSource, newSource);
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
            var substitute = Substitute.ForPartsOf<Foo>();
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
            var substitute = Substitute.ForPartsOf<Foo>();
        }
    }
}";
        await VerifyFix(oldSource, newSource);
    }

    public override async Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty(
        int diagnosticIndex)
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
            var substitute = Substitute.ForPartsOf<Foo>();
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
            var substitute = Substitute.ForPartsOf<Foo>();
        }
    }
}";
        await VerifyFix(oldSource, newSource);
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
            var substitute = Substitute.ForPartsOf<Foo.Bar>();
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
            var substitute = Substitute.ForPartsOf<Foo.Bar>();
        }
    }
}";
        await VerifyFix(oldSource, newSource);
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
            var substitute = Substitute.ForPartsOf<Foo>();
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
            var substitute = Substitute.ForPartsOf<Foo>();
        }
    }
}";

        await VerifyFix(oldSource, oldSource);
    }
}