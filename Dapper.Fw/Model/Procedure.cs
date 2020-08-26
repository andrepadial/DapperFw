using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.Fw.Model
{
    public class Procedure
    {
        public SqlConnection Conexao { set; get; }
        public string Nome { set; get; }
        public DynamicParameters Parametros { set; get; }
        public int TimedOut { set; get; }
        public CommandType CommandType { set; get; }

    }
}
