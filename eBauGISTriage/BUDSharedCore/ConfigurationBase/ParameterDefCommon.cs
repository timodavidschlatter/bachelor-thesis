
namespace BUDSharedCore.ConfigurationBase
{
    public class ParameterDefCommon : ParameterDefBase
    {
        public static ParameterType<string> DBVersionDef = new ParameterType<string>("DBVersion", "1.0", "Die vorliegende Datenbankversion");
        public static ParameterType<string> DefaultLanguageDef = new ParameterType<string>("DefaultLanguage", "de-CH", "Die Standardsprache des Systems");

        public readonly Parameter<string> DBVersion;
        public readonly Parameter<string> DefaultLanguage;

        public ParameterDefCommon(ICentralConfig config): base(config)
        {
            DBVersion = new Parameter<string>(DBVersionDef, _config);
            DefaultLanguage = new Parameter<string>(DefaultLanguageDef, _config);
        }
    }
}
