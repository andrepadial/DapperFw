using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace Dapper.Fw.Helper
{
    public class LogInfo
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static void GravarErro(string erro)
        {
            log.Error(erro);
        }
    }
}
