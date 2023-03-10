using System.Collections.Generic;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeQuestionData
    {
        static FakeQuestionData()
        {
            Init();
        }

        public static List<Question> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<Question>
            {
                // F16 CAPS S1 (FIRE/OVERHEAT/FUEL LEAK/ (GROUND))
                new Question
                {
                    Id = GenericIdentifiers._1ID,
                    QuizSectionId = GenericIdentifiers._1ID,
                    QuizSectionIndex = 1,
                    Body = "1. {THROTTLE OFF}",
                },
                new Question
                {
                    Id = GenericIdentifiers._2ID,
                    QuizSectionId = GenericIdentifiers._1ID,
                    QuizSectionIndex = 2,
                    Body = "2. {JFS OFF}",
                },
                new Question
                {
                    Id = GenericIdentifiers._3ID,
                    QuizSectionId = GenericIdentifiers._1ID,
                    QuizSectionIndex = 3,
                    Body = "3. {FUEL MASTER OFF}",
                },

                // F16 CAPS S2 (GROUND EGRESS)
                new Question
                {
                    Id = GenericIdentifiers._4ID,
                    QuizSectionId = GenericIdentifiers._2ID,
                    QuizSectionIndex = 1,
                    Body = "1. {THROTTLE OFF}",
                },
                new Question
                {
                    Id = GenericIdentifiers._5ID,
                    QuizSectionId = GenericIdentifiers._2ID,
                    QuizSectionIndex = 2,
                    Body = "2. {SEAT SAFE}",
                },
                new Question
                {
                    Id = GenericIdentifiers._6ID,
                    QuizSectionId = GenericIdentifiers._2ID,
                    QuizSectionIndex = 3,
                    Body = "3. {BELT KIT HARNESS G SUIT REL}",
                },


                // F16 CAPS S3 (BRAKE / ANTI-SKID FAILURE)
                new Question
                {
                    Id = GenericIdentifiers._7ID,
                    QuizSectionId = GenericIdentifiers._3ID,
                    QuizSectionIndex = 1,
                    Header =
                        "If failure occurs when taxiing in congested areas, PARKING BRAKE should be applied. During landing, if conditions permit, go around.",
                    Body = "1. {HOOK DOWN}",
                },
                new Question
                {
                    Id = GenericIdentifiers._8ID,
                    QuizSectionId = GenericIdentifiers._3ID,
                    QuizSectionIndex = 2,
                    Header =
                        "If failure occurs when taxiing in congested areas, PARKING BRAKE should be applied. During landing, if conditions permit, go around.",
                    Body = "2. {BRAKES CHAN 2}",
                },
                new Question
                {
                    Id = GenericIdentifiers._9ID,
                    QuizSectionId = GenericIdentifiers._3ID,
                    QuizSectionIndex = 3,
                    Header =
                        "If failure occurs when taxiing in congested areas, PARKING BRAKE should be applied. During landing, if conditions permit, go around.",
                    Body = "3. {ANTI SKID OFF}",
                },
                new Question
                {
                    Id = GenericIdentifiers._10ID,
                    QuizSectionId = GenericIdentifiers._3ID,
                    QuizSectionIndex = 4,
                    Header = "If hookwire is not available, or if at low groundspeed,",
                    Body = "4. {PARKING BRAKE ON INTERMITTENTLY}",
                },

                // F16 CAPS S4 (ABORT)
                new Question
                {
                    Id = GenericIdentifiers._11ID,
                    QuizSectionId = GenericIdentifiers._4ID,
                    QuizSectionIndex = 1,
                    Body = "1. {THROTTLE IDLE}",
                },
                new Question
                {
                    Id = GenericIdentifiers._12ID,
                    QuizSectionId = GenericIdentifiers._4ID,
                    QuizSectionIndex = 2,
                    Body = "2. {WHEEL BRAKES APPLY}",
                },
                new Question
                {
                    Id = GenericIdentifiers._13ID,
                    QuizSectionId = GenericIdentifiers._4ID,
                    QuizSectionIndex = 3,
                    Body = "3. {HOOK DOWN IF REQ}",
                },

                // F16 CAPS S5 (ENGINE FAILURE AFTER TAKE-OFF)
                new Question
                {
                    Id = GenericIdentifiers._14ID,
                    QuizSectionId = GenericIdentifiers._5ID,
                    QuizSectionIndex = 1,
                    Body = "1. {ZOOM}",
                },
                new Question
                {
                    Id = GenericIdentifiers._15ID,
                    QuizSectionId = GenericIdentifiers._5ID,
                    QuizSectionIndex = 2,
                    Body = "2. {STORES JETT IF POSS}",
                },
                new Question
                {
                    Id = GenericIdentifiers._16ID,
                    QuizSectionId = GenericIdentifiers._5ID,
                    QuizSectionIndex = 3,
                    Body = "3. {EJECT}",
                },

                // F16 CAPS S6 (ENGINE FAILURE AFTER TAKE-OFF)
                new Question
                {
                    Id = GenericIdentifiers._17ID,
                    QuizSectionId = GenericIdentifiers._6ID,
                    QuizSectionIndex = 1,
                    Header = "If AB fire is observed, retard throttle to MIL.",
                    Body = "1. {CLIMB}",
                },
                new Question
                {
                    Id = GenericIdentifiers._18ID,
                    QuizSectionId = GenericIdentifiers._6ID,
                    QuizSectionIndex = 2,
                    Body = "2. {STORES JETT IF REQ}",
                },

                // F16 CAPS S7 (A/B MALFUNCTION ON TAKE-OFF)
                new Question
                {
                    Id = GenericIdentifiers._19ID,
                    QuizSectionId = GenericIdentifiers._7ID,
                    QuizSectionIndex = 1,
                    Body = "1. {THROTTLE MIL}",
                },
                new Question
                {
                    Id = GenericIdentifiers._20ID,
                    QuizSectionId = GenericIdentifiers._7ID,
                    QuizSectionIndex = 2,
                    Header = "If normal thrust not available in MIL, carry out LOW THRUST ON TAKEOFF CAP.",
                    Body = "2. {STORES JETT IF REQ}",
                },

                // F16 CAPS S8 (LOW THRUST ON TAKE-OFF)
                new Question
                {
                    Id = GenericIdentifiers._21ID,
                    QuizSectionId = GenericIdentifiers._8ID,
                    QuizSectionIndex = 1,
                    Body = "1. {THROTTLE AB}",
                },
                new Question
                {
                    Id = GenericIdentifiers._22ID,
                    QuizSectionId = GenericIdentifiers._8ID,
                    QuizSectionIndex = 2,
                    Body = "2. {STORES JETT IF REQ}",
                },
                new Question
                {
                    Id = GenericIdentifiers._23ID,
                    QuizSectionId = GenericIdentifiers._8ID,
                    QuizSectionIndex = 3,
                    Header = "If PRI thrust is still insufficient to maintain level flight at a safe altitude:",
                    Body = "3. {ENG CONT SEC}",
                },

                // F16 CAPS S9 (ENGINE FAILURE / AIRSTART)
                new Question
                {
                    Id = GenericIdentifiers._24ID,
                    QuizSectionId = GenericIdentifiers._9ID,
                    QuizSectionIndex = 1,
                    Header="Do not attempt airstart if due to engine seizure or fuel starvation.",
                    Body = "1. {ZOOM}",
                },
                new Question
                {
                    Id = GenericIdentifiers._25ID,
                    QuizSectionId = GenericIdentifiers._9ID,
                    QuizSectionIndex = 2,
                    Header="Do not attempt airstart if due to engine seizure or fuel starvation.",
                    Body = "2. {STORES JETT IF REQ}",
                },
                new Question
                {
                    Id = GenericIdentifiers._26ID,
                    QuizSectionId = GenericIdentifiers._9ID,
                    QuizSectionIndex = 3,
                    Header="Do not attempt airstart if due to engine seizure or fuel starvation.",
                    Body = "3. {THROTTLE OFF THEN MIDRANGE}",
                },
                new Question
                {
                    Id = GenericIdentifiers._27ID,
                    QuizSectionId = GenericIdentifiers._9ID,
                    QuizSectionIndex = 4,
                    Header="Do not attempt airstart if due to engine seizure or fuel starvation.",
                    Body = "4. {AIRSPEED AS REQ}",
                },
                new Question
                {
                    Id = GenericIdentifiers._28ID,
                    QuizSectionId = GenericIdentifiers._9ID,
                    QuizSectionIndex = 5,
                    Header = "If RPM snaps to zero or tower shaft failure is suspected,",
                    Body = "5. {ENG CONT SEC}",
                },
                new Question
                {
                    Id = GenericIdentifiers._29ID,
                    QuizSectionId = GenericIdentifiers._9ID,
                    QuizSectionIndex = 6,
                    Header = "Below 20000ft and 400 KIAS,",
                    Body = "6. {JFS START 2}",
                },
                new Question
                {
                    Id = GenericIdentifiers._30ID,
                    QuizSectionId = GenericIdentifiers._9ID,
                    QuizSectionIndex = 7,
                    Body = "7. {EPU ON}",
                },

                // F16 CAPS S10 (OUT OF CONTROL RECOVERY)
                new Question
                {
                    Id = GenericIdentifiers._31ID,
                    QuizSectionId = GenericIdentifiers._10ID,
                    QuizSectionIndex = 1,
                    Body = "1. {CONTROLS REL}",
                },
                new Question
                {
                    Id = GenericIdentifiers._32ID,
                    QuizSectionId = GenericIdentifiers._10ID,
                    QuizSectionIndex = 2,
                    Body = "2. {THROTTLE IDLE}",
                },
                new Question
                {
                    Id = GenericIdentifiers._33ID,
                    QuizSectionId = GenericIdentifiers._10ID,
                    QuizSectionIndex = 3,
                    Body = "3. {FLCS RESET RESET}",
                },
                new Question
                {
                    Id = GenericIdentifiers._34ID,
                    QuizSectionId = GenericIdentifiers._10ID,
                    QuizSectionIndex = 4,
                    Header = "If aircraft remains in out of control conditions and/or in a deep stall,",
                    Body = "4. {MPO OVRD AND HOLD}",
                },
                new Question
                {
                    Id = GenericIdentifiers._35ID,
                    QuizSectionId = GenericIdentifiers._10ID,
                    QuizSectionIndex = 5,
                    Body = "5. {STICK CYCLE IN PHASE}",
                },








                // F16 Monthly S1
                new Question
                {
                    Id = GenericIdentifiers._36ID,
                    QuizSectionId = GenericIdentifiers._11ID,
                    QuizSectionIndex = 1,
                    Body = "Below airstart parameters, attempt to climb above {MSEA} and do not delay {EJECTION} except to avoid populace.",
                },
                new Question
                {
                    Id = GenericIdentifiers._37ID,
                    QuizSectionId = GenericIdentifiers._11ID,
                    QuizSectionIndex = 2,
                    Body ="If within FO parameters, priority is to execute {FO} approach followed by {AIRSTART} procedures.",
                },
                new Question
                {
                    Id = GenericIdentifiers._38ID,
                    QuizSectionId = GenericIdentifiers._11ID,
                    QuizSectionIndex = 3,
                    Body = "If outside FO parameters, priority is to execute {AIRSTART}.",
                },


                // F16 Monthly S2
                new Question
                {
                    Id = GenericIdentifiers._39ID,
                    QuizSectionId = GenericIdentifiers._12ID,
                    QuizSectionIndex = 1,
                    Body = "If engine malfunction occurs, attempt to climb above {MSEA} and track for {NEAREST KEY} position. Once established above min ejection altitude, analyse the EP.",
                },
                new Question
                {
                    Id = GenericIdentifiers._40ID,
                    QuizSectionId = GenericIdentifiers._12ID,
                    QuizSectionIndex = 2,
                    Body = "If in AB: SNAP THROTTLE TO {MIL}",
                },
                new Question
                {
                    Id = GenericIdentifiers._41ID,
                    QuizSectionId = GenericIdentifiers._12ID,
                    QuizSectionIndex = 3,
                    Body = "Check {RPM} and {FTIT}",
                },
                new Question
                {
                    Id = GenericIdentifiers._42ID,
                    QuizSectionId = GenericIdentifiers._12ID,
                    QuizSectionIndex = 4,
                    Body = "If both are stabilised in an operating range and an engine problem still exists, execute {LOW THRUST CAP} or {ABNORMAL ENG RESPONSE} checklist if appropriate.",
                },
                new Question
                {
                    Id = GenericIdentifiers._43ID,
                    QuizSectionId = GenericIdentifiers._12ID,
                    QuizSectionIndex = 5,
                    Body = "Check {RPM}, {FTIT} and {THROTTLE RESPONSE}",
                },
                new Question
                {
                    Id = GenericIdentifiers._45ID,
                    QuizSectionId = GenericIdentifiers._12ID,
                    QuizSectionIndex = 6,
                    Body = "If RPM decreases below {60}% at a moderate rate or FTIT increases above {1090}°C (ie compressor stall symptoms: RPM decrease, FTIT increase) and engine is not responding to throttle movement, initiate an {AIRSTART}.",
                },

                // F16 Monthly S3
                new Question
                {
                    Id = GenericIdentifiers._46ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 1,
                    Body ="Above 30000ft MSL, dive at {400} kts / {0.9} M",
                },
                new Question
                {
                    Id = GenericIdentifiers._147ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 2,
                    Body = "Below 30000ft MSL, establish {250} kts (no JFS)",
                },
                new Question
                {
                    Id = GenericIdentifiers._148ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 3,
                    Body = "All SEC (SEC Light on) Airstarts – {250} kts min",
                },

                new Question
                {
                    Id = GenericIdentifiers._47ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 4,
                    Header = "If JFS run light confirmed below 20,000ft,",
                    Body = "MAX RANGE C:{200} D:{210} D+:{215}"
                },
                new Question
                {
                    Id = GenericIdentifiers._48ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 5,
                    Header = "If JFS run light confirmed below 20,000ft,",
                    Body = "MAX ENDURANCE C:{170} D:{180} D+:{185}",
                },
                new Question
                {
                    Id = GenericIdentifiers._149ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 6,
                    Header = "If JFS run light confirmed below 20,000ft,",
                    Body = "* Add {5} kts for every 1000 lb of fuel/stores",
                },
                new Question
                {
                    Id = GenericIdentifiers._49ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 7,
                    Body = "Select MAX {RANGE} airspeed if within FO parameters to landing airfield. Otherwise select MAX {ENDURANCE} airspeed to maximise time available for airstart.",
                },
                new Question
                {
                    Id = GenericIdentifiers._50ID,
                    QuizSectionId = GenericIdentifiers._13ID,
                    QuizSectionIndex = 8,
                    Body = "After airstart is complete, do not turn off JFS and EPU if indicated RPM is below {60}% with adequate thrust (ie tower shaft failure)",
                },

                // F16 Monthly S4
                new Question
                {
                    Id = GenericIdentifiers._51ID,
                    QuizSectionId = GenericIdentifiers._14ID,
                    QuizSectionIndex = 1,
                    Body =  "Below {4000} ft AGL, there may be insufficient time to perform an airstart prior to {MSEA}.",
                },
                new Question
                {
                    Id = GenericIdentifiers._52ID,
                    QuizSectionId = GenericIdentifiers._14ID,
                    QuizSectionIndex = 2,
                    Body ="If airspeed is below {350} kts, maintain constant altitude deceleration to desired airspeed.",
                },
                new Question
                {
                    Id = GenericIdentifiers._53ID,
                    QuizSectionId = GenericIdentifiers._14ID,
                    QuizSectionIndex = 3,
                    Body = "At and above {350} kts, more time is available by a zoom climb using {3}G pullup to {30} deg climb, approaching the desired airspeed (use approximately {50} kts lead point) and then initiating a {0}G pushover.",
                },

                // F16 Monthly S5
                new Question
                {
                    Id = GenericIdentifiers._54ID,
                    QuizSectionId = GenericIdentifiers._15ID,
                    QuizSectionIndex = 1,
                    Body = "Throttle – {MIL} or {BELOW}",
                },
                new Question
                {
                    Id = GenericIdentifiers._55ID,
                    QuizSectionId = GenericIdentifiers._15ID,
                    QuizSectionIndex = 2,
                    Body = "Stores – {JETT} (if REQ)",
                },
                new Question
                {
                    Id = GenericIdentifiers._56ID,
                    QuizSectionId = GenericIdentifiers._15ID,
                    QuizSectionIndex = 3,
                    Body = "Airspeed – {250} kts",
                },
                new Question
                {
                    Id = GenericIdentifiers._57ID,
                    QuizSectionId = GenericIdentifiers._15ID,
                    QuizSectionIndex = 4,
                    Body = "If unable to maintain {1000} ft above min ejection alt/MSA:",
                },
                new Question
                {
                    Id = GenericIdentifiers._58ID,
                    QuizSectionId = GenericIdentifiers._15ID,
                    QuizSectionIndex = 5,
                    Body = "Engine Control Switch – {SEC}",
                },
                new Question
                {
                    Id = GenericIdentifiers._59ID,
                    QuizSectionId = GenericIdentifiers._15ID,
                    QuizSectionIndex = 6,
                    Body = "Throttle – {850}°C max if possible",
                },

                // F16 Monthly S6
                new Question
                {
                    Id = GenericIdentifiers._60ID,
                    QuizSectionId = GenericIdentifiers._16ID,
                    QuizSectionIndex = 1,
                    Body = "Keep fuel flow at {4000} PPH minimum",
                },

                // F16 Monthly S7
                new Question
                {
                    Id = GenericIdentifiers._61ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 1,
                    Body = "Max range airspeed is approximately {7} deg AOA",
                },
                new Question
                {
                    Id = GenericIdentifiers._62ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 2,
                    Body ="Max RNG (LG up)  C:{200} D:{210} D+:{215}",
                },
                new Question
                {
                    Id = GenericIdentifiers._63ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 3,
                    Body = "OPT (LG down) C:{190} D:{200} D+:{205}",
                },
                new Question
                {
                    Id = GenericIdentifiers._64ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 4,
                    Body = "Min (LG down) C:{180} D:{190} D+:{195}"
                },
                new Question
                {
                    Id = GenericIdentifiers._65ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 5,
                    Body = "Add {5} kts for every 1000 lb of fuel/stores",
                },
                new Question
                {
                    Id = GenericIdentifiers._66ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 6,
                    Header = "EPU quantity required for FO landing",
                    Body = "8nm Straight In JFS OFF:{45}% JFS ON:{40}%",
                },
                new Question
                {
                    Id = GenericIdentifiers._67ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 7,
                    Header = "EPU quantity required for FO landing",
                    Body = "High Key / 4nm JFS OFF:{25}% JFS ON:{20}%",
                },
                new Question
                {
                    Id = GenericIdentifiers._68ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 8,
                    Body = "Low LG when aimpoint is {11-17} deg below horizon Min. runway length (dry without arresting gear) – {8000} ft",
                },
                new Question
                {
                    Id = GenericIdentifiers._69ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 9,
                    Body = "For flameout approach, recommended min weather – {3000} ft AGL or computed {MSA} (whichever is applicable).",
                },
                new Question
                {
                    Id = GenericIdentifiers._70ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 10,
                    Body ="For degraded engine, verify engine is operating satisfactorily in {SEC}. Land via visual {STRAIGHT IN} or {INSTRUMENT} approach (marginal weather). Remain within SFO parameters for as long as practical (consider pilot proficiency). If engine operation is not satisfactory in SEC, execute {SFO} to land.",
                },
                new Question
                {
                    Id = GenericIdentifiers._71ID,
                    QuizSectionId = GenericIdentifiers._17ID,
                    QuizSectionIndex = 11,
                    Body = "If weather and operational conditions are not favourable and safety of populace cannot be assured during the approach, consider {DIVERSION} (if possible) or {EJECTION}.",
                },

                // F16 Monthly S8
                new Question
                {
                    Id = GenericIdentifiers._72ID,
                    QuizSectionId = GenericIdentifiers._18ID,
                    QuizSectionIndex = 1,
                    Header = "CONDITIONS: GRD START",
                    Body = "FTIT °C  {800}",
                    Footer = "During cold starts, oil pressure may be 100 PSI for up to 1 min",
                },
                new Question
                {
                    Id = GenericIdentifiers._73ID,
                    QuizSectionId = GenericIdentifiers._18ID,
                    QuizSectionIndex = 2,
                    Header = "CONDITIONS: GRD IDLE",
                    Body = "FTIT °C {625} | OIL PSI {15 MIN}",
                    Footer = "Maximum FTIT in SEC is 650°C",
                },
                new Question
                {
                    Id = GenericIdentifiers._74ID,
                    QuizSectionId = GenericIdentifiers._18ID,
                    QuizSectionIndex = 3,
                    Header = "CONDITIONS: GRD MIL / AB",
                    Body = "FTIT °C {1070} | RPM% {97} | OIL PSI {30-95}",
                    Footer = "At MIL and above, oil pressure must increase 15 PSI minimum above IDLE oil pressure. Use transient RPM limit for take-off. Note: Oil pressure drop (below 15 PSI) increase requirement) during power up is acceptable",
                },
                new Question
                {
                    Id = GenericIdentifiers._75ID,
                    QuizSectionId = GenericIdentifiers._18ID,
                    QuizSectionIndex = 4,
                    Header = "CONDITIONS: GRD TRANS",
                    Body = "FTIT °C {1090} | RPM% {98} | OIL PSI {30-95}",
                    Footer = "Time above 1070°C limited to 10 seconds",
                },
                new Question
                {
                    Id = GenericIdentifiers._76ID,
                    QuizSectionId = GenericIdentifiers._18ID,
                    QuizSectionIndex = 5,
                    Header = "CONDITIONS: GRD FLUX.",
                    Body = "FTIT °C {+/-10} | RPM% {+/-1} | OIL PSI IDLE {+/-5} | OIL PSI >IDLE {+/-10}",
                    Footer = "Flux within steady state limits. Nozzle flux limited to +/-2% at and above MIL. Flux not permitted below MIL",
                },

                // F16 Monthly S9
                new Question
                {
                    Id = GenericIdentifiers._77ID,
                    QuizSectionId = GenericIdentifiers._19ID,
                    QuizSectionIndex = 1,
                    Header = "CONDITIONS: INFLT AIRSTART",
                    Body = "FTIT °C {870}"
                },
                new Question
                {
                    Id = GenericIdentifiers._78ID,
                    QuizSectionId = GenericIdentifiers._19ID,
                    QuizSectionIndex = 2,
                    Header = "CONDITIONS: INFLT IDLE",
                    Body = "OIL PSI {15 MIN}"
                },
                new Question
                {
                    Id = GenericIdentifiers._79ID,
                    QuizSectionId = GenericIdentifiers._19ID,
                    QuizSectionIndex = 3,
                    Header = "CONDITIONS: INFLT MIL / AB",
                    Body = "FTIT °C {1070} | RPM% {97} | OIL PSI {30-95}",
                    Footer = "Oil pressure must increase as RPM increases. Use transient RPM limit with LG handle DN and for 3 minutes after LG handle is placed UP",
                },
                new Question
                {
                    Id = GenericIdentifiers._80ID,
                    QuizSectionId = GenericIdentifiers._19ID,
                    QuizSectionIndex = 4,
                    Header = "CONDITIONS: INFLT TRANS",
                    Body = "FTIT °C {1090} | RPM% {98} | OIL PSI {30-95}",
                    Footer = "Time above 1070°C limited to 10 seconds",
                },
                new Question
                {
                    Id = GenericIdentifiers._81ID,
                    QuizSectionId = GenericIdentifiers._19ID,
                    QuizSectionIndex = 5,
                    Header = "CONDITIONS: INFLT FLUX",
                    Body = "FTIT °C {+/-10} | RPM% {+/-1} | OIL PSI IDLE {+/-5} | OIL PSI >IDLE {+/-10}",
                    Footer ="Same as Grd Ops. Zero oil pressure is allowable for periods up to 1 minute during flight at less than +1G",
                },

                // F16 Monthly S10
                new Question
                {
                    Id = GenericIdentifiers._82ID,
                    QuizSectionId = GenericIdentifiers._20ID,
                    QuizSectionIndex = 1,
                    Header = "Avoid –ve G Flight If:",
                    Footer = "",
                    Body = "Engine Feed Knob: {OUT OF NORM}"
                },
                new Question
                {
                    Id = GenericIdentifiers._83ID,
                    QuizSectionId = GenericIdentifiers._20ID,
                    QuizSectionIndex = 2,
                    Header = "Avoid –ve G Flight If:",
                    Footer = "",
                    Body = "Forward / Aft Resevoir: {NOT FULL}"
                },

                // F16 Monthly S11
                new Question
                {
                    Id = GenericIdentifiers._84ID,
                    QuizSectionId = GenericIdentifiers._21ID,
                    QuizSectionIndex = 1,
                    Header = " -ve G Flight (with full resv. fuel)",
                    Footer = "",
                    Body = "MIL Thrust {30} secs Max"
                },
                new Question
                {
                    Id = GenericIdentifiers._85ID,
                    QuizSectionId = GenericIdentifiers._21ID,
                    QuizSectionIndex = 2,
                    Header = " -ve G Flight (with full resv. fuel)",
                    Footer = "",
                    Body = "AB Thrust {10} secs Max"
                },

                // F16 Monthly S12
                new Question
                {
                    Id = GenericIdentifiers._86ID,
                    QuizSectionId = GenericIdentifiers._22ID,
                    QuizSectionIndex = 1,
                    Header = "Max Arrestment",
                    Footer = "",
                    Body = "Routine {150} (CD) / {157} (D+) kts"
                },
                new Question
                {
                    Id = GenericIdentifiers._87ID,
                    QuizSectionId = GenericIdentifiers._22ID,
                    QuizSectionIndex = 2,
                    Header = "Speed",
                    Footer = "",
                    Body = "Emergency {160} (CD) / {164} (D+) kts"
                },
                new Question
                {
                    Id = GenericIdentifiers._88ID,
                    QuizSectionId = GenericIdentifiers._22ID,
                    QuizSectionIndex = 3,
                    Header = "Tire Speed (NLG / MLG)",
                    Footer = "",
                    Body = "{217} / {225} (CD)  /  {235} / {250} (D+) kts"
                },
                new Question
                {
                    Id = GenericIdentifiers._89ID,
                    QuizSectionId = GenericIdentifiers._22ID,
                    QuizSectionIndex = 4,
                    Header = "Canopy Open",
                    Footer = "",
                    Body = "{70} kts"
                },


                // F16 Monthly S13
                new Question
                {
                    Id = GenericIdentifiers._90ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 1,
                    Header = "LG Extending / Retracting",
                    Footer = "",
                    Body = "KIAS / MACH {300} / {0.65} | SYM G {+2} / {0}"
                },
                new Question
                {
                    Id = GenericIdentifiers._91ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 2,
                    Header = "LG Down and Locked",
                    Footer = "",
                    Body = "KIAS / MACH {300} / {0.65} | SYM G {+4} / {0}"
                },
                new Question
                {
                    Id = GenericIdentifiers._92ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 3,
                    Header = "Crosswind Limits (Day and Night) - Dry / Wet",
                    Footer = "For CAT D and above",
                    Body = "D/D+ Model* {20} / {20}"
                },
                new Question
                {
                    Id = GenericIdentifiers._93ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 4,
                    Header = "Crosswind Limits (Day and Night) - Dry / Wet",
                    Footer = "For CAT D and above",
                    Body = "C Model* {25} / {20}"
                },
                new Question
                {
                    Id = GenericIdentifiers._94ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 5,
                    Header = "Crosswind Limits (Day and Night) - Dry / Wet",
                    Footer = "",
                    Body = "OCU {15} / {15}"
                },
                new Question
                {
                    Id = GenericIdentifiers._95ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 6,
                    Header = "AR Door Opening / Closing",
                    Footer = "",
                    Body = "{400} / {0.85} KIAS/MACH"
                },
                new Question
                {
                    Id = GenericIdentifiers._96ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 7,
                    Header = "AR Door Open",
                    Footer = "",
                    Body = "{400} / {0.95} KIAS/MACH"
                },
                new Question
                {
                    Id = GenericIdentifiers._97ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex = 8,
                    Header = "Flight in Adverse WX",
                    Footer = "",
                    Body = "{300} KIAS"
                },
                new Question
                {
                    Id = GenericIdentifiers._98ID,
                    QuizSectionId = GenericIdentifiers._23ID,
                    QuizSectionIndex =9,
                    Header = "Flight in Turbulence (+/- 3G)",
                    Footer = "",
                    Body = "{500} KIAS"
                },
            };
        }
    }
}
