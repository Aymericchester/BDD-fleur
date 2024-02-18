DROP DATABASE IF EXISTS bellefleur;
CREATE DATABASE IF NOT EXISTS bellefleur;
USE bellefleur;

DROP TABLE IF EXISTS client;
CREATE TABLE IF NOT EXISTS client (
nom varchar(40), 
prenom varchar(40), 
num_tel varchar(40), 
courriel varchar(100) PRIMARY KEY NOT NULL, 
mdp varchar(40), 
adresse_facturation varchar(100), 
carte_de_credit varchar(40), 
statut_fidelite varchar(40), 
achat_passe varchar(400)); #mettre num_commande et date commande? il faudra aussi faire attention au minuscule/majuscule

DROP TABLE IF EXISTS magasin;
CREATE TABLE IF NOT EXISTS magasin (
id_magasin varchar(40) PRIMARY KEY NOT NULL, 
adresse_magasin varchar(40),
ca double);

DROP TABLE IF EXISTS boncommande;
CREATE TABLE IF NOT EXISTS boncommande (
num_commande int PRIMARY KEY NOT NULL, 
etat varchar(4), 
date_commande varchar(40), 
adresse_livraison varchar(100),
date_livraison_desire varchar(40), 
message varchar(400), 
courriel varchar(100), 
id_magasin varchar(40), 
FOREIGN KEY (courriel) REFERENCES client (courriel),
FOREIGN KEY (id_magasin) REFERENCES magasin (id_magasin));

DROP TABLE IF EXISTS fleur;
CREATE TABLE IF NOT EXISTS fleur (
nom_fleur varchar(40) PRIMARY KEY NOT NULL, 
prix_fleur double, 
origine_fleur varchar(40), 
stock_fleur int, 
disponibilite_fleur varchar(40), 
seuil_fleur int, 
id_magasin varchar(40),
nbvendu int,
FOREIGN KEY (id_magasin) REFERENCES magasin (id_magasin)); 

DROP TABLE IF EXISTS accessoire;
CREATE TABLE IF NOT EXISTS accessoire (
nom_acc varchar(40) PRIMARY KEY NOT NULL, 
prix_acc double, 
origine_acc varchar(40), 
stock_acc int, 
seuil_acc int,  
id_magasin varchar(40), 
FOREIGN KEY (id_magasin) REFERENCES magasin (id_magasin));

DROP TABLE IF EXISTS bouquet;
CREATE TABLE IF NOT EXISTS bouquet (
nom_bouquet varchar(40) PRIMARY KEY NOT NULL, 
prix_bouquet double, 
categorie_bouquet varchar(40), 
type_bouquet varchar(40), 
nom_fleur varchar(100), 
nom_acc varchar(100), 
num_commande int, 
nb_commande int, #
#FOREIGN KEY (nom_fleur) REFERENCES fleur (nom_fleur), 
#FOREIGN KEY (nom_acc) REFERENCES accessoire (nom_acc), 
FOREIGN KEY (num_commande) REFERENCES boncommande (num_commande));

DROP TABLE IF EXISTS temps;  #table pour gérer les réapprovisionnements
CREATE TABLE IF NOT EXISTS temps (
temps int);

