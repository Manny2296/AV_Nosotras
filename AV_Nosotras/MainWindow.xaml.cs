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
using MySql.Data.MySqlClient;

namespace AV_Nosotras
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 
      
    public partial class MainWindow : Window
    {
        MySqlConnection Conn_Local = new MySqlConnection();
        MySqlConnection Conn_Rmt = new MySqlConnection();
        ClassConsulta classCon;

        public MainWindow()
        {
            classCon = new ClassConsulta(Conn_Local, Conn_Rmt);
            InitializeComponent();
            funcVideoSelect(6, media_element_publicite);
            funcVisibility(0);
           
          
        }

        public void label_changeTextFunction(Label label, String text){
            try
            {
                label.Content = text;

            }
            catch (Exception e)
            {
                Console.WriteLine("label_changeText EXCEPTION:" + e.ToString());
            }
        }

        private void funcVisibility(int _idxvis)
        {
            switch (_idxvis)
            {
                //INICIO
                case 0:
                    {
                        GridTab_Publicidad.Visibility = Visibility.Visible;
                        GridTab_Menu.Visibility = Visibility.Hidden;
                        GridTab_menu_opciones.Visibility = Visibility.Visible;
                        GridTab_productos.Visibility = Visibility.Hidden;
                        break;
                    }
                case 1:
                    {
                        GridTab_Publicidad.Visibility = Visibility.Hidden;
                        GridTab_Menu.Visibility = Visibility.Visible;
                        GridTab_menu_opciones.Visibility =Visibility.Visible;
                        GridTab_productos.Visibility = Visibility.Hidden;
                        funcVideoSelect(0, media_element_inicio);
                        label_changeTextFunction(lbl_instrucciones, "Bienvenido, Seleccione una opción \npara continuar :");
                        break;
                    }

                case 3:
                    {
                        GridTab_menu_opciones.Visibility = Visibility.Hidden;
                        GridTab_productos.Visibility = Visibility.Visible;
                        btn_cancel.Visibility = Visibility.Hidden;
                        btn_validate.Visibility = Visibility.Hidden;
                    }break;
            }
        }
        private void funcVideoSelect(int _idxVid, MediaElement mediaelement)
        {
            try
            {
                mediaelement.Source = new Uri(ClassConsulta.VidFile(_idxVid),
                UriKind.Relative); mediaelement.Play();
            }
            catch (Exception f)
            {

                Console.WriteLine("excepcion video select" + f.ToString());
            }
        }
        /**
		 * [private void]
		 * Funcion Dispatcher 'http://msdn.microsoft.com/en-us/library/vstudio/system.windows.threading.dispatcher'
		 * para acceder a los objetos de la interfaz
		 * desde un thread distinto al que los creo es decir el Main.
		 * @param  {[int]} x [Index del video solicitado.]
		 * @return {[void]}
		 */
        private void funcVidDspatch(int pI_VidIdx, MediaElement mediaelement)
        {
            try
            {


                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(delegate()
                 {
                     
                     funcVideoSelect(pI_VidIdx, mediaelement);

                 }));
            }
            catch (Exception ex)
            { classCon.error1(ex.ToString(), ex.Message.ToString(), ex.Source.ToString()); }
        }

        private void media_element_publicite_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                media_element_publicite.Position = new TimeSpan(0, 0, 1); media_element_publicite.Play();

            }
            catch (Exception m)
            {

                Console.WriteLine(m.ToString());
            }
        }
        private void media_element_stop(MediaElement mediaelement)
        {
            try
            {
                mediaelement.Stop();
            }
            catch(Exception e){
                Console.WriteLine("Elemento de video " + mediaelement.Name + " puede ya estar detenido (EXCEPTION) : " + e.ToString());
            }
            
        }
        private void media_element_play(MediaElement mediaelement)
        {
            try
            {
                mediaelement.Play();
            }
            catch (Exception e)
            {
                Console.WriteLine("Elemento de video "+ mediaelement.Name +  "puede estar reproduciendose ya! :(EXCEPTION) : " + e.ToString());
            }

        }
        private void media_element_publicite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            media_element_stop(media_element_publicite);
            funcVisibility(1);
            
        }

        private void btn_comprar_producto_Click(object sender, RoutedEventArgs e)
        {
            funcVisibility(3);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            funcVisibility(1);
            rdb_product1.IsChecked = false;
            rdb_product2.IsChecked = false;
            rdb_product3.IsChecked = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            funcVisibility(0);
            rdb_product1.IsChecked = false;
            rdb_product2.IsChecked = false;
            rdb_product3.IsChecked = false;
        }

        private void rdb_product1_Checked(object sender, RoutedEventArgs e)
        {
            btn_cancel.Visibility = Visibility.Visible;
            btn_validate.Visibility = Visibility.Visible;
        }

        private void rdb_product2_Checked(object sender, RoutedEventArgs e)
        {
            btn_cancel.Visibility = Visibility.Visible;
            btn_validate.Visibility = Visibility.Visible;
        }

        private void rdb_product3_Checked(object sender, RoutedEventArgs e)
        {
            btn_cancel.Visibility = Visibility.Visible;
            btn_validate.Visibility = Visibility.Visible;
        }

       
        

     
    }
}
