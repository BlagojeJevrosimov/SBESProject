using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupServer
{
    public class BackupService : ISyslogServerBackupData
    {
        public void BackupLog(string message, byte[] sign)
        {
            throw new NotImplementedException();
        }
    }
}
