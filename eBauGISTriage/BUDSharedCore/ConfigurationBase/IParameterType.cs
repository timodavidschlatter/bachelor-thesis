
namespace BUDSharedCore.ConfigurationBase
{
    public interface IParameterType
    {
        string Key { get; }

        object DefaultValue { get; }

        string Description { get; }

        object[] ValidValueItems { get; }
    }
}
