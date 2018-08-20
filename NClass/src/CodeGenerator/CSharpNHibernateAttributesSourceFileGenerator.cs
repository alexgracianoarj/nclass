using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NClass.Core;
using NClass.CSharp;

using System.Linq;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpNHibernateAttributesSourceFileGenerator 
        : SourceFileGenerator
    {
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
            useLazyLoading = Settings.Default.DefaultLazyFetching;
            useLowercaseUnderscored = Settings.Default.UseUnderscoreAndLowercaseInDB;

            if (Type.IdGenerator == null)
                idGeneratorType = EnumExtensions.GetDescription(Settings.Default.DefaultIdentityGenerator);
            else
                idGeneratorType = EnumExtensions.GetDescription((IdentityGeneratorType)Enum.Parse(typeof(IdentityGeneratorType), Type.IdGenerator));

            WriteHeader();
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
            WriteXmlComments(type);

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
                WriteLine(
                    string.Format(
                        "[Class(Table = \"`{0}`\", Lazy = {1})]",
                        PrefixedText(
                            useLowercaseUnderscored
                            ? LowercaseAndUnderscoredWord(type.Name)
                            : string.IsNullOrEmpty(type.NHMTableName)
                            ? type.Name
                            : type.NHMTableName
                        ),
                        useLazyLoading.ToString().ToLower()
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

            if (needBlankLine)
                AddBlankLine();

            List<Operation> ids = new List<Operation>();

            if (Type is ClassType)
            {
                ids = type.Operations.Where(o => o is Property && o.IsIdentity).ToList<Operation>();
   
                WriteNHibernateAttributesIds(ids);
            }

            foreach (var operation in type.Operations.Where(o => !o.IsIdentity).ToList<Operation>())
            {
                WriteOperation(operation);
                AddBlankLine();
            }

            if (ids.Count > 1)
            {
                WriteEquals(ids);
                WriteGetHashCode(ids);
            }

            // Writing closing bracket of the type block
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteNHibernateAttributesIds(List<Operation> ids)
        {
            if (ids.Count > 1)
            {
                WriteLine("[CompositeId(0)]");

                int position = 1;
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id.ManyToOne))
                    {
                        WriteLine(
                            string.Format(
                                "[KeyManyToOne({0}, Name = \"{1}\", Column = \"`{2}`\", Class = \"{3}\", ClassType = typeof({4}))]",
                                position,
                                id.Name,
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(id.Name)
                                : string.IsNullOrEmpty(id.NHMColumnName)
                                ? id.Name
                                : id.NHMColumnName,
                                id.Type,
                                id.Type
                            ));
                    }
                    else
                    {
                        WriteLine(
                            string.Format(
                                "[KeyProperty({0}, Name = \"{1}\", Column = \"`{2}`\")]",
                                position,
                                id.Name,
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(id.Name)
                                : string.IsNullOrEmpty(id.NHMColumnName)
                                ? id.Name
                                : id.NHMColumnName
                            ));
                    }
                    position++;
                }

                foreach (var id in ids)
                {
                    if (Settings.Default.UseAutomaticProperties)
                    {
                        WriteLine(string.Format("{0} {{ get; set; }}", id.GetDeclaration()));
                    }
                    else
                    {
                        WriteLine(id.GetDeclaration());
                        WriteProperty((Property)id);
                    }

                    AddBlankLine();
                }
            }
            else if (ids.Count == 1)
            {
                WriteLine(
                    string.Format(
                        "[Id(0, Name = \"{0}\", Column = \"`{1}`\")]",
                        ids[0].Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(ids[0].Name)
                        : string.IsNullOrEmpty(ids[0].NHMColumnName)
                        ? ids[0].Name
                        : ids[0].NHMColumnName
                    ));

                WriteLine(
                    string.Format(
                        "[Generator(1, Class = \"{0}\")]",
                        idGeneratorType
                    ));

                if (Settings.Default.UseAutomaticProperties)
                {
                    WriteLine(string.Format("{0} {{ get; set; }}", ids[0].GetDeclaration()));
                }
                else
                {
                    WriteLine(ids[0].GetDeclaration());
                    WriteProperty((Property)ids[0]);
                }

                AddBlankLine();
            }
        }

        private void WriteEquals(List<Operation> ids)
        {
            WriteLine("// Needs this for composite id.");
            WriteLine("public override bool Equals(object obj)");
            WriteLine("{");
            IndentLevel++;
            WriteLine("if (obj == null) return false;");
            WriteLine(string.Format("var t = obj as {0};", Type.Name));
            WriteLine("if (t == null) return false;");
            Write("return ");
            foreach (var id in ids)
            {
                Write(string.Format("{0} == t.{0}", id.Name), false);
                if (id != ids.Last())
                    Write(" && ", false);
                else
                    Write(";", false);
            }
            WriteLine("", false);
            IndentLevel--;
            WriteLine("}");
            AddBlankLine();
        }

        private void WriteGetHashCode(List<Operation> ids)
        {
            WriteLine("// Needs this for composite id.");
            WriteLine("public override int GetHashCode()");
            WriteLine("{");
            IndentLevel++;
            Write("return (");
            foreach (var id in ids)
            {
                Write(id.Name, false);
                if (id != ids.Last())
                    Write(" + \"|\" + ", false);
                else
                    Write(").GetHashCode();", false);
            }
            WriteLine("", false);
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteNHibernateAttributesProperty(Property property)
        {
            if (!string.IsNullOrEmpty(property.ManyToOne))
            {
                WriteLine(
                    string.Format(
                        "[ManyToOne(0, Name = \"{0}\", Column = \"`{1}`\", {2}NotNull = {3}, ClassType = typeof({4}))]", 
                        property.Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsUnique
                        ? "Unique = true, "
                        : "",
                        property.IsNotNull.ToString().ToLower(),
                        property.Type
                    ));
                WriteLine("[Key(1)]");
            }
            else
            {
                WriteLine(
                    string.Format(
                        "[Property(Name = \"{0}\", Column = \"`{1}`\", {2}NotNull = {3})]", 
                        property.Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsUnique
                        ? "Unique = true, "
                        : "",
                        property.IsNotNull.ToString().ToLower()
                    ));
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
                WriteXmlComments(value);

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
            WriteXmlComments(field);

            WriteLine(field.GetDeclaration());
        }

        private void WriteOperation(Operation operation)
        {
            WriteXmlComments(operation);

			if (operation is Property)
			{
                WriteNHibernateAttributesProperty((Property)operation);

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
