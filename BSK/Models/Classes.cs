using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BSK.Models
{


    [Table("Autorzy", Schema = "public")]
    public class Autor
    {
        [Key]
        [Column("ID_Autora")]
        public int ID_Autora { get; set; }

        [Column("Imie")]
        public string Imie { get; set; }

        [Column("Nazwisko")]
        public string Nazwisko { get; set; }

        public virtual List<Ksiazka> Ksiazki { get; set; }
    }

    [Table("Kategorie", Schema = "public")]
    public class Kategoria
    {
        [Key]
        [Column("ID_Kategorii")]
        public int ID_Kategorii { get; set; }

        [Column("Nazwa")]
        public string Nazwa { get; set; }

        [Column("Opis")]
        public string Opis { get; set; }

        public virtual List<Ksiazka> Ksiazki { get; set; }
    }

    [Table("Ksiazki", Schema = "public")]
    public class Ksiazka
    {
        [Key]
        [Column("ID_Ksiazki")]
        public int ID_Ksiazki { get; set; }

        [Column("Tytul")]
        public string Tytul { get; set; }

        [Column("Liczba_dostepnych")]
        public int Liczba_dostepnych { get; set; }

        [Column("ISBN")]
        public string ISBN { get; set; }

        [ForeignKey("Autor")]
        [Column("ID_Autora")]
        public int ID_Autora { get; set; }
        public virtual Autor Autor { get; set; }

        [ForeignKey("Kategoria")]
        [Column("ID_Kategorii")]
        public int ID_Kategorii { get; set; }
        public virtual Kategoria Kategoria { get; set; }
    }

    [Table("Uprawnienia", Schema = "public")]
    public class Uprawnienie
    {
        [Key]
        [Column("ID_Uprawnienia")]
        public int ID_Uprawnienia { get; set; }

        [Column("Instrukcja")]
        public string Instrukcja { get; set; }

        [Column("Nazwa_tabeli")]
        public string Nazwa_tabeli { get; set; }

        public virtual List<Uprawnienie_Rola> Uprawnienie_Rola { get; set; }
    }

    [Table("Rolee", Schema = "public")]
    public class Rola
    {
        [Key]
        [Column("ID_Roli")]
        public int ID_Roli { get; set; }

        [Column("Nazwa")]
        public string Nazwa { get; set; }

        public virtual List<Uprawnienie_Rola> Uprawnienie_Rola { get; set; }
        public virtual List<Uzytkownik_Rola> Uzytkownik_Rola { get; set; }
    }

    [Table("Uzytkownicy", Schema = "public")]
    public class Uzytkownik
    {
        [Key]
        [Column("ID_Uzytkownika")]
        public int ID_Uzytkownika { get; set; }

        [Column("Login")]
        public string Login { get; set; }

        [Column("Nazwa")]
        public string Nazwa { get; set; }

        [Column("Haslo")]
        public string Haslo { get; set; }

        public virtual List<Uzytkownik_Rola> Uzytkownik_Rola { get; set; }

        public static string sha256(string password)   
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

    }

    [Table("Sesje", Schema = "public")]
    public class Sesja
    {
        [Key]
        [Column("ID_Sesji")]
        public string ID_Sesji { get; set; }

        [ForeignKey("Uzytkownik")]
        [Column("ID_Uzytkownika")]
        public int ID_Uzytkownika { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }

        [ForeignKey("Rola")]
        [Column("ID_Roli")]
        public int ID_Roli { get; set; }
        public virtual Rola Rola { get; set; }

        [Column("Data_waznosci")]
        public long Data_waznosci { get; set; }
    }

    [Table("Uzytkownicy_Role", Schema = "public")]
    public class Uzytkownik_Rola
    {
        [Key, Column("ID_Uzytkownika", Order = 1)]
        public int ID_Uzytkownika { get; set; }

        [Key, Column("ID_Roli", Order = 2)]
        public int ID_Roli { get; set; }

        public Uzytkownik Uzytkownik { get; set; }
        public Rola Rola { get; set; }
    }

    [Table("Uprawnienia_Role", Schema = "public")]
    public class Uprawnienie_Rola
    {
        [Key, Column("ID_Uprawnienia", Order = 1)]
        public int ID_Uprawnienia { get; set; }

        [Key, Column("ID_Roli", Order = 2)]
        public int ID_Roli { get; set; }

        public Uprawnienie Uprawnienie { get; set; }
        public Rola Rola { get; set; }
    }
    // koniec klas bazy

    public class LogInOdpowiedz
    {
        public string Nazwa { get; set; }
        public int ID_Uzytkownika { get; set; }
        public string ID_Sesji { get; set; }
        public Rola Rola { get; set; }
        public IEnumerable<Uprawnienie> Uprawnienia { get; set; }
        public long Data_waznosci { get; set; }
    }

    public class LogInZapytanie
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Haslo { get; set; }
        [Required]
        public int? Rola { get; set; }
    }

    public class LogOutZapytanie
    {
        public String ID_Sesji { get; set; }
    }
    
        
    public class NpgsqlConfiguration : System.Data.Entity.DbConfiguration
    {
        public NpgsqlConfiguration()
        {
            SetProviderServices("Npgsql", Npgsql.NpgsqlServices.Instance);
            SetProviderFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
            SetDefaultConnectionFactory(new Npgsql.NpgsqlConnectionFactory());
        }
    }
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class DB : DbContext
    {
        static String ConnectionString = "Server=localhost;User ID=postgres;Password=paulina;Database=Ksiegarnia;syncnotification=false;port=5432";
        public DB()
            : base(new NpgsqlConnection(ConnectionString), true) { }
        public DbSet<Autor> Autorzy { get; set; }
        public DbSet<Kategoria> Kategorie { get; set; }
        public DbSet<Ksiazka> Ksiazki { get; set; }
        public DbSet<Uprawnienie> Uprawnienia { get; set; }
        public DbSet<Rola> Rolee { get; set; }
        public DbSet<Sesja> Sesje { get; set; }
        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        public DbSet<Uzytkownik_Rola> Uzytkownicy_Role { get; set; }
        public DbSet<Uprawnienie_Rola> Uprawnienia_Role { get; set; }
    }
}


