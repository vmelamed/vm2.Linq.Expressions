// SPDX-License-Identifier: MIT

namespace vm2.Linq.Expressions.Tests;

public class Linq.ExpressionsApiTests
{
    [Fact]
    public void Echo_returns_value_when_present()
    {
        var result = Linq.ExpressionsApi.Echo("hi", "fallback");
        result.Should().Be("hi");
    }

    [Fact]
    public void Echo_returns_fallback_when_null()
    {
        var result = Linq.ExpressionsApi.Echo(null, "fallback");
        result.Should().Be("fallback");
    }
}
