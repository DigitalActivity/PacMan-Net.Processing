# PacMan-Net.Processing
Avec Patrons de conception

- Cours de Méthodologies de programmation
- Cegep de Saint-Jean-sur-Richelieu A2017
- Objectif : Mettre en pratique les bons principes de l'orienté objet vus dans le cours, ainsi que les patrons de conception
- Auteurs : Younes Rabdi et Jérémie Fortin
- Date : 2017 - 05 - 26

- Programme d'une version du jeu de Pacman, faite en Net.Processing.
- Le joueur contrôle son personnage avec les touches directionnelles ou WASD.
- Le but du programme est d'implémenter de façon efficace des patrons de conception vus en classe.

# consignes
Idéalement, vous voudrez tout implémenter, sans nécessairement reproduire exactement le look du jeu, mais vous n'y arriverez peut-être pas (dans le temps alloué). Il est donc très important que vous pensiez d'abord à une bonne conception de votre programme. Vous devez remettre quelque chose de fonctionnel, même si vous n'avez pas réussi à faire toutes les fonctionnalités ou tous les détails « esthétiques ». Vous verrez donc à appliquer une approche agile, qui ajoutera les diverses fonctionnalités étape par étape, mais prévoyez quand même, dès le départ, faire la bonne structure pour éviter d'avoir à refaire des parties du programme quand vous allez avancer.
- Le déplacement du personnage doit se faire par les touches du clavier classiques pour ce genre de jeu.
- Les « fantômes » de votre jeu ne doivent pas tous avoir le même comportement.
- Les personnages se déplacent avec une légère animation.
- Vous pouvez utiliser des images pour certains éléments du jeu, mais le labyrinthe lui-même devrait être dessiné (du moins principalement).
- Le jeu doit commencer par afficher un écran de lancement (splash screen), avec vos noms, le nom du jeu, un logo, etc. Un clic de souris, ou l'appui sur une touche du clavier passe à l'écran suivant.
- L'écran suivant est un écran d'aide qui donne des directives sur le fonctionnement du jeu, mais aussi des informations sur les éléments (nombre de points des fruits, noms et comportement des fantômes, etc., mais toujours en fonction de ce qui est implémenté dans votre programme). En cours de partie, on peut revenir à cet écran à tout moment, en appuyant sur la touche F1 (ou par un clic sur la ligne d'aide de l'écran de jeu). Le jeu est alors en pause. On passe à l'écran de jeu, par un clic de souris, ou par l'appui sur une touche du clavier autre que F1.
- L'écran de jeu affiche le labyrinthe, les personnages, etc., mais comporte aussi une ligne pour le score et autres informations dynamiques (nombre de fruits restants, etc.), et une ligne indiquant qu'on peut faire une pause et aller à l'écran d'aide en appuyant sur F1 (on peut aussi cliquer sur cette ligne).
- Lorsqu'un niveau est terminé par le joueur, ou s'il perd, un écran récapitulatif donne les statistiques et scores, et permet de retourner au prochain niveau de jeu, ou recommencer.
- Il doit donc y avoir plus d'un niveau et il doit être facile d'en créer d'autres ou de le modifier (si on a accès au code source !). Il faut entre autres que votre description de niveau permette de dessiner le labyrinthe.
Vous pourriez avoir d'autres écrans au besoin. Ce changement de fonctionnement doit être représenté dans le programme pour l'utilisation du patron de conception État. C'est obligatoire (et le seul indice d'une bonne conception que je vais donner, même si ce n'est pas le seul emploi des patrons de conception que l'on devrait trouver).
Tout le reste est laissé à votre discrétion. Mais je répète : le but n'est pas de faire un programme qui fait tout parfaitement extérieurement, mais qui est difficile ou impossible à modifier et améliorer. Le but est d'avoir la meilleure structuration possible à l'interne... Ce n'est pas parce que toutes les possibilités sont fonctionnelles que vous aurez 5/5. Inversement vous pourriez avoir 5/5 avec un jeu n'offrant pas toutes les possibilités mais permettant de les implémenter facilement !
