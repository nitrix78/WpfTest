using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfTutoSynthes.Model;

namespace WpfTutoSynthes.Pages {
    /// <summary>
    /// Logique d'interaction pour pgeMain.xaml
    /// </summary>
    public partial class pgeMain : Page {
        public pgeMain() {
            InitializeComponent();
            //this.DataContext = tblUsers.getAll("", 0);

            var ret = tblUsers.getAll("", 0);
            MessageBox.Show(ret.Count.ToString());
        }
    }
}
