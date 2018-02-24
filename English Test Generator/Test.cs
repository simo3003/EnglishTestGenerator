﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GetAPIResponse;

namespace English_Test_Generator
{
    class Test
    {
        public static void FillDictionary(string test_Words)
        {
            string[] splitByNewLine = test_Words.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitByNewLine.Length; i++)
            { 
                string[] splitByAsterisk = splitByNewLine[i].Split(new string[] {"*"}, StringSplitOptions.RemoveEmptyEntries);
                Form1.test_WordsAndTypes.Add(splitByAsterisk[0], splitByAsterisk[1]);
            }
            foreach (KeyValuePair<string, string> entry in Form1.test_WordsAndTypes)
            {
                MessageBox.Show(entry.Key + "\n" + entry.Value);
            }
        }
        public static string Generate(string test_Type, int test_ExcerciseAmount, string test_Name, Dictionary<string, string> test_Words, string region)
        {
            List<string> exercises = new List<string>();
            string finishedTest = "";
            Form1.fr.progressBar1.Visible = true;
            foreach (KeyValuePair<string, string> entry in test_Words)
            {

                switch (test_Type)
                {
                    case "Definitions":                    
                        Read(Definitions.get(entry.Value, entry.Key));
                        break;
                    case "Examples":
                        Read(Examples.get(entry.Value, entry.Key));
                        break;
                }
            }
            foreach (var exercise in exercises)
            {
                MessageBox.Show(exercise);
            }
            return "";
        }
        public static string Read(string source)
        {
            string[] lines = source.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string filteredSource = "";
            using (StringReader sr = new StringReader(source))
            {
                for (int i = 1; i <= lines.Length; i += 2)
                {
                    filteredSource += lines[i] + "\n";
                }
            }
            MessageBox.Show(filteredSource);
            return filteredSource;
        }
    }
}
