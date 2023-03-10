using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SwiftCaps.Converters
{
    public abstract class ValueConverterExtension : BindableObject, IMarkupExtension<IValueConverter>
    {
        public IValueConverter ProvideValue(IServiceProvider serviceProvider)
            => (IValueConverter)this;

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ((IMarkupExtension<IValueConverter>)this).ProvideValue(serviceProvider);
    }
}
