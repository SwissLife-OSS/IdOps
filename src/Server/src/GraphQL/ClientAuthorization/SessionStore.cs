using System.Collections.Generic;
using IdOps.Store;

namespace IdOps;

public class SessionStore : ISessionStore
{
    private Dictionary<string, Session> _sessions = new Dictionary<string, Session>();

    public Session GetSession(string id)
    {
        return _sessions[id];
    }

    public void SaveSession(string id, Session session)
    {
        _sessions.Add(id, session);
    }
}
