using Dapper.Fw.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;
using log4net;
using System.Linq;
using Dapper;
using Z.Dapper.Plus;
using System.Reflection;

namespace Dapper.Fw.DataAccess
{
    public class Execute
    {
        
        public static void ExecutarProcedure (Procedure procedure)
        {
            using (SqlConnection conn = new SqlConnection(procedure.Conexao.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                try
                {
                    conn.Execute(procedure.Nome, procedure.Parametros, null, procedure.TimedOut, procedure.CommandType);
                }
                catch(Exception ex)
                {
                    Helper.LogInfo.GravarErro(String.Concat(System.Reflection.MemberTypes.Method.GetType().Name, " - ", System.Reflection.MethodBase.GetCurrentMethod().Name, ": ", ex.Message.ToString()));   
                }
            }
        }

        public static List<T> ExecuteProcedureListaObjeto<T>(Procedure procedure)
        {
            List<T> lista = new List<T>();

            try
            {
                using (SqlConnection conn = new SqlConnection(procedure.Conexao.ConnectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    lista = conn.Query<T>(procedure.Nome, procedure.Parametros, null, commandTimeout: procedure.TimedOut, commandType: procedure.CommandType).ToList();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Helper.LogInfo.GravarErro(String.Concat(System.Reflection.MemberTypes.Method.GetType().Name, " - ", System.Reflection.MethodBase.GetCurrentMethod().Name, ": ", ex.Message.ToString()));
            }

            return lista;
        }

        public static T ExecutarProcedureObjeto<T>(Procedure procedure)
        {            
            T objeto = default(T);
            
            try
            {
                using (SqlConnection conn = new SqlConnection(procedure.Conexao.ConnectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    objeto = conn.Query<T>(procedure.Nome, procedure.Parametros, null, commandTimeout: procedure.TimedOut, commandType: procedure.CommandType).FirstOrDefault();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Helper.LogInfo.GravarErro(String.Concat(System.Reflection.MemberTypes.Method.GetType().Name, " - ", System.Reflection.MethodBase.GetCurrentMethod().Name, ": ", ex.Message.ToString()));                
            }

            return objeto;
        }

        public static void Incluir<T>(List<T> objetos, string tabelaBD, string connectionString)
        {
            Type t = objetos[0].GetType();
            string campos = String.Empty;
            string camposObjeto = String.Empty;
            object o = Activator.CreateInstance(t);
            PropertyInfo[] props = o.GetType().GetProperties();

            foreach(var p in props)
            {
                campos += String.Concat(p.Name, ", ");
            }            

            foreach (var p in props)
            {
                camposObjeto += String.Concat("\"\"@", p.Name, ", ");
            }


            campos = campos.Substring(0, campos.Length - 1);
            camposObjeto = camposObjeto.Substring(0, campos.Length - 1);

            string comandoSQL = "INSERT INTO " + tabelaBD + "( " + campos + " ) ";
            comandoSQL += " VALUES ( " + camposObjeto + " ) ";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    conn.Execute(comandoSQL, objetos, null, commandTimeout: 0, commandType: CommandType.Text);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Helper.LogInfo.GravarErro(String.Concat(System.Reflection.MemberTypes.Method.GetType().Name, " - ", System.Reflection.MethodBase.GetCurrentMethod().Name, ": ", ex.Message.ToString()));
            }
        }

        public static void Atualizar<T>(List<T> objetos, List<T> objetosAtualizados, string connectionString, string tabelaBD, List<string> identitiesTable)
        {
            Type t = objetosAtualizados[0].GetType();
            string campos = String.Empty;
            string camposWhere = String.Empty;
            object o = Activator.CreateInstance(t);            
            PropertyInfo[] props = o.GetType().GetProperties();            

            foreach (var p in props)
            {
                campos += String.Concat(p.Name, " = ", "\"\"@", p.Name, ", ");
            }

            camposWhere = " WHERE ";

            foreach(T item in objetos)
            {
                var row = props.Select(d => item.GetType()
                                                .GetProperty(d.Name)
                                                .GetValue(item, null)
                                                .ToString())
                                                .ToArray();
            }
            
            campos = campos.Substring(0, campos.Length - 1);           

            string comandoSQL = "UPDATE " + tabelaBD + " SET ";

            

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    conn.Execute(comandoSQL, objetosAtualizados, null, commandTimeout: 0, commandType: CommandType.Text);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Helper.LogInfo.GravarErro(String.Concat(System.Reflection.MemberTypes.Method.GetType().Name, " - ", System.Reflection.MethodBase.GetCurrentMethod().Name, ": ", ex.Message.ToString()));
            }
        }

    }
}
