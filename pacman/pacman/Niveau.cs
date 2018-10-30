using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetProcessing.Sketch;


/// <summary>
/// Classe d'un niveau. Chaque niveau possède une référence à un fichier texte qui sera lu,
/// caractère par caractère, afin de produire un tableau de jeu dans lequel peuvent circuler les
/// personnages.
/// Les objets que Pacman peut manger afin de gagner des points sont aussi générés à ce moment.
/// 
/// Auteurs : Younes Rabdi et Jérémie Fortin
/// Date : 2017 - 05 - 26
/// </summary>
namespace pacmangame
{
    // patron Iterateur pour fournir un moyen d'accès séquentiel aux niveaux

    /// <summary>
    /// Interface Iterateur
    /// </summary>
    /// <typeparam name="T">type </typeparam>
    public interface Iterator<T>  // ou classe abstraite
    {
        void First();
        void Next();
        bool IsDone();
        T Current { get; }  // Ou méthode « getter »
    }
    /// <summary>
    /// Fournit un moyen d'accès séquentiel aux niveaux
    /// </summary>
    public class ItérateurNiveaux : Iterator<Niveau>
    {
        private int m_numNiveau = 0;

        public void First()
            => m_numNiveau = 0;

        public void Next()
            => ++m_numNiveau;

        public bool IsDone()
            => m_numNiveau > Niveau.NOMBRE_NIVEAUX;

        public Niveau Current
        {
            get { return Niveau.ObtenirNiveau(m_numNiveau); }
        }
    }

    /// <summary>
    /// Classe de base pour les niveaux
    /// </summary>
    public abstract class Niveau
    {
        public const int hauteurBarreAide = (HAUTEUR_TABLEAU - 2) * Character.LARGEUR_CHARACTER;
        private Color COULOR_COULOIR = new Color("#000000"); // Noir est la couleur des passages
        private Color COULOR_OBSTACLE = new Color("#142b51"); // Noir est la couleur des passages
        public const int NOMBRE_NIVEAUX = 3;
        public const int LARGEUR_TABLEAU = 28;
        public const int HAUTEUR_TABLEAU = 36;
        public string NomNiveau { get; protected set; } // le nom du niveau ex "Niveau 1", "Niveau Bonus"...
        public abstract void SetupNiveau(); // Construire le niveau
        public List<Character> characters { get; private set; } // Les Characters deplaceables du jeu
        public HashSet<Biscuit> m_biscuits { get; private set; } // Collection de biscuits que pacman doit ramasser pour terminer le niveau
        public HashSet<Fruit> m_fruits { get; private set; } // Collection de fruits que pacman doit ramasser pour terminer le niveau
        public List<List<char>> m_tableauJeu { get; protected set; }
        // Iterateur niveaux 
        public static Iterator<Niveau> GetIterator()
            => new ItérateurNiveaux();

        // Notifier quand un niveau est terminé (tous les points ramassés)
        public delegate void NiveauTerminéEventHandler(object source, EventArgs e);
        public event NiveauTerminéEventHandler niveauterminéEvenement;
        // Notifier quand un niveau est terminé (contact avec un ennemi)
        public delegate void NiveauÉchouéEventHandler(object source, EventArgs e);
        public event NiveauÉchouéEventHandler niveauéchouéEvenement;
        // Notifier quand un biscuit est ramassé
        public delegate void BiscuitRamasséEventHandler(int p_points);
        public event BiscuitRamasséEventHandler biscuitRamasséEvenement;
        // Notifier quand un fruit est ramassé
        public delegate void FruitRamasséEventHandler(int p_points);
        public event FruitRamasséEventHandler fruitRamasséEvenement;

        protected virtual void notifierNiveauComplété()
        {
            if (niveauterminéEvenement != null)
                niveauterminéEvenement(this, EventArgs.Empty);
        }
        protected virtual void notifierNiveauÉchoué()
        {
            if (niveauéchouéEvenement != null)
                niveauéchouéEvenement(this, EventArgs.Empty);
        }

