using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiQWpfUtils.Controls.Extensions.DataGrid
{
    public abstract class LabelNameAttributeBase : Attribute
    {
        public abstract string GetProperText();
        public LabelNameAttributeBase(string label)
        {
            Label = label.ToLower();
        }

        public string Label { get; }
    }
}
