using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Plenario
{

    public class Login
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        public bool RegisterFormAutentification { get; set; }
    }

    public class LoginResponse
    {
        public int statusCode { get; set; }
        public string statusMessage { get; set; }
        public Content content { get; set; }
    }

    public class Content
    {
        public bool Autentificado { get; set; }
        public int CodError { get; set; }
        public object Message { get; set; }
        public int TipoAutentificacion { get; set; }
        public Token Token { get; set; }
        public string UserData { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class Token : IncluirToken
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int TipoUsuario { get; set; }
        public DateTime UltimoAcceso { get; set; }
    }

    public class IncluirToken
    {
        public string Token { get; set; }
    }


    public class Usuario
    {
        public bool Activo { get; set; }
        public string CodigoTarjeta { get; set; }
        public string DominioActiveDirectory { get; set; }
        public string EMail { get; set; }
        public DateTime FechaAdd { get; set; }
        public DateTime FechaMod { get; set; }
        public object Foto { get; set; }
        public int GrabarImagenesLectoraGaleria { get; set; }
        public int Habilidad { get; set; }
        public int Id { get; set; }
        public int Id_RolWeb { get; set; }
        public int Id_Sucursal { get; set; }
        public int Id_TipoLectora { get; set; }
        public object LogoImgBase64 { get; set; }
        public string Nombre { get; set; }
        public string PassWord { get; set; }
        public string Punto_Vta { get; set; }
        public bool UsaActiveDirectory { get; set; }
        public string UserAdd { get; set; }
        public string UserMod { get; set; }
        public string UserName { get; set; }
        public int VecesLoginError { get; set; }
        public int id_sector { get; set; }
    }



}
