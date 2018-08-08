using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace NClass.CodeGenerator
{
    public enum DotNetVersion
    {
        [Description("v4.7.2")]
        v472,
        [Description("v4.7.1")]
        v471,
        [Description("v4.7")]
        v47,
        [Description("v4.6.2")]
        v462,
        [Description("v4.6.1")]
        v461,
        [Description("v4.6")]
        v46,
        [Description("v4.5.2")]
        v452,
        [Description("v4.5.1")]
        v451,
        [Description("v4.5")]
        v45,
        [Description("v4.0")]
        v40,
        [Description("v3.5")]
        v35,
        [Description("v3.0")]
        v30,
        [Description("v2.0")]
        v20
    }
}
