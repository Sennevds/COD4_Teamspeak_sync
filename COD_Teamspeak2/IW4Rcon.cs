using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace COD_Teamspeak
{
    public class IW4Rcon : RCON
    {
        public IW4Rcon(string IP, ushort Port, string rconPassword) : base(IP, Port, rconPassword) { }
        public IW4Rcon(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }

        public override RCON_Response rconQuery(string queryString)
        {
            var Response = new RCON_Response(queryString);

            // prevent accidental flooding
            double timeSinceLastQuery = (int)(DateTime.Now - lastQuery).TotalMilliseconds;
            if (timeSinceLastQuery < 140 && timeSinceLastQuery > 0)
                Thread.Sleep(140 - (int)timeSinceLastQuery);
            lastQuery = DateTime.Now;

            // set up our socket
            //var test = new TcpClient();
            //test.Client.SendTimeout = 2500;
            //test.Client.ReceiveTimeout = 2500;
            rconConnection = new UdpClient();
            rconConnection.Client.SendTimeout = 2500;
            rconConnection.Client.ReceiveTimeout = 5000;


            if (queryString == "getstatus")
                queryString = string.Format("ÿÿÿÿ {0}", queryString);
            else
                queryString = string.Format("ÿÿÿÿrcon {0} {1}", this.rconPassword, queryString);

            if (queryString.Length > 255)
            {
                Response.Error = RCON_Error.REQUEST_TOO_LONG;
                return Response;
            }

            if (IP == IPAddress.Loopback)
            {
                Response.Error = RCON_Error.INVALID_IP;
                return Response;
            }

            var queryBytes = getRequestBytes(queryString);
            var endPoint = new IPEndPoint(IP, Port);
            var startTime = DateTime.Now;

            try
            {
                rconConnection.Connect(endPoint);
            }
            catch (ObjectDisposedException)
            {
                Response.Error = RCON_Error.CONNECTION_TERMINATED;
                return Response;
            }
            catch (SocketException)
            {
                Response.Error = RCON_Error.REQUEST_TIMED_OUT;
                return Response;
            }

            rconConnection.Send(queryBytes, queryBytes.Length);
            Thread.Sleep(500);
            var incomingString = new StringBuilder();
            var bufferRecv = new byte[ushort.MaxValue];
        ignoreException:
            try
            {
                do
                {
                    bufferRecv = rconConnection.Receive(ref endPoint);
                    incomingString.Append(Encoding.ASCII.GetString(bufferRecv, 0, bufferRecv.Length));
                } while (rconConnection.Available > 0);
            }

            catch (SocketException E)
            {
                if (E.SocketErrorCode == SocketError.ConnectionReset)
                    Response.Error = RCON_Error.CONNECTION_TERMINATED;
                else if (E.SocketErrorCode == SocketError.TimedOut)
                    Response.Error = RCON_Error.REQUEST_TIMED_OUT;
                else
                    goto ignoreException;
                return Response;
            }

            var splitResponse = stripColors(incomingString.ToString()).Split(new char[] { (char)10 }, StringSplitOptions.RemoveEmptyEntries);

            if (splitResponse.Length == 4)
            {
                Response.Response.Description = splitResponse[2].Trim();
                var splitValue = splitResponse[1].Split('\"');
                if (splitValue.Length == 7)
                {
                    Response.Response.Value = splitValue[3];
                    Response.Response.defaultValue = splitValue[5];
                }
            }

            else if (splitResponse.Length == 3)
            {
                Response.Response.Value = splitResponse[1];
            }

            else if (splitResponse.Length == 2)
            {
                Response.Response.Value = splitResponse[1];
            }

            else
                Response.Response.Value = stripColors(incomingString.ToString()).Substring(10);
            Response.responseTime = (int)(DateTime.Now - startTime).TotalMilliseconds;
            if (Response.Response.Value.Contains("Invalid password."))
            {
                Response.Error = RCON_Error.INVALID_PASSWORD;
                return Response;
            }

            Response.Error = RCON_Error.REQUEST_COMPLETE;
            Response.Success = true;

            rconConnection.Close();
            rconConnection = null;
            return Response;
        }

        public Dictionary<string, string> GetTeamspeakTeams()
        {
            var usersTeams = new Dictionary<string, string>();
            var resp = rconQuery("dvarlist");
            if (!resp.Success)
                throw new Exception("error");
            else
            {
                var initialSplit = resp.Response.Value.Split(new char[] { (char)10 }, StringSplitOptions.RemoveEmptyEntries);

                var dvars = initialSplit.Select(x => x.Trim().Replace("\"", "")).ToList();
                var teamspeak = dvars.FirstOrDefault(x => x.Contains("teamspeak"));
                if (string.IsNullOrEmpty(teamspeak)) return null;
                var teamspeakDvar = teamspeak.Trim().Split(new[] { ' ', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var teamspeakEnabled = false;
                if (teamspeakDvar.Length > 2)
                    teamspeakEnabled = teamspeakDvar[2] == "1";
                else if (teamspeakDvar.Length == 2)
                    teamspeakEnabled = teamspeakDvar[1] == "1";

                if (!teamspeakEnabled) return null;
                {
                    var users = dvars.Where(x => x.Contains("ts_team")).Select(x =>
                        x.Trim().Split(new[] { ' ', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var user in users)
                    {
                        usersTeams.Add(user[0].Replace("ts_team_", ""), user[1]);
                    }
                }
            }

            return usersTeams;
        }
        public override Dictionary<string, string> getStatus()
        {
            return rconQueryDict("getstatus");
        }

        public override Dictionary<string, string> serverInfo()
        {
            return rconQueryDict("serverinfo");
        }

        private static string stripColors(string str)
        {
            if (str == null)
                return "";
            return Regex.Replace(str, @"\^[0-9]", "");
        }
    }
}
