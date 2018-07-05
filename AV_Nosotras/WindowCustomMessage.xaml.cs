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

namespace AV_Nosotras
{
    /// <summary>
    /// Lógica de interacción para WindowCustomMessage.xaml
    /// </summary>
    public partial class WindowCustomMessage : Window
    {
        public WindowCustomMessage()
        {
            InitializeComponent();
        }

        public void showMessage(string message, string title)
        {
            lbl_message.Content = message;
            lbl_title.Content = title;
            Show();
        }
        public void closeWindow()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() { Close(); }));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
