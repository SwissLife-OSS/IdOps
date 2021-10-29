namespace IdOps.IdentityServer.Hashing
{
    public interface IPasswordProvider
    {
        /// <summary>
        /// Generates a Random password
        /// </summary>
        /// <param name="length">Number of chars</param>
        /// <param name="includeSpecialChars">Include spaecial chars (@-.'$!?*)</param>
        /// <returns>the generated password</returns>
        string GenerateRandomPassword(int length, bool includeSpecialChars = false);
    }
}
