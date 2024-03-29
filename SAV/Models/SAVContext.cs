﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace SAV.Models
{
    public class SAVContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<SAV.Models.SAVContext>());

        public SAVContext() : base("name=SAVContext")
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Viaje> Viajes { get; set; }

        public DbSet<ClienteViaje> ClienteViajes { get; set; }

        public DbSet<Conductor> Conductores { get; set; }

        public DbSet<Comision> Comisiones { get; set; }
        
        public DbSet<Domicilio> Domicilios { get; set; }

        public DbSet<Gasto> Gastos { get; set; }

        public DbSet<Localidad> Localidades { get; set; }

        public DbSet<Provincia> Provincias { get; set; }

        public DbSet<Parada> Paradas { get; set; }

        public DbSet<ComisionResponsable> ComisionResponsable { get; set; }

        public DbSet<CuentaCorriente> CuentaCorriente { get; set; }

        public DbSet<Pago> Pagos { get; set; }

        public DbSet<Nacionalidad> Nacionalidad { get; set; }

        public DbSet<DatosEmpresa> DatosEmpresa { get; set; }

        public DbSet<TipoGasto> TipoGasto { get; set; }

        public DbSet<FormaPago> FormaPago { get; set; }

        public DbSet<RegistroViaje> RegistroViaje { get; set; }

        public DbSet<ModalidadPrestacion> ModalidadPrestacion { get; set; }

        public DbSet<TipoAdicionalConductor> TipoAdicionalConductor { get; set; }

        public DbSet<AdicionalConductor> AdicionalConductor { get; set; }

        public DbSet<Configuracion> Configuracion { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
