using System.Collections.Generic;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeLeaderBoardData
    {
        static FakeLeaderBoardData()
        {
            Init();
        }

        public static IList<LeaderBoard> Data { get; set; }

        public static void Init()
        {
            var listLeaderBoards = new List<LeaderBoard>();
            foreach(var user in FakeUserData.Data)
            {
                listLeaderBoards.Add(new LeaderBoard
                {
                    UserId = user.Id,
                    UserName = user.Username,
                    GroupId = user.GroupId
                });
            }

            Data = null;
            Data = listLeaderBoards;
        }
    }
}
