using System.ComponentModel;
using System.Configuration;

namespace COD_Teamspeak.Classes
{
    public class UserTeam
    {
        public delegate void TeamChangedEventHandler(UserTeam sender, CancelEventArgs e, string newTeam);
        private string _team;
        public string User { get; set; }

        public string TsUser
        {
            get
            {
                var alias = ConfigurationManager.AppSettings[User];
                return string.IsNullOrEmpty(alias) ? User : alias;
            }
        }

        public string TsChannel
        {
            get
            {
                switch (Team)
                {
                    case "Allies":
                        return ConfigurationManager.AppSettings["Allies"];
                    case "Axis":
                        return ConfigurationManager.AppSettings["Axis"];
                    default:
                        return ConfigurationManager.AppSettings["Default_Channel"];
                }
            }
        }

        private string _oldTeam;
        public string Team
        {
            get => _team;
            set
            {
                _team = value;
                if (_oldTeam != _team)
                {
                    if (TeamChanged != null)
                    {
                        var e = new CancelEventArgs();
                        TeamChanged(this, e, _team);
                    }
                }

                _oldTeam = value;
            }
        }

        public event TeamChangedEventHandler TeamChanged;
    }
}