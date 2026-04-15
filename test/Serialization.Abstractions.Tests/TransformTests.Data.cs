namespace vm2.Linq.Expressions.Serialization.Tests;

public partial class TransformTests
{
    public static TheoryData<string, string, string, IdentifierConventions, bool> TransformIdentifiersData => new() {
        { TestLine(), "",                         "",                 IdentifierConventions.Camel, true  },
        { TestLine(), " ",                        "",                 IdentifierConventions.Camel, true  },
        { TestLine(), " Abc",                     "",                 IdentifierConventions.Camel, true  },
        { TestLine(), "1BC",                      "",                 IdentifierConventions.Camel, true  },
        { TestLine(), "@__camel__Case__ ",        "",                 IdentifierConventions.Camel, true  },
        { TestLine(), "a",                        "a",                IdentifierConventions.Camel, false },
        { TestLine(), "A",                        "a",                IdentifierConventions.Camel, false },
        { TestLine(), "ABC",                      "aBC",              IdentifierConventions.Camel, false },
        { TestLine(), "CamelCase",                "camelCase",        IdentifierConventions.Camel, false },
        { TestLine(), "camelCase",                "camelCase",        IdentifierConventions.Camel, false },
        { TestLine(), "@enum",                    "enum",             IdentifierConventions.Camel, false },
        { TestLine(), "_CamelCase",               "camelCase",        IdentifierConventions.Camel, false },
        { TestLine(), "__Camel_Case",             "camelCase",        IdentifierConventions.Camel, false },
        { TestLine(), "__camel_case",             "camelCase",        IdentifierConventions.Camel, false },
        { TestLine(), "@__camel_case",            "camelCase",        IdentifierConventions.Camel, false },
        { TestLine(), "@__camel_case__",          "camelCase",        IdentifierConventions.Camel, false },
        { TestLine(), "@__8camel_case__",         "8camelCase",       IdentifierConventions.Camel, false },
        { TestLine(), "@__camel_7case__",         "camel7case",       IdentifierConventions.Camel, false },
        { TestLine(), "@__camel_a_case__",        "camelACase",       IdentifierConventions.Camel, false },
        { TestLine(), "@__camel_with_5_cases__",  "camelWith5Cases",  IdentifierConventions.Camel, false },

        { TestLine(), "",                         "",                 IdentifierConventions.Pascal, true  },
        { TestLine(), " ",                        "",                 IdentifierConventions.Pascal, true  },
        { TestLine(), " Abc",                     "",                 IdentifierConventions.Pascal, true  },
        { TestLine(), "1BC",                      "",                 IdentifierConventions.Pascal, true  },
        { TestLine(), "@__camel__Case__ ",        "",                 IdentifierConventions.Pascal, true  },
        { TestLine(), "a",                        "A",                IdentifierConventions.Pascal, false },
        { TestLine(), "A",                        "A",                IdentifierConventions.Pascal, false },
        { TestLine(), "ABC",                      "ABC",              IdentifierConventions.Pascal, false },
        { TestLine(), "CamelCase",                "CamelCase",        IdentifierConventions.Pascal, false },
        { TestLine(), "camelCase",                "CamelCase",        IdentifierConventions.Pascal, false },
        { TestLine(), "@enum",                    "Enum",             IdentifierConventions.Pascal, false },
        { TestLine(), "_CamelCase",               "CamelCase",        IdentifierConventions.Pascal, false },
        { TestLine(), "__Camel_Case",             "CamelCase",        IdentifierConventions.Pascal, false },
        { TestLine(), "__camel_case",             "CamelCase",        IdentifierConventions.Pascal, false },
        { TestLine(), "@__camel_case",            "CamelCase",        IdentifierConventions.Pascal, false },
        { TestLine(), "@__camel_case__",          "CamelCase",        IdentifierConventions.Pascal, false },
        { TestLine(), "@__8camel_case__",         "8camelCase",       IdentifierConventions.Pascal, false },
        { TestLine(), "@__camel_7case__",         "Camel7case",       IdentifierConventions.Pascal, false },
        { TestLine(), "@__camel_a_case__",        "CamelACase",       IdentifierConventions.Pascal, false },
        { TestLine(), "@__camel_with_5_cases__",  "CamelWith5Cases",  IdentifierConventions.Pascal, false },

        { TestLine(), "",                         "",                    IdentifierConventions.SnakeLower, true  },
        { TestLine(), " ",                        "",                    IdentifierConventions.SnakeLower, true  },
        { TestLine(), " Abc",                     "",                    IdentifierConventions.SnakeLower, true  },
        { TestLine(), "1BC",                      "",                    IdentifierConventions.SnakeLower, true  },
        { TestLine(), "@__camel__Case__ ",        "",                    IdentifierConventions.SnakeLower, true  },
        { TestLine(), "a",                        "a",                   IdentifierConventions.SnakeLower, false },
        { TestLine(), "A",                        "a",                   IdentifierConventions.SnakeLower, false },
        { TestLine(), "ABC",                      "a_b_c",               IdentifierConventions.SnakeLower, false },
        { TestLine(), "CamelCase",                "camel_case",          IdentifierConventions.SnakeLower, false },
        { TestLine(), "camelCase",                "camel_case",          IdentifierConventions.SnakeLower, false },
        { TestLine(), "@enum",                    "enum",                IdentifierConventions.SnakeLower, false },
        { TestLine(), "_CamelCase",               "camel_case",          IdentifierConventions.SnakeLower, false },
        { TestLine(), "__Camel_Case",             "camel_case",          IdentifierConventions.SnakeLower, false },
        { TestLine(), "__camel_case",             "camel_case",          IdentifierConventions.SnakeLower, false },
        { TestLine(), "@__camel_case",            "camel_case",          IdentifierConventions.SnakeLower, false },
        { TestLine(), "@__camel_case__",          "camel_case",          IdentifierConventions.SnakeLower, false },
        { TestLine(), "@__8camel_case__",         "8camel_case",         IdentifierConventions.SnakeLower, false },
        { TestLine(), "@__camel_7case__",         "camel_7case",         IdentifierConventions.SnakeLower, false },
        { TestLine(), "@__camel_a_case__",        "camel_a_case",        IdentifierConventions.SnakeLower, false },
        { TestLine(), "@__camel_with_5_cases__",  "camel_with_5_cases",  IdentifierConventions.SnakeLower, false },

        { TestLine(), "",                         "",                    IdentifierConventions.SnakeUpper, true  },
        { TestLine(), " ",                        "",                    IdentifierConventions.SnakeUpper, true  },
        { TestLine(), " Abc",                     "",                    IdentifierConventions.SnakeUpper, true  },
        { TestLine(), "1BC",                      "",                    IdentifierConventions.SnakeUpper, true  },
        { TestLine(), "@__camel__Case__ ",        "",                    IdentifierConventions.SnakeUpper, true  },
        { TestLine(), "a",                        "A",                   IdentifierConventions.SnakeUpper, false },
        { TestLine(), "A",                        "A",                   IdentifierConventions.SnakeUpper, false },
        { TestLine(), "ABC",                      "A_B_C",               IdentifierConventions.SnakeUpper, false },
        { TestLine(), "CamelCase",                "CAMEL_CASE",          IdentifierConventions.SnakeUpper, false },
        { TestLine(), "camelCase",                "CAMEL_CASE",          IdentifierConventions.SnakeUpper, false },
        { TestLine(), "@enum",                    "ENUM",                IdentifierConventions.SnakeUpper, false },
        { TestLine(), "_CamelCase",               "CAMEL_CASE",          IdentifierConventions.SnakeUpper, false },
        { TestLine(), "__Camel_Case",             "CAMEL_CASE",          IdentifierConventions.SnakeUpper, false },
        { TestLine(), "__camel_case",             "CAMEL_CASE",          IdentifierConventions.SnakeUpper, false },
        { TestLine(), "@__camel_case",            "CAMEL_CASE",          IdentifierConventions.SnakeUpper, false },
        { TestLine(), "@__camel_case__",          "CAMEL_CASE",          IdentifierConventions.SnakeUpper, false },
        { TestLine(), "@__8camel_case__",         "8CAMEL_CASE",         IdentifierConventions.SnakeUpper, false },
        { TestLine(), "@__camel_7case__",         "CAMEL_7CASE",         IdentifierConventions.SnakeUpper, false },
        { TestLine(), "@__camel_a_case__",        "CAMEL_A_CASE",        IdentifierConventions.SnakeUpper, false },
        { TestLine(), "@__camel_with_5_cases__",  "CAMEL_WITH_5_CASES",  IdentifierConventions.SnakeUpper, false },

        { TestLine(), "",                         "",                    IdentifierConventions.KebabLower, true  },
        { TestLine(), " ",                        "",                    IdentifierConventions.KebabLower, true  },
        { TestLine(), " Abc",                     "",                    IdentifierConventions.KebabLower, true  },
        { TestLine(), "1BC",                      "",                    IdentifierConventions.KebabLower, true  },
        { TestLine(), "@__camel__Case__ ",        "",                    IdentifierConventions.KebabLower, true  },
        { TestLine(), "a",                        "a",                   IdentifierConventions.KebabLower, false },
        { TestLine(), "A",                        "a",                   IdentifierConventions.KebabLower, false },
        { TestLine(), "ABC",                      "a-b-c",               IdentifierConventions.KebabLower, false },
        { TestLine(), "CamelCase",                "camel-case",          IdentifierConventions.KebabLower, false },
        { TestLine(), "camelCase",                "camel-case",          IdentifierConventions.KebabLower, false },
        { TestLine(), "@enum",                    "enum",                IdentifierConventions.KebabLower, false },
        { TestLine(), "_CamelCase",               "camel-case",          IdentifierConventions.KebabLower, false },
        { TestLine(), "__Camel_Case",             "camel-case",          IdentifierConventions.KebabLower, false },
        { TestLine(), "__camel_case",             "camel-case",          IdentifierConventions.KebabLower, false },
        { TestLine(), "@__camel_case",            "camel-case",          IdentifierConventions.KebabLower, false },
        { TestLine(), "@__camel_case__",          "camel-case",          IdentifierConventions.KebabLower, false },
        { TestLine(), "@__8camel_case__",         "8camel-case",         IdentifierConventions.KebabLower, false },
        { TestLine(), "@__camel_7case__",         "camel-7case",         IdentifierConventions.KebabLower, false },
        { TestLine(), "@__camel_a_case__",        "camel-a-case",        IdentifierConventions.KebabLower, false },
        { TestLine(), "@__camel_with_5_cases__",  "camel-with-5-cases",  IdentifierConventions.KebabLower, false },

        { TestLine(), "",                         "",                    IdentifierConventions.KebabUpper, true  },
        { TestLine(), " ",                        "",                    IdentifierConventions.KebabUpper, true  },
        { TestLine(), " Abc",                     "",                    IdentifierConventions.KebabUpper, true  },
        { TestLine(), "1BC",                      "",                    IdentifierConventions.KebabUpper, true  },
        { TestLine(), "@__camel__Case__ ",        "",                    IdentifierConventions.KebabUpper, true  },
        { TestLine(), "a",                        "A",                   IdentifierConventions.KebabUpper, false },
        { TestLine(), "A",                        "A",                   IdentifierConventions.KebabUpper, false },
        { TestLine(), "ABC",                      "A-B-C",               IdentifierConventions.KebabUpper, false },
        { TestLine(), "CamelCase",                "CAMEL-CASE",          IdentifierConventions.KebabUpper, false },
        { TestLine(), "camelCase",                "CAMEL-CASE",          IdentifierConventions.KebabUpper, false },
        { TestLine(), "@enum",                    "ENUM",                IdentifierConventions.KebabUpper, false },
        { TestLine(), "_CamelCase",               "CAMEL-CASE",          IdentifierConventions.KebabUpper, false },
        { TestLine(), "__Camel_Case",             "CAMEL-CASE",          IdentifierConventions.KebabUpper, false },
        { TestLine(), "__camel_case",             "CAMEL-CASE",          IdentifierConventions.KebabUpper, false },
        { TestLine(), "@__camel_case",            "CAMEL-CASE",          IdentifierConventions.KebabUpper, false },
        { TestLine(), "@__camel_case__",          "CAMEL-CASE",          IdentifierConventions.KebabUpper, false },
        { TestLine(), "@__8camel_case__",         "8CAMEL-CASE",         IdentifierConventions.KebabUpper, false },
        { TestLine(), "@__camel_7case__",         "CAMEL-7CASE",         IdentifierConventions.KebabUpper, false },
        { TestLine(), "@__camel_a_case__",        "CAMEL-A-CASE",        IdentifierConventions.KebabUpper, false },
        { TestLine(), "@__camel_with_5_cases__",  "CAMEL-WITH-5-CASES",  IdentifierConventions.KebabUpper, false },
    };

