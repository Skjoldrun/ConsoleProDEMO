using ConsoleProDEMO.Utilities;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ConsoleProDEMO.Mailing
{
    public abstract class AMailerBase
    {
        public const string MailerConfigName = "MailerConfig.json";
        public const string LastMailSentjsonFileName = "lastMailSent.json";

        protected ConcurrentDictionary<int, DateTime> _lastMailSentDict = new ConcurrentDictionary<int, DateTime>();
        protected string _applicationName;

        /// <summary>
        /// Checks if a mail with given subject was sent within the last minutes as subjectAge.
        /// Reads and writes checked mails in _lastMailSentDict to a local folder if spamPreventonStored is true.
        /// </summary>
        /// <param name="subject">subject text of the mail</param>
        /// <param name="subjectAge">integer in minutes to prevent spam</param>
        /// <param name="spamPreventionStored">bool if dictionary for recognizing spam shoul be stored</param>
        /// <returns>true, if incoming subject is younger than subjectAge, else false</returns>
        public bool CheckIfSpam(string subject, int subjectAge = 30, bool spamPreventionStored = false)
        {
            bool isSpam = false;

            // if it is lower or equal to 0, spam prevention is disabled and everything gets sent.
            if (subjectAge <= 0)
                return isSpam;

            if (spamPreventionStored)
                ReadStoredLastMails();

            var subjectHash = subject.GetHashCode();

            if (_lastMailSentDict.TryGetValue(subjectHash, out DateTime lastValue))
            {
                if (lastValue.AddMinutes(subjectAge) < DateTime.Now)
                {
                    _lastMailSentDict.AddOrUpdate(subjectHash, DateTime.Now, (k, v) => DateTime.Now);
                    isSpam = false;
                }
                else
                {
                    isSpam = true;
                }
            }
            else
            {
                _lastMailSentDict.AddOrUpdate(subjectHash, DateTime.Now, (k, v) => DateTime.Now);
                isSpam = false;
            }

            if (spamPreventionStored)
                WriteStoredLastMails();

            return isSpam;
        }

        /// <summary>
        /// Adds a hash as key with DateTime.Now as value to the _lastMailSentDict if the hash as doesn't exist yet.
        /// <i>This mehtod is used in the UNitTest to test spam cases</i>.
        /// </summary>
        /// <param name="subjectHash">hash as key for the dict to add</param>
        public void AddHashToLastMailSentDict(int subjectHash)
        {
            if (_lastMailSentDict.ContainsKey(subjectHash))
                return;

            _lastMailSentDict.AddOrUpdate(subjectHash, DateTime.Now, (k, v) => DateTime.Now);
        }

        /// <summary>
        /// Reads the stored values for last mails from path.
        /// Doesn't overwrite the existing empty Dictionary in _lastMailSentDict field if the stored file in path doesn't exist.
        /// </summary>
        private void ReadStoredLastMails()
        {
            var path = Path.Combine(AppDataCommonPathProcessor.GetProgramDataCompanyFolderPath(_applicationName), LastMailSentjsonFileName);
            if (File.Exists(path))
            {
                var lastMailSentJson = File.ReadAllText(path);
                _lastMailSentDict = JsonSerializer.Deserialize<ConcurrentDictionary<int, DateTime>>(lastMailSentJson);
            }
        }

        /// <summary>
        /// Writes the in memory Dictionary _lastMailSentDict as json to the given path.
        /// </summary>
        /// <param name="path">path to json file with stored last mails</param>
        private void WriteStoredLastMails()
        {
            var path = Path.Combine(AppDataCommonPathProcessor.GetProgramDataCompanyFolderPath(_applicationName), LastMailSentjsonFileName);
            File.WriteAllText(path, JsonSerializer.Serialize(_lastMailSentDict));
        }
    }
}