using CommandLineParsing;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Alarm
{
    public partial class TimerForm
    {
        private class MyCommand : Command
        {
            private TimerForm form;

            public MyCommand(TimerForm form)
            {
                this.form = form;

                SubCommands.Add("quit", () =>
                {
                    form.canclose = true;
                    form.Close();
                    Application.Exit();
                });
                SubCommands.Add("work", new timeCommand(form, true));
                SubCommands.Add("break", new timeCommand(form, false));
            }

            private class timeCommand : Command
            {
                private TimerForm form;
                private bool work;
                private Regex regex;

                [NoName]
                private readonly Parameter<string[]> time;

                public timeCommand(TimerForm form, bool work)
                {
                    this.form = form;
                    this.work = work;
                    string h = "(?<hour>[0-9]{1,2})", m = "(?<minute>[0-9]{1,2})", s = "(?<second>[0-9]{1,2})";
                    this.regex = new Regex(string.Format("({0}:)?{1}:{2}|{1}", h, m, s));

                    time.Validate(x => x.Length == 1, "Exactly one time only please");
                    time.ValidateEach(x => regex.IsMatch(x), "Invalid time format");
                }

                protected override void Execute()
                {
                    var match = regex.Match(time.Value[0]);

                    var h = match.Groups["hour"].Value; if (h.Length == 0) h = "0";
                    var m = match.Groups["minute"].Value; if (m.Length == 0) m = "0";
                    var s = match.Groups["second"].Value; if (s.Length == 0) s = "0";

                    TimeSpan ts = new TimeSpan(int.Parse(h), int.Parse(m), int.Parse(s));
                    form.icon.Start(ts, work);
                    form.Visible = false;
                }
            }
        }
    }
}