        protected virtual void notifierBiscuitRamassé(int p_points)
        {
            if (biscuitRamasséEvenement != null)
                biscuitRamasséEvenement(p_points);
        }
        protected virtual void notifierFruitRamassé(int p_points)
        {
            if (fruitRamasséEvenement != null)
                fruitRamasséEvenement(p_points);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="p_nom"></param>
        protected Niveau(string p_nom)
        {
            this.NomNiveau = p_nom;
            characters = new List<Character>();
            m_biscuits = new HashSet<Biscuit>();
            m_fruits = new HashSet<Fruit>();
        }

        /// <summary>
        /// Indique quand la position passée en parametre contient un obstacle.
        /// (Détect la couleur du pixel et determine si c'est un obstacle)
        /// </summary>
        /// <param name="p_x">coordonée x</param>
        /// <param name="p_y">coordonée y</param>
        /// <returns>true quand la position est un obstacle</returns>
        public bool estUnObstacle(int p_x, int p_y)
        {
            try
            {
                return Program.Get(p_x, p_y).Equals(COULOR_OBSTACLE);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("{0}", e.ToString());
            }
            return true;
        }

        public void dessiner()
        {
            for (int i = 0; i < m_tableauJeu.Count(); i++)
            {
                for (int j = 0; j < m_tableauJeu[i].Count(); j++)
                {
                    if (m_tableauJeu[i][j] != '0')
                    {
                        Fill(COULOR_COULOIR);
                    }
                    else
                        Fill(COULOR_OBSTACLE);
                    NoStroke();
                    Rect(Character.LARGEUR_CHARACTER * j, Character.LARGEUR_CHARACTER * i,
                           Character.LARGEUR_CHARACTER * (j + 1), Character.LARGEUR_CHARACTER * (i + 1));
                }
            }

            Character pac = Program.Jeu.m_niveau.characters.Find(e => e.GetType() == typeof(Pacman));
            // dessiner les biscuits
            if (m_biscuits.Count() > 0 || m_fruits.Count() > 0)
            {
                if (m_biscuits.Count() > 0)
                    foreach (Biscuit b in m_biscuits)
                    {
                        b.dessiner();
                        if ((pac.Pos_x() - b.pos_x < 10 &&
                            pac.Pos_y() - b.pos_y < 10 &&
                            pac.Pos_x() - b.pos_x > -10 &&
                            pac.Pos_y() - b.pos_y > -10))
                            notifierBiscuitRamassé(b.valeur);
                    }
                if (m_fruits.Count() > 0)
                    foreach (Fruit f in m_fruits)
                    {
                        f.dessiner();
                        if ((pac.Pos_x() - f.pos_x < 10 &&
                            pac.Pos_y() - f.pos_y < 10 &&
                            pac.Pos_x() - f.pos_x > -10 &&
                            pac.Pos_y() - f.pos_y > -10))
                            notifierFruitRamassé(f.valeur);
                    }
                foreach (Character c in characters)
                {
                    if ((pac.Pos_x() - c.Pos_x() < Character.LARGEUR_CHARACTER / 2 &&
                        pac.Pos_y() - c.Pos_y() < Character.LARGEUR_CHARACTER / 2 &&
                        pac.Pos_x() - c.Pos_x() > -Character.LARGEUR_CHARACTER / 2 &&
                        pac.Pos_y() - c.Pos_y() > -Character.LARGEUR_CHARACTER / 2))
                        notifierNiveauÉchoué();
                }
            }
            else notifierNiveauComplété();

            m_biscuits.RemoveWhere(b => (pac.Pos_x() - b.pos_x < 10 &&
                    pac.Pos_y() - b.pos_y < 10 &&
                    pac.Pos_x() - b.pos_x > -10 &&
                    pac.Pos_y() - b.pos_y > -10));
            m_fruits.RemoveWhere(f => (pac.Pos_x() - f.pos_x < 10 &&
                   pac.Pos_y() - (f.pos_y + Character.LARGEUR_CHARACTER / 2) < 10 &&
                   pac.Pos_x() - (f.pos_x + Character.LARGEUR_CHARACTER / 2) > -10 &&
                   pac.Pos_y() - (f.pos_y + Character.LARGEUR_CHARACTER / 2) > -10));

            // Dessiner les characters
            foreach (Character c in characters)
            {
                c.Dessiner();
            }

            Rect(0, hauteurBarreAide,
                LARGEUR_TABLEAU * Character.LARGEUR_CHARACTER, HAUTEUR_TABLEAU * Character.LARGEUR_CHARACTER);
            Fill(COULOR_COULOIR);
            Text("Appuyez sur F1 ou cliquez ici pour les règles", Width / 2, hauteurBarreAide);
        }

        /// <summary>
        /// Affiche le tableau et génère les éléments appropriés dans les bonnes cases.
        /// </summary>
        /// <param name="nomFichier">Nom du fichier à charger</param>
        /// <returns></returns>
        protected bool ChargerTableau(string nomFichier)
        {
            m_tableauJeu = new List<List<char>>();
            try
            {
                // charger la matrice de jeu. Cette dernière servira pour être envoyée aux joueurs.
                char c;

                int cptY = 0;

                StreamReader reader = new StreamReader(nomFichier);
                m_tableauJeu.Add(new List<char>());
                do
                {
                    c = (char)reader.Read();
                    if (c == '\n')
                    {
                        cptY++;
                        m_tableauJeu.Add(new List<char>());
                    }
                    else
                    {
                        m_tableauJeu[cptY].Add(c);
                        if (c != '0')
                        {
                            if (c == 'P')
                                m_biscuits.Add(new GrosPoint(m_tableauJeu[cptY].Count() * Character.LARGEUR_CHARACTER - Character.LARGEUR_CHARACTER / 2,
                                cptY * Character.LARGEUR_CHARACTER + Character.LARGEUR_CHARACTER / 2));
                            else if (c == 'F')
                                m_fruits.Add(new Fraise(m_tableauJeu[cptY].Count() * Character.LARGEUR_CHARACTER - Character.LARGEUR_CHARACTER / 2,
                                    cptY * Character.LARGEUR_CHARACTER + Character.LARGEUR_CHARACTER / 2));
                            else
                                m_biscuits.Add(new Point(m_tableauJeu[cptY].Count() * Character.LARGEUR_CHARACTER - Character.LARGEUR_CHARACTER / 2,
                                        cptY * Character.LARGEUR_CHARACTER + Character.LARGEUR_CHARACTER / 2));
                        }
                    }
                } while (!reader.EndOfStream);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Fabrique simple pour la création des niveaux
        /// </summary>
        /// <param name="p_numero">index du niveau</param>
        /// <returns>Niveau</returns>
        public static Niveau ObtenirNiveau(int p_numero)
        {
            Niveau niveau = null;
            switch (p_numero)
            {
                case 1: niveau = new Niveau1(); break;
                case 2: break;
                case 3: break;
                default: niveau = new Niveau1(); break;
            }
            return niveau;
        }
    }

    /// <summary>
    /// Niveau 1
    /// </summary>
    public class Niveau1 : Niveau
    {
        // Constructeur
        public Niveau1() : base("Niveau 1")
        {
            ChargerTableau("../../assets/maze.txt");
        }

        /// <summary>
        /// Construire le niveau
        /// </summary>
        public override void SetupNiveau()
        {
            characters.Add(new Pacman(494, 564, 5));
            characters.Add(new Blinky(39, 109, 5));
            characters.Add(new Pinky(444, 275, 5));
            characters.Add(new Inky(153, 274, 5));
            characters.Add(new Clyde(373, 197, 5));
        }
    }

    /// <summary>
    /// Niveau 2
    /// </summary>
    public class Niveau2 : Niveau
    {
        // Constructeur
        public Niveau2() : base("Niveau 2")
        {
            ChargerTableau("../../assets/maze2.txt");
        }

        /// <summary>
        /// Construire le niveau
        /// </summary>
        public override void SetupNiveau()
        {
            characters.Add(new Pacman(494, 564, 5));
            characters.Add(new Blinky(39, 109, 5));
            characters.Add(new Pinky(444, 275, 5));
            characters.Add(new Inky(153, 274, 5));
            characters.Add(new Clyde(373, 197, 5));
        }
    }
}