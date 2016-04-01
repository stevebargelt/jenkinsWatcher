using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace JenkinsWatcherUni
{
    public class JenkinsJob
    {

        private const string namekey = "name";
        private const string urlkey = "url";
        private const string colorkey = "color";
        
        private string name;
        private string url;
        private string color;

        public JenkinsJob()
        {
            name = "";
            url = "";
            color = "";
        }

        public JenkinsJob(JsonObject jsonObject)
        {
            name = jsonObject.GetNamedString(namekey, "");
            url = jsonObject.GetNamedString(urlkey, "");
            color = jsonObject.GetNamedString(colorkey, "");
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                name = value;
            }
        }
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                url = value;
            }
        }

        public string Color
        {
            get
            {
                return color;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                color = value;
            }
        }


    }
}
