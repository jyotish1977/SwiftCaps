using System;
using System.Linq;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Infrastructure;
using Unity;

namespace SwiftCAPS.Mobile.UnitTest.Infrastructure
{
    public static class PageHelpers
    {
        public static BaseContentPage GetPage(string pageName)
        {
            var reg = BootStrapper.Container.Registrations;
            var pageType = reg.FirstOrDefault(x => x.Name == pageName);

            if (pageType == null)
                throw new Exception($"Page {pageType} is nor registered with ViewModelLocator");

            var page = BootStrapper.Container.Resolve(pageType.RegisteredType) as BaseContentPage;
            return page;
        }
    }
}
