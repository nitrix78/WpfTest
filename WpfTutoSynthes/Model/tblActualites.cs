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

namespace WpfTutoSynthes.Model {

    public class tblActualites : List<tblActualite> {

        /// <summary>Retourne tous les éléments de la table</summary>
        /// <param name="ordre">Champ à trier (sans le "ORDER BY")</param>
        /// <returns>Les éléments dans une liste.</returns>
        public static tblActualites getAll(string initiale, int catId, bool CatActifOnly = false, string ordre = "") {
            var ret = new tblActualites();
            if (!string.IsNullOrEmpty(ordre))
                ordre = string.Format(" ORDER BY {0}", ordre);
            List<string> filtres = new List<string>();
            if ((!string.IsNullOrEmpty(initiale)) && (initiale.Length==1))
                filtres.Add("LEFT(abcLib, 1) LIKE '" + initiale + "'");
            if (catId>0)
                filtres.Add("abcCatId=" + catId + "");
            if (CatActifOnly)
                filtres.Add("acaActif=1");

            string filtre = tblActualite.getFiltre(filtres);
            IDbConnection dbconn = null;
            using (var com = tblActualite.getComm("SELECT * FROM " + tblActualite.nomTable + " " + filtre + ordre, ref dbconn)) {
                using (IDataReader dr = com.ExecuteReader()) {
                    while (dr.Read()) {
                        /*tblActualite cl = new tblActualite((int)dr["abcId"], dr["abcLib"].ToString(), dr["abcTauxTva"].ToString(), dr["abcDesc"].ToString()
                            , int.Parse(dr["abcCatId"].ToString()), dr["abcRefDoc"].ToString(), dr["abcLien"].ToString(), (DateTime)dr["abcDateCrea"]);
                        ret.Add(cl); // */
                    }
                }
                dbconn.Close();
            }
            return ret;
        }

    }

    public class tblActualite : Provider {
        #region proprietes
        public static string nomTable = "ACTUALITES";
        public static string nomChampId = "ID_ACTUALITE";
        private int _id;
        private int Id { get { return _id; } set { _id = value; } }
        private int _miId;
        public int ModInstId { get { return _miId; } set { _miId = value; } }
        private int _diffId;
        public int DiffusionId { get { return _diffId; } set { _diffId = value; } }
        private string _guid;
        public string Guid { get { return _guid; } set { _guid = value; } }
        private string _lib;
        public string Lib { get { return _lib; } set { _lib = value; } }
        private DateTime _dateCrea;
        public DateTime DateCrea { get { return _dateCrea; } private set { _dateCrea = value; } }
        private DateTime? _dateModif;
        public DateTime? DateModif { get { return _dateModif; } set { _dateModif = value; } }
        private DateTime? _dateEven;
        public DateTime? DateEven { get { return _dateEven; } set { _dateEven = value; } }
        private DateTime? _datePubli;
        public DateTime? DatePubli { get { return _datePubli; } set { _datePubli = value; } }
        private DateTime? _dateFin;
        public DateTime? DateFin { get { return _dateFin; } set { _dateFin = value; } }
        private string _resume;
        public string Resume { get { return _resume; } set { _resume = value; } }
        private string _desc;
        public string Desc { get { return _desc; } set { _desc = value; } }
        private string _lieu;
        public string Lieu { get { return _lieu; } set { _lieu = value; } }
        private string _contact;
        public string Contacts { get { return _contact; } set { _contact = value; } }
        private bool _publi;
        public bool Publication { get { return _publi; } set { _publi = value; } }
        private bool _accueil;
        public bool Accueil { get { return _accueil; } set { _accueil = value; } }
        private bool _accueilTop;
        public bool AccueilTop { get { return _accueilTop; } set { _accueilTop = value; } }
        private bool _partage;
        public bool Partage { get { return _partage; } set { _partage = value; } }
        private bool _active;
        public bool Active { get { return _active; } set { _active = value; } }
        private bool _principal;
        public bool Principal { get { return _principal; } set { _principal = value; } }
        private bool? _mixte;
        public bool? Mixte { get { return _mixte; } set { _mixte = value; } }
        private bool? _force;
        public bool? Force { get { return _force; } set { _force = value; } }
        private int _catId;
        public int CatId { get { return _catId; } set { _catId = value; } }
        /*private tblPieceJointes _pjs = null;
        public tblPieceJointes PiecesJointes {
            get {
                if (_pjs == null) {
                    _pjs = tblPieceJointes.getAll(Id);
                }
                return _pjs;
            }
        }
        private tblLiens _liens = null;
        public tblLiens Liens {
            get {
                if (_liens == null) {
                    _liens = tblLiens.getAll(Id);
                }
                return _liens;
            }
        }
        private tblTypes _types = null;
        public tblTypes Types {
            get {
                if (_types == null) {
                    _types = tblTypes.getAll(Id);
                }
                return _types;
            }
        }*/
        #endregion

