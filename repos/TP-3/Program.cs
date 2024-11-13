/*EX1*/
using System;
namespace Test
{
    class Program
    {
        static int[] tableau = new int[] { 17, 12, 15, 38, 29, 157, 89, 22, 0, 5 };

        static int division(int indice, int diviseur)
        {
            
            if (indice < 0 || indice >= tableau.Length)
            {
                throw new ArgumentOutOfRangeException("indice", "L'indice doit être compris entre 0 et " + (tableau.Length - 1));
            }

            
            if (diviseur == 0)
            {
                throw new DivideByZeroException("Division par zéro");
            }

            return tableau[indice] / diviseur;
        }

        public static void Main(string[] args)
        {
            int x, y;

            Console.WriteLine("Entrez l'indice de l'entier à diviser : ");
            try
            {
                x = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Veuillez entrer un nombre entier.");
                return;
            }

            Console.WriteLine("Entrez le diviseur : ");
            try
            {
                y = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Veuillez entrer un nombre entier.");
                return;
            }

            try
            {
                Console.WriteLine("Le résultat de la division est : " + division(x, y));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Vous ne pouvez pas diviser par zéro.");
            }
        }
    }
}

EX2
using System;

public class Program
{
    static void Main(string[] args)
    {
        int[] tableau = { 2, 5, 8, 12, 16 };
        
        Console.WriteLine("entrer une valeure");
        int valeurRecherchee = int.Parse (Console.ReadLine());
        try
        {
            int position = Rechercher(tableau, valeurRecherchee);
            Console.WriteLine("La valeur {0} a été trouvée à la position {1}", valeurRecherchee, position);
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine("La valeur {0} n'a pas été trouvée dans le tableau.", valeurRecherchee);
        }
        finally
        {
            Console.WriteLine("Au revoir !");
        }
    }

    static int Rechercher(int[] tableau, int valeur)
    {
        for (int i = 0; i < tableau.Length; i++)
        {
            if (tableau[i] == valeur)
            {
                return i;
            }
        }

        throw new NoSuchElementException();
    }
}

public class NoSuchElementException : Exception
{
    public NoSuchElementException() : base("L'élément recherché n'a pas été trouvé.") { }
}
*/
EX3
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Personne
{
    public int Numero { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Numero: {Numero}, Nom: {Nom}, Prenom: {Prenom}";
    }
}

[Serializable]
public class Etudiant : Personne
{
    public string Filiere { get; set; } = string.Empty;
    public float[] Notes { get; set; } = new float[5];

    public double CalculMoy()
    {
        double somme = 0;
        for (int i = 0; i < Notes.Length; i++)
        {
            somme += Notes[i];
        }
        return Notes.Length > 0 ? somme / Notes.Length : 0;
    }

    public Etudiant() { }

    // Méthode de sérialisation binaire personnalisée
    public void WriteToBinary(BinaryWriter writer)
    {
        writer.Write(Numero);
        writer.Write(Nom);
        writer.Write(Prenom);
        writer.Write(Filiere);
        writer.Write(Notes.Length);
        foreach (var note in Notes)
        {
            writer.Write(note);
        }
    }

    // Méthode de désérialisation binaire personnalisée
    public static Etudiant ReadFromBinary(BinaryReader reader)
    {
        Etudiant etudiant = new Etudiant
        {
            Numero = reader.ReadInt32(),
            Nom = reader.ReadString(),
            Prenom = reader.ReadString(),
            Filiere = reader.ReadString()
        };

        int notesLength = reader.ReadInt32();
        etudiant.Notes = new float[notesLength];
        for (int i = 0; i < notesLength; i++)
        {
            etudiant.Notes[i] = reader.ReadSingle();
        }

        return etudiant;
    }
}

class Program
{
    static void Main()
    {
        List<Etudiant> listEtudiant = new List<Etudiant>();

        for (int i = 0; i < 2; i++)
        {
            Etudiant e = new Etudiant();
            Console.WriteLine($"Entrer les informations de l'étudiant {i + 1}");
            Console.Write("Numero: ");
            e.Numero = int.Parse(Console.ReadLine());
            Console.Write("Nom: ");
            e.Nom = Console.ReadLine();
            Console.Write("Prenom: ");
            e.Prenom = Console.ReadLine();
            Console.Write("Filiere: ");
            e.Filiere = Console.ReadLine();

            Random random = new Random();
            for (int j = 0; j < 5; j++)
            {
                e.Notes[j] = (float)(random.NextDouble() * 20);
            }
            listEtudiant.Add(e);
        }

        // Sérialisation en binaire
        string binaryFilePath = "etudiants.bin";
        using (FileStream fs = new FileStream(binaryFilePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            writer.Write(listEtudiant.Count);
            foreach (var etudiant in listEtudiant)
            {
                etudiant.WriteToBinary(writer);
            }
            Console.WriteLine("\nListe des étudiants sérialisée en mode binaire.");
        }

        // Désérialisation depuis le binaire
        List<Etudiant> etudiantsBinaire = new List<Etudiant>();
        using (FileStream fs = new FileStream(binaryFilePath, FileMode.Open))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                etudiantsBinaire.Add(Etudiant.ReadFromBinary(reader));
            }
        }
        Console.WriteLine("\nContenu de la liste désérialisée depuis le fichier binaire :");
        foreach (var etudiant in etudiantsBinaire)
        {
            Console.WriteLine(etudiant);
        }

        // Sérialisation en XML
        string xmlFilePath = "etudiants.xml";
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Etudiant>));
        using (FileStream xmlStream = new FileStream(xmlFilePath, FileMode.Create))
        {
            xmlSerializer.Serialize(xmlStream, listEtudiant);
            Console.WriteLine("\nListe des étudiants sérialisée en mode XML.");
        }

        // Désérialisation depuis XML
        using (FileStream xmlStream = new FileStream(xmlFilePath, FileMode.Open))
        {
            List<Etudiant> etudiantsXml = (List<Etudiant>)xmlSerializer.Deserialize(xmlStream);
            Console.WriteLine("\nContenu de la liste désérialisée depuis le fichier XML :");
            foreach (var etudiant in etudiantsXml)
            {
                Console.WriteLine(etudiant);
            }
        }
    }
}
*/
