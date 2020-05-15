using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace COD_Teamspeak
{
    public enum RCON_Error
    {
        INVALID_IP,
        INVALID_PORT,
        INVALID_PASSWORD,
        INVALID_RESPONSE,
        REQUEST_TIMED_OUT,
        REQUEST_TOO_LONG,
        REQUEST_NOT_SENT,
        REQUEST_COMPLETE,
        RESPONSE_INCOMPLETE,
        CONNECTION_TERMINATED,
    }

    public struct RCON_responseInfo
    {
        public string valueName;
        public string Value;
        public string defaultValue;
        public string Description;

        public RCON_responseInfo(string valueName)
        {
            this.valueName = valueName;
            Value = "";
            defaultValue = "";
            Description = "";
        }
    }

    public struct RCON_Response
    {
        public RCON_responseInfo Response;
        public int responseTime;
        public bool Success;
        public RCON_Error Error;

        public RCON_Response(string Request)
        {
            this.responseTime = -1;
            this.Response = new RCON_responseInfo(Request);
            this.Success = false;
            Error = RCON_Error.REQUEST_NOT_SENT;
        }

        public override string ToString()
        {
            var stringInfo = new StringBuilder();
            stringInfo.Append("==RCON Response==\r\n");
            stringInfo.AppendFormat("Response: {0}\r\nTime: {1}\r\nSucceeded: {2}\r\nError: {3}\r\n==End RCON Response==", this.Response, this.responseTime, this.Success, this.Error);
            return stringInfo.ToString();
        }
    }

    [Serializable]
    public abstract class RCON : ISerializable
    {
        protected IPAddress IP;
        protected ushort Port;
        protected string rconPassword;
        protected UdpClient rconConnection;
        protected DateTime lastQuery;

        public RCON(string IP, ushort Port, string rconPassword)
        {
            this.Port = Port;
            this.rconPassword = rconPassword;
            try
            {
                this.IP = Dns.GetHostAddresses(IP)[0];
            }
            catch (Exception)
            {
                if (!IPAddress.TryParse(IP, out this.IP))
                    this.IP = IPAddress.Loopback;
            }
        }

        public RCON(SerializationInfo info, StreamingContext ctxt)
        {
            IP = (IPAddress)info.GetValue("serverIP", typeof(IPAddress));
            Port = (ushort)info.GetValue("serverPort", typeof(ushort));
            rconPassword = (string)info.GetValue("serverPassword", typeof(string));
        }

        public abstract RCON_Response rconQuery(string queryString);
        public abstract Dictionary<string, string> getStatus();
        public abstract Dictionary<string, string> serverInfo();

        protected Dictionary<string, string> rconQueryDict(string queryString)
        {
            var serverInfo = new Dictionary<string, string>();
            var Resp = rconQuery(queryString);

            if (!Resp.Success)
                throw new Exception("error");
            else
            {
                if (queryString.Contains("dumpuser"))
                {
                    var initialSplit = Resp.Response.Value.Split(new char[] { (char)10 }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var S in initialSplit)
                    {
                        var clientInfo = S.Split(new[] { ' ', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (clientInfo.Length > 1)
                        {
                            var cName = new StringBuilder();
                            for (var i = 1; i < clientInfo.Length; i++)
                                if (clientInfo[i].Length > 0)
                                    cName.Append(clientInfo[i]);

                            serverInfo.Add(clientInfo[0], cName.ToString());
                        }

                    }
                }
                else
                {
                    var initialSplit = Resp.Response.Value.Split(new char[] { (char)10 }, StringSplitOptions.RemoveEmptyEntries);
                    string[] Responses;
                    if (initialSplit.Length == 0)
                        return serverInfo;
                    else if (initialSplit.Length == 1)
                        Responses = initialSplit[0].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    else
                        Responses = initialSplit[1].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    if (Responses.Length == 1) return null;
                    for (var i = 0; i < Responses.Length; i += 2)
                    {
                        serverInfo.Add(Responses[i], Responses[i + 1]);
                    }

                    if (queryString == "getstatus")
                    {
                        if (initialSplit.Length > 1)
                        {
                            var player = new string[initialSplit.Length - 3];

                            for (var i = 2; i < initialSplit.Length - 1; i++)
                            {
                                player[i - 2] = initialSplit[i];
                            }

                            serverInfo.Add("Players", string.Join(",", player));
                        }

                        else
                            serverInfo.Add("Players", "");

                    }
                }
            }
            return serverInfo;
        }

        public Dictionary<string, string> dumpUser(string Name)
        {
            return rconQueryDict("dumpuser \"" + Name + "\"");
        }

        protected byte[] getRequestBytes(string Request)
        {
            var initialRequestBytes = Encoding.Unicode.GetBytes(Request);
            var fixedRequest = new byte[initialRequestBytes.Length / 2];

            for (var i = 0; i < initialRequestBytes.Length; i++)
                if (initialRequestBytes[i] != 0)
                    fixedRequest[i / 2] = initialRequestBytes[i];

            return fixedRequest;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("serverIP", IP);
            info.AddValue("serverPort", Port);
            info.AddValue("serverPassword", rconPassword);
        }

        public override string ToString()
        {
            return IP.ToString() + ":" + Port;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().IsAssignableFrom(this.GetType()))
            {
                var ob = (RCON)obj;
                return ob.IP == this.IP && ob.Port == this.Port;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.IP.GetHashCode() + this.Port.GetHashCode();
        }
    }
}
