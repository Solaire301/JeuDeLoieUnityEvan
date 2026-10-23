Ballon d’or


Un jeu de l’oie autour du football, faites monter une équipe jusqu’au sommet dans un mélange jeu de l’oie et autobattler.


Le joueur contrôle un pion qui représente son équipe, chaque tour il lance un dé 6  et bouge du nombre de cases indiquées.
Les composants essentiels du jeu : 
* Le joueur
* Les cases


Composantes du jeu : 


Le joueur : Le joueur sur la vue du plateau contrôle un seul pion qui contient les données de son équipe, les données sont les suivantes et elles seront utiles aux minis-jeux.
* Une liste de joueurs (5 au départ, max 15) avec pour chaque joueur : 
   * Nom
   * Vitesse
   * Puissance des tirs
   * Compétence
Les cases : Chaque case contient plusieurs données qui influent sur le déroulement du jeu : 
* Numéro
* Effet : 
   * Avancer ou reculer de cases
   * Ajouter un joueur à l’équipe
   * Enlever un joueur de l’équipe (rare) (si l’équipe tombe à 0 c’est perdu)
   * Déclencher un mini jeu, en cas de victoire avance d’une case, en cas de défaite recule d’une case.
   * Dialogue d’un membre de l’équipe


La dernière case du plateau devrait avoir un match complexe et déclencher un écran de victoire en cas de réussite.
Le plateau est en 3D.


Structure des minis jeux : 


Lors des mini jeux un match se joue en autobattler entre l’équipe joueur et une équipe adverse au nombre et à la difficulté variable, chaque match devrait prendre environ deux minutes.
Le comportement des joueurs est le suivant : 


* Si un membre de l’équipe alliée possède la balle avancer à vitesse x 1.25 vers les cages ennemies
* Si lui-même a la balle avancer quelques secondes aléatoires puis faire un tir vers l’avant, il regarde dans un rayon de sa puissance de tir l’allié le plus loin de lui et lui fait une passe, selon un weighted bool random influencé par sa variable compétence il réussit sa passe et la balle est donnée à l’allié ou il échoue et la balle est envoyée sur un point aléatoire en amont dans un rayon puissance de tir
* Si la balle est sur le terrain que personne ne l’a l’équipier le plus proche de chaque équipe court la prendre
* Si un ennemi possède la balle, chaque équipier allié suit l’ennemi le plus proche de lui
* Selon la puissance du tir variable si l’équipier est assez proche des cages ennemies il tente d’y tirer la balle, il réussit ou échoue selon un weighted boolean influencé par sa variable compétence
* Si au bout de 3 minutes il n’y a aucun gagnant l’équipier en possession de la balle tire dans les cages ennemies


La balle n’a pas besoin d’avoir de réelle physique, juste prendre une position pour l’arrivée des tirs et la faire glisser, elle ne peut pas être interceptée.
Lorsque le joueur clique sur un des membres de son équipe il y a un feedback et pendant 2 secondes le joueur cliqué est rendu plus rapide.
Lorsqu’un match se termine échec ou victoire les variables de l'équipe du joueur augmentent pour l’aider.
Essayer de tester en match si tout marche bien faire les ajustements nécessaires.


Le mini jeu est en 3D. Normalement il répond aux besoins du devoir en termes d’IA, à voir.