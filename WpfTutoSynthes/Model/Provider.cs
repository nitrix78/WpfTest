using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfTutoSynthes.Model { // DebugConverter (classe inutile appelée depuis Xaml pour analyser les propriétés d'un objet lors du dégug).
    public class Provider { //INotifyPropertyChanged
        /*#region boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion // */

        //public static string conStrNom = "WpfTutoSynthes.Properties.Settings.connStr";
        public static string connect = WpfTutoSynthes.Properties.Settings.Default.connStr;
        //ConfigurationManager.ConnectionStrings[WpfTutoSynthes.Properties.Settings.Default.connStr].ConnectionString;
        public static string dsName = "Table";

        /// <summary>Retourne le provider pour le factory (nomConStr: Name dans le web.config).</summary>
        public static DbProviderFactory getProvider() {
            string provider = "System.Data.SqlClient"; // ConfigurationManager.ConnectionStrings[nomConStr].ProviderName;
            //provider = provider.Replace("SqlServerCe", "SqlServerCe.4.0"); // SqlServerCe.3.5 SqlServerCe.4.0
            return DbProviderFactories.GetFactory(provider); // System.Data.SqlClient System.Data.OleDb EntityClient
        }

        public static IDbCommand getComm(string req, ref IDbConnection dbconn) {
            return getComm(req, ref dbconn, "");
        }
        public static IDbCommand getComm(string req, ref IDbConnection dbconn, string nomConStr) {
            IDbDataAdapter da = null;
            return getComm(req, ref dbconn, ref da, nomConStr);
        }
        public static IDbCommand getComm(string req, ref IDbConnection dbconn, ref IDbDataAdapter da) {
            return getComm(req, ref dbconn, ref da, "");
        }
        public static IDbCommand getComm(string req, ref IDbConnection dbconn, ref IDbDataAdapter da, string nomConStr) {
            DbProviderFactory dbfactory = getProvider();
            da = dbfactory.CreateDataAdapter();
            dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = connect;
            dbconn.Open();
            IDbCommand commande = dbconn.CreateCommand();
            commande.CommandText = req; // Pour les dates "SET LANGUAGE FRENCH;"+  (FRENCH ENGLISH).
            //commande.CommandType = CommandType.Text; // On choisit la commande qui sera executé par le DataAdapter et on rempli le DataSet 
            return commande;
        }

        /// <summary>Execute une requete sans retour de données (UPDATE, INSERT, DELETE).</summary>
        /// <param name="req"></param>
        /// <param name="prms"></param>
        /// <returns>Le nombre d'éléments inpactés par la requete.</returns>
        public static int execQuery(string req, Hashtable prms = null) {
            IDbConnection dbconn = null;
            IDbCommand cmd = getComm(req, ref dbconn);
            //cmd.CommandType = CommandType.Text;
            /*Utils u = new Utils(); // Traceur pour DEBUG
            StreamWriter sw2 = File.CreateText(u.chemRep + "test.log");
            sw2.Write(req);
            sw2.Close(); // */
            if (prms != null) {
                foreach (DictionaryEntry prm in prms) {
                    IDbDataParameter p = cmd.CreateParameter();
                    p.ParameterName = prm.Key.ToString();
                    if (prm.Value != null) {
                        p.Value = prm.Value;
                    } else
                        p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }
            int ret = cmd.ExecuteNonQuery();
            dbconn.Close();
            return ret;

        }

        public static int getLastId(string nomTable, string nomChampId) {
            int ret = execScalar("SELECT COALESCE(MAX(" + nomChampId + "), 0) AS Id FROM " + nomTable);
            if (ret > 0) {
                return ret + 1;
            } else
                return 1;
        }

        /// <summary>Retourne les informations des providers installés.</summary>
        public static string getProvidersInstalles() {
            string ret = "";
            int i;
            DataTable providerTable = DbProviderFactories.GetFactoryClasses();
            foreach (DataRow row in providerTable.Rows) {
                i = 0;
                foreach (DataColumn col in providerTable.Columns) {
                    ret += col.ColumnName + ": " + row.ItemArray[i++].ToString() + "<br />";
                }
                ret += "<br />";
            }
            return ret;
        }

        /// <summary>Execute une requete qui retourne un seul résultat (Count(), ...).</summary>
        /// <param name="req"></param>
        /// <param name="prms"></param>
        /// <returns>Le résultat en entier.</returns>
        public static int execScalar(string req, Hashtable prms = null) {
            string strTmp = execScalarStr(req, prms);
            int ret;
            if (int.TryParse(strTmp, out ret)) {
                return int.Parse(strTmp);
            } else
                return 0;
        }

        public static string execScalarStr(string req, Hashtable prms = null) {
            IDbConnection dbconn = null;
            IDbCommand cmd = getComm(req, ref dbconn);
            if (prms != null) {
                foreach (DictionaryEntry prm in prms) {
                    IDbDataParameter p = cmd.CreateParameter();
                    p.ParameterName = prm.Key.ToString();
                    if (prm.Value != null) {
                        p.Value = prm.Value;
                    } else
                        p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }
            object o = cmd.ExecuteScalar();
            dbconn.Close();
            if (o != null) {
                return o.ToString();
            } else
                return "";
        }

        public static bool ExecScalarBool(string req) {
            IDbConnection dbconn = null;
            IDbCommand cmd = getComm(req, ref dbconn);
            bool result = (bool)cmd.ExecuteScalar();
            dbconn.Close();
            return result;
        }

        public static bool tableExiste(string nomTable) {
            return (execScalar("SELECT COUNT(*) FROM sys.objects WHERE object_id=OBJECT_ID(N'[dbo].[" + nomTable + "]') AND type in (N'U')") > 0);
        }

        /*/// <summary>Execute une requete qui retourne des valeurs.</summary>
        /// <param name="req"></param>
        /// <param name="prms"></param>
        public static void execRead(string req, Hashtable prms) {
            IDbCommand com = getComm(req, prms);
            using (IDataReader dr = com.ExecuteReader()) {
                while (dr.Read()) {
                    // do something with the data
                }
            }
        } // */

        /// <summary>Construit le where de la requête à partir d'une liste de conditions.</summary>
        /// <param name="filtres">Condition: "lib LIKE 'test%'", "id=1"</param>
        /// <returns>La chaine " WHERE ... AND ..."</returns>
        public static string getFiltre(List<string> filtres) {
            string ret = "";
            if (filtres.Count > 0) {
                foreach (var item in filtres) {
                    ret = (string.IsNullOrEmpty(ret)) ? string.Format(" WHERE {0}", item) : string.Format("{0} AND {1}", ret, item);
                }
            }
            return ret;
        }

        public static DataTable GetDs(string req) {
            return GetDs(req, null);
        }

        public static DataTable GetDs(string req, Hashtable prms) {
            DataSet ds = new DataSet(dsName); // Avec un IDbDataAdapter on ne peux nommer des tables. Y a une seule table et elle s'appelle Table.
            /* OleDbConnection oc = new OleDbConnection(); // SqlConnection oc = new SqlConnection();
            oc.ConnectionString = connect;
            OleDbDataAdapter oda = new OleDbDataAdapter(req, oc); // SqlDataAdapter oda = new SqlDataAdapter(req, oc);
            oda.Fill(ds, dsName);
            oc.Close(); // */
            if (!string.IsNullOrEmpty(req)) {
                IDbDataAdapter da = null;
                IDbConnection dbconn = null;
                IDbCommand cmd = getComm(req, ref dbconn, ref da);
                if (prms != null) {
                    foreach (DictionaryEntry prm in prms) {
                        IDbDataParameter p = cmd.CreateParameter();
                        p.ParameterName = prm.Key.ToString();
                        if (prm.Value != null) {
                            p.Value = prm.Value;
                        } else
                            p.Value = DBNull.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                da.SelectCommand = cmd;
                da.Fill(ds);
                dbconn.Close();
            }
            if ((ds.Tables.Count > 0)) { // && (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            } else
                return null;
        }

    }
}