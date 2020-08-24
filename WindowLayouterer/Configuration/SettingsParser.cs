using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowLayouterer.Domain;

namespace WindowLayouterer.Configuration
{
    public class SettingsParser
    {
        public Settings Parse(string settingsText)
        {
            var jobj = JsonConvert.DeserializeObject(settingsText) as JObject;
            var windows = jobj["Windows"].ToList();
            var screenAreas = jobj["ScreenAreas"].ToList();
            jobj.Remove("Windows");
            jobj.Remove("ScreenAreas");
            foreach (var layout in jobj["Layouts"].ToList())
            {
                ReplaceKey(layout);
                foreach (var s in layout["ScreenAreas"].ToList())
                {
                    var sa = s;
                    if (sa.Type == JTokenType.String)
                    {
                        var sa1 = screenAreas.SingleOrDefault(s => s["Name"].Value<string>() == sa.Value<string>());
                        if (sa1 == null)
                            throw new ApplicationException($"Screen area reference {sa.Value<string>()} could not be found");
                        var sa2 = sa1.DeepClone() as JObject;
                        sa.Replace(sa2);
                        sa = sa2;
                    }
                    else
                    {
                        var sa1 = screenAreas.SingleOrDefault(s => s["Name"].Value<string>() == sa["Name"].Value<string>());
                        if (sa1 != null)
                        {
                            var sa2 = sa1.DeepClone() as JObject;
                            sa2.Merge(sa);
                            sa.Replace(sa2);
                            sa = sa2;
                        }
                    }
                    foreach (var wi in sa["Windows"].ToList())
                    {
                        var w = wi; 
                        var w1 = windows.SingleOrDefault(s => s["Name"].Value<string>() == w.Value<string>());
                        if (w1 == null)
                            throw new ApplicationException($"Window reference {sa.Value<string>()} could not be found");
                        var w2 = w1.DeepClone() as JObject;
                        w.Replace(w2);
                        w = w2;
                    }
                    sa["Left"] = sa["Left"].Value<string>().Replace("%", "");
                    sa["Top"] = sa["Top"].Value<string>().Replace("%", "");
                    sa["Width"] = sa["Width"].Value<string>().Replace("%", "");
                    sa["Height"] = sa["Height"].Value<string>().Replace("%", "");
                    ReplaceKey(sa);
                } 
            }
            
            return jobj.ToObject<Settings>();
        }

        private void ReplaceKey(JToken sa)
        {
            sa["HotKey"].Replace(new JObject(new JProperty("Key", 0), new JProperty("Modifiers", 0)));
        }
    }
}
