using System.Collections.Generic;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeQuizSectionData
    {
        static FakeQuizSectionData()
        {
            Init();
        }

        public static List<QuizSection> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<QuizSection>
            {
                // F16 CAPS S1
                new QuizSection
                {
                    Id = GenericIdentifiers._1ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 1,
                    Description = "FIRE/OVERHEAT/FUEL LEAK/ (GROUND)"
                },

                // F16 CAPS S2
                new QuizSection
                {
                    Id = GenericIdentifiers._2ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 2,
                    Description = "GROUND EGRESS"
                },

                // F16 CAPS S3
                new QuizSection
                {
                    Id = GenericIdentifiers._3ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 3,
                    Description = "BRAKE / ANTI-SKID FAILURE"
                },

                 // F16 CAPS S4
                new QuizSection
                {
                    Id = GenericIdentifiers._4ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 4,
                    Description = "ABORT"
                },

                 // F16 CAPS S5
                new QuizSection
                {
                    Id = GenericIdentifiers._5ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 5,
                    Description = "ENGINE FAILURE AFTER TAKE-OFF"
                },

                 // F16 CAPS S6
                new QuizSection
                {
                    Id = GenericIdentifiers._6ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 6,
                    Description = "ENGINE FIRE AFTER TAKE-OFF"
                },

                // F16 CAPS S7
                new QuizSection
                {
                    Id = GenericIdentifiers._7ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 7,
                    Description = "A/B MALFUNCTION ON TAKE-OFF"
                },

                // F16 CAPS S8
                new QuizSection
                {
                    Id = GenericIdentifiers._8ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 8,
                    Description = "LOW THRUST ON TAKE-OFF"
                },

                // F16 CAPS S9
                new QuizSection
                {
                    Id = GenericIdentifiers._9ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 9,
                    Description = "ENGINE FAILURE / AIRSTART"
                },

                // F16 CAPS S10
                new QuizSection
                {
                    Id = GenericIdentifiers._10ID,
                    QuizId = GenericIdentifiers._1ID,
                    Index = 10,
                    Description = "OUT OF CONTROL RECOVERY"
                },




                // F16 MONTHLY S1
                new QuizSection
                {
                    Id = GenericIdentifiers._11ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 1,
                    Description = "GENERAL PRIORITIES – ENGINE FLAMEOUT"
                },

                 // F16 MONTHLY S2
                new QuizSection
                {
                    Id = GenericIdentifiers._12ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 2,
                    Description = "ENGINE MALFUNCTION ANALYSIS"
                },

                 // F16 MONTHLY S3
                new QuizSection
                {
                    Id = GenericIdentifiers._13ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 3,
                    Description = "AIRSTART (GENERAL)"
                },

                 // F16 MONTHLY S4
                new QuizSection
                {
                    Id = GenericIdentifiers._14ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 4,
                    Description = "AIRSTART (LOW ALTITUDE)"
                },
                
                // F16 MONTHLY S5
                new QuizSection
                {
                    Id = GenericIdentifiers._15ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 5,
                    Description = "FAILED OPEN, DAMAGED, MISSING NOZZLE"
                },

                 // F16 MONTHLY S6
                new QuizSection
                {
                    Id = GenericIdentifiers._16ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 6,
                    Description = "GRAVITY FEED / HOT FUEL"
                },

                // F16 MONTHLY S7
                new QuizSection
                {
                    Id = GenericIdentifiers._17ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 7,
                    Description = "FLAMEOUT APPROACH"
                },

                // F16 MONTHLY S8
                new QuizSection
                {
                    Id = GenericIdentifiers._18ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 8,
                    Description = ""
                },

                // F16 MONTHLY S9
                new QuizSection
                {
                    Id = GenericIdentifiers._19ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 9,
                    Description = ""
                },
                // F16 MONTHLY S10
                new QuizSection
                {
                    Id = GenericIdentifiers._20ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 10,
                    Description = ""
                },

                // F16 MONTHLY S11
                new QuizSection
                {
                    Id = GenericIdentifiers._21ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 11,
                    Description = ""
                },

                // F16 MONTHLY S11
                new QuizSection
                {
                    Id = GenericIdentifiers._22ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 12,
                    Description = ""
                },

                // F16 MONTHLY S11
                new QuizSection
                {
                    Id = GenericIdentifiers._23ID,
                    QuizId = GenericIdentifiers._2ID,
                    Index = 13,
                    Description = ""
                }
            };
        }
    }
}
