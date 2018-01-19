using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NClass.Core;
using NClass.CSharp;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpNHibernateAttributesSourceFileGenerator 
        : SourceFileGenerator
    {
        List<string> entities;
        List<string> properties;

        bool useLazyLoading;
        bool useLowercaseUnderscored;
        string idGeneratorType;

        /// <exception cref="NullReferenceException">
        /// <paramref name="type"/> is null.
        /// </exception>
        public CSharpNHibernateAttributesSourceFileGenerator
            (TypeBase type, string rootNamespace, Model model)
            : base(type, rootNamespace, model)
        {}

        protected override string Extension
        {
            get { return ".cs"; }
        }

        protected override void WriteFileContent()
        {
            if (Type is ClassType)
            {
                entities = new List<string>();
                foreach (IEntity entity in Model.Entities)
                {
                    entities.Add(entity.Name);
                }

                ClassType _class = (ClassType)Type;

                properties = new List<string>();
                foreach (Operation operation in _class.Operations)
                {
                    if (operation is Property)
                        properties.Add(operation.Name);
                }
            }

            useLazyLoading = Settings.Default.UseLazyLoading;
            useLowercaseUnderscored = Settings.Default.UseLowercaseAndUnderscoredWordsInDb;
            idGeneratorType = EnumExtensions.GetDescription(Settings.Default.IdGeneratorType);

            WriteUsings();
            OpenNamespace();
            WriteType(Type);
            CloseNamespace();
        }

        private void WriteUsings()
        {
            WriteLine("using System;");
            WriteLine("using NHibernate.Mapping.Attributes;");

            AddBlankLine();
        }

        private void OpenNamespace()
        {
            WriteLine("namespace " + RootNamespace);
            WriteLine("{");
            IndentLevel++;
        }

        private void CloseNamespace()
        {
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteType(TypeBase type)
        {
            if (type is CompositeType)
                WriteCompositeType((CompositeType)type);
            else if (type is EnumType)
                WriteEnum((EnumType)type);
            else if (type is DelegateType)
                WriteDelegate((DelegateType)type);
        }

        private void WriteCompositeType(CompositeType type)
        {
            if (type is ClassType)
            {
                WriteLine(string.Format("[Class(Table = \"`{0}`\", Lazy = {1})]",
                    PrefixedText(
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(type.Name)
                    : type.Name
                    ),
                    useLazyLoading
                    ? "true"
                    : "false"
                    ));
            }

            // Writing type declaration
            WriteLine(type.GetDeclaration());
            WriteLine("{");
            IndentLevel++;

            if (type is ClassType)
            {
                foreach (TypeBase nestedType in ((ClassType)type).NestedChilds)
                {
                    WriteType(nestedType);
                    AddBlankLine();
                }
            }

            if (type.SupportsFields)
            {
                foreach (Field field in type.Fields)
                    WriteField(field);
            }

            bool needBlankLine = (type.FieldCount > 0 && type.OperationCount > 0);

            foreach (Operation operation in type.Operations)
            {
                if (needBlankLine)
                    AddBlankLine();
                needBlankLine = true;
                
                WriteOperation(operation);
            }

            // Writing closing bracket of the type block
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteNHibernateAttributesProperty(Operation operation)
        {
            if (operation.Name == properties[0])
            {
                WriteLine(
                    string.Format(
                    "[Id(0, Name = \"{0}\", Column = \"`{1}`\")]",
                    operation.Name,
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(operation.Name)
                    : operation.Name));
                WriteLine(
                    string.Format(
                    "[Generator(1, Class = \"{0}\")]",
                    idGeneratorType));
            }
            else if (entities.Contains(operation.Type))
            {
                WriteLine(
                    string.Format(
                    "[ManyToOne(0, Name = \"{0}\", Column = \"`{1}`\", NotNull = true)]", 
                    operation.Name,
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(operation.Name)
                    : operation.Name));
                WriteLine("[Key(1)]");
            }
            else
            {
                WriteLine(
                    string.Format(
                    "[Property(Name = \"{0}\", Column = \"`{1}`\", NotNull = true)]", 
                    operation.Name,
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(operation.Name)
                    : operation.Name));
            }
        }

        private void WriteEnum(EnumType _enum)
        {
            // Writing type declaration
            WriteLine(_enum.GetDeclaration());
            WriteLine("{");
            IndentLevel++;

            int valuesRemained = _enum.ValueCount;
            foreach (EnumValue value in _enum.Values)
            {
                if (--valuesRemained > 0)
                    WriteLine(value.GetDeclaration() + ",");
                else
                    WriteLine(value.GetDeclaration());
            }

            // Writing closing bracket of the type block
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteDelegate(DelegateType _delegate)
        {
            WriteLine(_delegate.GetDeclaration());
        }

        private void WriteField(Field field)
        {
            WriteLine(field.GetDeclaration());
        }

        private void WriteOperation(Operation operation)
        {
			if (operation is Property)
			{
                WriteNHibernateAttributesProperty(operation);

                if (Settings.Default.UseAutomaticProperties)
                {
                    WriteLine(string.Format("{0} {{ get; set; }}", operation.GetDeclaration()));
                }
                else
                {
                    WriteLine(operation.GetDeclaration());
                    WriteProperty((Property)operation);
                }
			}
            else
            {
			    WriteLine(operation.GetDeclaration());

                if (operation.HasBody)
                {
                    if (operation is Event)
                    {
                        WriteLine("{");
                        IndentLevel++;
                        WriteLine("add {  }");
                        WriteLine("remove {  }");
                        IndentLevel--;
                        WriteLine("}");
                    }
                    else
                    {
                        WriteLine("{");
                        IndentLevel++;
                        WriteNotImplementedString();
                        IndentLevel--;
                        WriteLine("}");
                    }
                }
            }
        }

        private void WriteProperty(Property property)
        {
            WriteLine("{");
            IndentLevel++;

            if (!property.IsWriteonly)
            {
                if (property.HasImplementation)
                {
                    WriteLine("get");
                    WriteLine("{");
                    IndentLevel++;
                    WriteNotImplementedString();
                    IndentLevel--;
                    WriteLine("}");
                }
                else
                {
                    WriteLine("get;");
                }
            }
            if (!property.IsReadonly)
            {
                if (property.HasImplementation)
                {
                    WriteLine("set");
                    WriteLine("{");
                    IndentLevel++;
                    WriteNotImplementedString();
                    IndentLevel--;
                    WriteLine("}");
                }
                else
                {
                    WriteLine("set;");
                }
            }

            IndentLevel--;
            WriteLine("}");
        }

        private void WriteNotImplementedString()
        {
            if (Settings.Default.UseNotImplementedExceptions)
            {
                if (Settings.Default.CSharpImportList.Contains("System"))
                    WriteLine("throw new NotImplementedException();");
                else
                    WriteLine("throw new System.NotImplementedException();");
            }
            else
            {
                AddBlankLine(true);
            }
        }
    }
}