-- Insertion pour la table client.
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('koshka', 'yaroslav', '612345678', 'yaroslav.koshka@hotmail.com', 'azerty', '5 rue de la paix', '9247-7688-5573-6939', 'bronze', '1, 2');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('porochenko', 'petro', '645678901', 'petro.porochenko@gmail.com', 'oejizoeg', '10 avenue de la rose', '1316-6902-4007-0458', '0', '3');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('kurylenko', 'olga', '698765432', 'olga.kurylenko@outlook.com', 'zrujg', '15 rue des lilas', '8173-2568-9315-8217', 'or', '4, 5');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('mazepa', 'ivan', '623456789', 'ivan.mazepa@outlook.com', 'ZOIGA', '20 boulevard de la liberte', '3659-5481-3086-9752', '0', '');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('dorozhkina', 'marharyta', '634567890', 'marharyta.dorozhkina@outlook.com', 'qielhriog', '25 rue du soleil', '7022-9347-8158-5461', '0', '');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('kozlova', 'yevhen', '656789012', 'yevhen.kozlova@outlook.com', 'ZKEIJIAER', '30 avenue du lac', '2649-0065-7191-1335', 'bronze', '7');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('hrushevsky', 'serhiy', '689012345', 'serhiy.hrushevsky@outlook.com', 'mzugzera', '35 rue de la fontaine', '4825-0491-6677-2128', 'or', '10');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('firtash', 'dmytro', '667890123', 'dmytro.firtash@outlook.com', 'ZEIUFHAE', '40 boulevard des fleurs', '9034-1429-7716-4853', 'or', '11, 12');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('tihypko', 'lesya', '690123456', 'lesya.tihypko@outlook.com', 'idrjtrh', '45 rue des champs', '5761-8764-3110-9902', 'bronze', '13');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('konovalets', 'andriy', '634567890', 'andriy.konovalets@outlook.com', 'qeir', '55 rue du moulin', '1457-0695-4226-1815', '0', '14, 15');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('apanasenko', 'mykhailo', '623456789', 'mykhailo.apanasenko@outlook.com', 'qerijgeqsltb', '60 boulevard du chien qui dort', '7085-9800-6239-5531', 'or', '16, 17, 22, 23, 24, 25');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('glivenko', 'valery', '612345678', 'valery.glivenko@outlook.com', 'ltkpnpty', '23 rue d"anglais', '8312-0452-1274-3087', 'or', '18, 19, 26, 27, 28, 29');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('kovalenko', 'dmytro', '785963247', 'dmytro.kovalenko@outlook.com', 'eorgeqijr', '85 rue de rupari', '2658-7194-1489-4423', 'bronze', '30');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('bondartchouk', 'katerina', '536987412', 'kateryna.bilokur@gmail.com', 'eigjilqrkae', '10 rue de la complainderie', '9810-3774-2193-8071', '0', '');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES  ('chevtchenko', 'taras', '258361479', 'taras.chevtchenko@outlook.com', 'IKRJQEIO%S','42 rue de fresne hâché', '4418-2967-5823-3649', 'or', '81, 82, 83, 84, 85, 86');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('ivanov', 'svetlana', '514-555-1234', 'ivanovs@entreprise.com', 'motdepasse1', '10 rue des fleurs', '1234 5678 9012 3456', 'or', '50, 51, 52, 53, 54, 55, 56, 57, 58');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('kuznetsov', 'dimitri', '438-555-5678', 'kuznetsovd@entreprise.com', 'motdepasse2', '20 avenue des roses', '2345 6789 0123 4567', 'bronze', '44');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('petrovic', 'marija', '450-555-9876', 'petrovicm@entreprise.com', 'motdepasse3', '30 rue des lilas', '3456 7890 1234 5678', 'bronze', '70');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('novak', 'nikola', '514-555-2468', 'novakn@entreprise.com', 'motdepasse4', '40 boulevard des alpes', '4567 8901 2345 6789', '0', '');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('medvedev', 'anastasia', '450-555-1111', 'medvedeva@entreprise.com', 'motdepasse5', '50 avenue du lac', '5678 9012 3456 7890', '0', '');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('vasiliev', 'sergei', '514-555-7890', 'vasilievs@entreprise.com', 'motdepasse6', '60 rue de la fontaine', '6789 0123 4567 8901', 'or', '91, 92, 93, 94, 95, 96, 97');
INSERT INTO `bellefleur`.`client` (nom, prenom, num_tel, courriel, mdp, adresse_facturation, carte_de_credit, statut_fidelite, achat_passe) VALUES ('jankovic', 'ivana', '438-555-3333', 'jankovici@entreprise.com', 'motdepasse7', '70 avenue des jasmins', '7890 1234 5678 9012', '0', '');

-- Insertion pour la table Magasins.

INSERT INTO `bellefleur`.`magasin` (id_magasin, adresse_magasin, ca) VALUES ('A', '10 rue de la paix', 150);
INSERT INTO `bellefleur`.`magasin` (id_magasin, adresse_magasin, ca) VALUES ('B', '20 avenue des fleurs', 100);
INSERT INTO `bellefleur`.`magasin` (id_magasin, adresse_magasin, ca) VALUES ('C', '5 boulevard de la liberté', 80);

