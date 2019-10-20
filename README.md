# Projet_Gestion_Ressources_Renouvelables
JEUX DE GESTION DE RESSOURCES RENOUVELABLES
===========================================

Concept :
---
Le jeu est un jeu de gestion dans un milieu restreint avec des ressources très limités mais renouvelables, le coeur du gameplay consistera à bien gérer ses ressources pour croitre mais en veillant à renouveler ses ressources avant d'avoir tout comsommer.

Le joueur va contrôler une petite tribu de l’île de Pâques. Cette tribu est composée de différents types d’individus ( Chasseurs, Cueilleurs, Guerriers,...) qui vont s’occuper de différentes tâches dans le village. Le village est composé de plusieurs batiments (habitation, caserne, scieri, nexus ... ) avce lequel le joueur peut interagir.

La présence d'autres tribus hostiles sur l'îles viendra perturber la progression du joueur et apporter un challenge.

L'objectif est de survivre aux assault tout en continuant a croitre, le joueur est gagnant si il parvient a vaincre la ou les tribus rivales tout en évitant d'épuiser ses ressources. Il est considérer perdant si il succombre aux assault ou qu'il arrive a court de ressources.

GDD:
===

Population :
---
Le jeu cible :
- Des gens qui apprécie les jeux de gestions
- Les débutant, le jeu n'integrera pas de mécaniques complexes. Le jeu sera doté d'indicateur pour assister le joueur.
- Tranche d'âge : À partir de 10-12 ans. (Mécaniques de gestion non adaptés aux plus jeunes)
- Tout joueurs ayant un intéret pour les ressources renouvelables.
- Joueurs PC(pour l'instant, portage mobile possible).

Acteurs du jeu :
---

- Batiments :
  - Nexus : Coeur de la base, permet la production de villageois
  - Habitation : Pas d'interaction, nécéssaire à la production de villageois.
  - Scierie : Permet la conversion de villageois en bucherons, permet d'augmenter le nombre maximal de bucheron.
  - Caserne : Permet la conversion de villageois en Guerrier,permet d'augmenter le nombre maximal de Guerrier.
  - Entrepot de vivre : Permet de stocker les ressources "Nourriture".


- Humains:
  - Villageois : Unité de base, construit des batiments.
  - Bucherons : Récupérer du bois et replanter les arbres.
  - Chasseurs/Cueilleurs : Récupére de la nourriture (gibier ou baies).
  - Guerrier : Unité militaire qui peut infliger des dégats aux unités ennemies


- Ressources :
  - Arbres : Fournis du bois, peut être replanter.
  - Nourriture : Sert à créer des unités et à les maintenir en vies.
    - Animale : Proies qui peuvent se reproduirent.
    - Végétale : Baies présentent sur les arbres pouvant être soit ramassé par un ceuilleur soit replanté par le bucherons.


Caméra :
---
- Plan 2D vue de dessus, caméra mobile.(Caméra RTS)

Controles :
---
- Curseur dirigés sur les bord de l'écran et touches ZQSD : Déplacement de la caméra sur les axes X,Z.
- Molette souris : Déplacement de la caméra sur l'axe Y (Zoom-Dézoom)
- Clic&Drag souris : Sélection d'unité .
- Clic droit sur objet: interaction unité-objet (ordre direct)
- Touche clavier 'A/I' : passage de mode actif-inactifs (ordre indirect), l'unité se met au travail sans cible défini par le joueur.
- Touche clavier 'B' : Menu batiment.
- Clic gauche : Affiche les possibilité d'interaction d'une untié (exemple : Batiment -> Création d'unités, Destrctuction du batiment)

Feedback :
---
- Interaction unités :
  - Sélection d'une unité non active : Surbrillance de couleur 1
  - Chaque ordre qui met une unité en action : mouvement.
  - touche clavier 'A' : Changement de la couleur du surbrillance de l'unité sélection en couleur 2.
  - touche clavier 'I' : Changement de la couleur du surbrillance de l'unité sélection en couleur 1.
  - Clic droit sur objet après sélection: Surbrillance couleur 3
  - Lancement de la production d’une unité : Feedback sonore 1
  - Lancement de la production d’une unité mais sans les ressources nécessaire : Feedback sonore 2


- Interaction Bâtiment :
  - Placement d'un bâtiment pour construction : Bâtiment affiché avec de la transparence / Preview du bâtiment
  - Placement d’un bâtiment invalide : Feedback sonore 2
  - Construction d’un bâtiment sans les ressources nécessaire : Feedback sonore 2
  - Validation de l’emplacement du bâtiment : feedback sonore 1
  - Bâtiment en cours de construction : feedback sonore 3
  - Bâtiment fini de construire : Feedback sonore 4 & visuel

OCR :
---
Il existe trois types de boucles dans la structure OCR ( Objectif, Challenge, Reward ):
- La boucle micro: représente quelques secondes du jeu ( Combat contre un ennemi par exemple )
- La boucle moyenne: représente une quête, une mission ou un chapitre.
- La boucle macro: englobe tout le scénario du jeu.

    Dans notre jeu, on peut trouver ces trois types de boucles.

Exemples de boucles micro:
- Objectif: Récupérer une ressource. Challenge: Localiser la ressource et se diriger vers elle en évitant les dangers. Reward: La ressource.
- Objectif: Vaincre un ennemi. Challenge: La difficulté et la force de cet ennemi. Reward: Survie et potentiellement des ressources.

Exemples de boucles moyennes:
- Objectif: Construire un bâtiment. Challenge: Récupérer les ressources nécessaires. Reward: Les bonus que confère ce bâtiment.
- Objectif: Récupérer un territoire ou des ressources. Challenge: Vaincre les troupes ennemies. Reward: Les ressources et des terres.

Boucle Macro:
- Objectif: Être la tribu survivante sans épuiser les ressources de l’île. Challenge: Gérer les ressources de l’île et vaincre la tribu opposante. Reward: Gagner la partie.


Règles du jeu :
---
- Utiliser les ressources pour créer des unités et des bâtiments.
- Utiliser ces unités pour réaliser des actions (Construire, rassembler davantage de ressources , attaquer …)
- Utiliser les actions disponible pour gérer ses ressources et défaire la tribu adverse.
- Facteurs d’échec :
  - Mauvaise gestion des ressources renouvelables, conduisant à des ressources insuffisante pour survivre.
  - Défaite face à la tribu adverse.

HUD/IHM:
---
L’interface du jeu ne sera pas trop chargée. Elle comportera le strict nécessaire pour donner des informations pertinentes au joueur comme ses ressources et le nombre de villageois.
En haut de l’écran, une barre horizontale va contenir ces informations:
- Les différentes ressources que le joueur possède et leurs nombres correspondants.
- Le nombre de villageois. ( En cliquant sur ce nombre, une fenêtre d’informations complémentaire apparaîtra avec les détails de spécialisations des villageois. )
- Une date.
- Une bouton pour afficher le menu.

    Il y’a d’autres interfaces dans le jeu qui consistent en:
- Des micro menus pour chaque unité, bâtiment ou ressource qui va donner des informations et détails sur cet élément.
- Des menus qui listent les bâtiments à construire et unités à créer.
- Des barres de santé des personnages et des bâtiment. ( On peut choisir de masquer ces barres pour ne pas encombrer l’écran de jeu. )
- Les différents menus de début de jeu, de paramètres et de fin de jeu.
- Des messages de feedback.
- Une mini map du jeu.

