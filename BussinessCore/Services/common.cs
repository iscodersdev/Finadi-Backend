using System;  
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DAL.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Net.Mail;
using Excel.FinancialFunctions;
using System.Globalization;
using Humanizer;

namespace FINADICore
{
    public class common
    {
        public static int DiasDelMes(int mes)
        {
            switch (mes)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    return 28;
                default:
                    return 30;
            }
        }
        public class InMemoryFile
        {
            public string FileName { get; set; }
            public byte[] Content { get; set; }
        }
        public static int NiumeroRandom(int min, int max)
        {
            var rand = new Random();
            return rand.Next(min, max);
        }

        public static string NumeroALetrasNew(decimal value)
        {
            var culturaArgentina = CultureInfo.GetCultureInfo("es-AR");
            var completa = value.ToString(culturaArgentina);
            string[] sides = completa.Split(",");
            if (completa.Contains(",") && !sides[1].Contains("00"))
            {
                return Convert.ToInt32(sides[0]).ToWords(culturaArgentina) + " con " + Convert.ToInt32(sides[1]).ToWords(culturaArgentina);
            }
            else
            {
                return Convert.ToInt32(sides[0]).ToWords(culturaArgentina);
            }
        }
        public static string NumeroALetras(double value)
        {
            string num2Text; value = Math.Truncate(value);
            if (value == 0) num2Text = "CERO";
            else if (value == 1) num2Text = "UNO";
            else if (value == 2) num2Text = "DOS";
            else if (value == 3) num2Text = "TRES";
            else if (value == 4) num2Text = "CUATRO";
            else if (value == 5) num2Text = "CINCO";
            else if (value == 6) num2Text = "SEIS";
            else if (value == 7) num2Text = "SIETE";
            else if (value == 8) num2Text = "OCHO";
            else if (value == 9) num2Text = "NUEVE";
            else if (value == 10) num2Text = "DIEZ";
            else if (value == 11) num2Text = "ONCE";
            else if (value == 12) num2Text = "DOCE";
            else if (value == 13) num2Text = "TRECE";
            else if (value == 14) num2Text = "CATORCE";
            else if (value == 15) num2Text = "QUINCE";
            else if (value < 20) num2Text = "DIECI" + NumeroALetras(value - 10);
            else if (value == 20) num2Text = "VEINTE";
            else if (value < 30) num2Text = "VEINTI" + NumeroALetras(value - 20);
            else if (value == 30) num2Text = "TREINTA";
            else if (value == 40) num2Text = "CUARENTA";
            else if (value == 50) num2Text = "CINCUENTA";
            else if (value == 60) num2Text = "SESENTA";
            else if (value == 70) num2Text = "SETENTA";
            else if (value == 80) num2Text = "OCHENTA";
            else if (value == 90) num2Text = "NOVENTA";
            else if (value < 100) num2Text = NumeroALetras(Math.Truncate(value / 10) * 10) + " Y " + NumeroALetras(value % 10);
            else if (value == 100) num2Text = "CIEN";
            else if (value < 200) num2Text = "CIENTO " + NumeroALetras(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) num2Text = NumeroALetras(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) num2Text = "QUINIENTOS";
            else if (value == 700) num2Text = "SETECIENTOS";
            else if (value == 900) num2Text = "NOVECIENTOS";
            else if (value < 1000) num2Text = NumeroALetras(Math.Truncate(value / 100) * 100) + " " + NumeroALetras(value % 100);
            else if (value == 1000) num2Text = "MIL";
            else if (value < 2000) num2Text = "MIL " + NumeroALetras(value % 1000);
            else if (value < 1000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value % 1000);
                }
            }
            else if (value == 1000000)
            {
                num2Text = "UN MILLON";
            }
            else if (value < 2000000)
            {
                num2Text = "UN MILLON " + NumeroALetras(value % 1000000);
            }
            else if (value < 1000000000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000) * 1000000);
                }
            }
            else if (value == 1000000000000) num2Text = "UN BILLON";
            else if (value < 2000000000000) num2Text = "UN BILLON " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }
            }
            return num2Text;
        }
        public static byte[] GetZipArchive(List<InMemoryFile> files)
        {
            byte[] archiveFile;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(file.Content, 0, file.Content.Length);
                    }
                }
                archiveFile = archiveStream.ToArray();
            }
            return archiveFile;
        }
        public static string padl0(string texto, int longitud)
        {
            string convertir = "0000000000000000" + texto.Trim();
            return convertir.Substring(convertir.Length - longitud);
        }
        public static string padl0d(decimal texto, int longitud, int decimales)
        {
            string prefijo = "";
            if (texto < 0)
            {
                prefijo = "-";
            }
            string parteentera = texto.ToString("F2").Split(".")[0];
            string partedecimal = "00";
            if (texto.ToString("F2").Split(".").Length>1)
            {
                partedecimal = texto.ToString("F2").Split(".")[1];
            }
            string numero = "0000000000000000000" + prefijo + parteentera + partedecimal;
            return padr(numero, longitud + decimales);
        }
        public static string padr(string texto, int longitud)
        {
            string convertir = texto.Trim() + "                                                                  ";
            return convertir.Substring(0, longitud);
        }
        public class MailAPI
        {
            public string Mail { get; set; }
            public string Html { get; set; }
            public string Titulo { get; set; }
            public string Token { get; set; }
            public string Empresa { get; set; }
            public int Status { get; set; }
            public string Mensaje { get; set; }
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }
        public static Image ByteaImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] imageaByte(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public static bool EnviarMail(string destinatario, string titulo, string texto, string cliente, string imagen = null, byte[] Adjunto = null, string NombreArchivo = null)
        {
            MailAPI mail = new MailAPI();
            mail.Mail = destinatario;
            mail.Html = texto;
            mail.Titulo = titulo;
            DateTime oFec = DateTime.Now;
            var code = Encrypt(mail.Titulo + mail.Html, "SendMail"); ;
            mail.Token =code;
//            var resultado=  EnviarMailGmail(mail);
            var resultado = EnviarMailSendinBlue(mail, imagen);
            return true;
        }

        public static decimal CalculaCFT(double capital,int cantidadcuotas,double montocuota)
        {
            try
            {
                List<double> valores = new List<double>();
                valores.Add(capital * -1);
                for (int x = 0; x < cantidadcuotas; x++)
                {
                    valores.Add(montocuota);
                }
                double tir = Financial.Irr(valores);
                decimal cft = Math.Round(Convert.ToDecimal(tir * 12 * 100), 2);
                return cft;
            }
            catch
            {
                return 0;
            }
        }
        public static bool EnviarMailSendinBlue(MailAPI mail, string imagen = null)
        {
            if (mail.Mail == null)
            {
                return false;
            }
            try
            {
                string usuario = "albarracin_sergio@hotmail.com";
                string password = "w2cPVg3n9Xq6C7KO";
                //string usuario = "novedades@ampromm.org.ar";
                //string password = "BWSNmr7qGLdHYKz2";
                var origen = new MailAddress("noresponder@FINADI.org.ar", "FINADI");
                string host = "smtp-relay.sendinblue.com";
                int puerto = 587;
                bool ssl = true;
                NetworkCredential credenciales = new NetworkCredential(usuario, password);
                MailMessage correo = new MailMessage("noresponder@FINADI.org.ar", mail.Mail, mail.Titulo, cuerpoHTMLGmail(mail.Titulo, mail.Html, "", imagen));
                correo.From = origen;
                correo.IsBodyHtml = true;
                SmtpClient servicio = new SmtpClient(host, puerto);
                servicio.UseDefaultCredentials = true;
                servicio.Credentials = credenciales;
                servicio.EnableSsl = ssl;
                string token="";
                servicio.SendAsync(correo,token);
            }
            catch
            {
                return false;
            }
            return true;

        }
        public static bool EnviarMailGmail(MailAPI mail)
        {
            if (mail.Mail == null)
            {
                return false;
            }
            try
            {
                string usuario = "novedades@FINADI.org.ar";
                string password = "Nov2021**";
                var origen = new MailAddress("noresponder@FINADI.org.ar", "FINADI - " + mail.Empresa);
                string host = "c2310171.ferozo.com";
                int puerto = 465;
                bool ssl = true;
                NetworkCredential credenciales = new NetworkCredential(usuario, password);
                MailMessage correo = new MailMessage("noresponder@FINADI.org.ar", mail.Mail, mail.Titulo, cuerpoHTMLGmail(mail.Titulo, mail.Html, ""));
                correo.From = origen;
                correo.IsBodyHtml = true;
                SmtpClient servicio = new SmtpClient(host, puerto);
                servicio.UseDefaultCredentials = true;
                servicio.Credentials = credenciales;
                servicio.EnableSsl = ssl;
                servicio.Send(correo);
            }
            catch
            {
                return false;
            }
            return true;

        }
        public static string cuerpoHTMLGmail(string titulo, string texto, string cliente, string imagen = "")
        {
            if (imagen == "")
            {
                imagen = "<img src =http://web.finadi.com.ar/images/Logoemail.png><br/>";
            }

            string sHTML = "";
            sHTML += "<html>";
            sHTML += "<meta charset='UTF-8'>";
            sHTML += "<body>";
            sHTML += imagen;
            sHTML += "<h3>";
            sHTML += titulo;
            sHTML += "</h3><br><br>";
            sHTML += "<div id='Texto' style='font-size:12px; margin-bottom:20px; '>";
            sHTML += texto;
            sHTML += "</div>";
            sHTML += "<div id='footer' style='font-size:12px; text-align:left;'>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>(2023) FINADI</b></p>";
            sHTML += "<img src=http://web.finadi.com.ar/images/footer_mail.png><br/>";
            sHTML += "</div>";
            sHTML += "</body>";
            sHTML += "</html>";
            return sHTML;
        }
        //public static bool ValidaPermisos(Controller este,FINADIContext Context,int OpcionMenu)
        //{
        //    if (este.HttpContext.Session.GetString("PersonaNumeroDocumento") == null)
        //    {
        //        return false;
        //    }
        //    var permiso = Context.PerfilesPersonas.FirstOrDefault(x=>x.Persona.Paciente.Persona.NumeroDocumento==Convert.ToInt32(este.HttpContext.Session.GetString("PersonaNumeroDocumento")) && x.Perfil.Sistema.Id==1);
        //    if (permiso==null)
        //    {
        //        return false;
        //    }
        //    var opcion = Context.PerfilesOpcionesMenu.FirstOrDefault(x => x.Perfil.Id == permiso.Perfil.Id && x.OpcionMenu.Id == OpcionMenu);
        //    if (opcion==null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        public static bool ejecutaodbc(string strSQL, System.Data.OleDb.OleDbConnection dbf)
        {
            System.Data.OleDb.OleDbCommand comando = new System.Data.OleDb.OleDbCommand(strSQL);
            comando.Connection = dbf;
            comando.CommandTimeout = 0;
            comando.ExecuteNonQuery();
            return true;
        }
        public static DataTable leeodbc(string strSQL, System.Data.OleDb.OleDbConnection dbf)
        {
            DataSet ds = new DataSet();
            System.Data.OleDb.OleDbDataAdapter dataAdapter = new System.Data.OleDb.OleDbDataAdapter(strSQL,dbf);
            dataAdapter.Fill(ds);
            return ds.Tables[0];
        }

        public static System.Data.OleDb.OleDbConnection abreodbc(string carpeta)
        {
            string coneccion = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + carpeta + ";Extended Properties=dBASE IV;";
            //            string coneccion = "Provider=VFPOLEDB.1;Data Source=" + carpeta + ";Collating Sequence=general;";
            System.Data.OleDb.OleDbConnection dbf = new System.Data.OleDb.OleDbConnection(coneccion);
            dbf.Open();
            return dbf;
        }
        public static bool cierraodbc (System.Data.OleDb.OleDbConnection dbf)
        {
            dbf.Close();
            dbf.Dispose();
            return true;
        }
        public static string Encrypt(string toEncrypt, string secretKey)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            var md5Serv = System.Security.Cryptography.MD5.Create();
            keyArray = md5Serv.ComputeHash(UTF8Encoding.UTF8.GetBytes(secretKey));
            md5Serv.Dispose();
            var tdes = System.Security.Cryptography.TripleDES.Create();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            tdes.Dispose();
            return sin_simbolos(Convert.ToBase64String(resultArray, 0, resultArray.Length));
        }
        public static string sin_simbolos(String cadena)
        {
            try
            {
                return Regex.Replace(cadena, @"[^a-zA-Z0-9 ñÑ]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
        public static string cuerpoHTML(string titulo, string texto, string cliente)
        {
            string sHTML = "";
            sHTML += "<html>";
            sHTML += "<meta charset='UTF-8'>";
            sHTML += "<body>";
            sHTML += "<img src=https://www.cge.mil.ar/cgewebsite/images/bannercge.png><br/>";
            sHTML += "<div id='TituloNombre' style='font-size: 14px; color: #14456b; font-weight: bold; border-style: solid; border-top: 0px; border-left: 0px; border-right: 0px; margin-bottom: 10px;'>";
            sHTML += titulo;
            sHTML += "</div>";
            sHTML += "<div id='cuerpoMail' style='font-size:12px; margin-bottom:20px; color:#14456b;'>";
            sHTML += texto;
            sHTML += "</div>";
            sHTML += "<div id='footerMail' style='font-size:10px; text-align:left;'>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>Contaduria General del Ejército.</b></p>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>Somos el Ejército.</b></p>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><a href='https://www.cge.mil.ar'></a></p>";
            sHTML += "</div>";
            sHTML += "</body>";
            sHTML += "</html>";
            return sHTML;
        }

        public static byte[] ConvertFromBase64String(string input)
        {
            if (String.IsNullOrWhiteSpace(input)) return null;
            try
            {
                string working = input.Replace('-', '+').Replace('_', '/'); ;
                while (working.Length % 4 != 0)
                {
                    working += '=';
                }
                return Convert.FromBase64String(working);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static byte[] Reporting(string reporte, string parametros, string formato, FINADIContext Context)
        {
            var cabecera = Context.DatosEstructura.FirstOrDefault();
            System.Net.NetworkCredential credencial = new System.Net.NetworkCredential(cabecera.UsuarioReportes,cabecera.CredencialReportes);
            WebClient client = new WebClient();
            client.Credentials = credencial;
            string reportURL = cabecera.URLReportes  + reporte + "&rs:Command=Render" + parametros + "&rs:Format=" + formato;
            return client.DownloadData(reportURL);
        }
    }

    public class BaseDataAccess
    {
        protected string ConnectionString { get; set; }

        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }

        protected DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = new SqlCommand(commandText, connection as SqlConnection);
            command.CommandType = commandType;
            return command;
        }

        public Array GetParameter(string parameter1 = null, object value1 = null, string parameter2 = null, object value2 = null, string parameter3 = null, object value3 = null, string parameter4 = null, object value4 = null, string parameter5 = null, object value5 = null, string parameter6 = null, object value6 = null)
        {
            if (parameter1 == null)
                return null;
            else if (parameter2 == null)
            {
                SqlParameter[] parametros = new SqlParameter[1];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                return parametros;
            }
            else if (parameter3 == null)
            {
                SqlParameter[] parametros = new SqlParameter[2];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                return parametros;
            }
            else if (parameter4 == null)
            {
                SqlParameter[] parametros = new SqlParameter[3];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                return parametros;
            }
            else if (parameter5 == null)
            {
                SqlParameter[] parametros = new SqlParameter[4];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                SqlParameter parametro4 = new SqlParameter(parameter4, value4 != null ? value4 : DBNull.Value);
                parametro4.Direction = ParameterDirection.Input;
                parametros[3] = parametro4;
                return parametros;
            }
            else if (parameter6 == null)
            {
                SqlParameter[] parametros = new SqlParameter[5];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                SqlParameter parametro4 = new SqlParameter(parameter4, value4 != null ? value4 : DBNull.Value);
                parametro4.Direction = ParameterDirection.Input;
                parametros[3] = parametro4;
                SqlParameter parametro5 = new SqlParameter(parameter5, value5 != null ? value5 : DBNull.Value);
                parametro5.Direction = ParameterDirection.Input;
                parametros[4] = parametro5;
                return parametros;
            }
            else
            {
                SqlParameter[] parametros = new SqlParameter[6];
                SqlParameter parametro1 = new SqlParameter(parameter1, value1 != null ? value1 : DBNull.Value);
                parametro1.Direction = ParameterDirection.Input;
                parametros[0] = parametro1;
                SqlParameter parametro2 = new SqlParameter(parameter2, value2 != null ? value2 : DBNull.Value);
                parametro2.Direction = ParameterDirection.Input;
                parametros[1] = parametro2;
                SqlParameter parametro3 = new SqlParameter(parameter3, value3 != null ? value3 : DBNull.Value);
                parametro3.Direction = ParameterDirection.Input;
                parametros[2] = parametro3;
                SqlParameter parametro4 = new SqlParameter(parameter4, value4 != null ? value4 : DBNull.Value);
                parametro4.Direction = ParameterDirection.Input;
                parametros[3] = parametro4;
                SqlParameter parametro5 = new SqlParameter(parameter5, value5 != null ? value5 : DBNull.Value);
                parametro5.Direction = ParameterDirection.Input;
                parametros[4] = parametro5;
                SqlParameter parametro6 = new SqlParameter(parameter6, value5 != null ? value6 : DBNull.Value);
                parametro6.Direction = ParameterDirection.Input;
                parametros[5] = parametro6;
                return parametros;
            }
        }
        protected SqlParameter GetParameterOut(string parameter, SqlDbType type, object value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, type); ;

            if (type == SqlDbType.NVarChar || type == SqlDbType.VarChar || type == SqlDbType.NText || type == SqlDbType.Text)
            {
                parameterObject.Size = -1;
            }

            parameterObject.Direction = parameterDirection;

            if (value != null)
            {
                parameterObject.Value = value;
            }
            else
            {
                parameterObject.Value = DBNull.Value;
            }

            return parameterObject;
        }


        protected object ExecuteScalar(string procedureName, List<SqlParameter> parameters)
        {
            object returnValue = null;

            try
            {
                using (DbConnection connection = this.GetConnection())
                {
                    DbCommand cmd = this.GetCommand(connection, procedureName, CommandType.StoredProcedure);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                //LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
                throw;
            }

            return returnValue;
        }

        public DataTable GetDataTable(string procedureName, Array parameters)
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection("Server=rey;Database=haberes;user=sa;password=Cofranjalud1406;MultipleActiveResultSets=true;Connection Timeout=30");
            {
                connection.Open();
                SqlDataAdapter sqa = new SqlDataAdapter(procedureName, connection);
                if (parameters != null)
                {
                    sqa.SelectCommand.Parameters.AddRange(parameters);
                }
                sqa.Fill(ds);
                try
                {
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return ds.Tables[0];
        }
        public int ExecuteNonQuery(string procedureName, Array parameters)
        {
            int returnValue = -1;
            SqlConnection connection = new SqlConnection("Server=rey;Database=haberes;user=sa;password=Cofranjalud1406;MultipleActiveResultSets=true;Connection Timeout=0");
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procedureName, connection);
                command.CommandTimeout = 0;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                returnValue = command.ExecuteNonQuery();
                try
                {
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return returnValue;
        }

    }
}