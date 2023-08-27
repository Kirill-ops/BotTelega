using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BotTelega
{
    public class ToSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        //это WordTest
        //в это word_test
        public override string ConvertName(string name)
        {
            var res = Regex.Split(name, "(?=\\p{Lu})");

            var result = "";
            for (int i = 1; i < res.Length - 1; i++)
                result += res[i].ToLower() + "_";
            result += res[^1].ToLower();

            return result;
        }
    }
}
