namespace QDC_BLL.Interfaces
{
    public interface IRecaptchaService
    {
        Task<bool> VerifyAsync(string? token);
    }
}
