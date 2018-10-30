using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetProcessing.Sketch;

/// <summary>
/// Classe contenant les objets pouvant être mangées pas Pacman. Cela sert à lui fournir des points.
/// 
/// Auteurs : Younes Rabdi et Jérémie Fortin
/// Date : 2017 - 05 - 26
/// </summary>
namespace pacmangame
{
    /// <summary>
    /// Aussi appelé pac-dot. c'est les points qui peuvent être mangées par pac man.
    /// </summary>
    public abstract class Biscuit
    {
        public int pos_x { get; protected set; }
        public int pos_y { get; protected set; }
        public int valeur { get; protected set; } // valeur en points
        public abstract void dessiner();
    }

    /// <summary>
    /// Petit point un peu partout dans le tableau.
    /// </summary>
    public class Point : Biscuit
    {
        const int VALEUR_EN_POINT = 10;
        const int TAILLE = 10;
        Color couleur = new Color("#FFD700"); // Couleur or

        public Point(int p_pos_x, int p_pos_y) : base()
        {
            pos_x = p_pos_x;
            pos_y = p_pos_y;
            valeur = VALEUR_EN_POINT;
        }

        public override void dessiner()
        {
            Fill(couleur);
            Ellipse(pos_x, pos_y, TAILLE, TAILLE);
        }
    }

    /// <summary>
    /// Gros point dans le tableau
    /// </summary>
    public class GrosPoint : Biscuit
    {
        const int VALEUR_EN_POINT = 50;
        const int TAILLE = 20;
        Color couleur = new Color("#FFD700"); // Couleur or
        public GrosPoint(int p_posx, int p_posy) : base()
        {
            pos_x = p_posx;
            pos_y = p_posy;
            valeur = VALEUR_EN_POINT;
        }
        public override void dessiner()
        {
            Fill(couleur);
            Ellipse(pos_x, pos_y, TAILLE, TAILLE);
        }
    }

    /// <summary>
    /// Un peu comme les points, mais vaut beaucoup plus. N'est pas une forme primitive.
    /// </summary>
    public abstract class Fruit
    {
        public int pos_x { get; protected set; }
        public int pos_y { get; protected set; }
        public int valeur { get; protected set; } // valeur en points
        public abstract void dessiner();
    }

    /// <summary>
    /// Fraise
    /// </summary>
    public class Fraise : Fruit
    {
        const int VALEUR_EN_POINT = 200;
        PImage m_image = LoadImage("../../assets/fraise.png");
        public Fraise(int p_posx, int p_posy) : base()
        {
            pos_x = p_posx - Character.LARGEUR_CHARACTER / 2;
            pos_y = p_posy - Character.LARGEUR_CHARACTER / 2;
            valeur = VALEUR_EN_POINT;
        }
        public override void dessiner()
        {
            Image(m_image, pos_x, pos_y);
        }
    }
}
