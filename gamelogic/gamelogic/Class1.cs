using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace gamelogic
{
    public delegate void ShowPlayerHandler(Player player, bool visible);

    public class Ball : MarshalByRefObject
    {
        protected int x, y, width = 5, height = 5, speed = 5;
        protected bool goingLeft = false;
        protected bool goingTop = false;
        protected Game game;
        private Random r = new Random();

        public Ball(Game g)
        {
            initLocation();
        }

        public void initLocation()
        {
            x = game.Width / 2;
            y = r.Next(10, game.Height - 10);

            goingLeft = Convert.ToBoolean(r.Next(0, 1));
            goingTop = Convert.ToBoolean(r.Next(0, 1));
        }

        public void changeOrientation()
        {
            goingLeft = !goingLeft;
            goingTop = Convert.ToBoolean(r.Next(0, 1));
        }

        public void move()
        {
            if (isGoingLeft)
            {
                x -= speed;
            }
            else
            {
                x += speed;
            }

            if (isGoingTop)
            {
                y -= speed;
            }
            else
            {
                y += speed;
            }
        }

        public bool isGoingLeft
        {
            get
            {
                return goingLeft;
            }
            set
            {
                goingLeft = value;
            }
        }

        public bool isGoingTop
        {
            get
            {
                return goingTop;
            }
            set
            {
                goingTop = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }
    }

    public class Player : MarshalByRefObject
    {
        protected int x, y, width = 5, height = 50;
        protected Game game;
        protected int score = 0;

        public Player(Game g, int playerNum)
        {
            game = g;
            
            if (playerNum == 0)
            {
                x = 10;
            } else
            {
                x = g.Width - 10;
            }

            y = 0;
        }

        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }
    }

    public class Game : MarshalByRefObject
    {
        protected ArrayList players = new ArrayList(); // 0 player - left side; 1 player = right side;
        protected Ball ball;
        protected int width = 427, height = 209;

        public Game()
        {
            Console.WriteLine("Heloo!");
        }

        public Player Connect() {
            if (players.Count > 1)
            {
                return null;
            }

            Player p = new Player(this, players.Count);
            players.Add(p);
            OnShowPlayer(p, true);
            return p;
        }

        public void CreateBall()
        {
            ball = new Ball(this);
        }

        public void Disconnect(Player p) {
            OnShowPlayer(p, false);
            players.Remove(p);
        }

        public void OnShowPlayer(Player p, bool visible) {
            //if (ShowPlayer != null)
           //     ShowPlayer(p, visible);
        }

        public void AddScore(Player p)
        {
            p.Score += 1;
        }

        public void Tick()
        {
            if (ball.isGoingLeft)  
            {
                if (CollisionLeft(ball))    
                {
                    AddScore((Player)players[0]);     
                    ball.initLocation();
                }
                if (!CollisionPlayer((Player)players[0]))
                {
                    ball.move();
                }
                else
                {                              
                    ball.changeOrientation();
                }
            }
            else
            {
                if (CollisionRight(ball))  
                {
                    AddScore((Player)players[1]);
                    ball.initLocation();
                }
                if (!CollisionPlayer((Player)players[1]))
                {
                    ball.move();
                }
                else
                {
                    ball.changeOrientation();
                }
            }
        }

        public Boolean CollisionPlayer(Player player)
        {
            if ((Math.Abs(ball.X - player.X) < player.Width + ball.Width) &&
                (Math.Abs(ball.Y - player.Y) < player.Height + ball.Height))
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionLeft(Ball _ball)
        {
            if (_ball.X <= 0)
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionRight(Ball _ball)
        {
            if (_ball.X + ball.Width >= Width) 
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionUp(Ball _ball)
        {
            if (_ball.Y <= 0)    
            {
                return true;
            }
            return false;
        }

        public Boolean CollisionDown(Ball _ball)
        {
            if (_ball.Y + ball.Height >= Height) 
            {
                return true;
            }
            return false;
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

    }
}
