﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Tonkenizer.Filters.PreFilters
{
    public class OnlyWordsPreFilter : PreFilter
    {

        private static Dictionary<string, string> defaultAbbreviationsTable = new Dictionary<string, string>()
        {
            {"<3","Heart"},
            {"2C4U","Too cool for you"},
            {"2DFM","Too dumb for me"},
            {"2FB","Too Freaking Bad"},
            {"2H2H","Too hot to handle"},
            {"2L8","Too Late"},
            {"2MR","Tomorrow"},
            {"2U2","To you too"},
            {"4ever","Forever"},
            {"4N","Foreign"},
            {"4RL","For real"},
            {"4U","For You"},
            {"4ward","Forward"},
            {"A3","Anyplace, Anywhere, Anytime"},
            {"AAMOF","As A Matter Of Fact"},
            {"Ab","About"},
            {"ABT","About"},
            {"Addy","Address"},
            {"ADPIC","Always Dependably Politically Incorrect"},
            {"AFAICS","As Far As I Can See"},
            {"AFAIK","As Far As I Know"},
            {"AFAYC","As Far As You're Concerned"},
            {"AFDA","A Few Days Ago"},
            {"Agl","Angel"},
            {"AHFY","Always here for you"},
            {"AISI","As I See It"},
            {"Alt","A lot"},
            {"AMA","Ask Me Anything"},
            {"AML","All my love"},
            {"ANFSCD","And Now For Something Completely Different"},
            {"AOMM","Always on my mind"},
            {"ASAIC","As Soon As I Can"},
            {"ASP","After Show Party"},
            {"ATM","At The Moment"},
            {"Attwaction","Attraction To A Twitter User"},
            {"ATW","All the way"},
            {"AWHFY","Are We Having Fun Yet"},
            {"AWOL","Absent while online"},
            {"AYC","Are you coming"},
            {"AYPI","And Your Point Is"},
            {"AYSOS","Are You Stupid Or Something"},
            {"B/C","Because"},
            {"B4","Before"},
            {"BAF","Bring A Friend"},
            {"BAMF","Bad Ass Motherfucker"},
            {"BB","Bright blessings"},
            {"BBB","Boring Beyond Belief"},
            {"BBL","Be Back Later"},
            {"BBQ","Barbecue"},
            {"BBW","Be back whenever"},
            {"BCBG","Bon Chic Bon Genre"},
            {"BCO","Big crush on"},
            {"BD","Big Deal"},
            {"bday","Birthday"},
            {"BESTIE","Best Friend"},
            {"BF","Boyfriend"},
            {"BFF4L","Best Friends Forever For Life"},
            {"BFFN","Best friend for now"},
            {"BFK","Big Fat Kiss"},
            {"BFN","Bye For Now"},
            {"BFTD","Best friends till death"},
            {"BGD","Background"},
            {"BGF","Best Girl Friend"},
            {"BIL","Brother In Law"},
            {"BION","Believe It or Not"},
            {"BITS","Back in the Saddle"},
            {"blieve","Believe"},
            {"BMA","Best mates always"},
            {"BMGWL","Busting my gut with laughter"},
            {"BNO","Boys night out"},
            {"BR","Best regards"},
            {"BRB","Be Right Back"},
            {"Bro","Brother"},
            {"bst","Best"},
            {"bt","About"},
            {"BTW","By The Way"},
            {"BTWITILY","By the way, I think I love you"},
            {"BUF","Big, Ugly, Fat"},
            {"BUP","Backup Plan"},
            {"BW","Best Wishes"},
            {"BYOD","Bring Your Own Device"},
            {"C","See"},
            {"C&G","Chuckle and Grin"},
            {"C4N","Ciao For Now"},
            {"CHILLAX","Chill and relax"},
            {"CHK","Check"},
            {"cld","Could"},
            {"clk","Click"},
            {"CMIIW","Correct Me If I'm Wrong"},
            {"CNRHKYITF","Chuck Norris roundhouse kick you in the face"},
            {"CPE","Coolest Person Ever"},
            {"cre8","Create"},
            {"Crew","Group Of Friends"},
            {"CRTLA","Can't Remember The Three Letter Acronym"},
            {"CSB","Cool Story, Bro"},
            {"CSL","Can't stop laughing"},
            {"CTO","Check This Out"},
            {"CU2MR","See you tomorrow"},
            {"CX","Correction"},
            {"CYF","Check your Facebook"},
            {"CYL","Catch ya later"},
            {"CYM","Check your MySpace"},
            {"D","Direct"},
            {"D/C","Don't care"},
            {"DBTS","Don't Believe That Stuff"},
            {"dc","Daycare"},
            {"deets","Details"},
            {"DF","Dear Fiance"},
            {"DFTBA","Don't forget to be awesome"},
            {"DGA","Digital Guardian Angel"},
            {"DGAF","Don't give a fuck"},
            {"DGYF","Dang Girl, You're Fine"},
            {"DH","Dear Husband"},
            {"DIL","Daughter-in-law"},
            {"DILLIGAFF","Does it look like I give a flying flip"},
            {"DITG","Down in the Gutter"},
            {"DL","Doing Laundry"},
            {"DM","Direct Message"},
            {"DMY","Don't Mess Yourself"},
            {"DP","Dear partner"},
            {"DS","Dear Son"},
            {"DTF","Down to Fuck"},
            {"DTRT","Do The Right Thing"},
            {"DU","Darn you"},
            {"DUCT","Did You See That"},
            {"Dweet","Drunk Tweet"},
            {"DYAC","Damn You Auto Correct"},
            {"DYD","Don't you dare"},
            {"DYK","Did You Know"},
            {"e1","Everyone"},
            {"ELO","Electric Light Orchestra"},
            {"Em","Email"},
            {"EMA","Email Address"},
            {"EML","Email"},
            {"enuf","Enough"},
            {"EOR","End Of Rant"},
            {"EYC","Excited Yet Calm"},
            {"F2F","Face to Face"},
            {"F9","Fine"},
            {"Fab","Fabulous"},
            {"FAF","Funny as fluff"},
            {"Fav","Favorite"},
            {"favs","Favorites"},
            {"FAWC","For Anyone Who Cares"},
            {"FB","Facebook"},
            {"FBC","FaceBook Chat"},
            {"FBF","Facebook Friend"},
            {"FBO","Facebook Official"},
            {"FCOL","For Crying Out Loud"},
            {"FF","Friends Forever"},
            {"FFS","For Fuck's Sake"},
            {"FH","Future Husband"},
            {"FHO","Friends Hanging Out"},
            {"FIL","Father In Law"},
            {"FML","Fuck My Life"},
            {"FNB","Football and beer"},
            {"FOB","Fresh Off the Boat"},
            {"FOH","Fuck Outta Here"},
            {"FOMO","Fear of Missing Out"},
            {"FOMOH","Fear Of Missing Out On Hockey"},
            {"Fri","Endless Friend"},
            {"frnd","Friend"},
            {"FTF","Face to Face"},
            {"FTL","For The Loss"},
            {"FTR","For The Record"},
            {"FTTB","For The Time Being"},
            {"FTW","For The Win"},
            {"FUSIT","Fouled up situation"},
            {"FW","Future Wife"},
            {"FWIW","For What It's Worth"},
            {"FYA","For Your Amusement"},
            {"FYE","For Your Entertainment"},
            {"FYF","From your Friend"},
            {"FYI","For Your Information"},
            {"G2TU","Got to tell you"},
            {"G9","Genius"},
            {"GBH&K","Great Big Hug And Kiss"},
            {"GBY","God bless you"},
            {"GF","Girlfriend"},
            {"GFN","Gone For Now"},
            {"GG","Good Game"},
            {"GGG","Gotta get a grip"},
            {"GGGB","Good girl gone bad"},
            {"GIC","God's in control"},
            {"GIF","Graphics Interchange Format"},
            {"gma","Grandma"},
            {"GMAB","Give Me A Break"},
            {"GNO","Girls night out"},
            {"GPOY","Gratuitous Picture Of Yourself"},
            {"GPOYW","Gratuitous Picture Of Yourself Wednesday"},
            {"GPS","God's Positioning System"},
            {"GR8","Great"},
            {"GTFO","Get The Fuck Out"},
            {"GTG","Good To Go"},
            {"GUFN","Grounded Until Further Notice"},
            {"Gz","Congratulations"},
            {"H&K","Hugs and kisses"},
            {"H/T","Hat Tip"},
            {"H8","Hate"},
            {"H8t","Hate"},
            {"HB","Hug back"},
            {"HBBD","Happy Belated Birthday"},
            {"HC","How cool"},
            {"HLBD","Happy late birthday"},
            {"HMB","Hit Me Back"},
            {"HML","Hate My Life"},
            {"HMP","Help Me, Please"},
            {"HMU","Hit me up"},
            {"HNY","Happy New Year"},
            {"HOM","Hit or miss"},
            {"HOPE","Have Only Positive Expectations"},
            {"HPDC","Happy People Don't Complain"},
            {"HRU","How Are You"},
            {"HT","Heard Through"},
            {"HUMM","Hope You Miss Me"},
            {"HUN","Honey"},
            {"HWU","Hey, what's up"},
            {"HYFR","Heck Ya, Fucking Right"},
            {"IA","I agree"},
            {"IAB","In a bit"},
            {"IAG","It's all good"},
            {"IAGSMSOL","I am getting some money sooner or later"},
            {"IAIL","I am in love"},
            {"IC","I see"},
            {"ICYMI","In case you missed it"},
            {"ICYWW","In Case You Were Wondering"},
            {"IDC","I don't care"},
            {"IDD","Indeed"},
            {"IDEK","I don't even know"},
            {"IFSFY","I feel sorry for you"},
            {"IFVB","I feel very bad"},
            {"IHU","I hate you"},
            {"IIR","If I Remember"},
            {"IIRC","If I Remember Correctly"},
            {"IJDGAF","I just don't give a fuck"},
            {"IJWTS","I Just Want To Say"},
            {"IKR","I Know Right"},
            {"ILMM","I love my man"},
            {"ILUGTD","I love you guys to death"},
            {"ILYL","I love you lots"},
            {"ILYLC","I love you like crazy"},
            {"IM","Instant Message"},
            {"IMAO","In My Arrogant Opinion"},
            {"IMD","In my dreams"},
            {"IME","In My Experience"},
            {"IMHO","In My Humble Opinion"},
            {"IMO","In My Opinion"},
            {"IMPO","In My Personal Opinion"},
            {"IMSO","In my sovereign opinion"},
            {"IMU","I Miss You"},
            {"ion","I Don't"},
            {"IOU","I owe you"},
            {"IOW","In Other Words"},
            {"IR8","Irate"},
            {"IRL","In Real Life"},
            {"IRSTBO","It really sucks the big one"},
            {"ITMT","In the meantime"},
            {"itz","It is"},
            {"IWU","I want you"},
            {"J/K","Just Kidding"},
            {"JFK","Just Fucking Kidding"},
            {"JK","Just Kidding"},
            {"JLMK","Just let me know"},
            {"JMO","Just My Opinion"},
            {"JSYK","Just so you know"},
            {"JTLYK","Just to let you know"},
            {"JUIL","Just you I love"},
            {"K","Okay"},
            {"KEWL","Cool"},
            {"KHYF","Know How You Feel"},
            {"KIR","Keepin' it real"},
            {"KK","Cool Cool"},
            {"KMSL","Killing Myself Laughing"},
            {"KWIM","Know What I Mean"},
            {"KYSOTI","Keep your stick on the ice"},
            {"L2M","Listening to Music"},
            {"L8","Late"},
            {"L8er","Later"},
            {"L8R","Later"},
            {"LA","Laughing a lot"},
            {"LAFS","Love at first sight"},
            {"LAN","Local Area Network"},
            {"LBVS","Laughing But Very Serious"},
            {"LEP","Love emo people"},
            {"LFTD","Laugh for the day"},
            {"LI","LinkedIn"},
            {"LIG","Life is good"},
            {"LIL","Little"},
            {"lk","Like"},
            {"LLPOF","Liar Liar Pants On Fire"},
            {"LLS","Laughing like shit"},
            {"LMAO","Laughing My Ass Off"},
            {"LMFAO","Laughing My Fat Ass Off"},
            {"LMK","Let Me Know"},
            {"LML","Love My Life"},
            {"LMS","Like MY Status"},
            {"LOC","Load of crap"},
            {"LOL","Laugh Out Loud"},
            {"LOLOL","Lots of laugh out louds"},
            {"LOLZ","Laugh out louds"},
            {"LSS","Long story short"},
            {"LTD","Lovers till death"},
            {"LTR","Long Term Relationship"},
            {"LUGLS","Love you guys like sisters"},
            {"LUM","Love you man"},
            {"LYLAB","Love You Like A Brother"},
            {"LYSM","Love You So Much"},
            {"M02","My two cents"},
            {"MCM","Mancrush Monday"},
            {"Meh","Equivalent To Shrugging One's Shoulders"},
            {"MIL","Mother In Law"},
            {"MLIA","My life is average"},
            {"MM","Music Monday"},
            {"MMK","Umm, Ok"},
            {"MOFO","Motherfucker"},
            {"MRT","Modified Retweet"},
            {"MTF","More To Follow"},
            {"mum","Mom"},
            {"MYOB","Mind Your Own Business"},
            {"Mysp","MySpace"},
            {"NB","Not bad"},
            {"NBD","No Big Deal"},
            {"NGL","Not Gonna Lie"},
            {"njoy","Enjoy"},
            {"NLT","No Later Than"},
            {"NM","Not Much"},
            {"NOBMR","None of my business, right"},
            {"NOMB","None of my business"},
            {"NOYB","None Of Your Business"},
            {"NP","No Problem"},
            {"NSFW","Not Safe For Work"},
            {"NTS","Note to self"},
            {"NTW","Not To Worry"},
            {"nvm","Nevermind"},
            {"NW","No Way"},
            {"O RLY","Oh Really"},
            {"o_O","Raised Eyebrow"},
            {"Obv","Obviously"},
            {"OG","Original Gangster"},
            {"OH","Overheard"},
            {"OIC","Oh, I see"},
            {"OLTL","One Life to Live"},
            {"OME","Oh my Edward"},
            {"OMG","Oh My God"},
            {"OMGD","Oh my gosh, duh"},
            {"OMW","On My Way"},
            {"ONYD","Oh No You Didn't"},
            {"OOH","Out of here"},
            {"OOMF","One Of My Followers"},
            {"OOTD","Outfit of the day"},
            {"OTE","Over the edge"},
            {"OTS","On the Scene"},
            {"PBUH","Peace be upon him"},
            {"PC4PC","Picture comment for picture comment"},
            {"PDA","Public Display Of Affection"},
            {"PDH","Pretty darn hot"},
            {"Peeps","People"},
            {"Pic","Picture"},
            {"PIMP","Peeing In My Pants"},
            {"PIO","Pass it on"},
            {"PITA","Pain in the Ass"},
            {"PL0X","Please"},
            {"PLJ","Peace, love, joy"},
            {"plz","Please"},
            {"PMKI","Pretty Much Killing It"},
            {"POS","Piece Of Shit"},
            {"Poser","Pretender"},
            {"POTB","Pat on the back"},
            {"POTD","Photo of the Day"},
            {"POTUS","President of the United States"},
            {"PPL","People"},
            {"pple","People"},
            {"props","Proper Respect"},
            {"PRT","Please Retweet"},
            {"PSA","Public Service Announcement"},
            {"PSOG","Pure stroke of genius"},
            {"PTFO","Passed The Fuck Out"},
            {"PTL","Praise the Lord"},
            {"PWN","Own"},
            {"Pwnd","Owned"},
            {"Q","Question"},
            {"R","Are"},
            {"RATM","Rage Against the Machine"},
            {"RDF","Real deal feeling"},
            {"ridic","Ridiculous"},
            {"RLRT","Real Life Retweet"},
            {"RLY","Really"},
            {"ROFL","Rolling On The Floor Laughing"},
            {"ROFLMAO","Rolling On Floor Laughing My Ass Off"},
            {"ROTF","Rolling On The Floor"},
            {"rox","Rocks"},
            {"RT","Retweet"},
            {"rthx","Retweet Thanks"},
            {"RU","Are you"},
            {"S4S","Share For Share"},
            {"SAHD","Stay-At-Home Dad"},
            {"SAHM","Stay-at-home mom"},
            {"SAHP","Stay-at-Home Parent"},
            {"SAT","Sorry about that"},
            {"SD","Sweet Dreams"},
            {"SFF","So freaking funny"},
            {"SFW","Safe For Work"},
            {"shld","Should"},
            {"SIL","Sister In Law"},
            {"SLM","Socially limiting maneuver"},
            {"SMF","So Much Fun"},
            {"SMFH","Shaking My Fucking Head"},
            {"SMH","Shakes My Head"},
            {"SOAB","Son of a Bitch"},
            {"SOH","Sense Of Humor"},
            {"SOL","Shit Outta Luck"},
            {"SOS","Someone Special"},
            {"SPST","Same place, same time"},
            {"srs","Serious"},
            {"STFU","Shut The Fuck Up"},
            {"sup","What's Up"},
            {"SUX","Sucks"},
            {"Swag","Boasting About One's Skills"},
            {"TAC","That Ain't Cool"},
            {"TAY","Thinking about you"},
            {"TBH","To Be Honest"},
            {"TBR","To Be Read"},
            {"TBT","Throwback Thursday"},
            {"TC","Take Care"},
            {"TCOB","Takin' Care of Business"},
            {"TFF","Twitter Follower-Friend"},
            {"TFTA","Thanks for the Add"},
            {"TGFF","Thank God For Friday"},
            {"TGIF","Thank God It's Friday"},
            {"thx","Thanks"},
            {"TIA","Thanks In Advance"},
            {"TIC","Tongue In Cheek"},
            {"TIL","Today I Learned"},
            {"TISNF","That Is So Not Fair"},
            {"TITF","That is too funny"},
            {"TL","Time Line"},
            {"TLDR","Too Long, Didn't Read"},
            {"TMAI","Tell Me About It"},
            {"TMB","Tweet Me Back"},
            {"TMDF","To my dearest friend"},
            {"TMFT","Too Much Free Time"},
            {"TMG","That's My Girl"},
            {"TMI","Too Much Information"},
            {"TOH","The other half"},
            {"TPM","Tweets Per Minute"},
            {"TPT","Trailer park trash"},
            {"TQVM","Thank you very much"},
            {"TRS","That really sucks"},
            {"TT","Trending Topic"},
            {"TTTT","These Things Take Time"},
            {"TTY","Talk To You"},
            {"TTYL","Talk To You Later"},
            {"TTYN","Talk To You Never"},
            {"TTYOOTD","Talk to you one of these days"},
            {"TTYS","Talk To You Soon"},
            {"TU","Thank you"},
            {"Tweeps","Twitter Updates"},
            {"Twerminology","Twitter Terminology"},
            {"Twexplanation","Twitter Explanation"},
            {"Twiends","Twitter Friends"},
            {"Twike","Twitter Bike"},
            {"Twistory","Twitter Story"},
            {"Twittonary","Twitter Dictionary"},
            {"Twittsomnia","Twitter Insomnia"},
            {"Twittworking","Twitter Networking"},
            {"Twollower","Twitter Follower"},
            {"Twoops","Twitter Oops"},
            {"Tword","Twitter Word"},
            {"TWSI","That was so ironic"},
            {"TWSS","That's what she said"},
            {"TWTI","That was totally ironic"},
            {"tx","Thanks"},
            {"TXT","Text"},
            {"TY","Thank You"},
            {"TYT","Take Your Time"},
            {"TYVM","Thank You Very Much"},
            {"U","You"},
            {"U2U","Up to you"},
            {"ugh","Disgusted"},
            {"ul","Unlucky"},
            {"ULKGR8","You look great"},
            {"UOME","You owe me"},
            {"UR","Your"},
            {"URTB","You Are The Best"},
            {"u've","You Have"},
            {"V2V","Voice To Voice"},
            {"Vacay","Vacation"},
            {"VBG","Very Big Grin"},
            {"W","With"},
            {"w/","With"},
            {"W/E","Weekend"},
            {"W2F","Way too funny"},
            {"W2G","Way To Go"},
            {"WADR","With All Due Respect"},
            {"Wazzup","What's Up"},
            {"WCW","Womancrush Wednesday"},
            {"WDYT","What do you think"},
            {"WEG","Wicked Evil Grin"},
            {"whatddup","What's Up"},
            {"WL","Whatta loser"},
            {"wld","Would"},
            {"woz","Was"},
            {"WTF","What The Fuck"},
            {"WTFE","What the frick ever"},
            {"WTFNF","What the fucking fuck"},
            {"WTV","Whatever"},
            {"WV","With"},
            {"WYWH","Wish You Were Here"},
            {"xbf","Ex-Boyfriend"},
            {"XFER","Transfer"},
            {"xgf","Ex-Girlfriend"},
            {"XO","Kiss and Hug"},
            {"XTL","Cross The Line"},
            {"YDI","You deserved it"},
            {"YGG","You go girl"},
            {"YKWIM","You Know What I Mean"},
            {"YM","You're Welcome"},
            {"YMMV","Your Mileage May Vary"},
            {"YOLO","You Only Live Once"},
            {"Yr","Your"},
            {"YRAG","You are a geek"},
            {"YRL","You are lame"},
            {"YSJ","You're so jealin"},
            {"YT","YouTube"},
            {"YVW","You're Very Welcome"},
            {"YW","You're Welcome"},
            {"YYSS","Yeah yeah, sure sure"},
            {"ZOMG","Oh My God"}
        };
        
        
        private bool _removeLinks;
        private bool _replaceAbbreviations;
        private Dictionary<string, string> _abbreviationsTable;


        public OnlyWordsPreFilter(PreFilter next, bool removeLinks, bool replaceAbbreviations, bool byDefault, string filename)
            : base(next) 
        {
            _removeLinks = removeLinks;
            _replaceAbbreviations = replaceAbbreviations;
            _abbreviationsTable = byDefault ? defaultAbbreviationsTable : CreateDictionaryFromFile(filename);
        }

        private Dictionary<string, string> CreateDictionaryFromFile(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach(string line in lines)
            {
                string[] splittedString = line.Split('\t');
                ret.Add(splittedString[0], splittedString[1]);
            }
            return ret;
        }

        protected override List<string> DoFilter(List<string> docs)
        {
            List<string> ret = new List<string>();
            Regex linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex delimiter = new Regex("([ \\t{}():;. \n])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (string doc in docs)
            {
                string newDoc = doc;
                if (_removeLinks)
                {
                   newDoc = linkParser.Replace(doc, String.Empty);
                }
                if (_replaceAbbreviations)
                {
                    StringBuilder builder = new StringBuilder();
                    String[] tokens = delimiter.Split(newDoc);
                    foreach (string token in tokens)
                    {
                        if (defaultAbbreviationsTable.Keys.Contains(token))
                        {
                            builder.Append(defaultAbbreviationsTable[token]);
                        }
                        else
                        {
                            builder.Append(token);
                        }
                    }
                    newDoc = builder.ToString();
                }
                ret.Add(newDoc);
            }
            return ret;
        }

    }
}
