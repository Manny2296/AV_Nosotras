using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AV_Nosotras
{
   public class ClassConsulta
    {
        #region Constructor
        public ClassConsulta(MySqlConnection Conn_Local, MySqlConnection Conn_Rmt)
        {
            this.Conn_Local = Conn_Local;
            this.Conn_Rmt = Conn_Rmt;

        } 
        #endregion
        #region UseClasses
        /**
        * [private static extern long]
        * Implementa el DLL import que controla el mciSendString para playback de sonido en WPF.
        * 'http://msdn.microsoft.com/en-us/library/windows/desktop/dd757161(v=vs.85).aspx'
        * @param  {[string]} x [Comando multimedia de MCI media control interface]
        * @param  {[StringBuilder]} x [Clase mutable que nos almacenara el buffer para calcular la duracion del audio]
        * @param  {[int]} x [Almacenar la duracion del audio despues de la conversion]
        * @param  {[IntPtr]} x [Tipo para representar el pointer o handle, en este caso el handler para notificaciones, no usado aqui pero necesario para la invocacion]
        * @return {['null']}
        */
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLenght, IntPtr hwndCallback);
        #endregion
        #region Variables
        MySqlConnection Conn_Local,Conn_Rmt;
        public string vS_mac; public string vS_PicsPath = @"C:\xampp\htdocs\vtigeridm\"; public string vS_EyePath = @"..\..\..\..\..\data\Det.config"; public string origin;
        #endregion
        #region LogWorkers
        #region Bug And Error Method
        /**
		 * [public void]
		 * Anota los errores en un log.
		 * @param  {[string]} x [Excepcion]
		 * @param  {[string]} x [Mensaje de la excepcion]
		 * @param  {[string]} x [Fuente de la excepcion]
		 * @return {[void]}
		 */
        public void error1(string pS_log, string pS_mssg, string pS_src)
        {
            using (StreamWriter file_log = File.AppendText(@"configsh\log.config"))
            {
                file_log.WriteLine("Exepcion a las >>> " + DateTime.Now.ToString() + Environment.NewLine + Environment.NewLine
                    + pS_log + Environment.NewLine + Environment.NewLine + "Message >>> " + DateTime.Now.ToString()
                    + Environment.NewLine + Environment.NewLine + pS_mssg + Environment.NewLine + Environment.NewLine
                    + "Source >>> " + DateTime.Now.ToString() + Environment.NewLine + Environment.NewLine + pS_src
                    + Environment.NewLine + Environment.NewLine); file_log.Close();
            }
        }
        #endregion
        #region Network Interface To Database Method */
        /**
		 * [public void]
		 * Obtiene las interfaces de red del host para almacenarlas en la base de datos.
		 * @param  {[null]} x [null]
		 * @return {[void]}
		 */
        public void shownetworkinterfaces()
        {
            /* Get Localhost Info */
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] NetI_list = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in NetI_list)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                PhysicalAddress address = adapter.GetPhysicalAddress();
                if (adapter.OperationalStatus.ToString() == "Up")
                {
                    origin = Environment.MachineName.ToString();

                    vS_mac += "{[Nombre NetBIOS: " + Environment.MachineName.ToString() + "] [Nombre: " + adapter.Name.ToString() + "] [MAC: " + address.ToString() + "] [Tipo: " + adapter.NetworkInterfaceType.ToString() + "] [Estado: " + adapter.OperationalStatus.ToString() + "[Dominio de Red: " + Environment.UserDomainName.ToString() + "] [Usuario: " + Environment.UserName.ToString() + "]}" + Environment.NewLine; break;
                }
            }
        }
        #endregion
        #region Logging All Of The Network Interfaces From Host To Log Method */
        /**
		 * [public void]
		 * Obtiene la informacion del host para guardarla en un log de seguridad cada ves que el asistente se ejecuta.
		 * ESTE MODULO NO HA TENIDO UNA USABILIDAD REAL, SE MARCA COMO OBSOLETO 'DEPRECATED' DESDE LA VERSION 1.10.000
		 * @param  {[null]} x [null]
		 * @return {[void]}
		 */
        public void lognetwork()
        {
            /* Get Public Ip */
            //String IPdir = String.Empty;
            string vS_mac_addrs = string.Empty;
            /* WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
			WebResponse response = request.GetResponse();
			StreamReader stream = new StreamReader(response.GetResponseStream());
			IPdir = stream.ReadToEnd(); int vfirst = IPdir.IndexOf("Address: ") + 9; int vlast = IPdir.LastIndexOf("</body>");
			IPdir = IPdir.Substring(vfirst, vlast - vfirst);
			+ " <<< IP Publica >>> " + IPdir */
            /* Get Localhost Info */
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] NetI_list = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in NetI_list)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                PhysicalAddress address = adapter.GetPhysicalAddress();
                vS_mac_addrs += "{ [Nombre: " + adapter.Name.ToString() + "] [MAC: " + address.ToString() + "] [Info: " + adapter.Description.ToString()
                    + "] [Tipo: " + adapter.NetworkInterfaceType.ToString() + "] [Estado: " + adapter.OperationalStatus.ToString() + "] [Velocidad: "
                    + adapter.Speed.ToString() + "] [OS: " + Environment.OSVersion.ToString() + "] [Dominio de Red: " + Environment.UserDomainName.ToString()
                    + "] [Usuario: " + Environment.UserName.ToString() + "] [Nombre NetBIOS: " + Environment.MachineName.ToString()
                    + "] [x64?: " + Environment.Is64BitOperatingSystem.ToString() + "] }" + Environment.NewLine;
            }
            File.WriteAllText(@"configsh\ID.config", "Registro de seguridad.. Acceso:  >>> " + DateTime.Now.ToString() + Environment.NewLine + "Info LocalHost: >>> " + Environment.NewLine + vS_mac_addrs);
        }
        #endregion
        #endregion
        #region MediaWorkers
        #region MediaSelector
        /**
		 * [public static string]
		 * Funcion que selecciona el archivo de audio segun peticion del asistente, retorna el string de la ruta del audio.
		 * @param  {[int]} x [index del archivo de audio a ser cargado y reproducido]
		 * @return {[string]}
		 */
        public static string WavFile(int pI_name)
        {
            string vS_wav_nme = string.Empty;
            switch (pI_name)
            {          
                case 0:vS_wav_nme = @"audio\saludo_dias.wav"; break;
                case 1: vS_wav_nme = @"audio\saludo_tardes.wav"; break;
                case 2: vS_wav_nme = @"audio\saludo_noches.wav"; break;
                case 3: vS_wav_nme = @"audio\dia.wav"; break;
                case 4: vS_wav_nme = @"audio\tarde.wav"; break;
                case 5: vS_wav_nme = @"audio\noche.wav"; break;             
            }
            return vS_wav_nme;
        }
        #endregion
        #region VideoSelector
        /**
		 * [public static string]
		 * Selector de los videos solicitados por el asistente, retorna el string de la ruta del video.
		 * @param  {[int]} x [Index del video a reproducir]
		 * @return {[string]}
		 */
        public static string VidFile(int pI_vid_nme)
        {
            string vS_vid = string.Empty;
            switch (pI_vid_nme)
            {
                case 0:vS_vid = @"videos\Reposo.avi";break;            
                case 1: vS_vid = @"videos\Despedida.avi"; break;       
                case 2: vS_vid = @"videos\Saludo.avi"; break;
                case 3: vS_vid = @"videos\Informacion.avi"; break;
                case 4: vS_vid = @"videos\ReposoP.avi"; break;
                case 5: vS_vid = @"videos\SaludoP.avi"; break;
            }
            return vS_vid;
        }
        #endregion
        #region MediaLenght
        /**
		 * [public static int]
		 * Acepta el index del audio, lo analiza obteniendo su buffer y los convierte en un integer que sera devuelto como la longitud del audio solicitado.
		 * @param  {[int]} x [Index del audio a analizar para obtener la longitud del buffer y calcular su duracion.]
		 * @return {[int]}
		 */
        public static int MLenght(int pS_audio_swch)
        {
            StringBuilder lengthBuf = new StringBuilder(32); int vI_lenght = 0; string vS_filename = WavFile(pS_audio_swch);
            mciSendString(string.Format("open \"{0}\" type waveaudio alias wave", vS_filename), null, 0, IntPtr.Zero);
            mciSendString("status wave length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero); mciSendString("close wave", null, 0, IntPtr.Zero);
            int.TryParse(lengthBuf.ToString(), out vI_lenght); return vI_lenght;
        }
        #endregion
        #endregion
        #region SqlConsultas
        public static string SqlAllProducts()
        {
            string vS_sql_all = "SELECT * FROM av_idm.productos  order by Caracteristica6 asc";
            return vS_sql_all;
        }
        public static string SqlImagePathForEachProduct(int _productcode)
        {
            string vS_sql_imgpath = "SELECT * FROM av_idm.productos WHERE Cod_Producto = '" + _productcode + " '";
            return vS_sql_imgpath;
        }
        public static string SqlLoadContactProspectos()
        {
            //SELECT* FROM av_idm.prospectos
            string vS_sql_prospectos = "SELECT celular,telefono FROM av_idm.prospectos";
            return vS_sql_prospectos;
        }
        public static string SqlinsertInteresxProspecto(string _idprospecto, string _codproducto, string _consolainfo, DateTime fecharegistro)
        {
            string vS_sqlinsert = "INSERT INTO `av_idm`.`interes` (`ID_Prospecto`, `Cod_Producto`, `Consulta`, `Consola_Info`, `Fecha_Registro`) VALUES ('" + _idprospecto + "', '" + _codproducto + "', 'N', '" + _consolainfo + "','" + fecharegistro + "')";
            return vS_sqlinsert;
        }
        public static string SqlLoadProspectoxContact(string contact)
        {
            string vS_sql_loadporcontact = "SELECT * FROM av_idm.prospectos WHERE Celular = '" + contact + "'";
            return vS_sql_loadporcontact;
        }
        public static string SqlInsertLocalProspecto(string _celular, string _name, string _apll,string _cedula, string _correo)
        {
            string vS_sqlinsert = "INSERT INTO `av_idm`.`prospectos` (`Nombre`, `Apellido`, `Cedula`, `Celular`, `Telefono`, `Email`) VALUES ('" + _name + "', '" + _apll + "', '"+_cedula+ "', '" + _celular + "', '', '" + _correo + "')";
            return vS_sqlinsert;
            // INSERT INTO `av_idm`.`prospectos` (`Nombre`, `Apellido`, `Cedula`, `Celular`, `Telefono`, `Email`) VALUES('Miguel Antonio ', 'Caceres', '19992933', '3224565465', '2344556', 'caceresMiguel@gmail.com');
        }
        public static string SqlInsertRemoteProspecto(string _id,string _first, string _last, string _clientid,string _product,string _celular,string _consolainfo,string _fechareg)
        {
            string vS_SqlRmtInsert = "INSERT INTO `inmobiliaria_prueba`.`app_prospectos` (ID, First, Last, ClienteID, Product, Mobil, Consola_Info, Fecha_Registro) VALUES ( '"+_id+ "', '" + _first + "', '" + _last + "', '" + _clientid + "', '" + _product + "', '" + _celular + "', '" + _consolainfo + "', '" + _fechareg + "')";
           // Console.WriteLine("Ins" + vS_SqlRmtInsert);
            return vS_SqlRmtInsert;
        }
        public static string SqlUpdateInteres(string _id)
        {
            string vS_sqlupdate = "UPDATE interes SET Consulta = 'S' WHERE ID_Interes = '"+_id+ "'";
            return vS_sqlupdate;
        }
        public static string SqlSelecJoinProspecto()
        {
            string vS_sqlselect = "SELECT interes.ID_Interes,prospectos.Nombre,prospectos.Apellido,prospectos.ID_Prospecto,productos.Cod_Producto,productos.Tipo_Producto,productos.Caracteristica1,prospectos.Celular,interes.Consola_Info,interes.Fecha_Registro FROM prospectos inner join interes ON prospectos.ID_Prospecto = interes.ID_Prospecto inner join productos on interes.Cod_Producto = productos.Cod_Producto  WHERE interes.Consulta = 'N'";
            return vS_sqlselect;
        }
        public static string SqlSelectTipoInmnueble()
        {
            string vS_sqltim = "SELECT DISTINCT Tipo_Producto FROM av_idm.productos";
            return vS_sqltim;
        }
        public static string SqlSelectNegocio()
        {
            string vS_sqltim = "SELECT DISTINCT Producto_Para FROM av_idm.productos";
            return vS_sqltim;
        }
        public static string SqlSelectHabitaciones()
        {
            string vS_sqltim = "SELECT DISTINCT Caracteristica8 FROM av_idm.productos";
            return vS_sqltim;
        }
        public static string SqlSelectBanhos()
        {
            string vS_sqltim = "SELECT DISTINCT Caracteristica9 FROM av_idm.productos";
            return vS_sqltim;
        }
        public static string SqlSelectBarrios()
        {
            string vS_sqltim = "SELECT DISTINCT Caracteristica1 FROM av_idm.productos";
            return vS_sqltim;
        }
        public static string SqlBusqueda(string _ubicacion, string _tipo, string _negocio , string _habitaciones, string _banhos,string _price)
        {
            //string vs_sql = "SELECT * FROM av_idm.productos WHERE   Producto_Para = 'Venta' AND (Tipo_Producto ='Casa' OR Tipo_Producto ='Apartamento')";
            string vs_sql = "SELECT * FROM av_idm.productos WHERE Estado_Producto = 'Disponible'";


           
              if (_ubicacion != string.Empty && _ubicacion != null )
                {
                    vs_sql += " AND Caracteristica1 = '"+_ubicacion.Trim()+"'";
                }
              if(_tipo != string.Empty && _tipo != null)
                {
                    vs_sql += "  AND Tipo_Producto = '" + _tipo.Trim() + "'";
                }
            if (_negocio != string.Empty && _negocio != null)
            {
                 vs_sql += " AND Producto_Para = '" + _negocio.Trim() + "'";
               // vs_sql += " AND Producto_Para = 'VENTA'";
            }
            if (_habitaciones != string.Empty && _habitaciones != null)
            {
                vs_sql += " AND Caracteristica8 = '" + _habitaciones.Trim() + "'";
            }
            if (_banhos != string.Empty && _banhos != null)
            {
                vs_sql += " AND Caracteristica9 = '" + _banhos.Trim() + "'";
            }
          
            if (_price != string.Empty && _negocio == "Venta")
            {
               // if(_price == "33000000")
               // {
                    vs_sql += " AND Caracteristica6 BETWEEN 5000000  AND "+_price+"  ORDER BY Caracteristica6 ASC";
                //}
                //if (_price == "1366000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 1366000000";
                //}
                //if (_price == "2029000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 2029000000";
                //}
                //if (_price == "2692000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 2692000000";
                //}
                //if (_price == "3355000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 3355000000";
                //}
                //if (_price == "4018000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 4018000000";
                //}
                //if (_price == "4681000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 4681000000";
                //}
                //if (_price == "5344000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 5344000000";
                //}
                //if (_price == "6007000000")
                //{
                //    vs_sql += " AND Caracteristica6 BETWEEN 40000000 AND 6007000000";
                //}

                
            }
            if (_price != string.Empty && _negocio == "Arrendamiento")
            {
                vs_sql += " AND Caracteristica5 BETWEEN 500000  AND " + _price + "  ORDER BY Caracteristica5 ASC";
            }
            Console.WriteLine(vs_sql);
            return vs_sql; 
        }
        #endregion
        #region FuncionConnect
        public void funcConnect(string _name, string _user, string _password)
        {


            try
            {
                String vS_ConnStrLocal = "Server=127.0.0.1;Database=" + _name + ";User id=" + _user + ";Password=" + _password + "; Connection Timeout=7;";
                Conn_Local.ConnectionString = vS_ConnStrLocal;
                String vS_ConnStrRmt = "Server=201.244.110.144;Database=inmobiliaria_prueba;User id=IDM;pwd=Asistente#Virtual@APR2016;Connection Timeout=8; ";
                Conn_Rmt.ConnectionString = vS_ConnStrRmt;

            }
            catch (MySqlException ex)
            {
                error1(ex.ToString(), ex.Message.ToString(), ex.Source.ToString());

                if (Conn_Local.State != System.Data.ConnectionState.Open)
                { Conn_Local.Close(); }
                try
                {
                    if (Conn_Rmt.State != System.Data.ConnectionState.Open)
                    { Conn_Rmt.Close(); Conn_Rmt.ConnectionString = "Server=192.168.2.105;Database=inmobiliaria_aa;User id=root;pwd=Asistente#Virtual@APR2016;Connection Timeout=8;"; Conn_Rmt.Open(); }
                }
                catch (MySqlException)
                {
                    try
                    {
                        if (Conn_Rmt.State != System.Data.ConnectionState.Open)
                        {
                            Conn_Rmt.Close(); Conn_Rmt.ConnectionString = "Server=190.26.245.20;Database=vtigerGM;User id=root;pwd=APR@solution2014;Connection Timeout=8;"; Conn_Rmt.Open();
                        }
                    }
                    catch (MySqlException)
                    {
                        if (Conn_Rmt.State != System.Data.ConnectionState.Open)
                        { Conn_Rmt.Close(); }
                    }
                }
            }

        } 
        #endregion
        #region Open/Close Connection
        public void OpenConnection()
        {
            #region LocalConection

            if (Conn_Local.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    Conn_Local.Open();
               //MessageBox.Show("Conectado satisfactoriamente a BD: " + Conn_Local.Database + Conn_Local.State.ToString());

                }
                catch (Exception e)
                {

                  //  MessageBox.Show("Ya estyo conctado a Local" + Conn_Local.State.ToString() + "Excepcion" + e.ToString());
                }

            }
            #endregion
            #region RemoteConection

            if (Conn_Rmt.State != System.Data.ConnectionState.Open)
            {
                try
                {

                    Conn_Rmt.Open();
                    Console.WriteLine("Conectado satisfactoriamente a BD" + Conn_Rmt.Database + Conn_Rmt.State);
                }
                catch (Exception e)
                {

                    Console.WriteLine("Ya estyo conctado a RMT" + Conn_Local.State.ToString() + "Excepcion" + e.ToString());
                }

            }
            else
            {
                Console.WriteLine("Ya esta abierta");
            }
            #endregion
        }
        public void CloseConnection()
        {
            if (Conn_Local != null && Conn_Local.State == System.Data.ConnectionState.Open)
            {
                Conn_Local.Close();
            }
            if (Conn_Rmt != null && Conn_Rmt.State == System.Data.ConnectionState.Open)
            {
                Conn_Rmt.Close();
            }
        } 
        #endregion
    }
}
