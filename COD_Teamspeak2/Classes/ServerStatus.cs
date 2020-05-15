using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace COD_Teamspeak.Classes
{
	[XmlRoot(ElementName = "Data")]
	public class Data
	{
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "Game")]
	public class Game
	{
		[XmlElement(ElementName = "Data")]
		public List<Data> Data { get; set; }
		[XmlAttribute(AttributeName = "CapatureLimit")]
		public string CapatureLimit { get; set; }
		[XmlAttribute(AttributeName = "FragLimit")]
		public string FragLimit { get; set; }
		[XmlAttribute(AttributeName = "Map")]
		public string Map { get; set; }
		[XmlAttribute(AttributeName = "MapTime")]
		public string MapTime { get; set; }
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "RoundTime")]
		public string RoundTime { get; set; }
		[XmlAttribute(AttributeName = "Rounds")]
		public string Rounds { get; set; }
		[XmlAttribute(AttributeName = "TimeLimit")]
		public string TimeLimit { get; set; }
		[XmlAttribute(AttributeName = "Type")]
		public string Type { get; set; }
	}

	[XmlRoot(ElementName = "Client")]
	public class Client
	{
		[XmlAttribute(AttributeName = "CID")]
		public string CID { get; set; }
		[XmlAttribute(AttributeName = "ColorName")]
		public string ColorName { get; set; }
		[XmlAttribute(AttributeName = "DBID")]
		public string DBID { get; set; }
		[XmlAttribute(AttributeName = "IP")]
		public string IP { get; set; }
		[XmlAttribute(AttributeName = "PBID")]
		public string PBID { get; set; }
		[XmlAttribute(AttributeName = "Score")]
		public string Score { get; set; }
		[XmlAttribute(AttributeName = "Kills")]
		public string Kills { get; set; }
		[XmlAttribute(AttributeName = "Deaths")]
		public string Deaths { get; set; }
		[XmlAttribute(AttributeName = "Assists")]
		public string Assists { get; set; }
		[XmlAttribute(AttributeName = "Ping")]
		public string Ping { get; set; }
		[XmlAttribute(AttributeName = "Team")]
		public string Team { get; set; }
		[XmlAttribute(AttributeName = "TeamName")]
		public string TeamName { get; set; }
		[XmlAttribute(AttributeName = "Updated")]
		public string Updated { get; set; }
		[XmlAttribute(AttributeName = "power")]
		public string Power { get; set; }
		[XmlAttribute(AttributeName = "rank")]
		public string Rank { get; set; }

        public string TeamNameClean
        {
            get
            {
                var team = Team;
                switch(team)
                {
					case "1":
                        return "Axis";
					case "2":
                        return "Allies";
					default:
                        return "Lobby";
                }
            }
        }
	}

	[XmlRoot(ElementName = "Clients")]
	public class Clients
	{
		[XmlElement(ElementName = "Client")]
		public List<Client> Client { get; set; }
		[XmlAttribute(AttributeName = "Total")]
		public string Total { get; set; }
	}

	[XmlRoot(ElementName = "B3Status")]
	public class B3Status
	{
		[XmlElement(ElementName = "Game")]
		public Game Game { get; set; }
		[XmlElement(ElementName = "Clients")]
		public Clients Clients { get; set; }
		[XmlAttribute(AttributeName = "Time")]
		public string Time { get; set; }
		[XmlAttribute(AttributeName = "TimeStamp")]
		public string TimeStamp { get; set; }
	}

}
