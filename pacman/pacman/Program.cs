using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static NetProcessing.Sketch.Parameter;

/// <summary>
/// Programme d'une version du jeu de Pacman, faite en Net.Processing.
/// Le joueur contrôle son personnage avec les touches directionnelles ou WASD.
/// 
/// Le but du programme est d'implémenter de façon efficace des patrons de conception
/// vus en classe.
/// 
/// Auteurs : Younes Rabdi et Jérémie Fortin
/// Date : 2017 - 05 - 26
/// </summary>
namespace pacmangame
{
    public class Program : NetProcessing.Sketch
    {
        État m_état; // État du program

        // Singleton program
        public static Program InstanceJeu { get; set; }
        private Program() { }

        static void Main(string[] args)
        {
            //HideConsole();
            InstanceJeu = new Program();
            InstanceJeu.Start();
        }

        public override void Setup()
        {
            Size(Character.LARGEUR_CHARACTER * Niveau.LARGEUR_TABLEAU,
                Character.LARGEUR_CHARACTER * Niveau.HAUTEUR_TABLEAU);
            FrameRate(30);
            m_état = AffichageInitial.singleton();
        }

        /// <summary>
        /// État program
        /// </summary>
        interface État
        {
            void MettreÀJour(bool p_doubleClic, bool p_toucheTapée, bool p_clicSimple);
            void Dessiner();
        }

        /// <summary>
        /// Changer État du program
        /// </summary>
        /// <param name="p_état">état</param>
        void ChangerDÉtat(État p_état)
        {
            m_toucheTapée = false;
            m_doubleClic = false;
            m_clicSimple = false;
            m_état = p_état;
        }

        public override void Draw()
        {
            Background(255);
            m_état.MettreÀJour(m_doubleClic, m_toucheTapée, m_clicSimple);
            m_état.Dessiner();
            m_toucheTapée = false;
            m_doubleClic = false;
            m_clicSimple = false;
        }

        bool m_doubleClic = false;  // S'il y a eu un double-clic, on aura l'indication ici.
        bool m_toucheTapée = false; // S'il y a eu quelque chose de taper, on aura l'indication ici.
        bool m_clicSimple = false; // S'il y a eu un clic de souris, on aura l'indication ici.
        public override void MouseDoubleClicked()
            => m_doubleClic = true;
        public override void MouseClicked()
        {
            m_clicSimple = true;
            //Console.WriteLine("PositionClic x:{0}, y:{1}", MouseX, MouseY); // pour tests seulement
        }

        public override void KeyPressed()
            => m_toucheTapée = true;

        /// <summary>
        /// État Affichage Initial (splash screen)
        /// </summary>
        class AffichageInitial : État
        {
            // Image utilisée dans l'écrant principal
            public PImage ImageSplash { get; private set; }

            // Singleton AffichageInitial
            public static AffichageInitial singleton()
            {
                if (instance == null)
                    instance = new AffichageInitial();
                return instance;
            }
            private static AffichageInitial instance = null;
            private AffichageInitial()
            {
                ImageSplash = LoadImage("../../assets/PacMan_menu.jpg");
            }

            /// <summary>
            /// Mettre à jour l'état
            /// </summary>
            public void MettreÀJour(bool p_doubleClic, bool p_toucheTapée, bool p_clicSimple)
            {
                // N'import quelle touches ( certaines touches ne sont pas prises en compte )
                if (p_toucheTapée || p_clicSimple || p_doubleClic)
                    InstanceJeu.ChangerDÉtat(Pause.singleton());
            }

            /// <summary>
            /// Dessiner à l'ecrant
            /// </summary>
            public void Dessiner()
            {
                Background(ImageSplash);
            }
        }

        /// <summary>
        /// État Jeu
        /// </summary>
        public class Jeu : État
        {
            public static Niveau m_niveau { get; protected set; }
            public int Score { get; private set; }

            // Singleton Jeu
            public static Jeu singleton()
            {
                if (instance == null)
                    instance = new Jeu();
                return instance;
            }
            private static Jeu instance = null;
            private Jeu()
            {
                Score = 0;
                m_niveau = Niveau.GetIterator().Current;
                m_niveau.niveauterminéEvenement += niveauTerminé; // notifié quand niveau terminé
                m_niveau.niveauéchouéEvenement += niveauÉchoué; // notifié quand un fantôme nous tue.
                m_niveau.biscuitRamasséEvenement += mettreÀjourScore; // notifié quand un biscuit est ramassé
                m_niveau.fruitRamasséEvenement += mettreÀjourScore; // notifié quand un fruit est ramassé
                m_niveau.SetupNiveau();
            }

            public void MettreÀJour(bool p_doubleClic, bool p_toucheTapée, bool p_clicSimple)
            {
                if (p_toucheTapée)
                {
                    if (KeyCode == KC_F1)
                        InstanceJeu.ChangerDÉtat(Pause.singleton());
                }
                /* Si on veut ouvrir l,aide avec un clic sur la barre d'aide.
                 * Il semble y avoir un problème puisque cela prends plusieurs clics, mais
                 * semble tout de même fonctionner.. Surement dû au lag du programme */
                if (p_clicSimple || p_doubleClic)
                    if (MouseY > Niveau.hauteurBarreAide)
                        InstanceJeu.ChangerDÉtat(Pause.singleton());

            }

            /// <summary>
            /// Changer de niveau
            /// </summary>
            private void niveauTerminé(object source, EventArgs e)
            {
                Niveau.GetIterator().Next();
                m_niveau = Niveau.GetIterator().Current;
                m_niveau.SetupNiveau();
            }

            /// <summary>
            /// Recommencer le niveau.
            /// </summary>
            private void niveauÉchoué(object source, EventArgs e)
            {
                m_niveau = Niveau.GetIterator().Current;
                m_niveau.SetupNiveau();
            }

            /// <summary>
            /// mettre à jour le score
            /// </summary>
            private void mettreÀjourScore(int p_points)
            {
                Score += p_points;
            }

            // Dessiner les characters
            public void Dessiner()
            {
                m_niveau.dessiner();
                TextSize(30);
                TextAlign(Center, TOP);
                Fill(50, 50, 50, 180);
                Stroke(0);
                Fill("#FFFF00");
                Text($"Score : {Score}",
                     Width / 2, 10);
            }
        }

        /// <summary>
        /// État pause
        /// </summary>
        class Pause : État
        {
            // Singleton état pause
            public static Pause singleton()
            {
                if (instance == null)
                    instance = new Pause();
                return instance;
            }
            private static Pause instance = null;
            private Pause() { }

            public void MettreÀJour(bool p_doubleClic, bool p_toucheTapée, bool p_clicSimple)
            {
                if (p_toucheTapée || p_clicSimple || p_doubleClic)
                {
                    if (p_toucheTapée || p_clicSimple || p_doubleClic)
                    {
                        InstanceJeu.ChangerDÉtat(Jeu.singleton());
                    }
                }
            }

            public void Dessiner()
                => Background(LoadImage("../../assets/PacMan_regles.jpg"));
        }
    }
}
