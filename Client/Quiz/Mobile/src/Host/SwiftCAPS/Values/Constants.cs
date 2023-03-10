namespace SwiftCaps.Values
{
    public static class Constants
    {
        public const string Root = "root";
        public const string LoginRoot = "login";

        public static class ShellNavigation
        {
            public static readonly string LoginPagePath = $"//{LoginRoot}";
            public static readonly string QuizListPagePath = $"//{Root}/{"QuizListPage".ToLower()}";
            public static readonly string QuizTrackerPagePath = $"{"QuizTrackerPage".ToLower()}";
            public static readonly string QuizPagePath = $"{"QuizPage".ToLower()}";
        }
    }
}
