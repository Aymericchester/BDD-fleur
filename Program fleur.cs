using System;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Xml;

namespace BDDfleur
{
    public class Fleur
    {
        static void Main(string[] args)
        {
            Application();
        }

        static void Application() //Application du code
        {
            Console.WriteLine("Bienvenue chez les magasins de M Bellefleur.");
            Console.WriteLine("Saisissez votre identifiant de connexion, si vous êtes client merci d'entrer bozo.");
            string id = Console.ReadLine();
            Console.WriteLine("Saisissez votre mot de passe, si vous êtes client merci d'enter bozo.");
            string mdp = Console.ReadLine();
            if (id == "root" && mdp == "root") // Interface administrateur
            {
                Console.WriteLine("Bienvenue M. Bellefleur.");
                Console.WriteLine();
                Console.WriteLine("Voulez-vous interroger votre base de données? (oui ou non)");
                string r = Console.ReadLine();
                if (r == "oui")
                {
                    Console.WriteLine("Veuillez rentrer une requête SQL pour interroger votre BDD.");
                    string requete = Console.ReadLine();
                    Requetesql(requete);
                }
                Console.WriteLine("Voulez-vous regarder vos stocks ? (oui ou non)");
                string r1 = Console.ReadLine();
                if (r1 == "oui")
                {
                    Console.WriteLine();
                    Console.WriteLine("Voici l'état des stocks.");
                    Stock();
                }
                Console.WriteLine("Voulez-vous réaprovisionner vos stocks? (oui ou non)");
                string r2 = Console.ReadLine();
                if (r2 == "oui")
                {
                    Appro();
                    Console.WriteLine("Réaprovisionnement réussie");
                }
                Console.WriteLine();
                Console.WriteLine("Voulez-vous voir l'état des commandes? (oui ou non)");
                string r3 = Console.ReadLine();
                if (r3 == "oui")
                {
                    Console.WriteLine();
                    Etatcommande();
                    Console.WriteLine();
                }
                Console.WriteLine("Voulez-vous voir vos données statistiques? (oui ou non)");
                string r4 = Console.ReadLine();
                if (r4 == "oui")
                {
                    Etatcommande();
                    Console.WriteLine();
                    Console.WriteLine("Données statistiques :");
                    Console.WriteLine();
                    Console.WriteLine("Le prix moyen du bouquet acheté est (en euros) :");
                    Requetesql("Select Avg(prix_bouquet) from bouquet;");
                    Console.WriteLine();
                    Console.WriteLine("Le meilleur client du mois est (nbcommande, nom, prenom):"); // choix de prendre le client le plus fidèle avec le plus de commandes (refaire ces 2 requetes car elle ne retourne rien)
                    Requetesql("select count(client.courriel),nom,prenom from client join boncommande on client.courriel=boncommande.courriel where substr(boncommande.date_commande, 4,2) <= month(now()) and substr(boncommande.date_commande, 4,2) >= month(now())-1 group by(client.courriel) order by count(client.courriel) DESC LIMIT 1;");
                    Console.WriteLine();
                    Console.WriteLine("Le meilleur client de l'année est (nbcommande, nom, prenom):");
                    Requetesql("select count(client.courriel),nom,prenom from client join boncommande on client.courriel=boncommande.courriel where right(boncommande.date_commande, 4) <= year(now()) and right(boncommande.date_commande, 4) >= year(now())-1 group by(client.courriel) order by count(client.courriel) DESC LIMIT 1;");
                    Console.WriteLine();
                    Console.WriteLine("Le bouquet standard qui a eu le plus de succès est le bouquet:");
                    Requetesql("select nom_bouquet from bouquet order by nb_commande desc limit 1;");
                    Console.WriteLine();
                    Console.WriteLine("Le magasin qui a généré le plus de chiffre d'affaire est le magasin: (nom, argent)");
                    Requetesql("select id_magasin, ca from magasin order by ca desc limit 1;");
                    Console.WriteLine();
                    Console.WriteLine("La fleur exotique la moins vendue est la fleur:"); // Pour être plus representatif des volontés de la clientèle, une fleur exotique est considérée comme vendue dès lors qu'un client veut en acheter une en dehors d'un bouquet standard (où il n'y a pas de fleur exotique) meme si il ne l'achète pas à la fin
                    Requetesql("select nom_fleur from fleur where origine_fleur='exotique' order by nbvendu desc limit 1;");
                    Console.WriteLine();
                    Console.WriteLine("Voici les informations sur les clients n'ayant jamais commandés (nom, prenom, N°tel, courriel, mdp, adresse de facturation, carte de credit, statut de fidélité, achat passé):");
                    Requetesql("SELECT * FROM client WHERE courriel NOT IN (SELECT courriel FROM boncommande);");
                    Console.WriteLine();
                    Console.WriteLine("Voici les informations sur les clients qui ont commandés: (nom, prenom, numero de commande)");
                    Requetesql("SELECT client.nom, client.prenom, boncommande.num_commande FROM client, boncommande WHERE client.courriel=boncommande.courriel;");
                    Console.WriteLine();
                    Console.WriteLine("Voici les noms des fleurs et des accessoires avec leur stock dans le magasin 'a' :");
                    Requetesql("SELECT nom_fleur AS nom, stock_fleur AS stock, 'fleur' AS type FROM fleur WHERE id_magasin = 'a' UNION SELECT nom_acc AS nom, stock_acc AS stock, 'accessoire' AS type FROM accessoire WHERE id_magasin = 'a';");
                }
                Console.WriteLine();
                Console.WriteLine("Voulez-vous exporter les données des clients ayant commandés plusieurs fois durant le dernier mois en XML? (oui ou non)");
                string r5 = Console.ReadLine();
                if (r5 == "oui")
                {
                    Console.WriteLine();
                    exportXML();
                }
                Console.WriteLine("Voulez-vous exporter les données des clients n’ayant pas commandé depuis plus de 6 mois en JSON? (oui ou non)");
                string r6 = Console.ReadLine();
                if (r6 == "oui")
                {
                    Console.WriteLine();
                    exportjson();
                }
            }
            else if (id == "bozo" && mdp == "bozo") // Interface client
            {
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectué par l'employé ou admin
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "Select * from temps";
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                int j = 0;
                while (reader.Read())
                {
                    j = Int32.Parse(reader.GetValue(0).ToString());
                }
                if (Temps() - j > 3 || Temps() - j < -3)// Plus de 3 jours depuis le dernier approvisionnement ou tous les changements de mois.
                {
                    Appro(); // On réapprovisione
                    Requetesql("update temps set temps='" + Temps() + "';");
                }
                Console.WriteLine("Bienvenue sur l'espace client !");
                string email = nouveauclient(); // renvoie true si nouveau client, courriel du client sinon
                if (email == "true")
                {
                    email = Addclient(); //on ajoute le client à la BDD
                }
                Console.WriteLine("Voulez-vous effectuer une commande ? (oui ou non)");
                string rep = Console.ReadLine();
                if (rep == "oui")
                {
                    string fidélité = Fidelite(email); //MAJ de la fidélité du client, si nouveau client, on offre la fidélité bronze lors de la 1ere commande en cadeau de bienvenue
                    string[] commande = Creationboncommande(email);
                    if (fidélité == "or")
                    {
                        Console.WriteLine("Vous avez le pass or pour votre fidélité dans notre magasin, on vous offre 15% de réduction sur tout vos achats de bouquets !");
                    }
                    else if (fidélité == "bronze")
                    {
                        Console.WriteLine("Vous avez le pass bronze pour votre fidélité dans notre magasin, on vous offre 5% de réduction sur tout vos achats de bouquets !");
                    }
                    string bouquet = Choixbouquet();
                    if (bouquet == "personnalisé") // bouquet personnalisé
                    {
                        string[,] infobouq = Conceptionbouquet(email);
                        if (Dureecommande(commande) < 3)  // si durée de commande inférieur a 3 jours, on doit vérifier les stocks
                        {
                            Etat("cpav", commande[3]); //MAJ de l'état de la commande
                            Console.WriteLine();
                            Console.WriteLine("AVERTISSEMENT!! Vous avez demandé d'être livré en moins de 3 jours, nous vous informons qu'il y a un risque de pénurie avec votre commande");
                            string compo = Verificationstockcommande(infobouq, commande[0], "", email);
                            Console.WriteLine("Suite à votre demande et ce que nous avons en stock, votre bouquet sera donc composé de: " + compo);
                        }
                        else
                        {
                            Console.WriteLine("Nous avons votre commande en stock ! Elle sera livré à votre date désirée !");
                        }
                    }
                    else //bouquet standard
                    {
                        if (Dureecommande(commande) < 3) // si durée de commande inférieure a 3 jours on doit vérifier les stocks
                        {
                            Etat("vinv", commande[3]);
                            Console.WriteLine();
                            Console.WriteLine("AVERTISSEMENT!! Vous avez demandé d'être livré en moins de 3 jours, nous vous informons qu'il y a un risque de pénurie avec votre commande. Si tel est le cas les éléments manquant de votre bouquet ne seront pas présent dans ce dernier et vous ne serez pas remboursé");
                            string[] info = Commande(bouquet, email, commande[0]);  //MAJ du nombre de bouquets vendu et stockage des fleurs/accessoires/prix du bouquet
                            AnalyseStock(info, commande[0]);
                        }
                        else
                        {
                            Console.WriteLine("Nous avons votre commande en stock! Elle sera livrée à votre date désiré !");
                            string[] info = Commande(bouquet, email, commande[0]);
                        }
                    }
                    Console.WriteLine();
                    Etat("cc", commande[3]);
                    Console.WriteLine("Voulez-vous voir l'état de vos commandes? (oui ou non)");
                    string r3 = Console.ReadLine();
                    if (r3 == "oui")
                    {
                        Etatcommande1(email);
                        Console.WriteLine();
                    }
                    Etat("cal", commande[3]);
                    Etat("cl", commande[3]);
                    Console.WriteLine("Votre commande est expédiée!");
                }
            }
            else
            {
                Console.WriteLine("Erreur d'identifiant ou mot de passe inconnu.");
            }
            Console.WriteLine("Merci de votre visite chez M. Bellefleur, à bientôt!");
        }
        static string nouveauclient() // Vérifie si le client est nouveau ou pas
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides, connexion client ici!!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            Console.WriteLine("Merci de vous authentifier!");
            Console.WriteLine("Quel est votre courriel?");
            string courriel = Console.ReadLine();
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select courriel, nom, prenom, mdp from client;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                if (courriel == reader.GetValue(0).ToString())  // si le courriel est déjà dans la bdd
                {
                    Console.WriteLine("Vous êtes déjà inscrit, veuillez entrer votre mot de passe pour accéder à votre espace client");
                    string mdp = Console.ReadLine();
                    int cp = 0;
                    while (mdp != reader.GetValue(3).ToString() && cp < 3)  // nouvel essai de connexion tant que le mot de passe est incorrect
                    {
                        Console.WriteLine("Mot de passe incorrect, nouvelle saisie:");
                        mdp = Console.ReadLine();
                        cp++;
                    }
                    if (cp >= 3)
                    {
                        Console.WriteLine("Nombre d'essai dépassé, veuillez vous inscrire avec un autre email.");
                        return "true";
                    }
                    Console.WriteLine("Bienvenue dans votre espace client " + reader.GetValue(2).ToString() + " " + reader.GetValue(1).ToString());
                    reader.Close();
                    connection.Close();
                    return courriel;
                }
            }
            Console.WriteLine("Vous n'êtes pas encore inscrit, veuillez vous inscrire avant d'effectuer votre commande.");
            reader.Close();
            connection.Close();
            return "true";
        }

        static string Addclient() // Ajoute le nouveau client à la BDD
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //root pour pouvoir modifier la bdd, un bot ajoute le client à la bdd
            MySqlConnection connection = new MySqlConnection(connectionString);

            Console.WriteLine("Formulaire d'inscription:");
            Console.WriteLine("Veuillez renseigner votre courriel.");
            string courriel = Console.ReadLine();

            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select courriel from client;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())                           // parcours ligne par ligne
            {
                if (courriel == reader.GetValue(0).ToString()) // tant que le courriel existe déjà dans la bdd, on change de courriel
                {
                    Console.WriteLine("Avertissement ! Ce courriel existe déjà, veuillez saisir une autre combinaison courriel/mot de passe");
                    Addclient();
                }
            }
            reader.Close();

            Console.WriteLine("Veuillez renseigner votre nom");
            string nom = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre prenom");
            string prenom = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre numéro de téléphone");
            string numtel = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre mot de passe de connexion");
            string mdp = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre adresse de facturation");
            string adressefacturation = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre carte de crédit, ce site est protégé par verisure");
            string cartecredit = Console.ReadLine();
            string fidelité = "Aucune";
            string achatpassé = "";

            MySqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "INSERT INTO `bellefleur`.`client`(`nom`, `prenom`, `num_tel`, `courriel`, `mdp`, `adresse_facturation`,`carte_de_credit`,`statut_fidelite`,`achat_passe`) VALUES ('" + nom + "','" + prenom + "','" + numtel + "','" + courriel + "','" + mdp + "','" + adressefacturation + "','" + cartecredit + "','" + fidelité + "','" + achatpassé + "');";
            MySqlDataReader reader1 = command1.ExecuteReader();
            Console.WriteLine("Vous êtes bien enregistré, bienvenue dans l'espace client de chez M Bellefleur (M ou Mme) " + prenom + " " + nom);
            reader1.Close();
            connection.Close();
            return courriel;
        }

        static string[] Creationboncommande(string email) // Création du bon de commande
        {
            Console.WriteLine("Dans quel magasins souhaitez-vous faire votre commande? (A, B ou C)"); //Choix de notre part
            string magasin = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner votre adresse de livraison");
            string adresselivraison = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner les spécificités accompagnant votre commande");
            string message = Console.ReadLine();
            Console.WriteLine("Veuillez renseigner la date de livraison désirée (jj-mm-aaaa)");
            string datelivraisondesire = Console.ReadLine();
            Console.WriteLine();
            string etat = "";

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //root pour pouvoir modifier la bdd, client ajouté par bot informatique
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select max(num_commande)+1 from boncommande;"; // On sélectionne donc un nouveau numéro de commande max encore jamais sélectionné
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int numcommande = 0;// On assigne la variable avant la boucle
            while (reader.Read())
            {
                numcommande = Int32.Parse(reader.GetValue(0).ToString());
            }
            reader.Close();

            MySqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "SELECT NOW();"; // On sélectionne la date et l'heure de la commande
            MySqlDataReader reader1;
            reader1 = command1.ExecuteReader();
            string datecommande = "";// On assigne la variable avant la boucle
            while (reader1.Read())
            {
                datecommande = reader1.GetValue(0).ToString();
            }
            reader1.Close();

            MySqlCommand command2 = connection.CreateCommand();
            command2.CommandText = "INSERT INTO `bellefleur`.`boncommande` (`num_commande`, `etat`, `date_commande`, `adresse_livraison`, `date_livraison_desire`, `message`, `courriel`,`id_magasin`) VALUES ('" + numcommande + "','" + etat + "','" + datecommande + "','" + adresselivraison + "','" + datelivraisondesire + "','" + message + "','" + email + "','" + magasin + "');";
            MySqlDataReader reader2 = command2.ExecuteReader();
            Console.WriteLine("Votre commande est la commande N° " + numcommande + ", il va maintenant falloir choisir votre bouquet");
            reader2.Close();

            MySqlCommand command3 = connection.CreateCommand(); // On met à jour les achats passés du client
            command3.CommandText = "SELECT achat_passe from client WHERE courriel='" + email + "';";
            MySqlDataReader reader3 = command3.ExecuteReader();
            string achat = numcommande.ToString() + ", ";
            while (reader3.Read())
            {
                achat += reader3.GetValue(0).ToString();
            }
            reader3.Close();

            MySqlCommand command4 = connection.CreateCommand(); // On met à jour les achats passés du client
            command4.CommandText = "UPDATE client SET achat_passe='" + achat + "' WHERE courriel='" + email + "';";
            MySqlDataReader reader4 = command4.ExecuteReader();
            reader4.Close();

            connection.Close();
            string[] data = new string[4];
            data[0] = magasin;
            data[1] = datelivraisondesire;
            data[2] = datecommande;
            data[3] = numcommande.ToString();
            return data;
        }

        static string Choixbouquet() //Permet de stocker le choix du bouquet du client (standard ou personnalisé)
        {
            Console.WriteLine();
            Console.WriteLine("Voici la liste de nos bouquet standard (nom, prix, catégorie, type, compo_fleur, compo_accessoire, N°commande, nombre de commande: ");
            Console.WriteLine();
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select * from bouquet;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader.GetValue(i).ToString();  // recupération de la valeur de chaque cellule sous forme d'un string 
                    currentRowAsString += valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);
            }
            Console.WriteLine();
            Console.WriteLine("Veuillez choisir le nom du bouquet que vous souhaitez ou bien créer votre propre bouquet avec le nom 'personnalisé', RESPECTEZ LA SYNTAXE!");
            string nombouquet = Console.ReadLine();
            Console.WriteLine();
            reader.Close();
            connection.Close();
            return nombouquet;
        }

        static int Dureecommande(string[] commande) // renvoie la durée en jour entre le moment de la commande et la date de livraison désirée
        {
            string dlivraison = commande[1];
            string dcommande = commande[2];
            string j1 = dlivraison[0].ToString();
            string j2 = dlivraison[1].ToString();
            string m1 = dlivraison[3].ToString();
            string m2 = dlivraison[4].ToString();
            string a1 = dlivraison[6].ToString();
            string a2 = dlivraison[7].ToString();
            string a3 = dlivraison[8].ToString();
            string a4 = dlivraison[9].ToString();
            int anneel = Int32.Parse(a4 + a3 + a2 + a1);
            int moisl = Int32.Parse(m2 + m1);
            int jourl = Int32.Parse(j2 + j1);
            string j1c = dcommande[0].ToString();
            string j2c = dcommande[1].ToString();
            string m1c = dcommande[3].ToString();
            string m2c = dcommande[4].ToString();
            string a1c = dcommande[6].ToString();
            string a2c = dcommande[7].ToString();
            string a3c = dcommande[8].ToString();
            string a4c = dcommande[9].ToString();
            int anneec = Int32.Parse(a4c + a3c + a2c + a1c);
            int moisc = Int32.Parse(m2c + m1c);
            int jourc = Int32.Parse(j2c + j1c);
            int duree = 365 * (anneel - anneec) + 30 * (moisl - moisc) + (jourl - jourc);
            return duree;
        }

        static string[,] Conceptionbouquet(string email) // Conception du bouquet personnalisé 
        {
            Console.WriteLine();
            Console.WriteLine("Vous allez pouvoir concevoir votre bouquet ici");
            Console.WriteLine("Voici la liste des fleurs dont nous disposons (les prix indiqués prennent en compte vos réduction de fidélité):");
            Console.WriteLine();
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select * from fleur;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    if (Fidelite(email) == "or") // Reduction des prix dû au statut de fidélité
                    {
                        if (i == 1)
                        {
                            string valueAsString = (0.85 * Convert.ToDouble(reader.GetValue(i).ToString())).ToString() + " euros";
                            currentRowAsString += valueAsString + ", ";
                        }
                        else
                        {
                            string valueAsString = reader.GetValue(i).ToString();  // recupération de la valeur de chaque cellule sous forme d'un string 
                            currentRowAsString += valueAsString + ", ";
                        }
                    }
                    if (Fidelite(email) == "bronze")
                    {
                        if (i == 1)
                        {
                            string valueAsString = (0.95 * Convert.ToDouble(reader.GetValue(i).ToString())).ToString() + " euros";
                            currentRowAsString += valueAsString + ", ";
                        }
                        else
                        {
                            string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string 
                            currentRowAsString += valueAsString + ", ";
                        }
                    }
                    else
                    {
                        string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string 
                        currentRowAsString += valueAsString + ", ";
                    }
                }
                Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'un "grosse" string) sur la sortie standard
            }
            Console.WriteLine();
            Console.WriteLine("Voulez vous mettre des fleurs dans votre bouquet? (oui ou non)");
            string reponse = Console.ReadLine();
            string[,] infobouquet = new string[3, 20];  // 20 fleurs max par bouquet
            int cp = 0;
            while (reponse == "oui" && cp < 20)
            {
                Console.WriteLine("Veuillez écrire le nom de la fleur souhaité, RESPECTER LA SYNTAXE!");
                string fleur = Console.ReadLine();
                string connectionString2 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ
                MySqlConnection connection2 = new MySqlConnection(connectionString2);
                connection2.Open();

                MySqlCommand command3 = connection2.CreateCommand();
                command3.CommandText = "select nbvendu+1 from fleur where nom_fleur='" + fleur + "';";
                MySqlDataReader reader3;
                reader3 = command3.ExecuteReader();
                int vendu = 0;
                while (reader3.Read())
                {
                    vendu = Int32.Parse(reader3.GetValue(0).ToString()) + 1;
                }
                reader3.Close();
                connection2.Close();

                string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ
                MySqlConnection connection1 = new MySqlConnection(connectionString1);
                connection1.Open();

                MySqlCommand command2 = connection1.CreateCommand();
                command2.CommandText = "UPDATE fleur SET nbvendu='" + vendu + "' where nom_fleur='" + fleur + "';";
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                reader2.Close();
                connection1.Close();
                infobouquet[0, cp] = fleur;
                cp += 1;
                if (cp == 20)
                {
                    Console.WriteLine("Le nombre maximum de fleur pour le bouquet ont été choisis");
                }
                else
                {
                    Console.WriteLine("Voulez vous encore mettre des fleurs dans votre bouquet? (oui ou non)");
                    reponse = Console.ReadLine();
                }
            }
            reader.Close();

            Console.WriteLine();
            Console.WriteLine("Nous allons désormais passer au choix des accessoires");
            Console.WriteLine("Voici la liste des accesoires dont nous disposons (les prix indiqués prennent en compte vos réduction de fidélité):");
            Console.WriteLine();
            MySqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "Select * from accessoire;";
            MySqlDataReader reader1;
            reader1 = command1.ExecuteReader();

            while (reader1.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                for (int i = 0; i < reader1.FieldCount; i++)    // parcours cellule par cellule
                {
                    if (Fidelite(email) == "or") // Reduction des prix dû au statut de fidélité
                    {
                        if (i == 1)
                        {
                            string valueAsString = (0.85 * Convert.ToDouble(reader1.GetValue(i).ToString())).ToString() + " euros";
                            currentRowAsString += valueAsString + ", ";
                        }
                        else
                        {
                            string valueAsString = reader1.GetValue(i).ToString();  // Recupération de la valeur de chaque cellule sous forme d'un string 
                            currentRowAsString += valueAsString + ", ";
                        }
                    }
                    if (Fidelite(email) == "bronze")
                    {
                        if (i == 1)
                        {
                            string valueAsString = (0.95 * Convert.ToDouble(reader1.GetValue(i).ToString())).ToString() + " euros";
                            currentRowAsString += valueAsString + ", ";
                        }
                        else
                        {
                            string valueAsString = reader1.GetValue(i).ToString();  // Recupération de la valeur de chaque cellule sous forme d'un string  
                            currentRowAsString += valueAsString + ", ";
                        }
                    }
                    else //pas de fidélité
                    {
                        string valueAsString = reader1.GetValue(i).ToString();  // Recupération de la valeur de chaque cellule sous forme d'un string  
                        currentRowAsString += valueAsString + ", ";
                    }
                }
                Console.WriteLine(currentRowAsString);    // Affichage de la ligne (sous forme d'un "grosse" string) sur la sortie standard
            }
            Console.WriteLine();
            Console.WriteLine("Voulez-vous mettre des accessoires dans votre bouquet? (oui ou non)");
            string reponse2 = Console.ReadLine();
            int j = 0;
            while (reponse2 == "oui" && j < 20)
            {
                Console.WriteLine("Veuillez écrire le nom de l'accessoire souhaité, RESPECTER LA SYNTAXE!");
                string accessoire = Console.ReadLine();
                infobouquet[1, j] = accessoire;
                j += 1;
                if (j == 20)
                {
                    Console.WriteLine("Le nombre maximum d'accessoire pour le bouquet ont été choisis");
                }
                else
                {
                    Console.WriteLine("Voulez-vous encore mettre des accessoires dans votre bouquet? (oui ou non)");
                    reponse2 = Console.ReadLine();
                }
            }
            reader1.Close();
            connection.Close();
            Console.WriteLine("Quel est votre budget disponible pour ce bouquet?");
            string budget = Console.ReadLine();
            infobouquet[2, 0] = budget;
            return infobouquet;
        }

        static string Verificationstockcommande(string[,] infobouq, string magasin, string compobouquet, string email) //MAJ des stock des items lors de commande personnalisée
        {
            double budget = Convert.ToDouble(infobouq[2, 0]);
            double min = 1000000000; // plus que n'importe quel budget floral
            for (int i = 0; i < infobouq.GetLength(1); i++)  // stock fleur
            {
                if (infobouq[0, i] != "")
                {
                    string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectuée par l'employé ou admin
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "Select stock_fleur,prix_fleur,fleur.id_magasin,ca from fleur join magasin on fleur.id_magasin=magasin.id_magasin where nom_fleur='" + infobouq[0, i] + "';";
                    MySqlDataReader reader;
                    reader = command.ExecuteReader();
                    while (reader.Read())                           // parcours ligne par ligne
                    {
                        if (reader.GetValue(2).ToString() == magasin) //cas où le magasin de stockage est le même que celui de l'item
                        {
                            if (Int32.Parse(reader.GetValue(0).ToString()) == 0)  //rupture de stock dans le magasin
                            {
                                Console.WriteLine("La fleur " + infobouq[0, i] + " est en rupture de stock dans ce magasin");
                            }
                            else //MAJ du stock si suffisemment d'argent et en fonction de la fidélité du client
                            {
                                if (Fidelite(email) == "or")
                                {
                                    if (budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString())) > 0)
                                    {
                                        if (0.85 * Convert.ToDouble(reader.GetValue(1).ToString()) < min)
                                        {
                                            min = 0.85 * Convert.ToDouble(reader.GetValue(1).ToString());  //min stock l'info de l'item le moins cher
                                        }
                                        budget = budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString()));
                                        int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                        string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectué par employé ou admin
                                        MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                        connection1.Open();
                                        MySqlCommand command1 = connection1.CreateCommand();
                                        command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "' where nom_fleur='" + infobouq[0, i] + "' and id_magasin='" + magasin + "';";
                                        //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.85*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ magasin+"';");
                                        MySqlDataReader reader1;
                                        reader1 = command1.ExecuteReader();
                                        reader1.Close();
                                        connection1.Close();
                                        compobouquet += infobouq[0, i] + ", ";
                                    }
                                }
                                else if (Fidelite(email) == "bronze")
                                {
                                    if (budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString())) > 0)
                                    {
                                        if (0.95 * Convert.ToDouble(reader.GetValue(1).ToString()) < min)
                                        {
                                            min = 0.95 * Convert.ToDouble(reader.GetValue(1).ToString());
                                        }
                                        budget = budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString()));
                                        int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                        string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                                        MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                        connection1.Open();
                                        MySqlCommand command1 = connection1.CreateCommand();
                                        command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "' where nom_fleur='" + infobouq[0, i] + "' and id_magasin='" + magasin + "';";
                                        //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.95*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ magasin+"';");
                                        MySqlDataReader reader1;
                                        reader1 = command1.ExecuteReader();
                                        reader1.Close();
                                        connection1.Close();
                                        compobouquet += infobouq[0, i] + ", ";
                                    }
                                }
                                else // pas de fidélité
                                {
                                    if (budget - Convert.ToDouble(reader.GetValue(1).ToString()) > 0)
                                    {
                                        if (Convert.ToDouble(reader.GetValue(1).ToString()) < min)
                                        {
                                            min = Convert.ToDouble(reader.GetValue(1).ToString());
                                        }
                                        budget = budget - Convert.ToDouble(reader.GetValue(1).ToString());
                                        int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                        string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                                        MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                        connection1.Open();
                                        MySqlCommand command1 = connection1.CreateCommand();
                                        command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "' where nom_fleur='" + infobouq[0, i] + "' and id_magasin='" + magasin + "';";
                                        //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ magasin+"';");
                                        MySqlDataReader reader1;
                                        reader1 = command1.ExecuteReader();
                                        reader1.Close();
                                        connection1.Close();
                                        compobouquet += infobouq[0, i] + ", ";
                                    }
                                }
                            }
                        }
                        else //L'employé va voir dans un autre magasin
                        {
                            if (Int32.Parse(reader.GetValue(0).ToString()) == 0)
                            {
                                Console.WriteLine("La fleur " + infobouq[0, i] + " est en rupture de stock dans tout nos magasins");
                            }
                            else //MAJ du stock si suffisamment d'argent
                            {
                                if (Fidelite(email) == "or")
                                {
                                    if (budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2 > 0) // frais de port en plus
                                    {
                                        Console.WriteLine("Un employé vous informe que la fleur " + infobouq[0, i] + " n'est plus disponible dans votre magasin de commande mais qu'elle est disponible dans le magasin: " + reader.GetValue(2).ToString() + " pour 2 euros de frais de port en plus. Voulez-vous tout de même l'acheter? (oui ou non)");
                                        string achat = Console.ReadLine();
                                        if (achat == "oui")
                                        {
                                            if (0.85 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2 < min)
                                            {
                                                min = 0.85 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2;
                                            }
                                            budget = budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2;
                                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effectué par employé ou admin
                                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                            connection1.Open();
                                            MySqlCommand command1 = connection1.CreateCommand();
                                            command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "'where nom_fleur='" + infobouq[0, i] + "' and id_magasin='" + reader.GetValue(2).ToString() + "';";
                                            //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.85*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ reader.GetValue(2).ToString()+"';");
                                            MySqlDataReader reader1;
                                            reader1 = command1.ExecuteReader();
                                            reader1.Close();
                                            connection1.Close();
                                            compobouquet += infobouq[0, i] + ", ";
                                        }
                                    }
                                }
                                else if (Fidelite(email) == "bronze")
                                {
                                    if (budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2 > 0) // frais de port en plus
                                    {
                                        Console.WriteLine("Un employé vous informe que la fleur " + infobouq[0, i] + " n'est plus disponible dans votre magasin de commande mais qu'elle est disponible dans le magasin: " + reader.GetValue(2).ToString() + " pour 2 euros de frais de port en plus. Voulez-vous tout de même l'acheter? (oui ou non)");
                                        string achat = Console.ReadLine();
                                        if (achat == "oui")
                                        {
                                            if (0.95 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2 < min)
                                            {
                                                min = 0.95 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2;
                                            }
                                            budget = budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2;
                                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effectué par employé ou admin
                                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                            connection1.Open();
                                            MySqlCommand command1 = connection1.CreateCommand();
                                            command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "'where nom_fleur='" + infobouq[0, i] + "' and id_magasin='" + reader.GetValue(2).ToString() + "';";
                                            //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.95*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ reader.GetValue(2).ToString()+"';");
                                            MySqlDataReader reader1;
                                            reader1 = command1.ExecuteReader();
                                            reader1.Close();
                                            connection1.Close();
                                            compobouquet += infobouq[0, i] + ", ";
                                        }
                                    }
                                }
                                else
                                {
                                    if (budget - Convert.ToDouble(reader.GetValue(1).ToString()) - 2 > 0) // frais de port en plus
                                    {
                                        Console.WriteLine("Un employé vous informe que la fleur " + infobouq[0, i] + " n'est plus disponible dans votre magasin de commande mais qu'elle est disponible dans le magasin: " + reader.GetValue(2).ToString() + " pour 2 euros de frais de port en plus. Voulez-vous tout de même l'acheter? (oui ou non)");
                                        string achat = Console.ReadLine();
                                        if (achat == "oui")
                                        {
                                            if (Convert.ToDouble(reader.GetValue(1).ToString()) + 2 < min)
                                            {
                                                min = Convert.ToDouble(reader.GetValue(1).ToString()) + 2;
                                            }
                                            budget = budget - Convert.ToDouble(reader.GetValue(1).ToString()) - 2;
                                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                            connection1.Open();
                                            MySqlCommand command1 = connection1.CreateCommand();
                                            command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "'where nom_fleur='" + infobouq[0, i] + "' and id_magasin='" + reader.GetValue(2).ToString() + "';";
                                            //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ reader.GetValue(2).ToString()+"';");
                                            MySqlDataReader reader1;
                                            reader1 = command1.ExecuteReader();
                                            reader1.Close();
                                            connection1.Close();
                                            compobouquet += infobouq[0, i] + ", ";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            for (int i = 0; i < infobouq.GetLength(1); i++)  // Même chose pour l'accessoire
            {
                if (infobouq[1, i] != "")
                {
                    string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "Select stock_acc,prix_acc,accessoire.id_magasin,ca from accessoire join magasin on accessoire.id_magasin=magasin.id_magasin where nom_acc='" + infobouq[1, i] + "';";
                    MySqlDataReader reader;
                    reader = command.ExecuteReader();
                    while (reader.Read())                           // parcours ligne par ligne
                    {
                        if (reader.GetValue(2).ToString() == magasin)
                        {
                            if (Int32.Parse(reader.GetValue(0).ToString()) == 0)
                            {
                                Console.WriteLine("L'accessoire " + infobouq[1, i] + " est en rupture de stock dans ce magasin");
                            }
                            else
                            {
                                if (Fidelite(email) == "or")
                                {
                                    if (budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString())) > 0)
                                    {
                                        if (0.85 * Convert.ToDouble(reader.GetValue(1).ToString()) < min)
                                        {
                                            min = 0.85 * Convert.ToDouble(reader.GetValue(1).ToString());
                                        }
                                        budget = budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString()));
                                        int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                        string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                                        MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                        connection1.Open();
                                        MySqlCommand command1 = connection1.CreateCommand();
                                        command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "'where nom_acc='" + infobouq[1, i] + "' and id_magasin='" + magasin + "';";
                                        // Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.85*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ magasin+"';");
                                        MySqlDataReader reader1;
                                        reader1 = command1.ExecuteReader();
                                        reader1.Close();
                                        connection1.Close();
                                        compobouquet += infobouq[1, i] + ", ";
                                    }
                                }
                                else if (Fidelite(email) == "bronze")
                                {
                                    if (budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString())) > 0)
                                    {
                                        if (0.95 * Convert.ToDouble(reader.GetValue(1).ToString()) < min)
                                        {
                                            min = 0.95 * Convert.ToDouble(reader.GetValue(1).ToString());
                                        }
                                        budget = budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString()));
                                        int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                        string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                                        MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                        connection1.Open();
                                        MySqlCommand command1 = connection1.CreateCommand();
                                        command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "'where nom_acc='" + infobouq[1, i] + "' and id_magasin='" + magasin + "';";
                                        //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.95*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ magasin+"';");
                                        MySqlDataReader reader1;
                                        reader1 = command1.ExecuteReader();
                                        reader1.Close();
                                        connection1.Close();
                                        compobouquet += infobouq[1, i] + ", ";
                                    }
                                }
                                else
                                {
                                    if (budget - Convert.ToDouble(reader.GetValue(1).ToString()) > 0)
                                    {
                                        if (Convert.ToDouble(reader.GetValue(1).ToString()) < min)
                                        {
                                            min = Convert.ToDouble(reader.GetValue(1).ToString());
                                        }
                                        budget = budget - Convert.ToDouble(reader.GetValue(1).ToString());
                                        int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                        string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                                        MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                        connection1.Open();
                                        MySqlCommand command1 = connection1.CreateCommand();
                                        command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "'where nom_acc='" + infobouq[1, i] + "' and id_magasin='" + magasin + "';";
                                        //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ magasin+"';");
                                        MySqlDataReader reader1;
                                        reader1 = command1.ExecuteReader();
                                        reader1.Close();
                                        connection1.Close();
                                        compobouquet += infobouq[1, i] + ", ";
                                    }
                                }
                            }
                        }
                        else //L'employé va voir dans un autre magasin
                        {
                            if (Int32.Parse(reader.GetValue(0).ToString()) == 0)
                            {
                                Console.WriteLine("L'accessoire " + infobouq[1, i] + " est en rupture de stock dans tout nos magasins");
                            }
                            else //MAJ du stock si suffisemment d'argent
                            {
                                if (Fidelite(email) == "or")
                                {
                                    if (budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2 > 0) // frais de port en plus
                                    {
                                        Console.WriteLine("Un employé vous informe que l'accessoire " + infobouq[1, i] + " n'est plus disponible dans votre magasin de commande mais qu'il est disponible dans le magasin: " + reader.GetValue(2).ToString() + " pour 2 euros de plus avec frais de port. Voulez-vous tout de même l'acheter? (oui ou non)");
                                        string achat = Console.ReadLine();
                                        if (achat == "oui")
                                        {
                                            if (0.85 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2 < min)
                                            {
                                                min = 0.85 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2;
                                            }
                                            budget = budget - (0.85 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2;
                                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                            connection1.Open();
                                            MySqlCommand command1 = connection1.CreateCommand();
                                            command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "'where nom_acc='" + infobouq[1, i] + "' and id_magasin='" + reader.GetValue(2).ToString() + "';";
                                            // Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.85*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ reader.GetValue(2).ToString()+"';");
                                            MySqlDataReader reader1;
                                            reader1 = command1.ExecuteReader();
                                            reader1.Close();
                                            connection1.Close();
                                            compobouquet += infobouq[1, i] + ", ";
                                        }
                                    }
                                }
                                else if (Fidelite(email) == "bronze")
                                {
                                    if (budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2 > 0) // Frais de port en plus
                                    {
                                        Console.WriteLine("Un employé vous informe que l'accessoire " + infobouq[1, i] + " n'est plus disponible dans votre magasin de commande mais qu'il est disponible dans le magasin: " + reader.GetValue(2).ToString() + " pour 2 euros de plus avec frais de port. Voulez-vous tout de même l'acheter? (oui ou non)");
                                        string achat = Console.ReadLine();
                                        if (achat == "oui")
                                        {
                                            if (0.95 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2 < min)
                                            {
                                                min = 0.95 * Convert.ToDouble(reader.GetValue(1).ToString()) + 2;
                                            }
                                            budget = budget - (0.95 * Convert.ToDouble(reader.GetValue(1).ToString())) - 2;
                                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effectué par employé ou admin
                                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                            connection1.Open();
                                            MySqlCommand command1 = connection1.CreateCommand();
                                            command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "'where nom_acc='" + infobouq[1, i] + "' and id_magasin='" + reader.GetValue(2).ToString() + "';";
                                            //Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+0.95*Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ reader.GetValue(2).ToString()+"';");
                                            MySqlDataReader reader1;
                                            reader1 = command1.ExecuteReader();
                                            reader1.Close();
                                            connection1.Close();
                                            compobouquet += infobouq[1, i] + ", ";
                                        }
                                    }
                                }
                                else
                                {
                                    if (budget - Convert.ToDouble(reader.GetValue(1).ToString()) - 2 > 0) // Frais de port en plus
                                    {
                                        Console.WriteLine("Un employé vous informe que l'accessoire " + infobouq[1, i] + " n'est plus disponible dans votre magasin de commande mais qu'il est disponible dans le magasin: " + reader.GetValue(2).ToString() + " pour 2 euros de plus avec frais de port. Voulez-vous tout de même l'acheter? (oui ou non)");
                                        string achat = Console.ReadLine();
                                        if (achat == "oui")
                                        {
                                            if (Convert.ToDouble(reader.GetValue(1).ToString()) + 2 < min)
                                            {
                                                min = Convert.ToDouble(reader.GetValue(1).ToString()) + 2;
                                            }
                                            budget = budget - Convert.ToDouble(reader.GetValue(1).ToString()) - 2;
                                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effectué par employé ou admin
                                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                                            connection1.Open();
                                            MySqlCommand command1 = connection1.CreateCommand();
                                            command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "'where nom_acc='" + infobouq[1, i] + "' and id_magasin='" + reader.GetValue(2).ToString() + "';";
                                            // Requetesql("update magasin set ca='"+ Convert.ToDouble(reader.GetValue(3).ToString())+Convert.ToDouble(reader.GetValue(1).ToString()) +"' where id_magasin='"+ reader.GetValue(2).ToString()+"';");
                                            MySqlDataReader reader1;
                                            reader1 = command1.ExecuteReader();
                                            reader1.Close();
                                            connection1.Close();
                                            compobouquet += infobouq[1, i] + ", ";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            infobouq[2, 0] = budget.ToString();
            if (budget >= min) // Si on peut encore acheter les éléments souhaités, alors on continue les achats
            {
                Verificationstockcommande(infobouq, magasin, compobouquet, email); // récursivité
            }
            return compobouquet;
        }
        static string[] Commande(string bouquet, string email, string magasin) // Renvoie info bouquet standard choisis avec son prix
        {
            string[] info = new string[2];
            string connectionString2 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection2 = new MySqlConnection(connectionString2);
            connection2.Open();
            MySqlCommand command2 = connection2.CreateCommand();
            command2.CommandText = "Select ca from magasin where id_magasin='" + magasin + "';";
            MySqlDataReader reader2;
            reader2 = command2.ExecuteReader();
            double ca = 0;
            while (reader2.Read())
            {
                ca = Convert.ToDouble(reader2.GetValue(0).ToString());
            }
            reader2.Close();
            connection2.Close();
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select * from bouquet where nom_bouquet='" + bouquet + "';";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine("Voici les infos du bouquet choisi:");
            Console.WriteLine();
            string fleur = "";
            string acc = "";
            double prix = 0;
            string nb = "";
            while (reader.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                prix = Convert.ToDouble(reader.GetValue(1).ToString());
                nb = (reader.GetValue(7).ToString());
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader.GetValue(i).ToString();  // Récupération de la valeur de chaque cellule sous forme d'un string 
                    currentRowAsString += valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'un "gros" string) sur la sortie standard
                fleur += reader.GetValue(4).ToString();
                acc += reader.GetValue(5).ToString();
            }
            if (Fidelite(email) == "or")
            {
                double nprix = prix - (prix * 15 / 100);
                ca += nprix;
                Console.WriteLine();
                Console.WriteLine("Grâce a votre remise fidélité, ce bouquet ne vous couteras que " + nprix + " euros");
                //Requetesql("update magasin set ca="+Convert.ToDouble(ca)+" where id_magasin='" + magasin +"';");
            }
            else if (Fidelite(email) == "bronze")
            {
                double nprix = prix - (prix * 5 / 100);
                ca += nprix;
                Console.WriteLine();
                Console.WriteLine("Grâce a votre remise fidélité, ce bouquet ne vous couteras que " + nprix + " euros");
                //Requetesql("update magasin set ca='"+ca+"' where id_magasin='" + magasin+"';");
            }
            else
            {
                ca += prix;
                //Requetesql("update magasin set ca='"+ca+"' where id_magasin='" + magasin +"';");
            }
            reader.Close();
            connection.Close();
            info[0] = fleur;
            info[1] = acc;
            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";
            MySqlConnection connection1 = new MySqlConnection(connectionString1);
            connection1.Open();
            MySqlCommand command1 = connection1.CreateCommand();
            command1.CommandText = "UPDATE bouquet SET nb_commande='" + (Int32.Parse(nb) + 1).ToString() + "' where nom_bouquet='" + bouquet + "';";
            MySqlDataReader reader1;
            reader1 = command1.ExecuteReader();
            return info;
        }
        static void AnalyseStock(string[] info, string magasin) //Maj des stocks après la commande
        {
            string[] fl = info[0].Split(", ");
            string[] ac = info[1].Split(", ");
            int cp = 0;
            Console.WriteLine();
            for (int i = 0; i < fl.Length; i++)  // stock fleur
            {
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectué par l'employé ou admin
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "Select stock_fleur,id_magasin from fleur where nom_fleur='" + fl[i] + "';";
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                while (reader.Read())                           // parcours ligne par ligne
                {
                    if (reader.GetValue(1).ToString() == magasin)
                    {
                        if (Int32.Parse(reader.GetValue(0).ToString()) == 0)
                        {
                            Console.WriteLine("La fleur " + fl[i].ToUpper() + " est en rupture de stock dans ce magasin et ne sera donc pas présent dans votre bouquet");
                            cp += 1;
                        }
                        else //MAJ du stock 
                        {
                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effectuée par employé ou admin
                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                            connection1.Open();
                            MySqlCommand command1 = connection1.CreateCommand();
                            command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "' where nom_fleur='" + fl[i] + "' and id_magasin='" + magasin + "';";
                            MySqlDataReader reader1;
                            reader1 = command1.ExecuteReader();
                            reader1.Close();
                            connection1.Close();
                        }
                    }
                    else  // Cas où items en stock dans un autre magasin
                    {
                        cp += 1;
                        Console.WriteLine("La fleur " + fl[i].ToUpper() + " est en rupture de stock dans ce magasin, elle est présente dans d'autres magasins, mais contenue des délai de livraison demandé, elle ne seras pas présente dans votre bouquet.");
                        // Faire communiquer employé avec client pour faire une nouvelle commande ou changer date de livraison !
                    }
                }
            }
            for (int i = 0; i < fl.Length; i++)  // Stockage des accessoires
            {
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectué par l'employé ou admin
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "Select stock_acc,id_magasin from accessoire where nom_acc='" + ac[i] + "';";
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                while (reader.Read())                           // parcours ligne par ligne
                {
                    if (reader.GetValue(1).ToString() == magasin)
                    {
                        if (Int32.Parse(reader.GetValue(0).ToString()) == 0)
                        {
                            Console.WriteLine("L'accessoire " + ac[i].ToUpper() + " est en rupture de stock dans ce magasin et ne sera donc pas présent dans votre bouquet");
                            cp += 1;
                        }
                        else //MAJ du stock 
                        {
                            int newstock = Int32.Parse(reader.GetValue(0).ToString()) - 1;
                            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effecutué par employé ou admin
                            MySqlConnection connection1 = new MySqlConnection(connectionString1);
                            connection1.Open();
                            MySqlCommand command1 = connection1.CreateCommand();
                            command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "'where nom_acc='" + ac[i] + "' and id_magasin='" + magasin + "';";
                            MySqlDataReader reader1;
                            reader1 = command1.ExecuteReader();
                            reader1.Close();
                            connection1.Close();
                        }
                    }
                    else  // Cas où items en stock dans un autre magasin
                    {
                        cp += 1;
                        Console.WriteLine("L'accessoire " + ac[i].ToUpper() + " est en rupture de stock dans ce magasin, il est présent dans d'autres magasins, mais contenue des délai de livraison demandé, il ne seras pas présent dans votre bouquet.");
                    }
                }
            }
            if (cp == 0)
            {
                Console.WriteLine("Votre commande est complète et en stock dans notre magasin!");
            }
        }
        static void Etat(string etat, string numcommande) // MAJ du code, état de la commande
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectué par l'employé ou admin
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE boncommande SET etat='" + etat + "'where num_commande='" + Int32.Parse(numcommande) + "';";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();
            connection.Close();
        }
        static string Fidelite(string email) //MAJ du statut de fidélité du client
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select num_commande,date_commande from boncommande where courriel='" + email + "';";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            List<int> dure = new List<int>();
            List<string> date = new List<string>();
            while (reader.Read())
            {
                date.Add(reader.GetValue(1).ToString());
            }
            reader.Close();
            connection.Close();
            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection1 = new MySqlConnection(connectionString1);
            connection1.Open();
            MySqlCommand command1 = connection1.CreateCommand();
            command1.CommandText = "SELECT NOW();"; // On sélectionne la date et l'heure de la commande
            MySqlDataReader reader1;
            reader1 = command1.ExecuteReader();
            string dcommande = "";// on assigne la variable avant la boucle
            while (reader1.Read())
            {
                dcommande = reader1.GetValue(0).ToString();
            }
            reader1.Close();
            connection1.Close();
            for (int i = 0; i < date.Count; i++)
            {
                string datec = date[i];
                string j1 = datec[0].ToString();
                string j2 = datec[1].ToString();
                string m1 = datec[3].ToString();
                string m2 = datec[4].ToString();
                string a1 = datec[6].ToString();
                string a2 = datec[7].ToString();
                string a3 = datec[8].ToString();
                string a4 = datec[9].ToString();
                int anneel = Int32.Parse(a1 + a2 + a3 + a4);
                int moisl = Int32.Parse(m1 + m2);
                int jourl = Int32.Parse(j1 + j2);
                string j1c = dcommande[0].ToString();
                string j2c = dcommande[1].ToString();
                string m1c = dcommande[3].ToString();
                string m2c = dcommande[4].ToString();
                string a1c = dcommande[6].ToString();
                string a2c = dcommande[7].ToString();
                string a3c = dcommande[8].ToString();
                string a4c = dcommande[9].ToString();
                int anneec = Int32.Parse(a1c + a2c + a3c + a4c);
                int moisc = Int32.Parse(m1c + m2c);
                int jourc = Int32.Parse(j1c + j2c);
                int duree = 365 * (anneec - anneel) + 30 * (moisc - moisl) + (jourc - jourl);
                dure.Add(duree);
            }
            int bronze = 0;
            int or = 0;
            for (int i = 0; i < dure.Count; i++)
            {
                bronze += dure[i];
                if (dure[i] <= 30) //Un mois dure en moyenne 30 jours sur une année, on parle de fidélité or que si le client vient plus de 5 fois lors du dernier mois
                {
                    or++;
                }
            }
            if (Convert.ToDouble(bronze) / dure.Count < 30)  // Le client achète en moyenne 1 bouquet par mois
            {
                string connectionString2 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectué par l'employé ou admin
                MySqlConnection connection2 = new MySqlConnection(connectionString2);
                connection2.Open();
                MySqlCommand command2 = connection2.CreateCommand();
                command2.CommandText = "UPDATE client SET statut_fidelite='bronze' WHERE courriel='" + email + "';";
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                reader2.Close();
                connection2.Close();
            }
            if (or >= 5) // Le client a déjà acheté 5 bouquets ce mois-ci, on fait une fidélité or après puisque c'est un grade supérieur de fidélité
            {
                string connectionString2 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectué par l'employé ou admin
                MySqlConnection connection2 = new MySqlConnection(connectionString2);
                connection2.Open();
                MySqlCommand command2 = connection2.CreateCommand();
                command2.CommandText = "UPDATE client SET statut_fidelite='or' WHERE courriel='" + email + "';";
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                reader2.Close();
                connection2.Close();
                return "or";
            }
            if (Convert.ToDouble(bronze) / dure.Count >= 30 && or < 5) // Perte du statut de fidélité du client s'il en avait un
            {
                string connectionString2 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectuée par l'employé ou admin
                MySqlConnection connection2 = new MySqlConnection(connectionString2);
                connection2.Open();
                MySqlCommand command2 = connection2.CreateCommand();
                command2.CommandText = "UPDATE client SET statut_fidelite='Aucun' WHERE courriel='" + email + "';";
                MySqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                reader2.Close();
                connection2.Close();
                return "Aucun";
            }
            return "bronze";
        }

        static void Appro() //Restockage fleur et accessoire
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectuée par l'employé ou admin
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select * from fleur;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int stock = 0;
            int seuil = 0;
            while (reader.Read())                           // Parcours ligne par ligne
            {
                stock = Int32.Parse(reader.GetValue(3).ToString());
                seuil = Int32.Parse(reader.GetValue(5).ToString());
                if (seuil > stock && Disponible(reader.GetValue(0).ToString()) == true)
                {
                    Console.WriteLine("Danger, la fleur " + reader.GetValue(0).ToString() + " est pas loin d'une rupture de stock, il faut réapprovisionner");
                    int newstock = seuil + 20;
                    string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effectuée par employé ou admin
                    MySqlConnection connection1 = new MySqlConnection(connectionString1);
                    connection1.Open();
                    MySqlCommand command1 = connection1.CreateCommand();
                    command1.CommandText = "UPDATE fleur SET stock_fleur='" + newstock + "' where nom_fleur='" + reader.GetValue(0).ToString() + "' and id_magasin='" + reader.GetValue(6).ToString() + "';";
                    MySqlDataReader reader1;
                    reader1 = command1.ExecuteReader();
                    reader1.Close();
                    connection1.Close();
                }
            }
            reader.Close();
            connection.Close();

            string connectionString2 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectuée par l'employé ou admin
            MySqlConnection connection2 = new MySqlConnection(connectionString2);
            connection2.Open();
            MySqlCommand command2 = connection2.CreateCommand();
            command2.CommandText = "Select * from accessoire;";
            MySqlDataReader reader2;
            reader2 = command2.ExecuteReader();
            int stock2 = 0;
            int seuil2 = 0;
            while (reader2.Read())                           // parcours ligne par ligne
            {
                stock2 = Int32.Parse(reader2.GetValue(3).ToString());
                seuil2 = Int32.Parse(reader2.GetValue(4).ToString());
                if (seuil2 > stock2)
                {
                    Console.WriteLine("Danger, l'accessoire " + reader2.GetValue(0).ToString() + " est pas loin d'une rupture de stock, il faut réapprocissionner");
                    int newstock = Int32.Parse(reader2.GetValue(0).ToString()) + 20;
                    string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";//MAJ effectuée par employé ou admin
                    MySqlConnection connection1 = new MySqlConnection(connectionString1);
                    connection1.Open();
                    MySqlCommand command1 = connection1.CreateCommand();
                    command1.CommandText = "UPDATE accessoire SET stock_acc='" + newstock + "' where nom_acc='" + reader2.GetValue(0).ToString() + "' and id_magasin='" + reader2.GetValue(5).ToString() + "';";
                    MySqlDataReader reader1;
                    reader1 = command1.ExecuteReader();
                    reader1.Close();
                    connection1.Close();
                }
            }
            reader2.Close();
            connection2.Close();
        }
        static bool Disponible(string nom) // Verifie si la fleur est disponible ou non
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select disponibilite_fleur from fleur where nom_fleur='" + nom + "';";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                string dispo = reader.GetValue(0).ToString();
                if (dispo == "à lannée")
                {
                    return true;
                }
                string[] mois = dispo.Split(", ");
                for (int i = 0; i < mois.Length; i++)
                {
                    if (mois[i].ToLower() == "janvier")
                    {
                        mois[i] = "01";
                    }
                    if (mois[i].ToLower() == "février")
                    {
                        mois[i] = "02";
                    }
                    if (mois[i].ToLower() == "mars")
                    {
                        mois[i] = "03";
                    }
                    if (mois[i].ToLower() == "avril")
                    {
                        mois[i] = "04";
                    }
                    if (mois[i].ToLower() == "mai")
                    {
                        mois[i] = "05";
                    }
                    if (mois[i].ToLower() == "juin")
                    {
                        mois[i] = "06";
                    }
                    if (mois[i].ToLower() == "juillet")
                    {
                        mois[i] = "07";
                    }
                    if (mois[i].ToLower() == "août")
                    {
                        mois[i] = "08";
                    }
                    if (mois[i].ToLower() == "septembre")
                    {
                        mois[i] = "09";
                    }
                    if (mois[i].ToLower() == "octobre")
                    {
                        mois[i] = "10";
                    }
                    if (mois[i].ToLower() == "novembre")
                    {
                        mois[i] = "11";
                    }
                    if (mois[i].ToLower() == "décembre")
                    {
                        mois[i] = "12";
                    }
                }
                string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
                MySqlConnection connection1 = new MySqlConnection(connectionString1);
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT NOW();"; // On sélectionne la date et l'heure 
                MySqlDataReader reader1;
                reader1 = command1.ExecuteReader();
                string dcommande = "";// On assigne la variable avant la boucle
                while (reader1.Read())
                {
                    dcommande = reader1.GetValue(0).ToString();
                }
                reader1.Close();
                connection1.Close();
                string m1 = dcommande[3].ToString();
                string m2 = dcommande[4].ToString();
                string m = m1 + m2;
                if (mois.Contains(m))
                {
                    return true;
                }
            }
            reader.Close();
            connection.Close();
            return false;
        }

        static int Temps()
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides, connexion client ici!!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select now();";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string temps = "";
            while (reader.Read())                           // parcours ligne par ligne
            {
                temps = reader.GetValue(0).ToString();  // Récupération de la valeur de chaque cellule sous forme d'un string                
            }
            string j1 = temps[0].ToString();
            string j2 = temps[1].ToString();
            int j = Int32.Parse(j1 + j2);
            reader.Close();
            connection.Close();
            return j;
        }

        static void Requetesql(string requete)
        {
            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides, connexion admin ici!!!
            string connectionString5 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; // demande de M. Bellefleur
            MySqlConnection connection5 = new MySqlConnection(connectionString5);
            connection5.Open();
            MySqlCommand command5 = connection5.CreateCommand();
            command5.CommandText = requete;
            MySqlDataReader reader5;
            reader5 = command5.ExecuteReader();
            while (reader5.Read())                           // parcours ligne par ligne
            {
                string currentRowAsString = " ";
                for (int i = 0; i < reader5.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader5.GetValue(i).ToString();  // Récupération de la valeur de chaque cellule sous forme d'une string 
                    currentRowAsString += valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);    // Affichage de la ligne (sous forme d'un "grosse" string) sur la sortie standard
            }
            reader5.Close();
            connection5.Close();
        }

        static void Stock() //Permet d'afficher les stocks en magasin et d'afficher les items qui sont en risque de pénurie
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectuée par l'employé ou admin
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select stock_fleur,seuil_fleur,nom_fleur,id_magasin from fleur;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            Console.WriteLine("Stock:, Seuil:, Nom:, Magasin:");
            while (reader.Read())                           // parcours ligne par ligne
            {
                if (Int32.Parse(reader.GetValue(1).ToString()) > Int32.Parse(reader.GetValue(0).ToString()))
                {
                    Console.WriteLine("Attention la fleur'" + reader.GetValue(2).ToString() + "' est en risque de pénurie il faut réapprovisionner;");
                }
                string currentRowAsString = " ";
                for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader.GetValue(i).ToString();  // Récupération de la valeur de chaque cellule sous forme d'une string 
                    currentRowAsString += valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();
            connection.Close();
            string connectionString1 = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;"; //MAJ effectuée par l'employé ou admin
            MySqlConnection connection1 = new MySqlConnection(connectionString1);
            connection1.Open();
            MySqlCommand command1 = connection1.CreateCommand();
            command1.CommandText = "Select stock_acc, seuil_acc, nom_acc, id_magasin from accessoire;";
            MySqlDataReader reader1;
            reader1 = command1.ExecuteReader();
            while (reader1.Read())                           // parcours ligne par ligne
            {
                if (Int32.Parse(reader1.GetValue(1).ToString()) > Int32.Parse(reader1.GetValue(0).ToString()))
                {
                    Console.WriteLine("Attention l'accessoire +'" + reader1.GetValue(2).ToString() + "' est en risque de pénurie il faut réapprovisionner;");
                }
                string currentRowAsString = " ";
                for (int i = 0; i < reader1.FieldCount; i++)    // parcours cellule par cellule
                {
                    string valueAsString = reader1.GetValue(i).ToString();  // Récupération de la valeur de chaque cellule sous forme d'une string 
                    currentRowAsString += valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader1.Close();
            connection1.Close();
        }

        static void Etatcommande() //Permet de voir l'état des commandes des client
        {
            Console.WriteLine();
            Console.WriteLine("Etat:, magasin:, date_commande:");
            Requetesql("select etat, id_magasin, date_commande from boncommande order by id_magasin,now();");
            Console.WriteLine("'CC' = commande COMPLETE , 'CAL' = commande A LIVRER, 'CL'= commande LIVRER");
        }
        static void Etatcommande1(string email) //Permet de voir l'état des commandes du client
        {
            Console.WriteLine();
            Console.WriteLine("Etat:, magasin:, date_commande:");
            Requetesql("select etat, id_magasin, date_commande from boncommande where courriel='" + email + "' order by id_magasin,now();");
            Console.WriteLine("'CC' = commande COMPLETE , 'CAL' = commande A LIVRER, 'CL'= commande LIVRER");
        }
        // export Xml
        static void exportXML()
        {
            string Datea = DateTime.Now.ToString("dd-MM-yyyy");
            string Datem = DateTime.Now.AddMonths(-1).ToString("dd-MM-yyyy");
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=bozo;PASSWORD=bozo;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT nom, prenom, client.courriel, date_commande FROM client JOIN boncommande ON client.courriel = boncommande.courriel WHERE date_commande BETWEEN '" + Datem + "' AND '" + Datea + "';";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("clients");
            doc.AppendChild(root);

            while (reader.Read())
            {

                XmlElement client = doc.CreateElement("client");
                root.AppendChild(client);

                XmlElement nom = doc.CreateElement("nom");
                nom.InnerText = reader.GetValue(0).ToString();
                client.AppendChild(nom);
                XmlElement prenom = doc.CreateElement("prenom");
                prenom.InnerText = reader.GetValue(1).ToString();
                client.AppendChild(prenom);
                XmlElement courriel = doc.CreateElement("courriel");
                courriel.InnerText = reader.GetValue(2).ToString();
                client.AppendChild(courriel);
                XmlElement date_commande = doc.CreateElement("date_commande");
                date_commande.InnerText = reader.GetValue(3).ToString();
                client.AppendChild(date_commande);

            }
            reader.Close();

            // export des données XML dans un fichier
            doc.Save("C:\\Users\\aches\\Documents\\ESILV\\A3\\Info\\BDD\\Fleur\\donnees.xml");
            Console.WriteLine("Données exportées dans le fichier donnees.xml");
            Console.ReadLine();
        }
        static void exportjson()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=bellefleur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "SELECT * FROM client WHERE courriel NOT IN (SELECT courriel FROM boncommande WHERE date_commande >= DATE_SUB(NOW(), INTERVAL 6 MONTH));";
            MySqlCommand command = new MySqlCommand(query, connection);

            MySqlDataReader clients = command.ExecuteReader();
            List<Client> liste_clients = new List<Client>();
            while (clients.Read())
            {
                Client client = new Client();
                client.Courriel = Convert.ToString(clients["Courriel"]);
                liste_clients.Add(client);
            }
            // Sérialisation des clients
            string json = JsonConvert.SerializeObject(liste_clients, Newtonsoft.Json.Formatting.Indented);
            clients.Close();
            connection.Close();
            System.IO.File.WriteAllText("fleur.json", json);
            Console.WriteLine("Données exportées dans le fichier fleur.json");
        }


        public class Client //classe Client pour représenter les info du client
        {
            public string Courriel { get; set; }
            public Client() //constructeur par défaut
            {
            }
            public Client(string courriel)
            {
                Courriel = courriel;
            }
        }
    }
}
