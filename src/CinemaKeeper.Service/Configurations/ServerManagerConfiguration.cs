using System;
using System.Collections.Generic;

namespace CinemaKeeper.Service.Configurations
{
    [Serializable]
    public class ServerManager{
        public List<Server> AvailableServers {set;get;} = new List<Server>();
    }

    [Serializable]
    public class Server{
        public string ?Name {set;get;}
        public int ID {set;get;}
        public string ?APIKeyServer {set;get;}

    }

}