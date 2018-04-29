using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace NClass.CodeGenerator
{
    public interface ITextFormatter
    {
        string FormatText(string text);
        string PrefixRemoval { get; set; }
    }

    public abstract class AbstractTextFormatter : ITextFormatter
    {
        public virtual string FormatText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var result = RemovePrefix(text);

            // Cannot have class or property with not allowed chars
            result = result.Replace("%", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u");

            result = result.Replace(" ", "_");

            // Omit any chars except letters and numbers in class or properties.
            result = Regex.Replace(result, "[^a-zA-Z0-9_]", string.Empty); //And Underscore

            if (result.Length != 0 && char.IsNumber(result.ToCharArray(0, 1)[0]))
            {
                // Cannot start class or property with a number
                result = "_" + result;
            }

            return result;
        }

        private string RemovePrefix(string original)
        {
            if (PrefixRemoval == null || string.IsNullOrEmpty(PrefixRemoval) || string.IsNullOrEmpty(original))
                return original;

            return Regex.Replace(original, "^" + PrefixRemoval, string.Empty, RegexOptions.IgnoreCase);
        }

        public string PrefixRemoval { get; set; }
    }

    public class UnformattedTextFormatter : AbstractTextFormatter
    {
        public override string FormatText(string text)
        {
            return base.FormatText(text);
        }
    }

    public class PascalCaseTextFormatter : AbstractTextFormatter
    {
        public override string FormatText(string text)
        {
            if ((new Regex("^[A-Z][a-z]+(?:[A-Z][a-z]+)*$").IsMatch(text)))
                return text;

            if (String.IsNullOrEmpty(text))
                return text;

            var result = "";

            string[] words = base.FormatText(new LowercaseAndUnderscoredWordTextFormatter().FormatText(text)).Split('_');

            foreach (string word in words)
                result += char.ToUpper(word[0]) + word.Substring(1);

            return result;
        }
    }

    public class LowercaseAndUnderscoredWordTextFormatter : AbstractTextFormatter
    {
        public override string FormatText(string text)
        {
            return Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(base.FormatText(text),
                                @"([A-Z]+)([A-Z][a-z])", "$1_$2"
                                    ), @"([a-z\d])([A-Z])", "$1_$2"
                                        ), @"[-\s]"
                                        , "_").ToLower();
        }
    }

    public class PrefixedTextFormatter : AbstractTextFormatter
    {
        public PrefixedTextFormatter(string prefix)
        {
            Prefix = prefix;
        }

        private string Prefix { get; set; }

        public override string FormatText(string text)
        {
            return Prefix + base.FormatText(text);
        }
    }
}
