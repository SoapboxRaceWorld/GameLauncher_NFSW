using GameLauncher.App.Classes.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.Validator.JSON
{
    class IsJSONValid
    {
        public class ClassWithList
        {
            public List<object> list { get; set; }
        }

        public static bool ValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Log.Error(jex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    //General Exception
                    Log.Error(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /* Fun Fact the code below is broken - DavidCarbon */

        /* Bug: JSON Always Reports back as Being Empty for Non Empty JSON strings */
        public static bool EmptyJson(string strInput)
        {
            if (ValidJson(strInput) == true)
            {
                try
                {
                    var omy = JsonConvert.DeserializeObject<ClassWithList>(strInput);

                    if (omy.list != null && omy.list.Count > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
