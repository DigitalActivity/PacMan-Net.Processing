using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetProcessing.Sketch;

/// <summary>
/// Documents des stratégies de déplacement utilisés par les personnages. Ces stratégies
/// affectent la façon dont les personnages se déplacent dans le tableau de jeu.
/// Chaque personnage possède sa propre stratégie de déplacement.
/// 
/// Auteurs : Younes Rabdi et Jérémie Fortin
/// Date : 2017 - 05 - 26
/// </summary>
namespace pacmangame
{
    /// <summary>
    /// (Patron Strategie)
    /// Interface pour les strategies de deplacement
    /// </summary>
    public abstract class StrategieDeplacement
    {
        public enum Direction { UP, DOWN, LEFT, RIGHT }
        public int pos_x { get; protected set; } // Position x
        public int pos_y { get; protected set; } // Position y 
        protected int m_vitesse;

        /// <summary>
        /// Definir la prochaine prochaine position selon la strategy
        /// </summary>
        public abstract void deplacer();

        /// <summary>
        /// Definir la vitesse de deplacement du character
        /// </summary>
        /// <param name="p_vitesse"></param>
        public void set_vitesse(int p_vitesse)
        {
            this.m_vitesse = p_vitesse;
        }

        /// <summary>
        /// Verifier si le deplacement est possible
        /// </summary>
        /// <param name="p_direction"></param>
        /// <returns></returns>
        public bool depEstPossible(Direction p_direction)
        {
            bool possible = false;
            switch (p_direction)
            {
                case Direction.UP:
                    if (!Program.Jeu.m_niveau.estUnObstacle(pos_x, pos_y - m_vitesse - Character.LARGEUR_CHARACTER / 2))
                        possible = true; break;
                case Direction.DOWN:
                    if (!Program.Jeu.m_niveau.estUnObstacle(pos_x, pos_y + m_vitesse + Character.LARGEUR_CHARACTER / 2))
                        possible = true; break;
                case Direction.RIGHT:
                    if (!Program.Jeu.m_niveau.estUnObstacle(pos_x + m_vitesse + (Character.LARGEUR_CHARACTER / 2), pos_y))
                        possible = true; break;
                case Direction.LEFT:
                    if (!Program.Jeu.m_niveau.estUnObstacle(pos_x - m_vitesse - (Character.LARGEUR_CHARACTER / 2), pos_y))
                        possible = true; break;
            }
            return possible;
        }
    }

    /// <summary>
    /// Strategie de deplacement par le joueur
    /// </summary>
    public class DeplacementParJoueur : StrategieDeplacement
    {
        // public Direction m_direction { get; private set; }
        public DeplacementParJoueur(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            pos_x = p_pos_x;
            pos_y = p_pos_y;
            m_vitesse = p_vitesse;
        }

        public override void deplacer()
        {
            switch (KeyCode)
            {
                case (int)'W':
                case KC_UP: if (depEstPossible(Direction.UP)) pos_y -= m_vitesse;/* m_direction = Direction.UP;*/ break;
                case (int)'S':
                case KC_DOWN: if (depEstPossible(Direction.DOWN)) pos_y += m_vitesse; /*m_direction = Direction.DOWN;*/ break;
                case (int)'D':
                case KC_RIGHT: if (depEstPossible(Direction.RIGHT)) pos_x += m_vitesse;/* m_direction = Direction.RIGHT;*/ break;
                case (int)'A':
                case KC_LEFT: if (depEstPossible(Direction.LEFT)) pos_x -= m_vitesse; /*m_direction = Direction.LEFT;*/ break;
            }
        }
    }

    /// <summary>
    /// Deplacement agressif, tendance à pourchasser le joueur
    /// </summary>
    public class DeplacementAgressif : StrategieDeplacement
    {
        Character pac; // le character qui va etre poursuivis
        public DeplacementAgressif(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            pos_x = p_pos_x;
            pos_y = p_pos_y;
            m_vitesse = p_vitesse;
            pac = Program.Jeu.m_niveau.characters.Find(e => e.GetType() == typeof(Pacman));
        }

        public override void deplacer()
        {
            if (this.pos_x > pac.Pos_x() && depEstPossible(Direction.LEFT)) // probabilité qu'il va suivre pacman sur x
                pos_x -= m_vitesse;
            else if (this.pos_x < pac.Pos_x() && depEstPossible(Direction.RIGHT)) // probabilité qu'il va suivre pacman sur x
                pos_x += m_vitesse;
            else if (this.pos_y > pac.Pos_y() && depEstPossible(Direction.UP)) // probabilité qu'il va suivre pacman sur y
                pos_y -= m_vitesse;
            else if (this.pos_y < pac.Pos_y() && depEstPossible(Direction.DOWN)) // probabilité qu'il va suivre pacman sur y
                pos_y += m_vitesse;
        }
    }

