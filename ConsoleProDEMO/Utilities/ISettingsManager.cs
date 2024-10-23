namespace ConsoleProDEMO.Utilities
{
    public interface ISettingsManager
    {
        bool ConvertEnumNames { get; set; }

        T ReadFromFile<T>(string path = null);
        void SaveToFile<T>(T settings, string path = null);
    }
}