using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotTelega
{
    public class FromSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        //это word_test
        //в это WordTest
        public override string ConvertName(string name)
        {
            var parts = name.Split('_');
            for (int i = 0; i < parts.Length; i++)
            {
                var current = parts[i][0].ToString().ToUpperInvariant();
                parts[i] = current + parts[i][1..];
            }

            var result = "";
            foreach (var part in parts)
                result += part;

            return result;
        }
    }
}
