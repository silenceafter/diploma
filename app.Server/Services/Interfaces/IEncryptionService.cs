namespace app.Server.Services.Interfaces
{
    public interface IEncryptionService
    {
        string ComputeHash(string value);
        string Encrypt(string value);
        string Decrypt(string value);
    }
}
