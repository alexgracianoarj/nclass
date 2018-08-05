using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NClass.Core;
using NClass.CSharp;

using System.Linq;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpFluentNHibernateSourceFileGenerator 
        : SourceFileGenerator
    {
        bool useLazyLoading;
        bool useLowercaseUnderscored;
        string idGeneratorType;

		/// <exception cref="NullReferenceException">
		/// <paramref name="type"/> is null.
		/// </exception>
        public CSharpFluentNHibernateSourceFileGenerator
            (TypeBase type, string rootNamespace, Model model)
			: base(type, rootNamespace, model)
		{}

        protected override string Extension
        {
            get{ return "Map.cs"; }
        }

        protected override void WriteFileContent()
        {
            useLazyLoading = Settings.Default.DefaultLazyFetching;
            useLowercaseUnderscored = Settings.Default.UseLowercaseAndUnderscoredWordsInDb;

            if(Type.IdGenerator == null)
                idGeneratorType = Enum.GetName(typeof(IdGeneratorType), Settings.Default.DefaultIdGenerator);
            else
                idGeneratorType = Type.IdGenerator;

            WriteHeader();
            WriteUsings();
            OpenNamespace();
            WriteClass((ClassType)Type);
            CloseNamespace();
        }

        private void WriteUsings()
        {
            WriteLine("using FluentNHibernate.Mapping;");

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

        private void WriteClass(ClassType _class)
        {
            // Writing type declaration
            WriteLine(string.Format("{0} class {1}Map : ClassMap<{2}>", _class.Access.ToString().ToLower(), _class.Name, _class.Name));
            WriteLine("{");
            IndentLevel++;

            WriteLine(string.Format("{0} {1}Map()", _class.Access.ToString().ToLower(), _class.Name));
            WriteLine("{");
            IndentLevel++;

            WriteLine(string.Format("Table(\"`{0}`\");",
                PrefixedText(
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(_class.Name)
                    : string.IsNullOrEmpty(_class.NHMTableName)
                    ? _class.Name
                    : _class.NHMTableName
                )));

            WriteLine(
                useLazyLoading
                ? "LazyLoad();"
                : "Not.LazyLoad();"
                );

            List<Operation> ids = _class.Operations.Where(o => o is Property && o.IsPrimaryKey).ToList<Operation>();

            WriteIds(ids);

            foreach (var property in _class.Operations.Where(o => o is Property && !o.IsPrimaryKey).ToList<Operation>())
            {
                WriteProperty((Property)property);
            }

            // Writing closing bracket of the type block
            IndentLevel--;
            WriteLine("}");

            // Writing closing bracket of the type block
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteIds(List<Operation> ids)
        {
            if (ids.Count > 1)
            {
                WriteLine("CompositeId()");
                IndentLevel++;
                foreach (var id in ids)
                {
                    if (Model.Entities.Where(e => e.Name == id.Type).Count() > 0)
                    {
                        Write(
                            string.Format(
                                ".KeyReference(x => x.{0}, \"`{1}`\")",
                                id.Name,
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(id.Name)
                                : string.IsNullOrEmpty(id.NHMColumnName)
                                ? id.Name
                                : id.NHMColumnName
                            ));
                    }
                    else
                    {
                        Write(
                            string.Format(
                                ".KeyProperty(x => x.{0}, \"`{1}`\")",
                                id.Name,
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(id.Name)
                                : string.IsNullOrEmpty(id.NHMColumnName)
                                ? id.Name
                                : id.NHMColumnName
                            ));
                    }

                    if (id != ids.Last())
                        WriteLine("", false);
                    else
                        WriteLine(";", false);
                }
                IndentLevel--;
            }
            else if (ids.Count == 1)
            {
                WriteLine(
                    string.Format(
                        "Id(x => x.{0}).Column(\"`{1}`\").GeneratedBy.{2}();",
                        ids[0].Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(ids[0].Name)
                        : string.IsNullOrEmpty(ids[0].NHMColumnName)
                        ? ids[0].Name
                        : ids[0].NHMColumnName,
                        idGeneratorType
                    ));
            }
        }

        private void WriteProperty(Property property)
        {
            if (Model.Entities.Where(e => e.Name == property.Type).Count() > 0)
            {
                WriteLine(
                    string.Format(
                        "References(x => x.{0}).Column(\"`{1}`\"){2}.Nullable();",
                        property.Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsNotNull
                        ? ".Not"
                        : ""
                    ));
            }
            else
            {
                WriteLine(
                    string.Format(
                        "Map(x => x.{0}).Column(\"`{1}`\"){2}.Nullable();",
                        property.Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsNotNull
                        ? ".Not"
                        : ""
                    ));
            }
        }
    }
}
