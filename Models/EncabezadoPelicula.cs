//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaPOS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EncabezadoPelicula
    {
        public EncabezadoPelicula()
        {
            this.DetallePelicula = new HashSet<DetallePelicula>();
            this.Elenco = new HashSet<Elenco>();
            this.GeneroPelicula = new HashSet<GeneroPelicula>();
            this.ImpuestoPelicula = new HashSet<ImpuestoPelicula>();
            this.MedioPelicula = new HashSet<MedioPelicula>();
            this.PorcentajeParticipacion = new HashSet<PorcentajeParticipacion>();
            this.TeatroPelicula = new HashSet<TeatroPelicula>();
        }
    
        public int RowID { get; set; }
        public Nullable<int> DistribuidorID { get; set; }
        public Nullable<int> PaisID { get; set; }
        public Nullable<int> TipoClasificacionID { get; set; }
        public Nullable<int> TipoIdiomaOriginalID { get; set; }
        public string TituloLocal { get; set; }
        public string TituloOriginal { get; set; }
        public string NumeroActa { get; set; }
        public Nullable<int> Duracion { get; set; }
        public string Sinopsis { get; set; }
        public string WebOficial { get; set; }
        public Nullable<System.DateTime> FechaEstreno { get; set; }
        public Nullable<bool> DerechoCorto { get; set; }
        public Nullable<int> EstadoID { get; set; }
        public string CreadoPor { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<int> RowIDCentral { get; set; }
        public string Afiche { get; set; }
        public string Thumbnail { get; set; }
        public string Trailer { get; set; }
        public string Teaser { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
    
        public virtual Estado Estado { get; set; }
        public virtual Opcion Opcion { get; set; }
        public virtual Opcion Opcion1 { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Tercero Tercero { get; set; }
        public virtual ICollection<DetallePelicula> DetallePelicula { get; set; }
        public virtual ICollection<Elenco> Elenco { get; set; }
        public virtual ICollection<GeneroPelicula> GeneroPelicula { get; set; }
        public virtual ICollection<ImpuestoPelicula> ImpuestoPelicula { get; set; }
        public virtual ICollection<MedioPelicula> MedioPelicula { get; set; }
        public virtual ICollection<PorcentajeParticipacion> PorcentajeParticipacion { get; set; }
        public virtual ICollection<TeatroPelicula> TeatroPelicula { get; set; }
    }
}
