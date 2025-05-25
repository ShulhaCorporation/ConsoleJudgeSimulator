using System;
using System.Text.Json;

class Defendant
{
    public string name { get; set; }
    public string rightSentence { get; set; }
    public float sentenceValue { get; set; }
}

class Scenario
{
    public string story { get; set; }
    public Defendant[] defendants { get; set; }
    public string explanation { get; set; }
}

class Sentence
{
    public string name;
    public bool isValued;
    public float errorMultiplier;

    public Sentence(string name, bool isValued, float errorMultiplier)
    {
        this.name = name;
        this.isValued = isValued;
        this.errorMultiplier = errorMultiplier;
    }
}

    class Game
    {
        private static string story;
        private static Defendant[] defendants;
        private static string explanation;
        private static Scenario scenario;
        private static Dictionary<string, Sentence> sentences = new Dictionary<string, Sentence>
   {
       {"j", new Sentence("justify", false, 0f)},
       {"f", new Sentence("fine", true, 0.1f)},
       {"cs", new Sentence("comServ", true, 0.5f)},
       {"cl", new Sentence("corLabor", true, 2f)},
       {"con", new Sentence("confiscate", false, 0f)},
       {"ad", new Sentence("admArr", false, 0f)},
       {"pr", new Sentence("prison", true, 5f)},
       {"lp", new Sentence("lifePrison", false, 0f)},
       {"dp", new Sentence("death", false, 0f)}
   };

        public static void VerifyScenario(string path)
        {    bool errorFound = false;
            try
            {
                scenario = JsonSerializer.Deserialize<Scenario>(File.ReadAllText(path));
                defendants = scenario.defendants;

                if (scenario.story != null)
                {
                    story = scenario.story;
                }
                else
                {
                    Console.WriteLine(MainClass.LoadLabel("noStory") + path + MainClass.LoadLabel("noStory2"));
                    errorFound = true;
                }

                explanation = scenario.explanation;                 

                for(int i = 0; i < defendants.Count(); i++)
                {

                if (String.IsNullOrWhiteSpace(defendants[i].name))
                 { errorFound = true;
                    Console.WriteLine(MainClass.LoadLabel("defNo")+ (i+1) + MainClass.LoadLabel("noName"));
                 }

                if (String.IsNullOrWhiteSpace(defendants[i].rightSentence))
                {
                    errorFound = true;
                    Console.WriteLine(MainClass.LoadLabel("defNo") + (i+1) + MainClass.LoadLabel("hasNoSent"));
                }
                else if (!IsSentence(defendants[i].rightSentence))
                {
                    errorFound = true;
                    Console.WriteLine(MainClass.LoadLabel("sentence") + defendants[i].rightSentence + MainClass.LoadLabel("inDefNo") + (i+1) + MainClass.LoadLabel("notListed"));
                }
                }
            }
            catch (System.Text.Json.JsonException e)
            {
                Console.WriteLine(MainClass.LoadLabel("badJson") + path + MainClass.LoadLabel("badJson2"));
                throw e;
            }
        if (!errorFound)
        {
            Play();
        }
        else
        {
            Console.WriteLine(path + MainClass.LoadLabel("containsErrors"));
        }
        }

        public static void Play()
        {
            Console.Clear();
            Console.WriteLine(story);
            foreach (Defendant defendant in defendants)
            {
                Console.WriteLine(MainClass.LoadLabel("decideQue")+defendant.name + MainClass.LoadLabel("decideQue2"));
                while (true)
                {
                    string command = Console.ReadLine();
                    if (command.Equals("sl"))
                    {
                        foreach (string key in sentences.Keys)
                        {
                            Console.WriteLine(key +" | "+ MainClass.LoadLabel(sentences[key].name));
                        }
                    }
                    else if (sentences.ContainsKey(command))
                    {
                      CheckSentence(defendant, command);
                      break;
                    }
                    else
                    {
                    Console.WriteLine(MainClass.LoadLabel("noSentence"));
                    }
                }
            }
        if (explanation != null)
        {
            Console.WriteLine(MainClass.LoadLabel("explanation") + explanation);
        }
        }
    public static void CheckSentence(Defendant defendant, string shortName)
    {
        if (!sentences[shortName].name.ToLower().Equals(defendant.rightSentence.ToLower()))
        {
            MainClass.score -= 10f;
            Console.WriteLine(MainClass.LoadLabel("wrongSentence") + defendant.rightSentence);
            return;
        }

        if (!sentences[shortName].isValued)
        {
            MainClass.score += 10f;
            Console.WriteLine(MainClass.LoadLabel("correctSentence"));
            return;
        }

        Console.WriteLine(MainClass.LoadLabel("valueQue"));
        float answer;
        string ansStr = Console.ReadLine().Replace('.',','); //'.' turns into ',' because TryParse does not recognize '.' as a decimal point.
        float.TryParse(ansStr, out answer);
        float points = 10 - Math.Abs(defendant.sentenceValue - answer) * sentences[shortName].errorMultiplier;
        MainClass.score += points;
        if (points == 10)
        {
            Console.WriteLine(MainClass.LoadLabel("correctSentence"));
        }
        else
        { string plusSign = "";
            if (points > 0) {
                plusSign = "+";
            }
            Console.WriteLine(MainClass.LoadLabel("inaccurate") + plusSign + points + MainClass.LoadLabel("scorePoints"));
            Console.WriteLine(MainClass.LoadLabel("rightValue") + defendant.sentenceValue);
        }
    }
    public static bool IsSentence(string testedName)
    {
        bool isSentence = false;
        foreach(Sentence sentence in sentences.Values)
        {
            if (sentence.name.ToLower().Equals(testedName.ToLower()))
            {
                isSentence = true;
                break;
            }
        }
        return isSentence;
    }

    }