    class TestTypeNameConvention { }

    public static TheoryData<string, Type, string, TypeNameConventions, bool> TransformTypeNamesData => new() {
        { TestLine(), typeof(TestTypeNameConvention), typeof(TestTypeNameConvention).AssemblyQualifiedName!, TypeNameConventions.AssemblyQualifiedName, false },
        { TestLine(), typeof(TestTypeNameConvention), typeof(TestTypeNameConvention).FullName!, TypeNameConventions.FullName, false },
        { TestLine(), typeof(TestTypeNameConvention), nameof(TestTypeNameConvention), TypeNameConventions.Name, false },
    };

    public static TheoryData<string, string, TypeNameConventions, bool> TransformAnonymousTypeNamesLocalData
    {
        get
        {
            var anon = new { Abc = 123, Xyz = "xyz" };
            var anonType = anon.GetType();

            return new() {
                { TestLine(), anonType.AssemblyQualifiedName!, TypeNameConventions.AssemblyQualifiedName, false },
                { TestLine(), Transform.TypeName(anonType, TypeNameConventions.FullName), TypeNameConventions.FullName, false },
                { TestLine(), Transform.TypeName(anonType, TypeNameConventions.Name), TypeNameConventions.Name, false },
            };
        }
    }

    public static TheoryData<string, string, TypeNameConventions, bool> TransformGenericTypeNamesLocalData
    {
        get
        {
            var dictType = typeof(Dictionary<int, string>);

            return new() {
                { TestLine(), dictType.AssemblyQualifiedName!, TypeNameConventions.AssemblyQualifiedName, false },
                { TestLine(), Transform.TypeName(dictType, TypeNameConventions.FullName), TypeNameConventions.FullName, false },
                { TestLine(), Transform.TypeName(dictType, TypeNameConventions.Name), TypeNameConventions.Name, false },
            };
        }
    }
}