        #region constructeurs
        /// <summary>Constructeur pour les ajouts (add())</summary>
        public tblActualite(int pModInstId, int pDiffusionId, string pLib, string pResume, string pDesc, DateTime? pDateEven, DateTime? pDateModif, DateTime? pDatePubli, DateTime? pDateFin, bool pActive) {
            Lib = pLib;
            ModInstId = pModInstId;
            DiffusionId = pDiffusionId;
            DateEven = pDateEven;
            Desc = pDesc;
            Resume = pResume;
            DateModif = pDateModif;
            DatePubli = pDatePubli;
            DateFin = pDateFin;
            Active = pActive;
        }

        /// <summary>Constructeur pour la lecture (getAll())</summary>
        internal tblActualite(int pId, int pModInstId, int pDiffusionId, string pLib, string pResume, string pDesc, DateTime? pDateEven, DateTime? pDateModif, DateTime? pDatePubli, 
            DateTime? pDateFin, DateTime pDateCrea, bool pActive, bool pPublication, bool pAccueil, bool pAccueilTop, bool pPartage, bool pPrincipal, bool? pMixte, bool? pForce)
            : this(pModInstId, pDiffusionId, pLib, pResume, pDesc, pDateEven, pDateModif, pDatePubli, pDateFin, pActive) {
            Id = pId;
            DateCrea = pDateCrea;
            Publication = pPublication;
            Accueil = pAccueil;
            AccueilTop = pAccueilTop;
            Partage = pPartage;
            Principal = pPrincipal;
            Mixte = pMixte;
            Force = pForce;
        }
        #endregion

        #region CRUD
        /// <summary>Lecture d'un élément dans la table.</summary>
        /// <returns>L'éléments correspondant à l'ID.</returns>
        public static tblActualite get(int id) {
            tblActualite ret = null;
            IDbConnection dbconn = null;
            using (var com = getComm(string.Format("SELECT * FROM {0} WHERE {1}={2};", nomTable, nomChampId, id), ref dbconn)) {
                using (IDataReader dr = com.ExecuteReader()) {
                    if (dr.Read()) {
                        DateTime d;
                        DateTime? pDateEven = null, pDateModif = null, pDatePubli = null, pDateFin = null, pDateCrea = null;
                        if ((dr["DATE_CREATION"] != null) && (DateTime.TryParse(dr["DATE_CREATION"].ToString(), out d))) pDateCrea = d;
                        if ((dr["DATE_MODIFICATION"] != null) && (DateTime.TryParse(dr["DATE_MODIFICATION"].ToString(), out d))) pDateModif = d;
                        if ((dr["DATE_EVENEMENT"] != null) && (DateTime.TryParse(dr["DATE_EVENEMENT"].ToString(), out d))) pDateEven = d;
                        if ((dr["DATE_PUBLICATION"] != null) && (DateTime.TryParse(dr["DATE_PUBLICATION"].ToString(), out d))) pDatePubli = d;
                        if ((dr["DATE_PEREMPTION"] != null) && (DateTime.TryParse(dr["DATE_PEREMPTION"].ToString(), out d))) pDateFin = d;
                        bool b;
                        bool? pMixte = null, pForce = null;
                        if ((dr["MIXTE"] != null) && (bool.TryParse(dr["MIXTE"].ToString(), out b))) pMixte = b;
                        if ((dr["EST_FORCE"] != null) && (bool.TryParse(dr["EST_FORCE"].ToString(), out b))) pMixte = b;
                        ret = new tblActualite(id, int.Parse(dr["ID_MODULE_INSTANCE"].ToString()), int.Parse(dr["ID_ACTUALITE_DIFFUSION"].ToString()), dr["TITRE"].ToString(),
                            dr["RESUME"].ToString(), dr["CONTENU"].ToString(), pDateEven, pDateModif, pDatePubli, pDateFin, (DateTime)pDateCrea, bool.Parse(dr["ACTIVE"].ToString()),
                            bool.Parse(dr["PUBLICATION"].ToString()), bool.Parse(dr["AFFICHAGE_ACCUEIL"].ToString()), bool.Parse(dr["AFFICHAGE_ACCUEIL_TOP"].ToString()),
                            bool.Parse(dr["PARTAGE"].ToString()), bool.Parse(dr["PAGE_PRINCIPALE"].ToString()), pMixte, pForce); //ID_ACTUALITE_THEME GUID LIEU CONTACT ID_ACTUALITE_SOUS_THEME LISTE_PARTICIPATION DATE_MAJ FAQ		
                    }
                }
                dbconn.Close();
            }
            return ret;
        }

