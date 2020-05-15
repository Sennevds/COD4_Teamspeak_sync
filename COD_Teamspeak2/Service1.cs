using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using System.Configuration;
using System.IO;
using COD_Teamspeak.Classes;
using COD_Teamspeak.Helpers;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Notifications;
using TeamSpeak3QueryApi.Net.Specialized.Responses;

//using TSLib;
//using TSLib.Full;
//using TSLib.Helper;
//using TSLib.Messages;
//using TSLib.Query;
//using ClientType = TSLib.ClientType;


namespace COD_Teamspeak
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };
    public partial class Service1 : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private readonly Timer _timer = new Timer();
        //private readonly IW4Rcon _rcon;
        private readonly List<UserTeam> _followingUsers;
        private readonly EventLog _eventLog1;
        //private static TsQueryClient _tsQueryClient;
        private List<GetChannelListInfo> _channelList;
        private FileMonitor _serverStatusMonitor;
        private FileMonitor _configMonitor;
        private TeamSpeakClient _teamSpeakClient;
        private List<GetClientInfo> _cachedClients;

        public Service1()
        {
            InitializeComponent();
            try
            {
                _eventLog1 = new EventLog();
                if (!EventLog.SourceExists("COD4TEAMSPEAK"))
                {
                    EventLog.CreateEventSource(
                        "COD4TEAMSPEAK", "Application");
                }

                _eventLog1.Source = "COD4TEAMSPEAK";
                _eventLog1.Log = "Application";
                //_rcon = new IW4Rcon(ConfigurationManager.AppSettings["COD_IP"], ushort.Parse(ConfigurationManager.AppSettings["COD_PORT"]), ConfigurationManager.AppSettings["COD_PASSWORD"]);
                _eventLog1.WriteEntry("Constructor", EventLogEntryType.Information);
                _followingUsers = new List<UserTeam>();
            }
            catch (Exception e)
            {
                _eventLog1?.WriteEntry($"Failed: {e.Message}", EventLogEntryType.Error);
            }

        }

        private static void ConfigChanged(object sender, FileSystemEventArgs e)
        {
            IEnumerable<string> sections = ConfigurationManager.AppSettings["monitredSections"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!sections.Any()) return;
            foreach (var section in sections)
            {
                ConfigurationManager.RefreshSection(section);
            }
        }

        private async Task<List<GetChannelListInfo>> GetChannelList()
        {
            try
            {
                var resp = await _teamSpeakClient.GetChannels();
                return resp.ToList();
            }
            catch (Exception e)
            {
                _eventLog1?.WriteEntry($"Failed to get ChannelList: {e.Message}", EventLogEntryType.Error);
                return null;
            }

        }
        protected override void OnStart(string[] args)
        {
            try
            {
                _eventLog1.WriteEntry("Startings service");
                // Update the service state to Start Pending.
                var serviceStatus = new ServiceStatus
                {
                    dwCurrentState = ServiceState.SERVICE_START_PENDING,
                    dwWaitHint = 100000
                };
                SetServiceStatus(ServiceHandle, ref serviceStatus);
                _configMonitor = new FileMonitor(string.Concat(System.Reflection.Assembly.GetEntryAssembly()?.Location, ".config")) { FileChanged = ConfigChanged };
                _timer.Elapsed += OnElapsedTime;
                _timer.Interval = 120000; //number in milisecinds  
                _timer.Enabled = true;
                _timer.Start();
                _teamSpeakClient = ConnectToTeamspeak().Result;
                if(_teamSpeakClient == null)
                {
                    serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                    SetServiceStatus(ServiceHandle, ref serviceStatus);
                    Stop();
                    return;
                }

                //Debugger.Launch();
                _teamSpeakClient.Subscribe<ClientEnterView>(NewClientEntered);
                _channelList = GetChannelList().Result;
                // Update the service state to Running.
                _serverStatusMonitor = new FileMonitor(ConfigurationManager.AppSettings["ServerStatusPath"]) { FileChanged = ServerStatusChanged };
                ServerStatusChanged(null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(ConfigurationManager.AppSettings["ServerStatusPath"]), Path.GetFileName(ConfigurationManager.AppSettings["ServerStatusPath"])));
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(ServiceHandle, ref serviceStatus);
                _eventLog1.WriteEntry("Started service");
            }
            catch (Exception e)
            {
                _eventLog1?.WriteEntry($"Failed to start: {e.Message}", EventLogEntryType.Error);
            }
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            _teamSpeakClient.WhoAmI();
        }

        private void ServerStatusChanged(object sender, FileSystemEventArgs e)
        {
            if (ConfigurationManager.AppSettings["Debug"] == "True")
                _eventLog1.WriteEntry($"File: {e.Name} updated");
            var b3Status = Serializer.Deserialize<B3Status>(e.FullPath);
            var teamSpeakStatus = b3Status.Game.Data.FirstOrDefault(x => x.Name == "sv_syncTeamSpeak");
            if (teamSpeakStatus != null && teamSpeakStatus.Value == "0")
            {
                if (ConfigurationManager.AppSettings["Debug"] == "True")
                    _eventLog1.WriteEntry($"Sync service turned off");
                return;
            }
            if (ConfigurationManager.AppSettings["Debug"] == "True")
                _eventLog1.WriteEntry($"Sync service turned on");
            var currentlyConnectedClients = b3Status.Clients.Client;
            foreach (var client in currentlyConnectedClients)
            {
                if (_followingUsers.FirstOrDefault(x => x.User == client.ColorName) == null)
                {
                    var newUser = new UserTeam
                    {
                        User = client.ColorName,
                        Team = client.TeamNameClean
                    };
                    if (ConfigurationManager.AppSettings["Debug"] == "True")
                        _eventLog1.WriteEntry($"New user joined. Username: {client.ColorName} team: {client.TeamNameClean}");
                    newUser.TeamChanged += UserOnTeamChanged;
                    _followingUsers.Add(newUser);
                    UserOnTeamChanged(newUser, new CancelEventArgs(), newUser.Team);
                }
                else
                {
                    var changedUser = _followingUsers.FirstOrDefault(x => x.User == client.ColorName);
                    if (changedUser == null) return;//kan niet
                    changedUser.Team = client.TeamNameClean;
                }
            }
        }

        private void UserOnTeamChanged(UserTeam sender, CancelEventArgs e, string newteam)
        {
            if (ConfigurationManager.AppSettings["Debug"] == "True")
                _eventLog1.WriteEntry($"User: {sender.User} joined team: {newteam} and tsUserName: {sender.TsUser}");

            ChangeUserLobby(sender);
        }

        private async Task<TeamSpeakClient> ConnectToTeamspeak()
        {
            try
            {
                var rc = new TeamSpeakClient(ConfigurationManager.AppSettings["TS_IP"]); // Create rich client instance
                await rc.Connect(); // connect to the server

                await rc.Login(ConfigurationManager.AppSettings["TS_USER"], ConfigurationManager.AppSettings["TS_PASSWORD"]); // login to do some stuff that requires permission
                await rc.UseServer(1); // Use the server with id '1'
                await rc.RegisterServerNotification();
                //rc.Subscribe<ClientEnterView>(Test);
                return rc;
            }
            catch (Exception ex)
            {
                _eventLog1?.WriteEntry($"Failed to connect to ts3: {ex.Message}", EventLogEntryType.Error);
                return null;
            }
        }

        private void Test<T>(IReadOnlyCollection<T> obj) where T : Notification
        {
        }

        private void NewClientEntered(IReadOnlyCollection<ClientEnterView> data)
        {
            foreach(var clientEnterView in data.Where(x => x.Type == ClientType.FullClient))
            {
                var user = _followingUsers?.FirstOrDefault(x => x.TsUser == clientEnterView.NickName);
                if (user == null) continue;
                ChangeUserLobby(user, clientEnterView);
            }
        }

        private async Task<List<GetClientInfo>> GetClients()
        {
            try
            {
                var clients = await _teamSpeakClient.GetClients(GetClientOptions.Uid).TimeoutAfter(TimeSpan.FromSeconds(10));
                _cachedClients = clients.ToList();
                return clients.ToList();
            }
            catch(Exception e)
            {
                _eventLog1.WriteEntry($"Failed to retrieve CurrentClients using cached version message: {e.Message}", EventLogEntryType.Error);
                return _cachedClients;
            }
            
        }
        private void ChangeUserLobby(UserTeam userTeam, ClientEnterView clientEnterView = null)
        {
            try
            {
                if (_teamSpeakClient == null || !_teamSpeakClient.Client.IsConnected)
                {
                    _teamSpeakClient = ConnectToTeamspeak().Result;
                }

                if (_channelList == null || _channelList.Count == 0)
                    _channelList = GetChannelList().Result;
                var currentClients = GetClients().Result;
                if (currentClients == null)
                {
                    if (ConfigurationManager.AppSettings["Debug"] == "True")
                        _eventLog1.WriteEntry($"Failed to retrieve CurrentClients");
                    return;
                }

                TsUser tsUser;
                if(clientEnterView != null)
                {
                    tsUser = new TsUser
                    {
                        ChannelId = clientEnterView.TargetChannelId,
                        UserId = clientEnterView.Id,
                        UserName = clientEnterView.NickName
                    };
                }
                else
                {
                    var user = currentClients.FirstOrDefault(x =>
                        string.Equals(x.NickName, userTeam.TsUser, StringComparison.CurrentCultureIgnoreCase));
                    if(user == null)
                    {
                        if(ConfigurationManager.AppSettings["Debug"] == "True")
                            _eventLog1.WriteEntry($"User: {userTeam.TsUser} not found in ts3");
                        return;
                    }

                    tsUser = new TsUser
                    {
                        ChannelId = user.ChannelId,
                        UserId = user.Id,
                        UserName = user.NickName
                    };
                }

                if (ConfigurationManager.AppSettings["Debug"] == "True")
                    _eventLog1.WriteEntry($"User: {userTeam.TsUser} found in ts3");
                var channel = _channelList.FirstOrDefault(x => x.Name == userTeam.TsChannel);
                if (channel == null)
                {
                    if (ConfigurationManager.AppSettings["Debug"] == "True")
                        _eventLog1.WriteEntry($"Channel: {userTeam.TsChannel} not found in ts3");
                    return;
                }
                if (ConfigurationManager.AppSettings["Debug"] == "True")
                    _eventLog1.WriteEntry($"Channel: {userTeam.TsChannel} found in ts3 for user {userTeam.TsUser}");
                if (channel.Id == tsUser.ChannelId)
                {
                    if (ConfigurationManager.AppSettings["Debug"] == "True")
                        _eventLog1.WriteEntry($"User: {userTeam.TsUser} is already in channel: {channel.Name}");
                    return;
                }
                if (ConfigurationManager.AppSettings["Debug"] == "True")
                    _eventLog1.WriteEntry($"Change user: {tsUser.UserName} to channel {channel.Name}");
                _teamSpeakClient.MoveClient(tsUser.UserId, channel.Id);
            }
            catch (Exception e)
            {
                _eventLog1.WriteEntry(e.Message, EventLogEntryType.Error);
            }

        }
        protected override void OnStop()
        {
            if (_serverStatusMonitor != null)
                _serverStatusMonitor.FileChanged -= ServerStatusChanged;
            if (_configMonitor != null)
                _configMonitor.FileChanged -= ConfigChanged;

        }
    }

    public class TsUser
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int ChannelId { get; set; }
    }
}
