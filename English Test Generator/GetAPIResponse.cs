﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using English_Test_Generator;
using Newtonsoft.Json;
using JSON_lib;
using System.Windows.Forms;

namespace GetAPIResponse
{
    enum LexicalCategory
    {
        AllTypes, Adjective, Adverb, Noun, Idiomatic, Verb
    }                  
    class Definitions
    {
        public static string Request(string lexicalCategory, string word)
        {
            string cache = "";
            if (CacheWord.Check(word, "Definitions"))
            {
                return CacheWord.Read(word, "Definitions", lexicalCategory);
            }
            string definitions = ""; // variable to store the result
            string url = "https://od-api.oxforddictionaries.com:443/api/v1/entries/en/" + word + "/definitions;regions=" + Form1.region; // URL for the request 
            HttpClient client = new HttpClient(); // creates an HTTP Client
            HttpResponseMessage response = new HttpResponseMessage(); // used to get the API Response            
            client.BaseAddress = new Uri(url); // sets the client address to the specified url
            client.DefaultRequestHeaders.Add("app_id", Form1.app_Id); // adds the id to the headers
            client.DefaultRequestHeaders.Add("app_key", Form1.app_Key); // adds the key to the headers
            try { response = client.GetAsync(url).Result; }// gets the respone headers   
            catch (Exception) { MessageBox.Show("Unable to connect to the internet. Restart the program with internet connectivity at least once!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);}
            if (response.IsSuccessStatusCode) // checks if the response code is equal to 200
            {
                string content = response.Content.ReadAsStringAsync().Result; // receives the API response              
                var result = JsonConvert.DeserializeObject<GetResponse>(content); // Converts the API response to the format that the program can understand
                for (int i = 0; i < result.Results.First().LexicalEntries.Length; i++) // i = all entries from the API response
                {                
                    for (int j = 0; j < result.Results.First().LexicalEntries[i].Entries.Length ; j++) // j = all senses from the API response
                    {                        
                        for (int k = 0; k < result.Results.First().LexicalEntries[i].Entries[j].Senses.Length; k++) // k = all definitions from the API response 
                        {
                            if (result.Results.First().LexicalEntries[i].LexicalCategory.ToLower() == lexicalCategory || lexicalCategory == "") // checks if the current lexicalCategory matches the one designated by the user
                            {
                                definitions += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - DEFINITIONS]\n"
                                + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Definitions.First() + "\n"; // adds the definition to the variable                               
                            }
                            cache += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - DEFINITIONS]\n"
                                + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Definitions.First() + "\n";
                            if (result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses !=null) // checks if there is at least one subsense in the current sense 
                            {
                                for (int l = 0; l < result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses.Length; l++) // l = all subsense definitions from the API response
                                {
                                    if (result.Results.First().LexicalEntries[i].LexicalCategory.ToLower() == lexicalCategory || lexicalCategory == "") // checks if the current lexicalCategory matches the one designated by the user
                                    {
                                        definitions += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - DEFINITIONS]\n"
                                        + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses[l].Definitions.First() + "\n"; // adds the definition to the variable
                                    }
                                    cache += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - DEFINITIONS]\n"
                                        + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses[l].Definitions.First() + "\n";
                                }
                            }
                        }                    
                    }
                }
                CacheWord.Write(word, "Definitions", cache);
                return definitions; // returns the result 
            }
            else // if the response code is different than 200
            {                
                return "Couldn't find " + word + " sorry about that. Status: " + response.StatusCode; // error while trying to access the API 
            }           
        }
        public static string get(LexicalCategory category, string word)
        {          
            switch (category) // requests the method for the corresponding category
            {
                case LexicalCategory.AllTypes:
                    return Request("",word);                  
                case LexicalCategory.Adjective:
                    return Request("adjective", word);                                    
                case LexicalCategory.Adverb:
                    return Request("adverb", word);                
                case LexicalCategory.Noun:
                    return Request("noun", word);                  
                case LexicalCategory.Idiomatic:
                    return Request("idiomatic", word);
                case LexicalCategory.Verb:
                    return  Request("verb", word);
                default:
                    return "Couldn't find the specified lexical category!";
            }          
        }
        public static string get(string category, string word)
        {
           return get(map.FirstOrDefault(x => x.Value == category).Key, word); // uses the map to call the get method with the proper arguments
        }                               
        public static Dictionary<LexicalCategory, string> map = // dictionary used as a "map" for each type
            new Dictionary<LexicalCategory, string>
            {
                { LexicalCategory.AllTypes, "All Types"},
                { LexicalCategory.Adjective, "adjective"},
                { LexicalCategory.Adverb, "adverb"},
                { LexicalCategory.Noun, "noun"},
                { LexicalCategory.Idiomatic, "idiomatic"},
                { LexicalCategory.Verb, "verb"},
            };
    }
    class Examples
    {
        public static string Request(string lexicalCategory, string word)
        {
            string cache = ""; 
            if (CacheWord.Check(word, "Examples"))
            {
                return CacheWord.Read(word, "Examples", lexicalCategory);
            }
            string examples = "";
            string url = "https://od-api.oxforddictionaries.com:443/api/v1/entries/en/" + word + "/examples;regions=" + Form1.region; // URL for the request 
            HttpClient client = new HttpClient(); // creates an HTTP Client
            HttpResponseMessage response; // used to get the API Response            
            client.BaseAddress = new Uri(url); // sets the client address to the specified url
            client.DefaultRequestHeaders.Add("app_id", Form1.app_Id); // adds the id to the headers
            client.DefaultRequestHeaders.Add("app_key", Form1.app_Key); // adds the key to the headers
            response = client.GetAsync(url).Result; // gets the response
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result; // receives the API response              
                var result = JsonConvert.DeserializeObject<GetResponse>(content); // Converts the API response to the format that the program can understand
                for (int i = 0; i < result.Results.First().LexicalEntries.Length; i++) // i = all entries from the API response
                {
                    for (int j = 0; j < result.Results.First().LexicalEntries[i].Entries.Length; j++) // j = all senses from the API response
                    {
                        for (int k = 0; k < result.Results.First().LexicalEntries[i].Entries[j].Senses.Length; k++) // k = all examples from the API response 
                        {
                            for (int l = 0; l < result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Examples.Length; l++) // l = all text in the current example from the API response
                            {
                                if (result.Results.First().LexicalEntries[i].LexicalCategory.ToLower() == lexicalCategory || lexicalCategory == "") // checks if the current lexicalCategory matches the one designated by the user
                                {
                                    examples += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - EXAMPLES]\n"
                                        + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Examples[l].Text + "\n"; // adds the example to the variable
                                }
                                cache += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - EXAMPLES]\n"
                                        + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Examples[l].Text + "\n";
                            }
                            if (result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses != null) // checks if there is at least one subsense in the current sense 
                            {
                                for (int l = 0; l < result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses.Length; l++) // l = all subsense definitions from the API response
                                {
                                    for (int m = 0; m < result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses[l].Examples.Length;  m++) // m = all text in the current example from the API response
                                    {
                                        if (result.Results.First().LexicalEntries[i].LexicalCategory.ToLower() == lexicalCategory || lexicalCategory == "") // checks if the current lexicalCategory matches the one designated by the user
                                        {
                                            examples += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - EXAMPLES]\n"
                                                + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses[l].Examples[m].Text + "\n"; // adds the example to the variable 
                                        }
                                        cache += "[" + result.Results.First().LexicalEntries[i].LexicalCategory.ToUpper() + " - EXAMPLES]\n"
                                                + result.Results.First().LexicalEntries[i].Entries[j].Senses[k].Subsenses[l].Examples[m].Text + "\n";
                                    }
                                }
                            }
                        }
                    }
                }
                CacheWord.Write(word, "Examples", cache);
                return examples;
            }
            else // if the response code is different than 200
            {
                return "Couldn't find " + word + " sorry about that. Status: " + response.StatusCode; // error while trying to access the API 
            }
        }       
    }
}