        public static int getLastId() {
            return getLastId(nomTable, nomChampId);
        }

        /// <summary>Fait le mappage des parametres avec les valeurs des champs.</summary>
        /// <returns></returns>
        private Hashtable mapChamps(int mode=0) {
            Hashtable ret = new Hashtable();
            ret.Add("@ID_ACTUALITE_DIFFUSION", DiffusionId);
            ret.Add("@TITRE", Lib.Trim());
            ret.Add("@RESUME", Resume);
            ret.Add("@CONTENU", Desc);
            ret.Add("@DATE_MODIFICATION", DateModif);
            ret.Add("@DATE_EVENEMENT", DateEven);
            ret.Add("@DATE_PUBLICATION", DatePubli);
            ret.Add("@DATE_PEREMPTION", DateFin);
            ret.Add("@ACTIVE", Active);
            if (mode > 0) {
                ret.Add("@ID_MODULE_INSTANCE", ModInstId);
                ret.Add("@PARTAGE", Partage);
                ret.Add("@PUBLICATION", Publication);
                ret.Add("@AFFICHAGE_ACCUEIL", Accueil);
                ret.Add("@AFFICHAGE_ACCUEIL_TOP", AccueilTop);
                ret.Add("@PAGE_PRINCIPALE", Principal);
            }
            return ret;
        }

        /// <summary>Ajout ou modification d'un élément dans la table, selon Id.</summary>
        /// <returns>Nombre d'éléments impactés (1=OK) ou l'ID inséré.</returns>
        public int Apply() {
            Hashtable prms = mapChamps(1);
            if (Id > 0) { // MAJ
                string champs = string.Join(",", prms.Cast<DictionaryEntry>().Select(x => x.Key.ToString().Substring(1) + "=" + x.Key.ToString()).ToArray());
                /*return execQuery(string.Format("UPDATE {0} SET ID_MODULE_INSTANCE=@ID_MODULE_INSTANCE, ID_ACTUALITE_DIFFUSION=@ID_ACTUALITE_DIFFUSION, TITRE=@TITRE, RESUME=@RESUME,"
                    + "CONTENU=@CONTENU, DATE_CREATION=@DATE_CREATION, DATE_MODIFICATION=@DATE_MODIFICATION, DATE_EVENEMENT=@DATE_EVENEMENT, DATE_PUBLICATION=@DATE_PUBLICATION, "
                    + "DATE_PEREMPTION=@DATE_PEREMPTION, ACTIVE=@ACTIVE, PUBLICATION=@PUBLICATION, AFFICHAGE_ACCUEIL=@AFFICHAGE_ACCUEIL, AFFICHAGE_ACCUEIL_TOP=@AFFICHAGE_ACCUEIL_TOP, "
                    + "PARTAGE=@PARTAGE, PAGE_PRINCIPALE=@PAGE_PRINCIPALE WHERE {1}={2};", nomTable, nomChampId, Id), prms); // */
                return execQuery(string.Format("UPDATE {0} SET {3} WHERE {1}={2};", nomTable, nomChampId, Id, champs), prms); // */
            } else { // Ajout
                prms.Add("@DATE_CREATION", DateTime.Now);
                string champs = string.Join(",", prms.Keys.Cast<object>().Select(x => x.ToString().Substring(1)).ToArray());
                string values = string.Join(",", prms.Keys.Cast<object>().Select(x => x.ToString()).ToArray());
                return execScalar(string.Format("INSERT INTO {0} ({2}) output inserted.{1} VALUES ({3});", nomTable, nomChampId, champs, values), prms); // */
            }
        }
        #endregion
    }
}