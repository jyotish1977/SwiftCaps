using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SwiftCaps.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageHeaderControlTemplate : ControlTemplate
    {
        public PageHeaderControlTemplate()
        {
            InitializeComponent();
        }
    }
}