-- Insertion des données dans la table boncommande.
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (1, 'vinv', '08-05-2023', '10 rue des fleurs', '10-05-2023', 'bonjour, jaimerais que la commande soit emballée dans du papier cadeau.', 'yaroslav.koshka@hotmail.com', 'A');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (2, 'cpav', '08-05-2023', '20 avenue des roses', '10-05-2023', 'je voudrais que la commande soit livrée après 16h.', 'petro.porochenko@gmail.com', 'B');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (3, 'cal', '09-05-2023', '30 rue des lilas', '11-05-2023', 'bonjour, je serais absent entre 14h et 16h, merci de livrer après 16h.', 'olga.kurylenko@outlook.com', 'C');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (4, 'cc', '10-05-2023', '40 boulevard des alpes', '15-05-2023', 'pas de message particulier.', 'ivan.mazepa@outlook.com', 'A');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (5, 'cl', '01-05-2023', '50 avenue du lac', '05-05-2023', 'bonjour, je ne serai pas disponible pour réceptionner la commande, merci de la laisser à la conciergerie.', 'marharyta.dorozhkina@outlook.com', 'B');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (6, 'cc', '03-05-2023', '60 rue de la fontaine', '08-05-2023', 'pas de message particulier.', 'yevhen.kozlova@outlook.com', 'C');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (7, 'vinv', '02-05-2023', '70 avenue de la paix', '06-05-2023', 'bonjour, je voudrais que la commande soit livrée avant 14h.', 'serhiy.hrushevsky@outlook.com', 'A');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (8, 'cal', '12-03-2023', '80 rue des roses', '15-04-2023', 'pas de message particulier.', 'kateryna.bilokur@gmail.com', 'B');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (9, 'cl', '12-02-2023', '90 boulevard de la liberté', '19-02-2023', 'bonjour, je voudrais que la commande soit livrée après 18h.', 'jankovici@entreprise.com', 'C');
INSERT INTO `bellefleur`.`boncommande` (num_commande, etat, date_commande, adresse_livraison, date_livraison_desire, message, courriel, id_magasin) VALUES (10, 'cl', '22-01-2022', '100 avenue des fleurs', '30-03-2022', 'bonjour, jaimerais que la commande soit emballée dans une boîte en carton.', 'vasilievs@entreprise.com', 'A');

-- Insertion dans la table accessoire.

INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('ruban de satin', 2.50, 'france', 100, 20, 'A');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('perles en verre', 4.20, 'Italie', 75, 15, 'B');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('feuillage', 1.90, 'canada', 120, 30, 'A');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('ficelle en jute', 3.80, 'Br‚sil', 50, 10, 'C');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('noeud', 6.50, 'afrique du sud', 40, 5, 'B');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('perles', 2.80, 'inde', 90, 25, 'A');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('verdure', 5.10, 'chine', 60, 12, 'C');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('fleur en tissu', 2.30, 'france', 110, 28, 'B');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('ruban', 3.50, 'espagne', 85, 18, 'A');
INSERT INTO `bellefleur`.`accessoire` (nom_acc, prix_acc, origine_acc, stock_acc, seuil_acc, id_magasin) VALUES ('perles de rocaille', 1.70, 'japon', 150, 35, 'C');

-- Insertion pour la table fleurs.

INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('marguerites', 5.0, 'france', 50, 'à lannée', 10, 'A', 1); 
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('roses blanches', 7.0, 'hollande', 20, 'mai', 5, 'B', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('roses rouges', 8.0, 'equateur', 100, 'juin', 20, 'A', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('ginger', 12.0, 'exotique', 10, 'mai, juin', 2, 'C', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('oiseau du paradis', 15.0, 'exotique', 5, 'avril, juillet', 1, 'B', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('roses', 6.0, 'france', 80, 'septembre', 15, 'C', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('genet', 4.0, 'espagne', 30, 'août', 8, 'A', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('gerbera', 3.0, 'pays-Bas', 60, 'decembre', 10, 'B', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('lys', 6.0, 'france', 40, 'octobre', 12, 'C', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('alstroméria', 2.0, 'italie', 100, 'novembre', 25, 'A', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('orchidées', 20.0, 'exotique', 5, 'février', 1, 'B', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('fleurs de lotus', 10.0, 'exotique', 10, 'à lannée', 2, 'C', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('oeillets', 4.0, 'france', 30, 'à lannée', 6, 'A', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('jacinthes', 3.0, 'hollande', 50, 'à lannée', 10, 'B', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('iris', 5.0, 'france', 20, 'à lannée', 5, 'C', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('dahlias', 4.0, 'espagne', 40, 'à lannée', 8, 'A', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('lys des incas', 8.0, 'exotique', 15, 'à lannée', 3, 'B', 1);
INSERT INTO `bellefleur`.`fleur` (nom_fleur, prix_fleur, origine_fleur, stock_fleur, disponibilite_fleur, seuil_fleur, id_magasin, nbvendu) VALUES ('glaieuls', 10.0, 'afrique du sud', 15, 'janvier', 10, 'C', 1);

-- Insertion pour la table bouquet.

INSERT INTO `bellefleur`.`bouquet` (nom_bouquet, prix_bouquet, categorie_bouquet, type_bouquet, nom_fleur, nom_acc, num_commande, nb_commande) VALUES ('gros merci', 45, 'toute occasion', 'standard', 'marguerites', 'Verdure', 1, 2);  
INSERT INTO `bellefleur`.`bouquet` (nom_bouquet, prix_bouquet, categorie_bouquet, type_bouquet, nom_fleur, nom_acc, num_commande, nb_commande) VALUES ('l amoureux', 65, 'st valentin', 'standard', 'roses blanches, roses rouges', 'noeud', 2, 2);
INSERT INTO `bellefleur`.`bouquet` (nom_bouquet, prix_bouquet, categorie_bouquet, type_bouquet, nom_fleur, nom_acc, num_commande, nb_commande) VALUES ('l exotique', 40, 'toute occasion', 'standard', 'ginger, oiseau du paradis, roses, genet', 'perles', 3, 2);
INSERT INTO `bellefleur`.`bouquet` (nom_bouquet, prix_bouquet, categorie_bouquet, type_bouquet, nom_fleur, nom_acc, num_commande, nb_commande) VALUES ('maman', 80, 'fête des mères', 'standard', 'gerbera, roses blanches, lys, alstroméria', 'ruban', 4, 2);
INSERT INTO `bellefleur`.`bouquet` (nom_bouquet, prix_bouquet, categorie_bouquet, type_bouquet, nom_fleur, nom_acc, num_commande, nb_commande) VALUES ('vive la mariée', 120, 'mariage', 'standard', 'lys, orchidées', 'feuillage', 5, 2);

-- Insertion pour la table temps.

INSERT INTO `bellefleur`.`temps` (temps) VALUES (1);

# Test de requete:

Select Avg(prix_bouquet) from bouquet;
select max(num_commande)+1 from boncommande;
SELECT month(NOW());
select stock_fleur from fleur where nom_fleur="ginger";
Select stock_acc,prix_acc,id_magasin from accessoire where nom_acc='perles en verre';
select year(date_commande) from boncommande ; 
select count(client.courriel),nom,prenom from client join boncommande on client.courriel=boncommande.courriel 
where right(boncommande.date_commande, 4) <= year(now()) and right(boncommande.date_commande, 4) >= year(now())-1 group by(client.courriel) order by count(client.courriel) DESC LIMIT 1;
select right(boncommande.date_commande, 4) from boncommande;
select etat, id_magasin, date_commande from boncommande order by id_magasin,now();
select nom_bouquet from bouquet order by nb_commande desc limit 1;
select nom_fleur from fleur where origine_fleur="exotique" order by nbvendu desc limit 1;
select id_magasin, ca from magasin order by ca desc limit 1;
update magasin set ca=680 where id_magasin="A";
select * from magasin;

-- Requête synchronisée pour récupérer les informations sur des clients n'ayant jamais commandés :

SELECT * FROM client WHERE courriel NOT IN (SELECT courriel FROM boncommande);

-- Exemple de requête avec auto-jointure pour récupérer les clients qui ont commandé:

SELECT client.nom, client.prenom, boncommande.num_commande FROM client, boncommande WHERE client.courriel=boncommande.courriel;

-- Exemple de requête avec une union pour récupérer les noms des fleurs et des accessoires avec leur stock dans le magasin "a" :

SELECT nom_fleur AS nom, stock_fleur AS stock, 'fleur' AS type FROM fleur WHERE id_magasin = 'a' UNION SELECT nom_acc AS nom, stock_acc AS stock, 'accessoire' AS type FROM accessoire WHERE id_magasin = 'a';

# création du client
DROP USER'bozo'@'localhost';
CREATE USER'bozo'@'localhost' IDENTIFIED BY 'bozo';
GRANT SELECT ON bellefleur.* TO 'bozo'@'localhost';








