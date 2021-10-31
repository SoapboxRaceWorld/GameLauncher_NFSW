using GameLauncher.App.Classes.LauncherCore.Logger;
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
            public List<object> List { get; set; }
        }

        public static bool ValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
            {
                return false;
            }
            else
            {
                try
                {
                    strInput = strInput.Trim();
                    if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || /* For object */
                        (strInput.StartsWith("[") && strInput.EndsWith("]"))) /* For array */
                    {
                        try
                        {
                            var obj = JToken.Parse(strInput);
                            return true;
                        }
                        catch (JsonReaderException Error)
                        {
                            /* Exception in parsing json */
                            LogToFileAddons.OpenLog("VALID JSON", null, Error, null, true);
                            return false;
                        }
                        catch (Exception Error)
                        {
                            /* General Exception */
                            LogToFileAddons.OpenLog("VALID JSON", null, Error, null, true);
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("VALID JSON", null, Error, null, true);
                    return false;
                }
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

                    if (omy.List != null && omy.List.Count > 0)
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
