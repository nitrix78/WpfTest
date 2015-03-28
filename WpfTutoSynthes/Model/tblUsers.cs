using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
 // http://blog.soat.fr/2012/02/simplifier-lecriture-de-inotifypropertychanged-en-c/


namespace WpfTutoSynthes.Model {

    public class tblUsers : List<tblUser> { // ObservableCollection

        /// <summary>Retourne tous les éléments de la table</summary>
        /// <param name="ordre">Champ à trier (sans le "ORDER BY")</param>
        /// <returns>Les éléments dans une liste.</returns>
        public static tblUsers getAll(string pNom, int srvId, bool actifOnly = false, string ordre = "usrNom") {
            var ret = new tblUsers();
            if (!string.IsNullOrEmpty(ordre))
                ordre = string.Format(" ORDER BY {0}", ordre);
            List<string> filtres = new List<string>();
            if ((!string.IsNullOrEmpty(pNom)))
                filtres.Add("usrNom LIKE '" + pNom + "'");
            if (srvId > 0)
                filtres.Add("usrSrvId=" + srvId + "");
            if (actifOnly)
                filtres.Add("usrActif=1");
            string filtre = ""; // tblUser.getFiltre(filtres);
            IDbConnection dbconn = null;
            using (var com = tblUser.getComm("SELECT * FROM " + tblUser.nomTable + " " + filtre + ordre, ref dbconn)) {
                using (IDataReader dr = com.ExecuteReader()) {
                    while (dr.Read()) {
                        DateTime d;
                        DateTime? pDateNaiss = null;
                        if ((dr["usrDateNaiss"] != null) && (DateTime.TryParse(dr["usrDateNaiss"].ToString(), out d))) pDateNaiss = d;
                        tblUser cl = new tblUser() { Id=(int)dr["usrId"], Nom=dr["usrNom"].ToString(), Prenom=dr["usrPrenom"].ToString(),
                            DateCrea = DateTime.Parse(dr["usrDateCrea"].ToString()), DateNaissance=pDateNaiss,
                            Actif=bool.Parse(dr["usrActif"].ToString()) };
                        ret.Add(cl); // */
                    }
                }
                dbconn.Close();
            }
            return ret;
        }
    }

    public class tblUser : Provider {

        #region Proprietés
        public static string nomTable = "tblUsers";
        public static string nomChampId = "usrId";
        private int id;
        public int Id { get { return id; } set { id = value; } } //Sans CallerMemberName: set { SetField(ref name, value, "Name"); }
        private string nom;
        public string Nom { get { return nom; } set { nom = value; } } //Sans CallerMemberName: set { SetField(ref name, value, "Name"); }
        private string prenom;
        public string Prenom { get { return prenom; } set { prenom = value; } }
        private DateTime? dateNaissance;
        public DateTime? DateNaissance { get { return dateNaissance; } set { dateNaissance = value; } }
        private DateTime dateCrea;
        public DateTime DateCrea { get { return dateCrea; } set { dateCrea = value; } }
        private bool actif;
        public bool Actif { get { return actif; } set { actif = value; } }
        #endregion

        #region Constructeur
        public tblUser() { }
        #endregion

        public override string ToString() { return nom.ToUpper() + " " + prenom.ToLower(); }

        /// <summary>Fait le mappage des parametres avec les valeurs des champs.</summary>
        /// <returns></returns>
        private Hashtable mapChamps(int mode = 0) {
            Hashtable ret = new Hashtable();
            ret.Add("@usrNom", Nom);
            ret.Add("@usrPrenom", Prenom);
            ret.Add("@usrActif", Actif);
            ret.Add("@usrDateNaiss", dateNaissance);
            if (mode > 0) {
                ret.Add("@usrDateCrea", dateCrea);
            }
            return ret;
        }

        #region CRUD
        /// <summary>Lecture d'un élément dans la table.</summary>
        /// <returns>L'éléments correspondant à l'ID.</returns>
        public static tblUser get(int pId) {
            tblUser ret = null;
            IDbConnection dbconn = null;
            using (var com = getComm(string.Format("SELECT * FROM {0} WHERE {1}={2};", nomTable, nomChampId, pId), ref dbconn)) {
                using (IDataReader dr = com.ExecuteReader()) {
                    if (dr.Read()) {
                        DateTime d;
                        DateTime? pDateNaiss = null;
                        if ((dr["usrDateNaiss"] != null) && (DateTime.TryParse(dr["usrDateNaiss"].ToString(), out d))) pDateNaiss = d;
                        ret = new tblUser() {
                            Id = pId, nom = dr["usrNom"].ToString(), Prenom = dr["usrPrenom"].ToString(), dateNaissance = pDateNaiss,
                            Actif = bool.Parse(dr["usrPrenom"].ToString())
                        };
                    }
                }
                dbconn.Close();
            }
            return ret;
        }

