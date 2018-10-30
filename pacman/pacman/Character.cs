using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetProcessing.Sketch;

/// <summary>
/// Classe des personnages du jeu de Pacman qui interagisseront ensemble.
/// 
/// Auteurs : Younes Rabdi et Jérémie Fortin
/// Date : 2017 - 05 - 26
/// </summary>
namespace pacmangame
{
    /// <summary>
    /// Classe de base pour les characters déplaceables du jeu.
    /// Le patron Strategie est utilisé pour la gestion des strategies de deplacements
    /// </summary>
    public abstract class Character
    {
        protected int tempsDernièreImage = 0; // Valeur en frame du temps de la dernière image
        protected double framesEntreImages = 0.5 * FrameRateValue; // Nombre de frames entre deux images d'animation différentes.
        public const int LARGEUR_CHARACTER = 24;
        protected PImage m_imageActuelle; // Image du personnage
        protected Color couleur;
        public StrategieDeplacement stgDeplacement { get; private set; }
        public abstract void Dessiner();

        public void set_StrategieDeplacement(StrategieDeplacement p_strategie)
            => stgDeplacement = p_strategie;

        public int Pos_x()
            => stgDeplacement.pos_x;

        public int Pos_y()
            => stgDeplacement.pos_y;
    }

    /// <summary>
    /// Pacman est le character principal, controlé par le joueur
    /// </summary>
    public class Pacman : Character
    {
        PImage ouvert = LoadImage("../../assets/Pac_open.png");
        PImage fermé = LoadImage("../../assets/Pac_close.png");
        public Pacman(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            m_imageActuelle = fermé;
            couleur = new Color(255, 200, 0);
            Color couleur2 = new Color(0, 200, 0);
            this.set_StrategieDeplacement(new DeplacementParJoueur(p_pos_x, p_pos_y, p_vitesse));
        }

        public override void Dessiner()
        {
            stgDeplacement.deplacer();
            if (FrameCount - tempsDernièreImage >= framesEntreImages)
            {
                if (m_imageActuelle == fermé)
                    m_imageActuelle = ouvert;
                else
                    m_imageActuelle = fermé;
                tempsDernièreImage = FrameCount;
            }
            else
                Image(m_imageActuelle, Pos_x() - LARGEUR_CHARACTER / 2, Pos_y() - LARGEUR_CHARACTER / 2);
            //    Fill(couleur);

            //Ellipse(Pos_x(), Pos_y(), LARGEUR_CHARACTER, LARGEUR_CHARACTER);
        }
    }

    /// <summary>
    /// Chaser character : NPC agressif
    /// </summary>
    public class Blinky : Character
    {
        public Blinky(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            m_imageActuelle = LoadImage("../../assets/blinky.png");
            couleur = new Color(250, 0, 0);
            this.set_StrategieDeplacement(new DeplacementAgressif(p_pos_x, p_pos_y, p_vitesse));
        }

        public override void Dessiner()
        {
            stgDeplacement.deplacer();
            Image(m_imageActuelle, Pos_x() - LARGEUR_CHARACTER / 2, Pos_y() - LARGEUR_CHARACTER / 2);
            //Fill(couleur);
            //Ellipse(Pos_x(), Pos_y(), LARGEUR_CHARACTER, LARGEUR_CHARACTER);
        }
    }

    /// <summary>
    /// Chaser character : NPC agressif
    /// </summary>
    public class Pinky : Character
    {
        public Pinky(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            m_imageActuelle = LoadImage("../../assets/pinky.png");
            couleur = new Color(250, 55, 150);
            this.set_StrategieDeplacement(new DeplacementAmbuscade(p_pos_x, p_pos_y, p_vitesse));
        }

        public override void Dessiner()
        {
            stgDeplacement.deplacer();
            Image(m_imageActuelle, Pos_x() - LARGEUR_CHARACTER / 2, Pos_y() - LARGEUR_CHARACTER / 2);
            //Fill(couleur);
            //Ellipse(Pos_x(), Pos_y(), LARGEUR_CHARACTER, LARGEUR_CHARACTER);
        }
    }

    /// <summary>
    /// Chaser character : NPC agressif
    /// </summary>
    public class Inky : Character
    {
        public Inky(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            m_imageActuelle = LoadImage("../../assets/inky.png");
            couleur = new Color(50, 150, 250);
            this.set_StrategieDeplacement(new DeplacementPeureux(p_pos_x, p_pos_y, p_vitesse));
        }

        public override void Dessiner()
        {
            stgDeplacement.deplacer();
            Image(m_imageActuelle, Pos_x() - LARGEUR_CHARACTER / 2, Pos_y() - LARGEUR_CHARACTER / 2);
            //Fill(couleur);
            //Ellipse(Pos_x(), Pos_y(), LARGEUR_CHARACTER, LARGEUR_CHARACTER);
        }
    }

    /// <summary>
    /// Chaser character : NPC agressif
    /// </summary>
    public class Clyde : Character
    {
        public Clyde(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            m_imageActuelle = LoadImage("../../assets/clyde.png");
            couleur = new Color(250, 100, 0);
            this.set_StrategieDeplacement(new DeplacementHasard(p_pos_x, p_pos_y, p_vitesse));
        }

        public override void Dessiner()
        {
            stgDeplacement.deplacer();
            Image(m_imageActuelle, Pos_x() - LARGEUR_CHARACTER / 2, Pos_y() - LARGEUR_CHARACTER / 2);
            //Fill(couleur);
            //Ellipse(Pos_x(), Pos_y(), LARGEUR_CHARACTER, LARGEUR_CHARACTER);
        }
    }
}
