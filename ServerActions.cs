using System;

namespace client
{
    public enum ServerActions
    {
        Ping,
        GetClients,
        
        Election,

        VoteForLeader,

        Vote,

        KeepFollower,

        GetLeader,

        SendToLeader,

        GetFromLeader,

        SendDataToClient,

        GetDataFromLeader

    }
}
