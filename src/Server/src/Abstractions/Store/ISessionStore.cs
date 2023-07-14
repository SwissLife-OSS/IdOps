namespace IdOps.Store;

public interface ISessionStore
{
    Session GetSession(string key);

    void SaveSession(string key, Session session);
}
