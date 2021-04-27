namespace WorkTracker.Contracts
{
    public interface IEncryptionHelper
    {
        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Encrypt(string data);
    }
}