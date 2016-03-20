namespace SAV.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aa : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Domicilios",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Provincia = c.String(),
                        Localidad = c.String(),
                        Calle = c.String(),
                        Numero = c.String(),
                        Piso = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ClienteViajes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Pago = c.Boolean(nullable: false),
                        Presente = c.Boolean(nullable: false),
                        Cliente_ID = c.Int(nullable: false),
                        Viaje_ID = c.Int(nullable: false),
                        Parada_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clientes", t => t.Cliente_ID, cascadeDelete: true)
                .ForeignKey("dbo.Viajes", t => t.Viaje_ID, cascadeDelete: true)
                .ForeignKey("dbo.Paradas", t => t.Parada_ID, cascadeDelete: true)
                .Index(t => t.Cliente_ID)
                .Index(t => t.Viaje_ID)
                .Index(t => t.Parada_ID);
            
            CreateTable(
                "dbo.Viajes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Asientos = c.Int(nullable: false),
                        FechaSalida = c.DateTime(nullable: false),
                        FechaArribo = c.DateTime(nullable: false),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Estado = c.Int(nullable: false),
                        Servicio = c.Int(nullable: false),
                        Conductor_ID = c.Int(),
                        Origen_ID = c.Int(),
                        Destino_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Conductors", t => t.Conductor_ID)
                .ForeignKey("dbo.Localidads", t => t.Origen_ID)
                .ForeignKey("dbo.Localidads", t => t.Destino_ID)
                .Index(t => t.Conductor_ID)
                .Index(t => t.Origen_ID)
                .Index(t => t.Destino_ID);
            
            CreateTable(
                "dbo.Comisions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        DNI = c.Long(nullable: false),
                        Telefono = c.String(),
                        Email = c.String(),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Domicilio_ID = c.Int(),
                        Viaje_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Domicilios", t => t.Domicilio_ID)
                .ForeignKey("dbo.Viajes", t => t.Viaje_ID)
                .Index(t => t.Domicilio_ID)
                .Index(t => t.Viaje_ID);
            
            CreateTable(
                "dbo.Gastoes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RazonSocial = c.String(),
                        CUIT = c.Long(nullable: false),
                        NroTicket = c.Long(nullable: false),
                        Hora = c.String(),
                        Monto = c.String(),
                        Viaje_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Viajes", t => t.Viaje_ID)
                .Index(t => t.Viaje_ID);
            
            CreateTable(
                "dbo.Conductors",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        DNI = c.Long(nullable: false),
                        Telefono = c.String(),
                        Email = c.String(),
                        Domicilio_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Domicilios", t => t.Domicilio_ID)
                .Index(t => t.Domicilio_ID);
            
            CreateTable(
                "dbo.Localidads",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Provincia_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Provincias", t => t.Provincia_ID)
                .Index(t => t.Provincia_ID);
            
            CreateTable(
                "dbo.Paradas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        LocalidadParada_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.LocalidadParadas", t => t.LocalidadParada_ID)
                .Index(t => t.LocalidadParada_ID);
            
            CreateTable(
                "dbo.Provincias",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LocalidadParadas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Localidad_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Localidads", t => t.Localidad_ID, cascadeDelete: true)
                .Index(t => t.Localidad_ID);
            
            AddColumn("dbo.Clientes", "DNI", c => c.Long(nullable: false));
            AddColumn("dbo.Clientes", "Telefono", c => c.String(nullable: false));
            AddColumn("dbo.Clientes", "Email", c => c.String());
            AddColumn("dbo.Clientes", "Domicilio_ID", c => c.Int(nullable: false));
            AddColumn("dbo.Clientes", "Apellido", c => c.String(nullable: false));
            AlterColumn("dbo.Clientes", "Nombre", c => c.String(nullable: false));
            AlterColumn("dbo.Clientes", "Apellido", c => c.String(nullable: false));
            AddForeignKey("dbo.Clientes", "Domicilio_ID", "dbo.Domicilios", "ID", cascadeDelete: true);
            CreateIndex("dbo.Clientes", "Domicilio_ID");

        }
        
        public override void Down()
        {
            DropIndex("dbo.LocalidadParadas", new[] { "Localidad_ID" });
            DropIndex("dbo.Paradas", new[] { "LocalidadParada_ID" });
            DropIndex("dbo.Localidads", new[] { "Provincia_ID" });
            DropIndex("dbo.Conductors", new[] { "Domicilio_ID" });
            DropIndex("dbo.Gastoes", new[] { "Viaje_ID" });
            DropIndex("dbo.Comisions", new[] { "Viaje_ID" });
            DropIndex("dbo.Comisions", new[] { "Domicilio_ID" });
            DropIndex("dbo.Viajes", new[] { "Destino_ID" });
            DropIndex("dbo.Viajes", new[] { "Origen_ID" });
            DropIndex("dbo.Viajes", new[] { "Conductor_ID" });
            DropIndex("dbo.ClienteViajes", new[] { "Parada_ID" });
            DropIndex("dbo.ClienteViajes", new[] { "Viaje_ID" });
            DropIndex("dbo.ClienteViajes", new[] { "Cliente_ID" });
            DropIndex("dbo.Clientes", new[] { "Domicilio_ID" });
            DropForeignKey("dbo.LocalidadParadas", "Localidad_ID", "dbo.Localidads");
            DropForeignKey("dbo.Paradas", "LocalidadParada_ID", "dbo.LocalidadParadas");
            DropForeignKey("dbo.Localidads", "Provincia_ID", "dbo.Provincias");
            DropForeignKey("dbo.Conductors", "Domicilio_ID", "dbo.Domicilios");
            DropForeignKey("dbo.Gastoes", "Viaje_ID", "dbo.Viajes");
            DropForeignKey("dbo.Comisions", "Viaje_ID", "dbo.Viajes");
            DropForeignKey("dbo.Comisions", "Domicilio_ID", "dbo.Domicilios");
            DropForeignKey("dbo.Viajes", "Destino_ID", "dbo.Localidads");
            DropForeignKey("dbo.Viajes", "Origen_ID", "dbo.Localidads");
            DropForeignKey("dbo.Viajes", "Conductor_ID", "dbo.Conductors");
            DropForeignKey("dbo.ClienteViajes", "Parada_ID", "dbo.Paradas");
            DropForeignKey("dbo.ClienteViajes", "Viaje_ID", "dbo.Viajes");
            DropForeignKey("dbo.ClienteViajes", "Cliente_ID", "dbo.Clientes");
            DropForeignKey("dbo.Clientes", "Domicilio_ID", "dbo.Domicilios");
            AlterColumn("dbo.Clientes", "Apellido", c => c.String());
            AlterColumn("dbo.Clientes", "Nombre", c => c.String());
            DropColumn("dbo.Clientes", "Domicilio_ID");
            DropColumn("dbo.Clientes", "Email");
            DropColumn("dbo.Clientes", "Telefono");
            DropColumn("dbo.Clientes", "DNI");
            DropTable("dbo.LocalidadParadas");
            DropTable("dbo.Provincias");
            DropTable("dbo.Paradas");
            DropTable("dbo.Localidads");
            DropTable("dbo.Conductors");
            DropTable("dbo.Gastoes");
            DropTable("dbo.Comisions");
            DropTable("dbo.Viajes");
            DropTable("dbo.ClienteViajes");
            DropTable("dbo.Domicilios");
        }
    }
}
