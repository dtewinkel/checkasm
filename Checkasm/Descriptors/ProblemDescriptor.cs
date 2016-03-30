using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckAsm.Descriptors
{
    [Serializable]
    public class ProblemDescriptor
    {
        public virtual ProblemLevelBase Level { get; set; }
        public virtual string Title { get; set; }
        public virtual string Detail { get; set; }
        public virtual AsmData Source { get; set; }
        public Guid Id { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1} in {2}: {3}",Level.Name, Title, Source.Name, Detail);
        }

        public ProblemDescriptor()
        {

        }

        public ListViewItem GetListViewItem()
        {
            var item = new ListViewItem(Level.Name, Level.Name.ToLower() +".png");
            var title = new ListViewItem.ListViewSubItem(item, Title);
            item.SubItems.Add(title);
            var detail = new ListViewItem.ListViewSubItem(item, Detail);
            item.SubItems.Add(detail);
            item.Tag = this;
            return item;
        }
    }

    [Serializable]
    public sealed class WarningProblemDescriptor : ProblemDescriptor
    {
        public WarningProblemDescriptor(string id, AsmData source, string title, string detail)
        {
            Source = source;
            Level = ProblemLevels.Warning;
            Id = new Guid(id);
            Title = title;
            Detail = detail;
            Trace.WriteLine(string.Format("Created {0}, id{3}, for {1}: {2}\r\n{4}",this.GetType().Name, source, title, id, detail));
        }
    }

    [Serializable]
    public class ExceptionProblemDescriptor : ProblemDescriptor
    {
        public Exception Exception { get; set; }

        public ExceptionProblemDescriptor(string id, AsmData source, Exception ex)
        {
            Exception = ex;
            Source = source;
            Level = ProblemLevels.Error;
            Id = new Guid(id);
            Title = "Exception thrown";
            Trace.WriteLine(string.Format("Created {0}, id{1}, for \"{2}\": {3}", this.GetType().Name, id, source, ex));
        }

       
    }

    [Serializable]
    public sealed class NotificationProblemDescriptor:ProblemDescriptor
    {

        public NotificationProblemDescriptor(string id, AsmData source, string title, string detail)
        {
            Source = source;
            Level = ProblemLevels.Info;
            Id = new Guid(id);
            Title = title;
            Detail = detail;
            Trace.WriteLine(string.Format("Created {0}, id{3}, for {1}: {2}\r\n{4}", this.GetType().Name, source, title, id, detail));
        }
    }

    [Serializable]
    public sealed class AssemblyNotFoundProblemDescriptor : ProblemDescriptor
    {
        public string ExpectedPath { get; set; }
        
        public override string Detail
        {
            get
            {
                return "Assembly could not be found in path " + ExpectedPath;
            }
        }


        public AssemblyNotFoundProblemDescriptor(string id, AsmData source, string expectedPath)
        {
            Title = "Assembly not found";
            Source = source;
            ExpectedPath = expectedPath;
            Level = ProblemLevels.Error;
            Id = new Guid(id);
            Trace.WriteLine(string.Format("Created {0}, id{1}, for {2}: {3}", this.GetType().Name, id, source, expectedPath));
        }
    }

    [Serializable]
    public sealed class ConfigFileFormatProblemDescriptor : ExceptionProblemDescriptor
    {
        public string ExpectedPath { get; set; }

        private string detail;

        public override string Detail
        {
            get
            {
                if (string.IsNullOrEmpty(detail))
                    return String.Format("Assembly configuration file {0} could not be read.", ExpectedPath);
                else
                    return detail;
            }
            set
            {
                detail = value;
            }
        }

        public ConfigFileFormatProblemDescriptor(string id, AsmData source, string expectedPath, Exception innerException):base(id,source,innerException)
        {
            Title = "Assembly Configuration file issue";
            Source = source;
            ExpectedPath = expectedPath;
            Level = ProblemLevels.Error;
            Id = new Guid(id);
            Trace.WriteLine(string.Format("Created {0}, id{1}, for {2}: {3}", this.GetType().Name, id, source, expectedPath));
        }
    }
}
