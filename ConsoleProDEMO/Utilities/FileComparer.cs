namespace ConsoleProDEMO.Utilities
{
    public class FileComparer : IEqualityComparer<FileInfo>
    {
        /// <summary>
        /// Compares two files by name, length and lastWriteTime.
        /// </summary>
        /// <param name="f1">first file to compare</param>
        /// <param name="f2">second file to compare</param>
        /// <returns>true if equal, else false</returns>
        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length &&
                    f1.LastWriteTime == f2.LastWriteTime);
        }

        /// <summary>
        /// Return a hash that reflects the comparison criteria. According to the rules for IEqualityComparer<T>,
        /// if Equals is true, then the hash codes must also be equal. Because equality as defined here is a simple value equality,
        /// not reference identity, it is possible that two or more objects will produce the same hash code.
        /// </summary>
        /// <param name="fi">file for the hashCode</param>
        /// <returns>generated hashcode</returns>
        public int GetHashCode(FileInfo fi)
        {
            string s = $"{fi.Name}{fi.Length}{fi.LastWriteTime}";
            return s.GetHashCode();
        }
    }
}