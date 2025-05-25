using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
class MainClass
{   public static float score = 0f; 
    static Dictionary<string, string> labels;
    static string currentPackage = null;
    static string currentInfo = null;
    static void Main()
    {
       LoadFiles();
        Console.WriteLine(LoadLabel("hello"));
        while (true)
        {
            string command = Console.ReadLine();
            switch (command) {
                case "ex":
                    Exit();
                    break;
                case "h":
                    Console.WriteLine(LoadLabel("help"));
                    break;
                case "p":
                    PickScenario();
                    break;
                case "pc":
                    if (currentPackage != null)
                    {
                        Console.WriteLine(LoadLabel("currentPackage") + currentPackage);
                        Console.WriteLine(LoadLabel("currentInfo") + currentInfo);
                    }
                    else
                    {
                        Console.WriteLine(LoadLabel("noPackage"));
                    }
                    break;
                case "rn":
                    RunSpecified();
                    break;
                case "rs":
                    ResetScore();
                    break;
                case "sc":
                    Console.WriteLine(score);
                    break;
                case "sw":
                    SwitchPackage();
                    break;
                default:
                    Console.WriteLine(LoadLabel("noCommand"));
                    break;
             }
        }
    }
    public static string LoadLabel(string key)
    {
        if (labels.ContainsKey(key))
        {
            return labels[key]; 
        }
        else
        {
            return "LABEL NOT FOUND IN used.json";
        }
    }
    static void ResetScore()
    {
        Console.WriteLine(LoadLabel("resetScoreQue"));
        if(Console.ReadLine().ToLower().Equals("y"))
        {
            score = 0f;
            Console.WriteLine(LoadLabel("done"));
        }
    }
    static void Exit()
    {
        Console.WriteLine(LoadLabel("exitQue"));
        if (Console.ReadLine().ToLower().Equals("y"))
        {
            Console.WriteLine(LoadLabel("goodbye"));
            Environment.Exit(42);
        }
    }
    static void SwitchPackage()
    {
        try
        {
            Console.WriteLine(LoadLabel("selectPackage"));
            string[] packList = Directory.GetDirectories("Packages");

            for (int i = 0; i < packList.Count(); i++)
            {
                Console.WriteLine(i + 1 + " | " + packList[i].Remove(0, 9)); //Remove(0, 9) removes  Packages\ substring
            }

            Console.Write(LoadLabel("packNumberQue"));
            int packId = 0;
            int.TryParse(Console.ReadLine(), out packId);

            if (packId >= 1 && packId <= packList.Count())
            {
                currentPackage = packList[packId-1].Remove(0, 9);
                try
                {
                    currentInfo = File.ReadAllText(packList[packId-1] + "\\About.txt");
                 }
                catch (FileNotFoundException e)
                {
                    currentInfo = LoadLabel("noInfo");
                }
            }
            else
            {
                Console.WriteLine(LoadLabel("wrongPackNumber")+ packList.Count()+ LoadLabel("!"));  
            }
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(LoadLabel("packFolderError"));
            Environment.Exit(45);
        }
    }
    static void LoadFiles()
    {
        try
        {
            labels = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Languages\\used.json"));
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("LANGUAGE FOLDER NOT FOUND! The folder must be named \"Languages\"");
            Environment.Exit(43);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("LANGUAGE FILE NOT FOUND! The file must be named \"used.json\"");
            Environment.Exit(44);
        }
        
    }
    static void PickScenario()
    {
        if (currentPackage == null)
        {
            Console.WriteLine(LoadLabel("noPackage"));
            return;
        }
        string[] scenarios = Directory.GetFiles("Packages\\" + currentPackage, "*.json");
        if (scenarios.Count() == 0)
        {
            Console.WriteLine(LoadLabel("emptyPackage"));
            return;
        }
        Random rnd = new Random();
        Game.VerifyScenario(scenarios[rnd.Next(scenarios.Count())]);
    }
    static void RunSpecified()
    {
        string path = null;
        Console.WriteLine(LoadLabel("pathQue"));
        path = Console.ReadLine();
        if (!File.Exists(path))
        {
            Console.WriteLine(LoadLabel("noPath"));
            return;
        }
        else if(!Regex.IsMatch(path, @"\.json$", RegexOptions.IgnoreCase)) //I don't know regex. Generated by Deepseek.
        {
            Console.WriteLine(LoadLabel("notJson"));
            return;
        }
        Game.VerifyScenario(path);
    }
}