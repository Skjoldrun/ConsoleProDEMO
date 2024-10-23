namespace ConsoleProDEMO.Utilities
{
    public class FolderComparer
    {
        private readonly string _path1 = string.Empty;
        private readonly string _path2 = string.Empty;
        private DirectoryInfo _dirInfo1;
        private DirectoryInfo _dirInfo2;
        private IEnumerable<FileInfo> _fileInfos1;
        private IEnumerable<FileInfo> _fileInfos2;
        private FileComparer _fileComparer;

        public FolderComparer(string path1, string path2, FileComparer fileComparer)
        {
            _fileComparer = fileComparer;

            _path1 = path1;
            _path2 = path2;

            _dirInfo1 = new DirectoryInfo(_path1);
            _dirInfo2 = new DirectoryInfo(_path2);

            _fileInfos1 = _dirInfo1.GetFiles("*.*", SearchOption.AllDirectories);
            _fileInfos2 = _dirInfo2.GetFiles("*.*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Compares the two given lists of IEnumerable<FileInfo> for equality.
        /// </summary>
        /// <returns>true if equal, else false</returns>
        public bool AreFoldersEqual()
        {
            return _fileInfos1.SequenceEqual(_fileInfos2, _fileComparer);
        }

        /// <summary>
        /// Gets all filepaths as List of strings for common files in both paths.
        /// </summary>
        /// <returns>List of common files</returns>
        public List<string> GetEqualFiles()
        {
            var result = new List<string>();
            var queryCommonFiles = _fileInfos1.Intersect(_fileInfos2, _fileComparer);

            if (queryCommonFiles.Any())
            {
                foreach (var file in queryCommonFiles)
                {
                    result.Add(file.FullName);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the files which are exclusive in the given sourceList compared to the targetList.
        /// </summary>
        /// <param name="sourceList">List to search for exclusive files</param>
        /// <param name="targetlist">List to compare</param>
        /// <returns>List of exclusive files in sourceList</returns>
        public List<string> GetFilesExclusiveFirstParamList(IEnumerable<FileInfo> sourceList, IEnumerable<FileInfo> targetlist)
        {
            var result = new List<string>();
            var querySourceOnly = (from file in sourceList
                                   select file).Except(targetlist, _fileComparer);

            foreach (var file in querySourceOnly)
            {
                result.Add(file.FullName);
            }

            return result;
        }

        /// <summary>
        /// Gets the files which are exclusive in the constructor given sourceList compared to the targetList.
        /// </summary>
        /// <returns>List of exclusive files in sourceList</returns>
        public List<string> GetFilesExclusiveSourceList()
        {
            return GetFilesExclusiveFirstParamList(_fileInfos1, _fileInfos2);
        }
    }
}