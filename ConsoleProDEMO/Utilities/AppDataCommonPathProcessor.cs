using System.Reflection;

namespace ConsoleProDEMO.Utilities
{
    public static class AppDataCommonPathProcessor
    {
        private const string CompanyName = "Skjoldrun";

        /// <summary>
        /// Gets a common path for settings and files for the application in the LocalAppData folder, combined with company name and application name.
        /// Creates the directories of the path unless they don't exist.
        /// </summary>
        /// <param name="applicationName">A subfolder with the given application string gets created if provided.</param>
        /// <returns>Path to appsettings folder</returns>
        public static string GetCompanyFolderPath(string applicationName = "")
        {
            string company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute), false)).Company;
            company = string.IsNullOrEmpty(company) ? CompanyName : company.Replace(' ', '_');

            string companyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), company, applicationName);
            Directory.CreateDirectory(companyPath);
            return companyPath;
        }

        /// <summary>
        /// Gets a common path for settings and files for the application in the C:\ProgramData folder, combined with company name and application name.
        /// Creates the directories of the path unless they don't exist.
        /// </summary>
        /// <param name="applicationName">A subfolder with the given application string gets created if provided.</param>
        /// <returns>Path to appsettings folder</returns>
        public static string GetProgramDataCompanyFolderPath(string applicationName = "")
        {
            string company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute), false)).Company;
            company = string.IsNullOrEmpty(company) ? CompanyName : company.Replace(' ', '_');

            string companyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), company, applicationName);
            Directory.CreateDirectory(companyPath);
            return companyPath;
        }

        /// <summary>
        /// Clears files from a directory where the file name matches a given string phrase.
        /// </summary>
        /// <param name="folderPath">path to be cleared</param>
        /// <param name="match">string to recognize files to be deleted, e.g. ".pdf"</param>
        /// <param name="caseSensitive">true for case sensitive matching, false converts to lower case before comparision</param>
        /// <param name="ageInDays">number of days to hold old files</param>
        public static void ClearFilesFromPath(string folderPath, string match, bool caseSensitive = true, int ageInDays = 5)
        {
            string[] files = Directory.GetFiles(folderPath);

            if (caseSensitive == true)
                match = match.ToLower();

            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);

                string fileName;
                if (caseSensitive == true)
                    fileName = fi.Name.ToLower();
                else
                    fileName = fi.Name;

                if (fi.LastWriteTime < DateTime.Now.AddDays(-ageInDays) && fileName.Contains(match))
                    fi.Delete();
            }
        }
    }
}