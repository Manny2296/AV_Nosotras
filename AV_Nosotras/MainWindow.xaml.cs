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
using System.Windows.Threading;
using MySql.Data.MySqlClient;

//JULIO INICIO
using System.IO;
using System.IO.Ports;
using System.Threading;
//JULIO FIN

namespace AV_Nosotras
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 
      
    public partial class MainWindow : Window
    {
        //JULIO INICIO
        //SerialPort serialPort1 = new SerialPort();
        SerialPort serialPort2 = new SerialPort();
        //JULIO FIN
        MySqlConnection Conn_Local = new MySqlConnection();
        MySqlConnection Conn_Rmt = new MySqlConnection();
        ClassConsulta classCon;
        DispatcherTimer _timerReset, _timerValidacionCompra, _timer_tiempo_validacion;
        int numero_producto = 0;
        string respuesta_compra;
        public int CANTIDAD_PRODUCTO_1 = 20;
        public int CANTIDAD_PRODUCTO_2 = 20;
        public int CANTIDAD_PRODUCTO_3 = 20;
        public MainWindow()
        {


            /*
            //JULIO INICIO
            ////////JULIO COM ARDUINO PROXIMIDAD
            serialPort1.BaudRate = 9600; // Baudios. Tiene que ser el mismo al que usas de Arduino.
            serialPort1.PortName = "COM1"; // Puerto COM4, en mi caso, el que usa Arduino.
            serialPort1.Parity = Parity.None; // Nada de paridad.
            serialPort1.DataBits = 8; // 8 Bits.
            serialPort1.StopBits = StopBits.One; // Funciona mejor en 2 bits de Stop o parada.
            abrirpuerto1();
            cerrarpuerto1();
            */



            //
            ////////JULIO COM ARDUINO COMPRA
            serialPort2.BaudRate = 9600; // Baudios. Tiene que ser el mismo al que usas de Arduino.
            serialPort2.PortName = "COM2"; // Puerto COM4, en mi caso, el que usa Arduino.
            serialPort2.Parity = Parity.None; // Nada de paridad.
            serialPort2.DataBits = 8; // 8 Bits.
            serialPort2.StopBits = StopBits.One; // Funciona mejor en 2 bits de Stop o parada.
            abrirpuerto2();
            cerrarpuerto2();


            //JULIO FIN

            classCon = new ClassConsulta(Conn_Local, Conn_Rmt);
            InitializeComponent();
            funcVideoSelect(6, media_element_publicite);
            funcVisibility(0);
            _timerReset = new DispatcherTimer();
            _timerReset.Tick += Reset_Tick;
            _timerReset.Interval = new TimeSpan(0, 0, 20);
            _timerValidacionCompra = new DispatcherTimer();
            _timerValidacionCompra.Tick += Compra_Tick;
            _timerValidacionCompra.Interval = new TimeSpan(0, 0, 1);
            _timer_tiempo_validacion = new DispatcherTimer();
            _timer_tiempo_validacion.Tick += Valida_Tick;
            _timer_tiempo_validacion.Interval = new TimeSpan(0, 0, 20);

           
          
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

        public void Reset_Tick(object sender, EventArgs e)
        {
            media_element_stop(media_element_inicio);
            funcVisibility(0);
            _timerReset.Stop();
        }
        public void Valida_Tick(object sender, EventArgs e)
        {
            ClassAutoMBX.Show("Interaccion terminada " + numero_producto + " para terminar", "Compra", 4000);
            _timerValidacionCompra.Stop();
            _timerReset.Stop();
            _timer_tiempo_validacion.Stop();
        }

        public void Compra_Tick(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    //NADA,PRODUCT_1,PRODUCT_2,PRODUCT_3
                    Thread.Sleep(5000);
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
                    {
                        //lbl_identidadcam.Content = _cam.Content;
                        //
                       // respuesta_compra = "PRODUCT_1";
                        respuesta_compra = Compra();

                        //if ((string)lbl_identidadcam.Content == "ALGUIEN" && _valId == 0)
                

                        if (respuesta_compra == "PRODUCT_1")
                        {
                            CANTIDAD_PRODUCTO_1 = CANTIDAD_PRODUCTO_1--;
                             funcVisibility(0);
                             _timer_tiempo_validacion.Stop();
                            _timerReset.Stop();
                            _timerValidacionCompra.Stop();
                           

                         
                        }
                        else if (respuesta_compra == "PRODUCT_2")
                        {
                            CANTIDAD_PRODUCTO_2 = CANTIDAD_PRODUCTO_2--;
                            funcVisibility(0);
                            _timer_tiempo_validacion.Stop();
                            _timerReset.Stop();
                            _timerValidacionCompra.Stop();
                           
                        }
                        else if (respuesta_compra == "PRODUCT_3")
                        {
                            CANTIDAD_PRODUCTO_3 = CANTIDAD_PRODUCTO_3--;
                            
                            funcVisibility(0);
                            _timer_tiempo_validacion.Stop();
                            _timerReset.Stop();
                            _timerValidacionCompra.Stop();
                        }

                       
                    }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION :" + ex.ToString());
                }
            }
        } 
        

        private void funcVisibility(int _idxvis)
        {
            switch (_idxvis)
            {
                //INICIO
                case 0:
                    {
                        media_element_stop(media_element_inicio);
                        GridTab_Publicidad.Visibility = Visibility.Visible;
                        GridTab_menu_opciones.Visibility = Visibility.Visible;
                        GridTab_Menu.Visibility = Visibility.Hidden;
                       
                        GridTab_productos.Visibility = Visibility.Hidden;
                        media_element_play(media_element_publicite);
                        break;
                    }
                    // Menu Opciones
                case 1:
                    {
                        GridTab_Publicidad.Visibility = Visibility.Hidden;
                        GridTab_menu_opciones.Visibility = Visibility.Visible;
                        GridTab_inscription_events.Visibility = Visibility.Hidden;
                        GridTab_Menu.Visibility = Visibility.Visible;
                        GridTab_free_product.Visibility = Visibility.Hidden;
                        GridTab_productos.Visibility = Visibility.Hidden;
                        funcVideoSelect(7, media_element_inicio);
                        label_changeTextFunction(lbl_instrucciones, "Bienvenido, Seleccione una opción \npara continuar :");
                        break;
                    }

                    // Menu Comprar producti
                case 3:
                    {
                        GridTab_menu_opciones.Visibility = Visibility.Hidden;
                        GridTab_productos.Visibility = Visibility.Visible;
                        rdb_producto1.IsChecked = false;
                        rdb_producto2.IsChecked = false;
                        rdb_producto3.IsChecked = false;
                        btn_cancel.Visibility = Visibility.Hidden;
                        btn_validate.Visibility = Visibility.Hidden;
                    }break;

                    // Menu Inscripcion
                case 4:
                    {
                        GridTab_menu_opciones.Visibility = Visibility.Hidden;
                        web_element_inscription.Source = new Uri("https://www.nosotrasonline.com.co/Colombia/Registro/Registro-Colombia/");
                        GridTab_inscription_events.Visibility = Visibility.Visible;

                    }
                    break;
                // Menu Free Product
                case 5:
                    {
                          GridTab_menu_opciones.Visibility = Visibility.Hidden;
                          GridTab_free_product.Visibility = Visibility.Visible;
                    }
                    break;
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
            _timerReset.Start();
            funcVisibility(1);
            media_element_stop(media_element_publicite);
            

            
        }

        private void btn_comprar_producto_Click(object sender, RoutedEventArgs e)
        {
            _timerReset.Stop();
          
            lbl_cantidad_product1.Content = "Cantidad :" + CANTIDAD_PRODUCTO_1;
            lbl_cantidad_product2.Content = "Cantidad :" + CANTIDAD_PRODUCTO_2;
            lbl_cantidad_product3.Content = "Cantidad :" + CANTIDAD_PRODUCTO_3;
            funcVisibility(3);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _timerReset.Start();
            
            funcVisibility(1);
        
            
        }

        private void Button_validerclick(object sender, RoutedEventArgs e)
        {
           // _timerValidacionCompra.Start();
          //  _timer_tiempo_validacion.Start();
           // _timerValidacionCompra.Start();
            _timerReset.Stop();
            ClassAutoMBX.Show("Inserte la moneda en la perilla " + numero_producto + " para terminar", "Compra", 4000);
            funcVisibility(0);
            rdb_producto1.IsChecked = false;
            rdb_producto2.IsChecked = false;
            rdb_producto3.IsChecked = false;
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

        private void btn_inscripcionEventos_Click(object sender, RoutedEventArgs e)
        {
            _timerReset.Stop();
            funcVisibility(4);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _timerReset.Start();
            funcVisibility(1);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            _timerReset.Stop();
            funcVisibility(0);
        }

        private void media_element_inicio_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                media_element_inicio.Position = new TimeSpan(0, 0, 1); media_element_inicio.Play();

            }
            catch (Exception m)
            {

                Console.WriteLine(m.ToString());
            }

        }

       

        private void btn_valider_regalo_Click(object sender, RoutedEventArgs e)
        {
            _timerReset.Stop();
            funcVisibility(0);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            _timerReset.Start();
            funcVisibility(1);
        }

        private void btn_free_product_Click(object sender, RoutedEventArgs e)
        {
            _timerReset.Stop();
            funcVisibility(5);
        }






        //JULIO INICIO
        #region Arduino





