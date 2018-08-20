using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

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
        [Description("native")]
        Native,
        [Description("assigned")]
        Assigned,
        [Description("foreign")]
        Foreign
    }
}
