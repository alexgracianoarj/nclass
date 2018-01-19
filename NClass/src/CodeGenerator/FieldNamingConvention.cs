namespace NClass.CodeGenerator
{
    public enum FieldNamingConvention
    {
        SameAsDatabase,
        CamelCase,
        Prefixed,
        LowercaseAndUnderscoredWord,
        /// <summary>
        /// Upper camel case.
        /// </summary>
        PascalCase
    }

    public enum FieldGenerationConvention
    {
        Field,
        Property,
        AutoProperty
    }
}