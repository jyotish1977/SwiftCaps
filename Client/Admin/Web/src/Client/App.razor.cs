using System;
using System.Reflection;
using SwiftCAPS.Admin.Web.Client.Shared;

namespace SwiftCAPS.Admin.Web.Client
{
    public partial class App
    {
        private Assembly AppAssembly { get; set; }
        private Type MainLayout { get; set; }

        protected override void OnInitialized()
        {
            AppAssembly = typeof(Program).Assembly;
            MainLayout = typeof(MainLayout);
        }
    }
}