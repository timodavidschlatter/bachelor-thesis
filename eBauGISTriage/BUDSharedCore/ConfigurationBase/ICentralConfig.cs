
namespace BUDSharedCore.ConfigurationBase
{
    public interface ICentralConfig
    {
        string GetValue(string key, string defaultValue);
    }
}
