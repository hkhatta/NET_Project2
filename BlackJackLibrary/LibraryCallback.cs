using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackLibrary
{
    //Class Library Callback
    //Defines the properties that are going to be updated
    //when a callback is requested
    [DataContract]
    public class LibraryCallback
    {
        //[DataMember]
        //public uint NumCards { get; private set; }
        //[DataMember]
        //public uint NumDecks { get; private set; }
        //[DataMember]
        //public bool IsEmptyHand { get; private set; }
     
        [DataMember]
        public HashSet<Client> Clients { get; private set; }

        public LibraryCallback(HashSet<Client> clients)
        {            
            Clients = clients;
        }

    }
}
