using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SlimViews.Templates
{
    public class ConstantStringExtension : MarkupExtension
    {
        public string Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return typeof(ConstantStrings).GetField(Key)?.GetValue(null);
        }
    }
}
