using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackLibrary
{
    [DataContract]
    public  class Client
    {
        [DataMember]
        public uint ClientID { get; set; }
        [DataMember]
        public uint TotalPoints { get; set; }
        [DataMember]
        public string ClientName { get; set; }

        public Client(uint clientID, uint totalPoints, string clientName)
        {
            ClientID = clientID;
            TotalPoints = totalPoints;
            ClientName = clientName;
        }
    }
}
