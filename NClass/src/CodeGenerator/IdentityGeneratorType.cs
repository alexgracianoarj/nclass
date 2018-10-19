using System.Collections.Generic;

using System.ComponentModel;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NClass.CodeGenerator
{
    public enum IdentityGeneratorType
    {
        [Description("increment")]
        Increment,
        [Description("identity")]
        Identity,
        [Description("sequence")]
        Sequence,
        [Description("hilo")]
        HiLo,
        [Description("seqhilo")]
        SeqHiLo,
        [Description("uuid.hex")]
        UuidHex,
        [Description("uuid.string")]
        UuidString,
        [Description("guid")]
        Guid,
        [Description("guid.comb")]
        GuidComb,
        [Description("guid.native")]
        GuidNative,
        [Description("select")]
        Select,
        [Description("sequence-identity")]
        SequenceIdentity,
        [Description("trigger-identity")]
        TriggerIdentity,
        [Description("native")]
        Native,
        [Description("assigned")]
        Assigned,
        [Description("foreign")]
        Foreign,
        [Description("custom")]
        Custom
    }

    public class ForeignIdentityGeneratorParameters
    {
        [Category("Generator Parameters")]
        public string Property { get; set; }
    }

    public class HiLoIdentityGeneratorParameters
    {
        [Category("Generator Parameters")]
        public string Table { get; set; }
        [Category("Generator Parameters")]
        public string Column { get; set; }
        [Category("Generator Parameters")]
        public int MaxLo { get; set; }
        [Category("Generator Parameters")]
        public string Where { get; set; }
    }

    public class SeqHiLoIdentityGeneratorParameters
    {
        [Category("Generator Parameters")]
        public string Sequence { get; set; }
        [Category("Generator Parameters")]
        public int MaxLo { get; set; }
    }

    public class SequenceIdentityGeneratorParameters
    {
        [Category("Generator Parameters")]
        public string Sequence { get; set; }
    }

    public class UuidHexIdentityGeneratorParameters
    {
        [Category("Generator Parameters")]
        public string Format { get; set; }
        [Category("Generator Parameters")]
        public string Separator { get; set; }
    }

    public class CustomIdentityGeneratorParameters
    {
        public CustomIdentityGeneratorParameters()
        {
            Parameters = new List<NewParameter>();
        }
        [Category("Generator Parameters")]
        public string Class { get; set; }
        [Category("Generator Parameters")]
        public List<NewParameter> Parameters { get; set; }
    }

    public static class GeneratorParametersDeSerializer
    {
        public static T Deserialize<T>(this string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(toDeserialize))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string Serialize<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            using (StringWriter stream = new StringWriter())
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    xmlSerializer.Serialize(writer, toSerialize, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
                    return stream.ToString();
                }
        }
    }
}

public class NewParameter
{
    [Category("Parameter")]
    public string Name { get; set; }
    [Category("Parameter")]
    public string Value { get; set; }
}