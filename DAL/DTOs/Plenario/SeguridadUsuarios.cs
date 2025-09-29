using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.Plenario
{
    public class SeguridadUsuarios
    {
        public int Id { get; set; }
        public int Id_Sucursal { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string EMail { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaAdd { get; set; }
        public DateTime? FechaMod { get; set; }
        public string UserAdd { get; set; }
        public string UserMod { get; set; }
        public int id_sector { get; set; }
        public int Id_TipoLectora { get; set; }
        public int GrabarImagenesLectoraGaleria { get; set; }
        public string Punto_Vta { get; set; }
        public int Habilidad { get; set; }
        public byte[] Foto { get; set; }
        public string CodigoTarjeta { get; set; }
        public bool UsaActiveDirectory { get; set; }
        public string DominioActiveDirectory { get; set; }
        public int VecesLoginError { get; set; }
        public int Id_RolWeb { get; set; }
        public string Name { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
    }

}
