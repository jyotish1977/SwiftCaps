using System;

namespace build
{
    partial class Program
    {
        private const string Prefix = "SwiftCaps.Quiz.Web";
        
        private static readonly string[] ProjectFiles = new [] { 
            $"./Client/Quiz/Web/src/Server" 
        };
        
        private static readonly string[] TestProjFiles = Array.Empty<string>();
    }
}
