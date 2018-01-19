using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NClass.Core;
using NClass.CSharp;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpNHibernateByCodeSourceFileGenerator 
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
        public CSharpNHibernateByCodeSourceFileGenerator
            (TypeBase type, string rootNamespace, Model model)
            : base(type, rootNamespace, model)
        {}

        protected override string Extension
        {
            get { return "Mapping.cs"; }
        }

        protected override void WriteFileContent()
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

            useLazyLoading = Settings.Default.UseLazyLoading;
            useLowercaseUnderscored = Settings.Default.UseLowercaseAndUnderscoredWordsInDb;
            idGeneratorType = Enum.GetName(typeof(IdGeneratorType), Settings.Default.IdGeneratorType);

            if (idGeneratorType == "HiLo")
                idGeneratorType = "HighLow";

            WriteUsings();
            OpenNamespace();
            WriteClass(_class);
            CloseNamespace();
        }

        private void WriteUsings()
        {
            WriteLine("using NHibernate.Mapping.ByCode;");
            WriteLine("using NHibernate.Mapping.ByCode.Conformist;");

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
            WriteLine(string.Format("{0} class {1}Mapping : ClassMapping<{2}>", _class.Access.ToString().ToLower(), _class.Name, _class.Name));
            WriteLine("{");
            IndentLevel++;

            // Writing constructor
            WriteLine(string.Format("{0} {1}Mapping()", _class.Access.ToString().ToLower(), _class.Name));
            WriteLine("{");
            IndentLevel++;

            WriteLine(string.Format("Table(\"`{0}`\");",
                PrefixedText(
                useLowercaseUnderscored
                ? LowercaseAndUnderscoredWord(_class.Name)
                : _class.Name
                )));
            WriteLine(string.Format("Lazy({0});", 
                useLazyLoading
                ? "true"
                : "false"));

            foreach (Operation operation in _class.Operations)
            {
                if (operation is Property)
                    WriteProperty((Property)operation);
            }

            // Writing closing bracket of the type block
            IndentLevel--;
            WriteLine("}");

            // Writing closing bracket of the type block
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteProperty(Property property)
        {
            if (property.Name == properties[0])
            {
                WriteLine(
                    string.Format(
                    "Id(x => x.{0}, map => {{ map.Column(\"`{1}`\"); map.Generator(Generators.{2}); }});", 
                    property.Name, 
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(property.Name)
                    : property.Name,
                    idGeneratorType
                    ));
            }
            else if (entities.Contains(property.Type))
            {
                WriteLine(
                    string.Format(
                    "ManyToOne(x => x.{0}, map => {{ map.Class(typeof({1})); map.Column(\"`{2}`\"); }});",
                    property.Name, 
                    property.Type,
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(property.Name)
                    : property.Name
                    ));
            }
            else
            {
                WriteLine(
                    string.Format(
                    "Property(x => x.{0}, map => {{ map.Column(\"`{1}`\"); map.NotNullable(true); }});", 
                    property.Name, 
                    useLowercaseUnderscored
                    ? LowercaseAndUnderscoredWord(property.Name)
                    : property.Name
                    ));
            }
        }
    }
}
