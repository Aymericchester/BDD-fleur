using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Bcpg;
using System.IO;
using System.Collections.Generic;
using System;

namespace ConsoleApp1
{
    public class Fleur
    {
        static void Main(string[] args)
        {
            //Requetesql();
            //Client();
            Application();
            //Exo2();
            //Exo3();
            //Exo4();
            //Exo5();
            //Exo6();
            //Exo7();
            //Exo8();
            //Exo9();
            //Exo10();
            //Exo11();
        }

        static void Requetesql()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides, connexion admin ici!!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; 
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select * from client;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string 
                    currentRowAsString +=  valueAsString + ", ";
                }
                Console.WriteLine( currentRowAsString );    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }

            connection.Close();
        }

        static void Client()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides, connexion client ici!!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select Avg(prix_bouquet) from bouquet;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = reader.GetValue(0).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string                
                Console.WriteLine("Le prix moyen d'un bouquet est de : " + currentRowAsString + " euros.");    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }

            connection.Close();
        }

        static void Application()
        {
            Console.WriteLine("Bienvenue chez les magasins de M Bellefleur");
            Console.WriteLine("Saisissez votre identifiant de connexion, si vous etes client merci d'entrer bozo");
            string id=Console.ReadLine();
            Console.WriteLine("Saisissez votre mot de passe, si vous etes client merci d'enter bozo");
            string mdp = Console.ReadLine();
            if (id=="root" && mdp=="root")
            {
                Console.WriteLine("Bienvenue M Bellefleur");
            }
            if (id=="bozo" && mdp=="bozo")
            {
                Console.WriteLine("Bienvenue sur l'espace client");
                if (nouveauclient()==true)
                {
                    Addclient();
                    //à coder
                }
                else
                {
                    // à coder
                }
            }
            else
            {
                Console.WriteLine("Erreur identifiant ou mot de passe inconnu");
            }
        }
        static bool nouveauclient()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides, connexion client ici!!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            Console.WriteLine("Quel est votre nom?");
            string nom = Console.ReadLine();
            Console.WriteLine("Quel est votre prenom?");
            string prenom = Console.ReadLine();
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select nom,prenom from client;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                if (nom==reader.GetValue(0).ToString() && prenom==reader.GetValue(1).ToString())
                {
                    Console.WriteLine("Vous êtes déjà inscrit, bienvenue dans votre espace " + prenom + " " + nom);
                    reader.Close();
                    connection.Close();
                    return false;
                }                                               
            }
            Console.WriteLine("Vous n'êtes pas encore inscrit, veuillez vous inscrire avant d'effectuer votre commande");
            reader.Close();
            connection.Close();
            return true;
        }

        static void Addclient()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //root pour pouvoir modifier la bdd
            MySqlConnection connection = new MySqlConnection(connectionString);
            Console.WriteLine("Formulaire d'inscription:");
            Console.WriteLine("Veuillez renseigner votre nom");
            string nom = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre prenom");
            string prenom = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre numéro de téléphone");
            string numtel = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre mot de passe de connexion");
            string mdp = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre courriel");
            string courriel= Console.ReadLine();

            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select courriel from client;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                while (courriel==reader.GetValue(0).ToString()) // tant que le courriel existe déjà dans la bdd on change de courriel
                {
                    Console.WriteLine("Avertissement ce courriel existe déjà, veuillez saisir une autre combinaison courriel/mot de passe");
                    Console.WriteLine("Veuillez renseigner votre mot de passe de connexion");
                    mdp = Console.ReadLine();  
                    Console.WriteLine("Veuillez renseigner votre courriel");
                    courriel = Console.ReadLine();
                }
            }
            reader.Close();
            
            Console.WriteLine("Veuillez renseigner votre adresse de facturation");
            string adressefacturation = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre carte de crédit, ce site est protégé par verisure");
            string cartecredit = Console.ReadLine();
            string fidelité = "Aucune";
            string achatpassé = "";

            MySqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "INSERT INTO `bellefleur`.`client`(`nom`, `prenom`, `num_tel`, `courriel`, `mdp`, `adresse_facturation`,`carte_de_credit`,`statut_fidelite`,`achat_passe`) VALUES ('" + nom + "','" + prenom +"','" + numtel +"','" + courriel +"','" + mdp +"','" + adressefacturation +"','" + cartecredit +"','" + fidelité +"','" + achatpassé +"');";
            MySqlDataReader reader1 = command1.ExecuteReader(); 
            Console.WriteLine("Vous êtes bien enregistrer, bienvenue chez M Bellefleur "+ prenom +" " + nom);
            reader1.Close();
            connection.Close();
        }












        static void Exo2()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT pseudo, marque, modele, immat FROM proprietaire JOIN voiture ON voiture.codep = proprietaire.codep;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    currentRowAsString +=  valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }

            connection.Close();
        }

        static void Exo3()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT marque, modele, prixj FROM voiture ORDER BY prixj DESC;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    if (i==0)
                    {
                        if (valueAsString.Length == 1)
                            valueAsString=(char.ToUpper(valueAsString[0])).ToString();
                        else
                            valueAsString=char.ToUpper(valueAsString[0]) + valueAsString.Substring(1);
                    }
                    currentRowAsString +=  valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }

            connection.Close();
        }

        static void Exo4()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT prixj FROM voiture;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string currentRowAsString = "Moyenne des prix d'une voiture à la journée: ";
            double value = 0;
            double cp = 0;
            while (reader.Read())                           // parcours ligne par ligne
            {
                value += Int32.Parse(reader.GetValue(0).ToString());  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)             
                cp+=1;
            }
            double moy = value/cp;
            currentRowAsString +=  moy + " euros ";
            Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard

            connection.Close();
        }

        static void Exo5()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT modele, prixj FROM voiture;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            if (reader.Read())                           // parcours 1 ligne
            {
                string currentRowAsString = "";
                double prixj = Int32.Parse(reader.GetValue(1).ToString());
                double min = prixj;
                double max = prixj;
                while (reader.Read())
                {
                    double prixj2 = Int32.Parse(reader.GetValue(1).ToString());
                    if (prixj2 < min)
                    {
                        min=prixj2;
                    }
                    else if (prixj2 > max)
                    {
                        max=prixj2;
                    }
                }
                currentRowAsString+="Le prix min est de : " + min + " euros, " + "Le prix max est de: " + max + " euros.";
                Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }

            connection.Close();
        }

        static void Exo6()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT prixj FROM voiture Order By prixj ASC;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())                           // parcours ligne 1
            {
                if (reader.Read())    // on arrive à la 2ème lignes
                {
                    int prix = reader.GetInt32(0);  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    Console.WriteLine("Le 2ème prix de journée est : " + prix);
                }
                // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }

            connection.Close();
        }

        static void Exo7()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT count(*) FROM voiture;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string currentRowAsString = "Médianne des prix d'une voiture à la journée: ";
            double mediane = 0;
            int cp = 0;
            if (reader.Read())                           // parcours ligne 1
            {
                cp=Int32.Parse(reader.GetValue(0).ToString()); //cp est le nombre de valeur de prixj dans la table voiture
            }
            connection.Close(); // fin 1ère requête

            int indice = cp/2;
            int ligne = 0;
            connection.Open();
            command.CommandText = "SELECT prixj FROM voiture ORDER BY prixj;";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                ligne+=1;
                if (ligne==indice)
                {
                    mediane += Int32.Parse(reader.GetValue(0).ToString());
                }
            }
            currentRowAsString +=  mediane + " euros ";
            Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            connection.Close();

        }
        static void Exo8()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Saisir le code du proprietaire entre apostrophe");
            string codep = Console.ReadLine();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT immat FROM voiture JOIN proprietaire ON proprietaire.codep=voiture.codep WHERE proprietaire.codep="+codep;

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string currentRowAsString = "Liste des immatriculations des véhicules du propriétaires: ";
            Console.WriteLine(currentRowAsString);
            while (reader.Read())                           // parcours ligne par ligne
            {
                string valueAsString = reader.GetValue(0).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                currentRowAsString =  valueAsString;
                Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }
            connection.Close();
        }
        static void Exo9()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Saisir le code du client entre apostrophe");
            string codec = Console.ReadLine();
            Console.WriteLine("Saisir la categorie du véhicule entre apostrophe");
            string categorie = Console.ReadLine();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT modele FROM voiture JOIN location ON location.immat=voiture.immat JOIN client ON location.codec=client.codec WHERE client.codec=" + codec + " AND voiture.categorie=" + categorie;

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string currentRowAsString = "Liste des modèles des véhicules du client dans la catégorie que vous avez mentionnez: ";
            Console.WriteLine(currentRowAsString);
            while (reader.Read())                           // parcours ligne par ligne
            {
                string valueAsString = reader.GetValue(0).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                currentRowAsString =  valueAsString;
                Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            }
            connection.Close();
        }
        static void Exo10()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT AVG(prixj) FROM voiture;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            double M = 0;
            string currentRowAsString = "Moyenne des prix d'une voiture à la journée: ";
            if (reader.Read())
            {
                M = Convert.ToDouble(reader.GetValue(0).ToString());
                currentRowAsString += M + " euros ";
            }
            Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            connection.Close();

            connection.Open();
            command.CommandText = "SELECT prixJ FROM voiture;";
            reader = command.ExecuteReader();
            List<double> delta = new List<double>();
            while (reader.Read())
            {
                double delta0 = (Convert.ToDouble(reader.GetValue(0).ToString())-M)*(Convert.ToDouble(reader.GetValue(0).ToString())-M);
                delta.Add(delta0);
            }
            double somme = 0;
            int cp = 0;
            foreach (double delta0 in delta)
            {
                somme+=Convert.ToDouble(delta0.ToString());
                cp+=1;
            }
            double M1 = somme/cp;
            Console.WriteLine("Moyenne M1: " + M1);
            connection.Close();
        }
        static void Exo11()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT MAX(codec) FROM client;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            string currentRowAsString = "Codec max du client: ";
            while (reader.Read())                           // parcours ligne par ligne
            {
                string valueAsString = reader.GetValue(0).ToString();
                currentRowAsString=valueAsString;// Pas trop compris ce qu'il faalait faire sur c# pour cette question (travail fais sur mysql)
            }
            //Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
            connection.Close();
            var read = new StreamReader(File.OpenRead(@"C:\\ProgramData\\MySQL\\MySQL Server 5.7\\Uploads\\clients.csv"));
            int k = 0;
            while (!read.EndOfStream)
            {
                var line = read.ReadLine();
                var values = line.Split(';');
                k+=1;

                connection.Open();
                MySqlCommand command1 = connection.CreateCommand();
                string test = currentRowAsString.Substring(0, 3);
                int fin = 0;
                if (Int32.Parse(currentRowAsString.Substring(3))+k<10)
                {
                    fin=Int32.Parse(currentRowAsString.Substring(3))+k;
                }
                else if (Int32.Parse(currentRowAsString.Substring(2))+k<100)
                {
                    test = currentRowAsString.Substring(0, 2);
                    fin=Int32.Parse(currentRowAsString.Substring(2))+k;
                }
                else
                {
                    test = currentRowAsString.Substring(0, 1);
                    fin=Int32.Parse(currentRowAsString.Substring(1))+k;
                }
                string codec1 = test+fin.ToString();
                Console.WriteLine(codec1 + ", " +values[0] + ", " + values[1] + ", "+ values[2] +", "+  values[3] +", "+  values[4] +", "+ values[5]);
                command1.CommandText ="INSERT INTO `loueur`.`client`(`codeC`, `nom`, `prenom`, `age`, `permis`, `adresse`, `ville`) VALUES ('" + codec1+ "','" + values[0] +"','" + values[1] +"'," + Int32.Parse(values[2]) +",'" + values[3] +"','" + values[4] +"','" + values[5] +"');";

                MySqlDataReader reader1 = command1.ExecuteReader();
                string[] valueString = new string[reader1.FieldCount];
                /*while (reader1.Read())
                {
                    string currentRowAsString2 = " ";
                    for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                    {
                        string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                        currentRowAsString2 +=  valueAsString + ", ";
                    }
                    Console.WriteLine(currentRowAsString2 + values[0] + values[1] + values[2] + values[3] + values[4] + values[5]);
                }*/
                reader1.Close();
                connection.Close();
            }
        }
    }
}