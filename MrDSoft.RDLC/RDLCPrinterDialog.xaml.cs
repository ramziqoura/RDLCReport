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
using System.Windows.Shapes;
using System.Drawing;
using System.Printing;
using System.Drawing.Printing;
using Microsoft.Reporting.WinForms;
using System.IO;
using DSoft;
using DSoft.MethodExtension;
using DSoft.Localization;
using DSoft.RDLCReport;

namespace DSoft.RDLC
{
    /// <summary>
    /// Interaction logic for RDLCPrinterDialog.xaml
    /// </summary>
    public partial class RDLCPrinterDialog : Window
    {
        private PrintDocument _printer = new PrintDocument();
        private RDLCPrinter _report = null;
        private int _nbrCopies = 1;
        private int _nbrPageRapport = 1;
        private PrintQueue _currentPrinter = null;
        private LocalPrintServer _printServer = new LocalPrintServer();
        private List<PrintQueue> _printerList = new List<PrintQueue>();
        private string ImgSource; 

        //bool? _landscape = null;

        public int NbrPageRapport
        {
            get
            {
                return this._nbrPageRapport;
            }
            set 
            {
                _nbrPageRapport = value;
            }
            
        }

        public RDLCPrinter CurrentReport
        {
            get
            {
                return _report;
            }

            set
            {
                _report = value;

                if (_report.isDefaultLandscape == true)
                {
                    _printer.DefaultPageSettings.Landscape = true;                    
                }
                else
                {
                    
                }
            }
        }

        public RDLCPrinterDialog()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshWindow();
            //disable Paste (coller) pour les TextBox du choix des pages
            DataObject.AddPastingHandler(FirstPage, this.OnCancelCommand);
            DataObject.AddPastingHandler(LastPage, this.OnCancelCommand);
        }

        private void RefreshWindow()
        {
            
            
            if (this._report.NombreCopies >= 1)
            {
                NumPager.Text = this._report.NombreCopies.ToString();
                this._nbrCopies = this._report.NombreCopies;
            }
            FirstPage.Text = "1";

            //obtien le nombre de page du rapport
            LastPage.Text = _nbrPageRapport.ToString();

            //Obtien la liste des imprimantes (local et réseaux) pour les ajouter au combobox
            cboImprimanteNom.ItemsSource = _printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections }).Cast<PrintQueue>();
            cboImprimanteNom.DisplayMemberPath = "FullName";
            cboImprimanteNom.SelectedValue = "FullName";

            //selectionne l'imprimante courante a l'ouverture de la fenêtre
            _currentPrinter = LocalPrintServer.GetDefaultPrintQueue();
            for (int i = 0; i < cboImprimanteNom.Items.Count; i++)
            {
                PrintQueue testPrint = (PrintQueue)cboImprimanteNom.Items[i];
                if (testPrint.FullName.ToString() == _currentPrinter.FullName.ToString())
                {
                    cboImprimanteNom.SelectedIndex = i;
                }
            }

            //check si l'imprimante est disponible pour l'usage
            if (_currentPrinter.IsNotAvailable == false)
            {
                lblImprimanteStatus.Content = LocManager.ResourceManager.GetString("ReadyString");
                ImgSource = @"pack://application:,,,/DSoft.RDLC;component/Resources/Button-Blank-Green.ico";

            }
            else
            {
                lblImprimanteStatus.Content = LocManager.ResourceManager.GetString("OfflineString");
                ImgSource = @"pack://application:,,,/DSoft.RDLC;component/Resources/Button-Blank-Red.ico";

            }
            ReadyImage.Source = new BitmapImage(new Uri(ImgSource));
        }

        /// <summary>
        /// Sélectionne l'imprimante que l'utilisateur choisi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboImprimanetNom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            lblImprimanteStatus.Content = "";

            _currentPrinter = (PrintQueue)cboImprimanteNom.SelectedItem;

            if (_currentPrinter.IsNotAvailable == false)
            {
                lblImprimanteStatus.Content = LocManager.ResourceManager.GetString("ReadyString");
                //lblImprimanteStatus.Content = "Prêt";
                _printer.PrinterSettings.PrinterName = _currentPrinter.FullName;
                lblEmplacementImprimante.Content = _currentPrinter.QueuePort.Name;
                ImgSource = @"pack://application:,,,/DSoft.RDLC;component/Resources/Button-Blank-Green.ico";
            }
            else
            {
                lblImprimanteStatus.Content = LocManager.ResourceManager.GetString("OfflineString");
                ImgSource = @"pack://application:,,,/DSoft.RDLC;component/Resources/Button-Blank-Red.ico";
            }

            ReadyImage.Source = new BitmapImage(new Uri(ImgSource));
        }

        /// <summary>
        /// au click du bouton OK le call de l'impression ce produit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {


            if (_nbrCopies >= 30)
            {
                if (MessageBox.Show(LocManager.ResourceManager.GetString("CopiesWarningString"), LocManager.ResourceManager.GetString("CopiesWarningTitleString") + " (" + _nbrCopies.ToString() + ")", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    PreparePrint();
                    CurrentReport.PrintDoc = this._printer;
                    CurrentReport.NombreCopies = this._nbrCopies;
                    CurrentReport.Print();
                    this.Close();
                }
            }
            else
            {
                PreparePrint();
                CurrentReport.PrintDoc = this._printer;
                CurrentReport.NombreCopies = this._nbrCopies;
                CurrentReport.Print();
                this.Close();
            }
        }

        /// <summary>
        /// Prépare les pages pour l'impressioh
        /// </summary>
        /// <returns></returns>
        private void PreparePrint()
        {
            if (cmdAllPageButton.IsChecked == false)
            {
                _printer.PrinterSettings.FromPage = Convert.ToInt32(FirstPage.Text);
                _printer.PrinterSettings.ToPage = Convert.ToInt32(LastPage.Text);
            }
        }


        /// <summary>
        /// ferme la fenêtre 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void SpinnerUp_Click(object sender, RoutedEventArgs e)
        {
            _nbrCopies += 1;
            NumPager.Text = _nbrCopies.ToString();
        }

        private void SpinnerDown_Click(object sender, RoutedEventArgs e)
        {
            if (_nbrCopies > 1)
                _nbrCopies -= 1;

            NumPager.Text = _nbrCopies.ToString();
        }

        private void FirstPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsNumericKey())
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void LastPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsNumericKey())
                e.Handled = false;
            else
                e.Handled = true;
        }

        /// <summary>
        /// Cencel ctrl-v (paste)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }

        private void cmdAllPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmdAllPageButton.IsChecked == true)
            {
                PageChoiceStackPanel.IsEnabled = false;
                FirstPage.Text = "1";
                LastPage.Text = _nbrPageRapport.ToString();
            }
            else
                PageChoiceStackPanel.IsEnabled = true;
        }

        private void FirstPage_LostFocus(object sender, RoutedEventArgs e)
        {
            if(FirstPage.Text.Trim() == "")
                FirstPage.Text = "1";

            if (Convert.ToInt32(FirstPage.Text) < 1 || Convert.ToInt32(FirstPage.Text) > _nbrPageRapport)
                FirstPage.Text = "1";
        }

        private void LastPage_LostFocus(object sender, RoutedEventArgs e)
        {
            if(LastPage.Text.Trim() == "")
                LastPage.Text = _nbrPageRapport.ToString();

            if (Convert.ToInt32(LastPage.Text) < 1 || Convert.ToInt32(LastPage.Text) > _nbrPageRapport)
                LastPage.Text = _nbrPageRapport.ToString();
        }

    }
}
