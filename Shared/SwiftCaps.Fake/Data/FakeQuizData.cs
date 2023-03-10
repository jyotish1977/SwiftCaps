using System.Collections.Generic;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeQuizData
    {
        static FakeQuizData()
        {
            Init();
        }

        public static List<Quiz> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<Quiz>
            {
                new Quiz
                {
                    Id = GenericIdentifiers._1ID,
                    Name = "F-16 CAPs",
                    Description = "F-16 CAPs",
                    InfoMarkdown ="# NOTES \n" +
                        "1  For all take-off emergencies, if conditions permit, abort.  \n\n" +
                        "2  Take-off phase is defined as the phase of flight from lift-off to below 2000ft / 350 KIAS.  \n\n" +
                        "3  After maintaining control of the aircraft and time permitting, refer to T.O SN1F-16CJ-1CL-1 and complete the remaining steps for the emergency, if any.  \n\n" +
                        "\n"+
                        "# ACCEPTABLE ABBREVIATIONS  \n\n" +
                        "1.	REL – RELEASE  \n" +
                        "2.	REQ – REQUIRED  \n" +
                        "3.	JETT – JETTISON  \n" +
                        "4.	POSS – POSSIBLE \n"
                },
                new Quiz
                {
                    Id = GenericIdentifiers._2ID,
                    Name = "F16 AIRCRAFT SYSTEMS SUPPLEMENTARY QUIZ (Dual qual fill up for both CD and D+)",
                    Description = "The quiz is not prescriptive and is not a substitute for airmanship, sound judgement and good piloting",
                    InfoMarkdown = 
                        "# ABBREVIATIONS  \n\n" +
                        "1. MSEA – Min Safe Ejection Altitude \n" +
                        "2.	FO – Flameout  \n" +
                        "3.	JETT – Jettison  \n" +
                        //"4.	ST-in – Straight In  \n" +
                        "4.	Instr – Instrument"
                }
            };
        }
    }
}
