using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Utilities
{
    public interface ITools
    {
        string SHA256Hash(string input);
        string GetEntropyIdTail(string id);
        long ToUnixEpoch(DateTime dateTime);
        DateTime ToDateTime(long unixEpoch);
        DateTime DateTimeUtcToBogota(DateTime dateTimeUtc);
        string DateTimeUtcToBogotaAsString(DateTime dateTimeUtc);
        DateTime DateTimeBogotaToUtc(DateTime dateTimeBogota);
        string DateTimeBogotaToUtcAsString(DateTime dateTimeBogota);
        DateTime DateTimeBogotaToUtc(string dateTimeBogota);
        string DateTimeBogotaToUtcAsString(string dateTimeBogota);
        string DateTimeToString(DateTime dateTime);
        DateTime DateStringToDateTime(string dateTimeString);
    }
}
