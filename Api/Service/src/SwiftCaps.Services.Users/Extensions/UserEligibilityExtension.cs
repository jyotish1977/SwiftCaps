using System.Linq;
using Newtonsoft.Json.Linq;

namespace SwiftCaps.Services.User.Extensions
{
    public static class UserEligibilityExtension
    {
        public static bool IsPartOfAnySquadrons(this JArray groupTokens)
        {
            var partOfAnySquadrons = groupTokens.Any(token =>
                IsGraphGroup(token) && IsSquadronGroup(token));
            return partOfAnySquadrons;
        }

        private static bool IsGraphGroup(JToken token)
        {
            return token["@odata.type"].Value<string>().Equals("#microsoft.graph.group");
        }

        private static bool IsSquadronGroup(JToken token)
        {
            return token["displayName"].Value<string>().StartsWith("grp-sqn");
        }
    }
}
