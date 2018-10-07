using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NClass.Core;
using NClass.CSharp;

using System.Linq;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpNHibernateByCodeSourceFileGenerator 
        : SourceFileGenerator
    {
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
            useLazyLoading = Settings.Default.DefaultLazyFetching;
            useLowercaseUnderscored = Settings.Default.UseUnderscoreAndLowercaseInDB;

            if (Type.IdGenerator == null)
                idGeneratorType = Enum.GetName(typeof(IdentityGeneratorType), Settings.Default.DefaultIdentityGenerator);
            else
                idGeneratorType = Type.IdGenerator;

            if (idGeneratorType == "HiLo")
                idGeneratorType = "HighLow";

            WriteHeader();
            WriteUsings();
            OpenNamespace();
            WriteClass((ClassType)Type);
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

            WriteLine(
                string.Format(
                    "Table(\"`{0}`\");",
                    PrefixedText(
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(_class.Name)
                        : string.IsNullOrEmpty(_class.NHMTableName)
                        ? _class.Name
                        : _class.NHMTableName
                    )
                ));

            WriteLine(
                string.Format(
                    "Lazy({0});", 
                    useLazyLoading.ToString().ToLower()
                ));

            List<Operation> ids = _class.Operations.Where(o => o is Property && o.IsIdentity).ToList<Operation>();

            WriteIds(ids);

            foreach (var property in _class.Operations.Where(o => o is Property && !o.IsIdentity).ToList<Operation>())
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
                WriteLine("ComposedId(map => { ");
                IndentLevel++;
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id.ManyToOne))
                    {
                        WriteLine(
                            string.Format(
                                "map.ManyToOne(x => x.{0}, m => {{ m.Class(typeof({1})); m.Column(\"`{2}`\"); }}); ",
                                id.Name,
                                id.Type,
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(id.Name)
                                : string.IsNullOrEmpty(id.NHMColumnName)
                                ? id.Name
                                : id.NHMColumnName
                            ));
                    }
                    else
                    {
                        WriteLine(
                            string.Format(
                                "map.Property(x => x.{0}, m => {{ m.Column(\"`{1}`\"); }}); ",
                                id.Name,
                                useLowercaseUnderscored
                                ? LowercaseAndUnderscoredWord(id.Name)
                                : string.IsNullOrEmpty(id.NHMColumnName)
                                ? id.Name
                                : id.NHMColumnName,
                                id.IsNotNull.ToString().ToLower()
                            ));
                    }
                }
                IndentLevel--;
                WriteLine("});");
            }
            else if (ids.Count == 1)
            {
                ClassType _class = (ClassType)Type;

                string generatorParams = null;

                if (_class.IdGenerator == "HiLo")
                {
                    HiLoIdentityGeneratorParameters hiLo = GeneratorParametersDeSerializer.Deserialize<HiLoIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParams = string.Format(
                                            ", gmap => gmap.Params(new {{ table = \"{0}\", column = \"{1}\", max_lo = \"{2}\", where = \"{3}\" }})",
                                            hiLo.Table,
                                            hiLo.Column,
                                            hiLo.MaxLo,
                                            hiLo.Where
                                            );
                }
                else if (_class.IdGenerator == "SeqHiLo")
                {
                    SeqHiLoIdentityGeneratorParameters seqHiLo = GeneratorParametersDeSerializer.Deserialize<SeqHiLoIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParams = string.Format(
                                            ", gmap => gmap.Params(new {{ sequence = \"{0}\", max_lo = \"{1}\" }})",
                                            seqHiLo.Sequence,
                                            seqHiLo.MaxLo
                                            );
                }
                else if (_class.IdGenerator == "Sequence")
                {
                    SequenceIdentityGeneratorParameters sequence = GeneratorParametersDeSerializer.Deserialize<SequenceIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParams = string.Format(
                                            ", gmap => gmap.Params(new {{ sequence = \"{0}\" }})",
                                            sequence.Sequence
                                            );
                }
                else if (_class.IdGenerator == "UuidHex")
                {
                    UuidHexIdentityGeneratorParameters uuidHex = GeneratorParametersDeSerializer.Deserialize<UuidHexIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParams = string.Format(
                                            ", gmap => gmap.Params(new {{ format_value = \"{0}\", separator_value = \"{1}\" }})",
                                            uuidHex.Format,
                                            uuidHex.Separator
                                            );
                }
                else if (_class.IdGenerator == "Foreign")
                {
                    ForeignIdentityGeneratorParameters foreign = GeneratorParametersDeSerializer.Deserialize<ForeignIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParams = string.Format(
                                            ", gmap => gmap.Params(new {{ property = \"{0}\" }})",
                                            foreign.Property
                                            );
                }
                else if (_class.IdGenerator == "Custom")
                {
                    CustomIdentityGeneratorParameters custom = GeneratorParametersDeSerializer.Deserialize<CustomIdentityGeneratorParameters>(_class.GeneratorParameters);

                    idGeneratorType = custom.Class;

                    generatorParams = ", gmap => gmap.Params(new { ";

                    for (int i = 0; i <= (custom.Parameters.Count - 1); i++)
                    {
                        generatorParams += custom.Parameters[i].Name + " = \"" + custom.Parameters[i].Value + "\"";

                        if (i < (custom.Parameters.Count - 1))
                            generatorParams += ", ";
                    }

                    generatorParams += " })";
                }

                WriteLine(
                    string.Format(
                        "Id(x => x.{0}, map => {{ map.Column(\"`{1}`\"); map.Generator(Generators.{2}{3}); }});",
                        ids[0].Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(ids[0].Name)
                        : string.IsNullOrEmpty(ids[0].NHMColumnName)
                        ? ids[0].Name
                        : ids[0].NHMColumnName,
                        idGeneratorType,
                        generatorParams
                    ));
            }
        }

        private void WriteProperty(Property property)
        {
            if (!string.IsNullOrEmpty(property.ManyToOne))
            {
                WriteLine(
                    string.Format(
                        "ManyToOne(x => x.{0}, map => {{ map.Class(typeof({1})); map.Column(\"`{2}`\");{3} map.NotNullable({4}); }});",
                        property.Name, 
                        property.Type,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsUnique
                        ? " map.Unique(true);"
                        : "",
                        property.IsNotNull.ToString().ToLower()
                    ));
            }
            else
            {
                WriteLine(
                    string.Format(
                        "Property(x => x.{0}, map => {{ map.Column(\"`{1}`\");{2} map.NotNullable({3}); }});", 
                        property.Name, 
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsUnique
                        ? " map.Unique(true);"
                        : "",
                        property.IsNotNull.ToString().ToLower()
                    ));
            }
        }
    }
}
