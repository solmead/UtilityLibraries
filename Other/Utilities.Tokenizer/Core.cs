using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Utilities.Poco;

namespace Utilities.Tokenizer
{
    public static class Core
    {
        public static string Tokenize(this string message, object obj, string propertyBaseName = "")
        {
            var dc = obj.PropertiesAsDictionary();
            if (!string.IsNullOrWhiteSpace(propertyBaseName))
            {
                var dic = dc;
                dc = new Dictionary<string, string>();
                foreach(var it in dic)
                {
                    dc.Add(propertyBaseName + it.Key, it.Value);
                }
            }

            return message.Tokenize(dc);
        }
        //public static string Tokenize(this string message, object obj)
        //{
        //    return message.Tokenize(obj.PropertiesAsDictionary());
        //}
        public static string Tokenize(this string message, string key, string value)
        {
            var dc = new Dictionary<string, string>
            {
                { key, value }
            };

            return message.Tokenize(dc);
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
            //c = c.RemoveBetween("{[", "]}");
            var dataEvaluate = c.GetNextBetween("[/", "/]");
            while (!string.IsNullOrWhiteSpace(formulaEvaluate))
            {
                var finEval = "";
                if (tokens.ContainsKey(dataEvaluate))
                {
                    finEval = tokens[dataEvaluate];
                }
                else
                {
                    finEval = "[/" + dataEvaluate + "/]";
                }
                c = c.Replace("[/" + dataEvaluate + "/]", finEval);
                dataEvaluate = c.GetNextBetween("[/", "/]");
            }
            //c = c.RemoveBetween("[/", "/]");
            return c;
        }

        public static string ClearRemainingTokens(this string message)
        {
            var c = message;
            c = c.RemoveBetween("{[", "]}");
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
