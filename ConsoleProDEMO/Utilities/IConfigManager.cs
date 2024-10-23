namespace ConsoleProDEMO.Utilities
{
    public interface IConfigManager
    {
        bool ConvertEnumNames { get; set; }

        T DeserializeConfig<T>(string fileName);
    }
}