    /// <summary>
    /// Deplacement peureux, tendance à evite le joueur
    /// </summary>
    public class DeplacementPeureux : StrategieDeplacement
    {
        Character pac; // le character duquel s'enfuir
        public DeplacementPeureux(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            pos_x = p_pos_x;
            pos_y = p_pos_y;
            m_vitesse = p_vitesse;
            pac = Program.Jeu.m_niveau.characters.Find(e => e.GetType() == typeof(Pacman));
        }

        public override void deplacer()
        {
            if (this.pos_x > pac.Pos_x() && depEstPossible(Direction.RIGHT)) // probabilité qu'il va s'enfuir de pacman sur x
                pos_x += m_vitesse;
            else if (this.pos_x < pac.Pos_x() && depEstPossible(Direction.LEFT)) // probabilité qu'il va s'enfuir de pacman sur x
                pos_x -= m_vitesse;
            else if (this.pos_y > pac.Pos_y() && depEstPossible(Direction.DOWN)) // probabilité qu'il va s'enfuir de pacman sur y
                pos_y += m_vitesse;
            else if (this.pos_y < pac.Pos_y() && depEstPossible(Direction.UP)) // probabilité qu'il va s'enfuir de pacman sur y
                pos_y -= m_vitesse;
        }
    }

    /// <summary>
    /// Deplacement au hasard, choisit une direction au hasard.
    /// </summary>
    public class DeplacementHasard : StrategieDeplacement
    {
        Direction direction;
        int m_intervalleChangement = 1; // En secondes
        int m_frameCount = 0;
        public DeplacementHasard(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            direction = new Direction();
            pos_x = p_pos_x;
            pos_y = p_pos_y;
            m_vitesse = p_vitesse;
        }

        public override void deplacer()
        {
            if (m_frameCount >= m_intervalleChangement * FrameRateValue)
            {
                directionAuHasard();
            }
            else
            {
                if (direction == Direction.UP && depEstPossible(Direction.UP))
                    pos_y -= m_vitesse;
                else if (direction == Direction.UP && depEstPossible(Direction.UP))
                    pos_y -= m_vitesse;
                else if (direction == Direction.UP && depEstPossible(Direction.UP))
                    pos_y -= m_vitesse;
                else if (direction == Direction.UP && depEstPossible(Direction.UP))
                    pos_y -= m_vitesse;
                else
                    directionAuHasard();
            }
            m_frameCount++;
        }

        private void directionAuHasard()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());

            if (random.Next(0, 10) < 4 && depEstPossible(Direction.LEFT)) // probabilité qu'il va se déplacer sur x
                pos_x -= m_vitesse;
            else if (random.Next(0, 10) < 4 && depEstPossible(Direction.RIGHT)) // probabilité qu'il va se déplacer sur x
                pos_x += m_vitesse;
            else if (random.Next(0, 10) < 6 && depEstPossible(Direction.UP)) // probabilité qu'il va se déplacer sur y
                pos_y -= m_vitesse;
            else if (random.Next(0, 10) > 5 && depEstPossible(Direction.DOWN)) // probabilité qu'il va se déplacer sur y
                pos_y += m_vitesse;
            m_frameCount = 0;
        }
    }

    /// <summary>
    /// Deplacement pour ambuscade, vise l'endroit ou Pacman se dirige.
    /// </summary>
    public class DeplacementAmbuscade : StrategieDeplacement
    {
        Character pac; // le character qui va etre chassé
        public DeplacementAmbuscade(int p_pos_x, int p_pos_y, int p_vitesse) : base()
        {
            pos_x = p_pos_x;
            pos_y = p_pos_y;
            m_vitesse = p_vitesse;
            pac = Program.Jeu.m_niveau.characters.Find(e => e.GetType() == typeof(Pacman));
        }

        public override void deplacer()
        {
            /* PRÉAENTEMENT, PAREIL AU DÉPLACEMENT AGGRESSIF */

            //Direction dirPacMan = ((DeplacementParJoueur)(pac.stgDeplacement)).m_direction;
            Random random = new Random();
            if (random.Next(0, 10) < 4 && this.pos_x > pac.Pos_x() && depEstPossible(Direction.LEFT)) // probabilité qu'il va suivre pacman sur x
                pos_x -= m_vitesse;
            else if (this.pos_x < pac.Pos_x() && depEstPossible(Direction.RIGHT)) // probabilité qu'il va suivre pacman sur x
                pos_x += m_vitesse;
            else if (random.Next(0, 10) < 4 && this.pos_y > pac.Pos_y() && depEstPossible(Direction.UP)) // probabilité qu'il va suivre pacman sur y
                pos_y -= m_vitesse;
            else if (this.pos_y < pac.Pos_y() && depEstPossible(Direction.DOWN)) // probabilité qu'il va suivre pacman sur y
                pos_y += m_vitesse;
        }
    }
}
