﻿using System;
using System.Collections.Generic;
using NClass.Core;

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
            useLowercaseUnderscored = Settings.Default.UseUnderscoreAndLowercaseInDB;

            if(Type.IdGenerator == null)
                idGeneratorType = Enum.GetName(typeof(IdentityGeneratorType), Settings.Default.DefaultIdentityGenerator);
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
                WriteLine("CompositeId()");
                IndentLevel++;
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id.ManyToOne))
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
                ClassType _class = (ClassType)Type;

                string generatorParameters = null;

                if (_class.IdGenerator == "HiLo")
                {
                    HiLoIdentityGeneratorParameters hiLo = GeneratorParametersDeSerializer.Deserialize<HiLoIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParameters = string.Format(
                                            "\"{0}\", \"{1}\", \"{2}\", \"{3}\"",
                                            hiLo.Table,
                                            hiLo.Column,
                                            hiLo.MaxLo,
                                            hiLo.Where
                                            );
                }
                else if (_class.IdGenerator == "SeqHiLo")
                {
                    SeqHiLoIdentityGeneratorParameters seqHiLo = GeneratorParametersDeSerializer.Deserialize<SeqHiLoIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParameters = string.Format(
                                            "\"{0}\", \"{1}\"",
                                            seqHiLo.Sequence,
                                            seqHiLo.MaxLo
                                            );
                }
                else if (_class.IdGenerator == "Sequence")
                {
                    SequenceIdentityGeneratorParameters sequence = GeneratorParametersDeSerializer.Deserialize<SequenceIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParameters = string.Format(
                                            "\"{0}\"",
                                            sequence.Sequence
                                            );
                }
                else if (_class.IdGenerator == "UuidHex")
                {
                    UuidHexIdentityGeneratorParameters uuidHex = GeneratorParametersDeSerializer.Deserialize<UuidHexIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParameters = string.Format(
                                            "\"{0}\", \"{1}\"",
                                            uuidHex.Format,
                                            uuidHex.Separator
                                            );
                }
                else if (_class.IdGenerator == "Foreign")
                {
                    ForeignIdentityGeneratorParameters foreign = GeneratorParametersDeSerializer.Deserialize<ForeignIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParameters = string.Format(
                                            "\"{0}\"",
                                            foreign.Property
                                            );
                }
                else if (_class.IdGenerator == "Custom")
                {
                    CustomIdentityGeneratorParameters custom = GeneratorParametersDeSerializer.Deserialize<CustomIdentityGeneratorParameters>(_class.GeneratorParameters);

                    generatorParameters = string.Format(
                                    "\"{0}\", p => {{ ",
                                    custom.Class
                                    );

                    for (int i = 0; i <= (custom.Parameters.Count - 1); i++)
                    {
                        generatorParameters += "p.AddParam(\"" + custom.Parameters[i].Name + "\", \"" + custom.Parameters[i].Value + "\"); ";
                    }

                    generatorParameters += "}";
                }

                WriteLine(
                    string.Format(
                        "Id(x => x.{0}).Column(\"`{1}`\").GeneratedBy.{2}({3});",
                        ids[0].Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(ids[0].Name)
                        : string.IsNullOrEmpty(ids[0].NHMColumnName)
                        ? ids[0].Name
                        : ids[0].NHMColumnName,
                        idGeneratorType,
                        generatorParameters
                    ));
            }
        }

        private void WriteProperty(Property property)
        {
            if (!string.IsNullOrEmpty(property.ManyToOne))
            {
                WriteLine(
                    string.Format(
                        "References(x => x.{0}).Column(\"`{1}`\"){2}{3}.Nullable();",
                        property.Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsUnique
                        ? ".Unique()"
                        : "",
                        property.IsNotNull
                        ? ".Not"
                        : ""
                    ));
            }
            else
            {
                WriteLine(
                    string.Format(
                        "Map(x => x.{0}).Column(\"`{1}`\"){2}{3}.Nullable();",
                        property.Name,
                        useLowercaseUnderscored
                        ? LowercaseAndUnderscoredWord(property.Name)
                        : string.IsNullOrEmpty(property.NHMColumnName)
                        ? property.Name
                        : property.NHMColumnName,
                        property.IsUnique
                        ? ".Unique()"
                        : "",
                        property.IsNotNull
                        ? ".Not"
                        : ""
                    ));
            }
        }
    }
}
