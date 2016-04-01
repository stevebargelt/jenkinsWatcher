using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jenkinsWatcher;
using System.Configuration;

namespace JenkinsWatcherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["jenkinsUrl"];
            var apiToken = ConfigurationManager.AppSettings["jenkinsApiKey"];
            var username = ConfigurationManager.AppSettings["jenkinsUserName"];

            var jenkins = new jenkinsWatcher.jenkinsWatcher(url, apiToken, username);
            var result = jenkins.GetAllStatuses();
            Console.WriteLine("Getting all results");
            foreach (Job jo in result)
            {
                Console.WriteLine(jo.name + " " + jo.color);
            }

            var singleResult = jenkins.GetStatus("Test Project99");
            Console.WriteLine("\n\nGetting single result");
            Console.WriteLine(singleResult.name + " " + singleResult.color);

            var searchNames = new List<String> { "Test Project", "TestProject"};
            var multiResult = jenkins.GetStatus( searchNames );
            Console.WriteLine("\n\nGetting multi result");
            foreach (Job jo in multiResult)
            {
                Console.WriteLine(jo.name + " " + jo.color);
            }

            Console.ReadLine();

        }
    }
}