/*
        //FUNCION DETECTSENSE RESPONDE 
        //ALGUIEN,NADIE
        public string DetectSense()
        {


            abrirpuerto1();  //JULIO

            string line3 = string.Empty;
            line3 = escripuerto1("proxi");

            cerrarpuerto1(); //JULIO

            return line3.Trim();

        }
        private void abrirpuerto1()
        {
            // Abrir puerto mientras se ejecuta esta aplicación.
            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Open();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void cerrarpuerto1()
        {

            try
            {
                serialPort1.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private string escripuerto1(String s)
        {

            abrirpuerto1();

            byte[] mBuffer = Encoding.ASCII.GetBytes(s);
            serialPort1.Write(mBuffer, 0, mBuffer.Length);
            serialPort1.DiscardInBuffer();
            string line = string.Empty;
            line = serialPort1.ReadLine();

            cerrarpuerto1();

            return line;


        }


*/




        //FUNCION COMPRA RESPONDE 
        //NADA,PRODUCT_1,PRODUCT_2,PRODUCT_3
        public string Compra()
        {


            abrirpuerto2();  //JULIO

            string line3 = string.Empty;
            line3 = escripuerto2("compra");

            cerrarpuerto2(); //JULIO

            return line3.Trim();

        }
        private void abrirpuerto2()
        {
            // Abrir puerto mientras se ejecuta esta aplicación.
            if (!serialPort2.IsOpen)
            {
                try
                {
                    serialPort2.Open();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void cerrarpuerto2()
        {

            try
            {
                serialPort2.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private string escripuerto2(String s)
        {

            abrirpuerto2();

            byte[] mBuffer = Encoding.ASCII.GetBytes(s);
            serialPort2.Write(mBuffer, 0, mBuffer.Length);
            serialPort2.DiscardInBuffer();
            string line = string.Empty;
            line = serialPort2.ReadLine();

            cerrarpuerto2();

            return line;


        }


        #endregion

        private void rdb_producto1_Checked(object sender, RoutedEventArgs e)
        {
            numero_producto = 1;
            btn_validate.Visibility = Visibility.Visible;
            btn_cancel.Visibility = Visibility.Visible;
            

        }

        private void rdb_producto2_Checked(object sender, RoutedEventArgs e)
        {
            numero_producto = 2;
            btn_validate.Visibility = Visibility.Visible;
            btn_cancel.Visibility = Visibility.Visible;
        }

        private void rdb_producto2_Copy_Checked(object sender, RoutedEventArgs e)
        {
            numero_producto = 3;
            btn_validate.Visibility = Visibility.Visible;
            btn_cancel.Visibility = Visibility.Visible;
        }

        private void btn_product1_Click(object sender, RoutedEventArgs e)
        {
            rdb_producto1.IsChecked = true;
        }

        private void btn_product2_Click(object sender, RoutedEventArgs e)
        {
            rdb_producto2.IsChecked = true;
        }

        private void btn_product3_Click(object sender, RoutedEventArgs e)
        {
            rdb_producto3.IsChecked = true;
        }



        //JULIO FIN



    }
}
