using System.Collections.Generic;
using Utilities.Poco;

namespace Utilities.Tokenizer
{
    public static class Core
    {
        public static string Tokenize(this string message, object obj)
        {
            return message.Tokenize(obj.PropertiesAsDictionary());
        }

        public static string Tokenize(this string message, Dictionary<string, string> tokens)
        {
            var c = message;
            //{[CreditType=Invoice,"mailed to your billing address for payment on [ChargeDate]"]}
            var formulaEvaluate = c.GetNextBetween("{[", "]}");
            while (!string.IsNullOrWhiteSpace(formulaEvaluate))
            {
                var finEval = "";

                c = c.Replace("{[" + formulaEvaluate + "]}", finEval);
                formulaEvaluate = c.GetNextBetween("{[", "]}");
            }
            c = c.RemoveBetween("{[", "]}");

            foreach (var key in tokens.Keys)
            {
                c = c.Replace("[/" + key + "/]", tokens[key]);
            }
            c = c.RemoveBetween("[/", "/]");
            return c;
        }
        

        public static string StripHTML(this string Content)
        {
            return RemoveBetween(Content, "<", ">").Replace("&nbsp;", " ");
        }
        
        public static string RemoveBetween(this string Content, string BeginChar, string EndChar)
        {
            int i = Content.IndexOf(BeginChar);
            int cnt = 0;
            while (i >= 0 && cnt < 10000)
            {
                var E = Content.IndexOf(EndChar, i + BeginChar.Length);
                if (E > i + BeginChar.Length)
                {
                    Content = Content.Substring(0, i) + Content.Substring(E + EndChar.Length);
                }
                else
                {
                    cnt = 10001;
                }
                i = Content.IndexOf(BeginChar);
            }
            return Content;
        }
        
        public static string GetNextBetween(this string Content, string BeginChar, string EndChar, int StartPos = 0)
        {
            int i = Content.IndexOf(BeginChar, StartPos);
            int cnt = 0;
            while (i >= 0 && cnt < 10000)
            {
                var E = Content.IndexOf(EndChar, i + BeginChar.Length);
                if (E > i + BeginChar.Length)
                {
                    i = i + BeginChar.Length;
                    return Content.Substring(i, E - i);
                }
                else
                {
                    cnt = 10001;
                }
                i = Content.IndexOf(BeginChar);
            }
            return "";
        }


    }
}