        public static int getLastId() {
            return getLastId(nomTable, nomChampId);
        }
        
        /// <summary>Ajout ou modification d'un élément dans la table, selon Id.</summary>
        /// <returns>Nombre d'éléments impactés (1=OK) ou l'ID inséré.</returns>
        public int Apply() {
            Hashtable prms = mapChamps(0);
            if (Id > 0) { // MAJ
                string champs = string.Join(",", prms.Cast<DictionaryEntry>().Select(x => x.Key.ToString().Substring(1) + "=" + x.Key.ToString()).ToArray());
                return execQuery(string.Format("UPDATE {0} SET {3} WHERE {1}={2};", nomTable, nomChampId, Id, champs), prms); // */
            } else { // Ajout
                prms.Add("@usrDateNaiss", DateTime.Now);
                string champs = string.Join(",", prms.Keys.Cast<object>().Select(x => x.ToString().Substring(1)).ToArray());
                string values = string.Join(",", prms.Keys.Cast<object>().Select(x => x.ToString()).ToArray());
                return execScalar(string.Format("INSERT INTO {0} ({2}) output inserted.{1} VALUES ({3});", nomTable, nomChampId, champs, values), prms); // */
            }
        }
        #endregion

        /*#region insertDetail
        public SqlCommand objcmd;
        public void Insetrt() {
            SqlConnection objCon = new SqlConnection(""); //connection name
            try {
                objCon.Open();
                objcmd = new SqlCommand(string.Format("insert into {0} values (@Id,@usrNom,@usrPrenom) ",nomTable), objCon);
                objcmd.Parameters.AddWithValue("@Id", Id);
                objcmd.Parameters.AddWithValue("@usrNom", nom);
                objcmd.Parameters.AddWithValue("@usrPrenom", Prenom);
                //On property changed
                OnPropertyChanged("Id");
                OnPropertyChanged("nom");
                OnPropertyChanged("Prenom");
                objcmd.ExecuteNonQuery();
            } catch (Exception ex) {
                //MessageBox.Show("" + ex);
            } finally {
                objcmd.Dispose();
                if (objCon.State == ConnectionState.Open) {
                    objCon.Close();
                    objCon.Dispose();
                }
            }
        }
        #endregion

        #region LoadDetails
        public void Load() {
            SqlConnection objCon = new SqlConnection(""); //connection name
            objCon.Open();
            objcmd = new SqlCommand(string.Format("Select * from {0} where {1}=@Id", nomTable, nomChampId), objCon);//select command
            objcmd.Parameters.Add(new SqlParameter("@Id", id));
            try {
                SqlDataReader objRDR = objcmd.ExecuteReader();
                if (objRDR.Read()) {
                    nom = objRDR["usrNom"].ToString();
                    prenom = objRDR["usrPrenom"].ToString();
                    //property changed
                    OnPropertyChanged("Id");
                    OnPropertyChanged("nom");
                    OnPropertyChanged("Prenom");
                } else {
                    //MessageBox.Show("RecordNot Found");
                }
            } catch (Exception ex) {
                //MessageBox.Show("" + ex);
            } finally {
                objcmd.Dispose();
                if (objCon.State == ConnectionState.Open) {
                    objCon.Close();
                    objCon.Dispose();
                }
            }
        }
        #endregion

        #region UpdateDetails
        public void update() {
            SqlConnection objCon = new SqlConnection(""); //connection name
            objCon.Open();
            objcmd = new SqlCommand(string.Format("update {0} set usrNom=@usrNom,usrPrenom=@usrPrenom where {1}=@Id ", nomTable, nomChampId), objCon);
            objcmd.Parameters.Add(new SqlParameter("@Id", id));
            try {
                objcmd.Parameters.AddWithValue("@usrNom", nom);
                objcmd.Parameters.AddWithValue("@usrPrenom", Prenom);
                //propertychanged              
                OnPropertyChanged("Id");
                OnPropertyChanged("nom");
                OnPropertyChanged("Prenom");
                objcmd.ExecuteNonQuery();
            } catch (Exception ex) {
                //MessageBox.Show("" + ex);
            } finally {
                objcmd.Dispose();
                if (objCon.State == ConnectionState.Open) {
                    objCon.Close();
                    objCon.Dispose();
                }
            }
        }
        #endregion // */
    }
}
