using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jenkinsWatcher
{
    public class JenkinsObject
    {
        public List<Assignedlabel> assignedLabels { get; set; }
        public string mode { get; set; }
        public string nodeDescription { get; set; }
        public string nodeName { get; set; }
        public int numExecutors { get; set; }
        public object description { get; set; }
        public List<Job> jobs { get; set; }
        public Overallload overallLoad { get; set; }
        public Primaryview primaryView { get; set; }
        public bool quietingDown { get; set; }
        public int slaveAgentPort { get; set; }
        public Unlabeledload unlabeledLoad { get; set; }
        public bool useCrumbs { get; set; }
        public bool useSecurity { get; set; }
        public List<View> views { get; set; }
    }

    public class Overallload
    {
        public string load { get; set; }
    }

    public class Primaryview
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Unlabeledload
    {
        public string unlabeled { get; set; }
    }

    public class Assignedlabel
    {
        public string assignedlabel { get; set; }
    }

    public class Job
    {
        public string name { get; set; }
        public string url { get; set; }
        public string color { get; set; }
    }

    public class View
    {
        public string name { get; set; }
        public string url { get; set; }
    }

}
